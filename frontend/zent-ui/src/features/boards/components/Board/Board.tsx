import { useMemo, useRef, useState, type CSSProperties } from "react";
import {
  closestCenter,
  DndContext,
  DragOverlay,
  type DragEndEvent,
  type DragOverEvent,
  type DragStartEvent,
  PointerSensor,
  useSensor,
  useSensors,
} from "@dnd-kit/core";
import {
  restrictToHorizontalAxis,
  restrictToParentElement,
} from "@dnd-kit/modifiers";
import {
  arrayMove,
  horizontalListSortingStrategy,
  SortableContext,
} from "@dnd-kit/sortable";

import type {
  BoardColumnDto,
  BoardTaskDto,
} from "@/features/boards/model/types";
import BoardColumn from "../BoardColumn/BoardColumn";
import SortableBoardColumn from "../SortableBoardColumn/SortableBoardColumn";
import AddColumnRail from "../AddColumnRail/AddColumnRail";

import styles from "./Board.module.css";
import DeleteColumnModal from "../DeleteColumnModal/DeleteColumnModal";
import CreateTaskModal from "@/features/tasks/components/CreateTaskModal/CreateTaskModal";
import type {
  AddTaskRequest,
  MoveTaskRequest,
} from "@/features/tasks/model/types";
import type { ProjectMemberDto } from "@/features/projects/model/types";

import TaskCard from "@/features/tasks/components/TaskCard/TaskCard";
import { useNavigate, useParams } from "react-router-dom";

interface BoardProps {
  boardId: string;
  columns: BoardColumnDto[];
  isCreatingColumn: boolean;
  isDeletingColumn: boolean;
  isCreatingTask: boolean;
  projectMembers?: ProjectMemberDto[];
  areProjectMembersLoading?: boolean;

  onCreateColumn: () => Promise<string>;

  onUpdateColumn: (
    boardId: string,
    columnId: string,
    title: string,
    isFinal: boolean,
  ) => Promise<void>;

  onMoveColumn: (
    boardId: string,
    columnId: string,
    targetOrder: number,
  ) => Promise<void>;

  onDeleteColumn: (boardId: string, columnId: string) => Promise<void>;

  onCreateTask: (
    boardId: string,
    columnId: string,
    data: AddTaskRequest,
  ) => Promise<void>;

  onMoveTask: (
    boardId: string,
    taskId: string,
    data: MoveTaskRequest,
  ) => Promise<void>;
}

const Board = ({
  boardId,
  columns,
  isCreatingColumn,
  isDeletingColumn,
  isCreatingTask,
  projectMembers = [],
  areProjectMembersLoading = false,
  onCreateColumn,
  onUpdateColumn,
  onMoveColumn,
  onDeleteColumn,
  onCreateTask,
  onMoveTask,
}: BoardProps) => {
  const navigate = useNavigate();
  const { teamId } = useParams<{ teamId: string }>();

  const [isSlotHovered, setIsSlotHovered] = useState(false);
  const [editingColumnId, setEditingColumnId] = useState<string | null>(null);
  const [pendingEditColumnId, setPendingEditColumnId] = useState<string | null>(
    null,
  );
  const [recentlyCreatedColumnId, setRecentlyCreatedColumnId] = useState<
    string | null
  >(null);
  const [orderedColumnIds, setOrderedColumnIds] = useState<string[]>([]);
  const [activeColumnId, setActiveColumnId] = useState<string | null>(null);
  const [overColumnId, setOverColumnId] = useState<string | null>(null);
  const [columnPendingDeleteId, setColumnPendingDeleteId] = useState<
    string | null
  >(null);
  const [taskColumnId, setTaskColumnId] = useState<string | null>(null);
  const [createTaskError, setCreateTaskError] = useState<string | null>(null);
  const [activeColumnWidth, setActiveColumnWidth] = useState<number | null>(
    null,
  );

  const [activeTask, setActiveTask] = useState<BoardTaskDto | null>(null);
  const [activeTaskWidth, setActiveTaskWidth] = useState<number | null>(null);
  const [activeTaskIsCompleted, setActiveTaskIsCompleted] = useState(false);
  const [activeDragType, setActiveDragType] = useState<
    "column" | "task" | null
  >(null);

  const [taskDragColumns, setTaskDragColumns] = useState<
    BoardColumnDto[] | null
  >(null);

  const [taskDragSnapshot, setTaskDragSnapshot] = useState<
    BoardColumnDto[] | null
  >(null);

  const animationTimeoutRef = useRef<number | null>(null);
  const lastTaskDragOverKeyRef = useRef<string | null>(null);
  const taskDragSnapshotRef = useRef<BoardColumnDto[] | null>(null);

  const sensors = useSensors(
    useSensor(PointerSensor, {
      activationConstraint: {
        distance: 8,
      },
    }),
  );

  const isAddSlotExpanded = isSlotHovered;
  const activeEditingColumnId = editingColumnId ?? pendingEditColumnId;

  const handleCreateColumn = async () => {
    const columnId = await onCreateColumn();

    setPendingEditColumnId(columnId);
    setRecentlyCreatedColumnId(columnId);

    if (animationTimeoutRef.current) {
      window.clearTimeout(animationTimeoutRef.current);
    }

    animationTimeoutRef.current = window.setTimeout(() => {
      setRecentlyCreatedColumnId(null);
    }, 900);
  };

  const handleStartColumnTitleEditing = (columnId: string) => {
    setPendingEditColumnId(null);
    setEditingColumnId(columnId);
  };

  const handleCancelColumnTitleEditing = () => {
    setPendingEditColumnId(null);
    setEditingColumnId(null);
  };

  const handleSaveColumn = async (
    columnId: string,
    title: string,
    isFinal: boolean,
  ) => {
    const normalizedTitle = title.trim();

    if (normalizedTitle.length < 2 || normalizedTitle.length > 32) {
      return;
    }

    await onUpdateColumn(boardId, columnId, normalizedTitle, isFinal);

    setPendingEditColumnId(null);
    setEditingColumnId(null);
  };

  const columnsById = new Map(columns.map((column) => [column.id, column]));

  const canUseOrderedColumns =
    orderedColumnIds.length === columns.length &&
    orderedColumnIds.every((columnId) => columnsById.has(columnId));

  const visibleColumns = canUseOrderedColumns
    ? orderedColumnIds.map((columnId) => columnsById.get(columnId)!)
    : columns;

  const visibleColumnIds = visibleColumns.map((column) => column.id);

  const displayedColumns = taskDragColumns ?? visibleColumns;

  const activeColumn = activeColumnId
    ? (visibleColumns.find((column) => column.id === activeColumnId) ?? null)
    : null;

  const taskColumn = taskColumnId
    ? (columns.find((column) => column.id === taskColumnId) ?? null)
    : null;

  const columnPendingDelete = columnPendingDeleteId
    ? (columns.find((column) => column.id === columnPendingDeleteId) ?? null)
    : null;

  const dropIndicator = useMemo(() => {
    if (!activeColumnId || !overColumnId || activeColumnId === overColumnId) {
      return null;
    }

    const activeIndex = visibleColumns.findIndex(
      (column) => column.id === activeColumnId,
    );

    const overIndex = visibleColumns.findIndex(
      (column) => column.id === overColumnId,
    );

    if (activeIndex === -1 || overIndex === -1) {
      return null;
    }

    return {
      overColumnId,
      side: activeIndex < overIndex ? "right" : "left",
    };
  }, [activeColumnId, overColumnId, visibleColumns]);

  const handleDragEnd = async (event: DragEndEvent) => {
    const { active, over } = event;

    const activeType = active.data.current?.type as
      | "column"
      | "task"
      | undefined;

    if (!over) {
      if (activeType === "task") {
        setTaskDragColumns(taskDragSnapshot);
        resetVisualDragState();
        setTaskDragSnapshot(null);
        return;
      }

      resetDragState();
      return;
    }

    if (activeType === "task") {
      const activeTaskId = String(active.id);
      const finalColumns = taskDragColumns ?? visibleColumns;

      const targetColumn = finalColumns.find((column) =>
        column.tasks.some((task) => task.id === activeTaskId),
      );

      if (!targetColumn) {
        setTaskDragColumns(taskDragSnapshot);
        resetVisualDragState();
        setTaskDragSnapshot(null);
        return;
      }

      const targetOrder =
        targetColumn.tasks.findIndex((task) => task.id === activeTaskId) + 1;

      try {
        await onMoveTask(boardId, activeTaskId, {
          targetColumnId: targetColumn.id,
          targetOrder,
        });

        setTaskDragColumns(null);
        setTaskDragSnapshot(null);
      } catch {
        setTaskDragColumns(taskDragSnapshot);
        setTaskDragSnapshot(null);
      } finally {
        resetVisualDragState();
      }

      return;
    }

    if (activeType !== "column") {
      resetDragState();
      return;
    }

    if (active.id === over.id) {
      resetDragState();
      return;
    }

    const activeColumnId = String(active.id);
    const overColumnId = String(over.id);

    const oldIndex = visibleColumns.findIndex(
      (column) => column.id === activeColumnId,
    );

    const newIndex = visibleColumns.findIndex(
      (column) => column.id === overColumnId,
    );

    if (oldIndex === -1 || newIndex === -1) {
      resetDragState();
      return;
    }

    const reorderedColumns = arrayMove(visibleColumns, oldIndex, newIndex);
    const reorderedColumnIds = reorderedColumns.map((column) => column.id);

    setOrderedColumnIds(reorderedColumnIds);

    const movedColumn = reorderedColumns[newIndex];
    const targetOrder = newIndex + 1;

    try {
      await onMoveColumn(boardId, movedColumn.id, targetOrder);
    } catch {
      setOrderedColumnIds(columns.map((column) => column.id));
    } finally {
      resetDragState();
    }
  };

  const handleDragStart = (event: DragStartEvent) => {
    const activeType = event.active.data.current?.type as
      | "column"
      | "task"
      | undefined;

    setActiveDragType(activeType ?? null);

    if (activeType === "column") {
      const columnId = String(event.active.id);

      setActiveColumnId(columnId);

      const columnElement = document.querySelector<HTMLElement>(
        `[data-column-id="${columnId}"]`,
      );

      setActiveColumnWidth(
        columnElement?.getBoundingClientRect().width ?? null,
      );

      return;
    }

    if (activeType === "task") {
      const taskId = String(event.active.id);
      const result = findTaskWithColumn(taskId, visibleColumns);

      if (!result) {
        return;
      }

      taskDragSnapshotRef.current = visibleColumns;
      setTaskDragColumns(visibleColumns);
      setTaskDragSnapshot(visibleColumns);

      setActiveTask(result.task);
      setActiveTaskIsCompleted(result.column.isFinal);

      const taskElement = document.querySelector<HTMLElement>(
        `[data-task-id="${taskId}"]`,
      );

      setActiveTaskWidth(taskElement?.getBoundingClientRect().width ?? null);

      return;
    }
  };

  const handleDragOver = (event: DragOverEvent) => {
    const { active, over } = event;

    if (!over) return;

    const activeType = active.data.current?.type as
      | "column"
      | "task"
      | undefined;

    if (activeType === "task") {
      const overType = over.data.current?.type as
        | "task"
        | "task-dropzone"
        | "column"
        | undefined;

      if (overType !== "task" && overType !== "task-dropzone") return;

      const activeTaskId = String(active.id);
      const overId = String(over.id);
      const overColumnId =
        overType === "task-dropzone"
          ? String(over.data.current?.columnId)
          : undefined;

      const dragOverKey = `${activeTaskId}:${overId}:${overType}:${overColumnId ?? ""}`;

      if (lastTaskDragOverKeyRef.current === dragOverKey) return;
      lastTaskDragOverKeyRef.current = dragOverKey;

      setTaskDragColumns((currentColumns) => {
        const baseColumns =
          currentColumns ?? taskDragSnapshotRef.current ?? visibleColumns;
        const nextColumns = moveTaskLocally(
          baseColumns,
          activeTaskId,
          overId,
          overType,
          overColumnId,
        );
        return nextColumns === baseColumns ? currentColumns : nextColumns;
      });

      return;
    }

    if (activeType !== "column") {
      return;
    }

    const overType = over.data.current?.type as
      | "column"
      | "task"
      | "task-dropzone"
      | undefined;

    if (overType !== "column") {
      return;
    }

    const nextOverColumnId = String(over.id);

    setOverColumnId((currentOverColumnId) => {
      if (currentOverColumnId === nextOverColumnId) {
        return currentOverColumnId;
      }

      return nextOverColumnId;
    });
  };

  const resetVisualDragState = () => {
    lastTaskDragOverKeyRef.current = null;

    setActiveColumnId(null);
    setActiveColumnWidth(null);

    setActiveTask(null);
    setActiveTaskWidth(null);
    setActiveTaskIsCompleted(false);

    setActiveDragType(null);
    setOverColumnId(null);
  };

  const resetTaskDragPreview = () => {
    taskDragSnapshotRef.current = null;
    setTaskDragColumns(null);
    setTaskDragSnapshot(null);
  };

  const resetDragState = () => {
    taskDragSnapshotRef.current = null;
    resetVisualDragState();
    resetTaskDragPreview();
  };

  const moveTaskLocally = (
    sourceColumns: BoardColumnDto[],
    activeTaskId: string,
    overId: string,
    overType: string | undefined,
    overColumnId?: string,
  ) => {
    if (activeTaskId === overId) {
      return sourceColumns;
    }

    const sourceColumnIndex = sourceColumns.findIndex((column) =>
      column.tasks.some((task) => task.id === activeTaskId),
    );

    if (sourceColumnIndex === -1) {
      return sourceColumns;
    }

    const sourceColumn = sourceColumns[sourceColumnIndex];

    const activeTaskIndex = sourceColumn.tasks.findIndex(
      (task) => task.id === activeTaskId,
    );

    const activeTask = sourceColumn.tasks[activeTaskIndex];

    if (!activeTask) {
      return sourceColumns;
    }

    const targetColumnIndex =
      overType === "task-dropzone" && overColumnId
        ? sourceColumns.findIndex((column) => column.id === overColumnId)
        : sourceColumns.findIndex((column) =>
            column.tasks.some((task) => task.id === overId),
          );

    if (targetColumnIndex === -1) {
      return sourceColumns;
    }

    const targetColumn = sourceColumns[targetColumnIndex];

    if (sourceColumn.id === targetColumn.id) {
      const overTaskIndex = targetColumn.tasks.findIndex(
        (task) => task.id === overId,
      );

      if (overTaskIndex === -1 || activeTaskIndex === overTaskIndex) {
        return sourceColumns;
      }

      return sourceColumns.map((column) => {
        if (column.id !== sourceColumn.id) {
          return column;
        }

        return {
          ...column,
          tasks: arrayMove(column.tasks, activeTaskIndex, overTaskIndex),
        };
      });
    }

    const targetTaskIndex =
      overType === "task-dropzone"
        ? targetColumn.tasks.length
        : targetColumn.tasks.findIndex((task) => task.id === overId);

    const insertIndex =
      targetTaskIndex === -1 ? targetColumn.tasks.length : targetTaskIndex;

    return sourceColumns.map((column) => {
      if (column.id === sourceColumn.id) {
        return {
          ...column,
          tasks: column.tasks.filter((task) => task.id !== activeTaskId),
        };
      }

      if (column.id === targetColumn.id) {
        const nextTasks = [...column.tasks];

        nextTasks.splice(insertIndex, 0, activeTask);

        return {
          ...column,
          tasks: nextTasks,
        };
      }

      return column;
    });
  };

  const handleRequestDeleteColumn = (columnId: string) => {
    setColumnPendingDeleteId(columnId);
  };

  const handleConfirmDeleteColumn = async () => {
    if (!columnPendingDelete) {
      return;
    }

    await onDeleteColumn(boardId, columnPendingDelete.id);

    setColumnPendingDeleteId(null);
  };

  const findTaskWithColumn = (
    taskId: string,
    sourceColumns: BoardColumnDto[] = displayedColumns,
  ) => {
    for (const column of sourceColumns) {
      const task = column.tasks.find((item) => item.id === taskId);

      if (task) {
        return {
          task,
          column,
        };
      }
    }

    return null;
  };

  const dragOverlayContent = useMemo(() => {
    if (activeDragType === "column" && activeColumn) {
      return (
        <div
          className={styles.dragOverlayColumn}
          style={
            {
              "--drag-column-width": activeColumnWidth
                ? `${activeColumnWidth}px`
                : "320px",
            } as CSSProperties
          }
        >
          <BoardColumn
            column={activeColumn}
            isTitleEditing={false}
            onAddTask={() => undefined}
          />
        </div>
      );
    }

    if (activeDragType === "task" && activeTask) {
      return (
        <div
          className={styles.dragOverlayTask}
          style={
            {
              "--drag-task-width": activeTaskWidth
                ? `${activeTaskWidth}px`
                : "280px",
            } as CSSProperties
          }
        >
          <TaskCard task={activeTask} isCompleted={activeTaskIsCompleted} />
        </div>
      );
    }

    return null;
  }, [
    activeDragType,
    activeColumn,
    activeTask,
    activeColumnWidth,
    activeTaskWidth,
    activeTaskIsCompleted,
  ]);

  const dndModifiers =
    activeDragType === "column"
      ? [restrictToHorizontalAxis, restrictToParentElement]
      : [];

  return (
    <div
      className={`${styles.boardWrap} ${
        isAddSlotExpanded ? styles.boardWrapExpanded : ""
      }`}
    >
      <DndContext
        sensors={sensors}
        collisionDetection={closestCenter}
        modifiers={dndModifiers}
        onDragStart={handleDragStart}
        onDragOver={handleDragOver}
        onDragEnd={handleDragEnd}
        onDragCancel={resetDragState}
      >
        <SortableContext
          items={visibleColumnIds}
          strategy={horizontalListSortingStrategy}
        >
          <div className={styles.kanban}>
            {displayedColumns.map((column) => (
              <SortableBoardColumn
                key={column.id}
                columnId={column.id}
                isDragging={activeColumnId === column.id}
              >
                {({ attributes, listeners }) => (
                  <div className={styles.columnSlot} data-column-id={column.id}>
                    {dropIndicator?.overColumnId === column.id && (
                      <div
                        className={`${styles.dropIndicator} ${
                          dropIndicator.side === "left"
                            ? styles.dropIndicatorLeft
                            : styles.dropIndicatorRight
                        }`}
                      />
                    )}

                    <BoardColumn
                      column={column}
                      isRecentlyCreated={recentlyCreatedColumnId === column.id}
                      isTitleEditing={activeEditingColumnId === column.id}
                      isTaskDragging={activeDragType === "task"}
                      dragHandleAttributes={attributes}
                      dragHandleListeners={listeners}
                      onStartTitleEditing={() =>
                        handleStartColumnTitleEditing(column.id)
                      }
                      onCancelTitleEditing={handleCancelColumnTitleEditing}
                      onSaveTitle={(title) =>
                        handleSaveColumn(column.id, title, column.isFinal)
                      }
                      onToggleFinal={() =>
                        handleSaveColumn(
                          column.id,
                          column.title,
                          !column.isFinal,
                        )
                      }
                      onDeleteColumn={() =>
                        handleRequestDeleteColumn(column.id)
                      }
                      onAddTask={() => {
                        setCreateTaskError(null);
                        setTaskColumnId(column.id);
                      }}
                      onTaskClick={(taskId) => {
                        if (!teamId) {
                          return;
                        }

                        navigate(`/app/${teamId}/tasks/${taskId}`);
                      }}
                    />
                  </div>
                )}
              </SortableBoardColumn>
            ))}
          </div>
        </SortableContext>

        <DragOverlay adjustScale={false}>{dragOverlayContent}</DragOverlay>
      </DndContext>

      <aside
        className={styles.addColumnSlot}
        onMouseEnter={() => setIsSlotHovered(true)}
        onMouseLeave={() => setIsSlotHovered(false)}
      >
        <AddColumnRail
          isExpanded={isAddSlotExpanded}
          disabled={isCreatingColumn}
          onClick={handleCreateColumn}
        />
      </aside>
      {columnPendingDelete && (
        <DeleteColumnModal
          columnTitle={columnPendingDelete.title}
          tasksCount={columnPendingDelete.tasks.length}
          isDeleting={isDeletingColumn}
          onClose={() => setColumnPendingDeleteId(null)}
          onConfirm={handleConfirmDeleteColumn}
        />
      )}
      {taskColumn && (
        <CreateTaskModal
          columnTitle={taskColumn.title}
          projectMembers={projectMembers}
          areProjectMembersLoading={areProjectMembersLoading}
          isSubmitting={isCreatingTask}
          submitError={createTaskError}
          onClose={() => {
            setCreateTaskError(null);
            setTaskColumnId(null);
          }}
          onSubmit={async (data) => {
            try {
              setCreateTaskError(null);

              await onCreateTask(boardId, taskColumn.id, data);

              setTaskColumnId(null);
            } catch {
              setCreateTaskError("Failed to create task. Please try again.");
            }
          }}
        />
      )}
    </div>
  );
};

export default Board;
