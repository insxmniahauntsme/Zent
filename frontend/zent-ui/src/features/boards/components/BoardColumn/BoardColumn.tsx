import { useEffect, useRef, useState } from "react";

import { useDroppable, type DraggableAttributes } from "@dnd-kit/core";
import type { SyntheticListenerMap } from "@dnd-kit/core/dist/hooks/utilities";

import type { BoardColumnDto } from "@/features/boards/model/types";

import styles from "./BoardColumn.module.css";
import editIcon from "@/assets/icons/edit-icon.svg";
import binIcon from "@/assets/icons/bin-icon.svg";

import {
  SortableContext,
  verticalListSortingStrategy,
} from "@dnd-kit/sortable";
import SortableTaskCard from "@/features/tasks/components/SortableTaskCard/SortableTaskCard";

interface BoardColumnProps {
  column: BoardColumnDto;
  isRecentlyCreated?: boolean;
  isTitleEditing?: boolean;
  isSavingTitle?: boolean;
  isTaskDragging?: boolean;
  dragHandleAttributes?: DraggableAttributes;
  dragHandleListeners?: SyntheticListenerMap;
  onStartTitleEditing?: () => void;
  onCancelTitleEditing?: () => void;
  onSaveTitle?: (title: string) => Promise<void>;
  onToggleFinal?: () => Promise<void> | void;
  onDeleteColumn?: () => Promise<void> | void;
  onAddTask?: () => void;
  onTaskClick?: (taskId: string) => void;
}

const BoardColumn = ({
  column,
  isRecentlyCreated = false,
  isTitleEditing = false,
  isSavingTitle = false,
  isTaskDragging = false,
  dragHandleAttributes,
  dragHandleListeners,
  onStartTitleEditing,
  onCancelTitleEditing,
  onSaveTitle,
  onToggleFinal,
  onDeleteColumn,
  onAddTask,
  onTaskClick,
}: BoardColumnProps) => {
  const [draftTitle, setDraftTitle] = useState(column.title);
  const [titleError, setTitleError] = useState<string | null>(null);

  const inputRef = useRef<HTMLInputElement | null>(null);
  const isSavingRef = useRef(false);

  const { setNodeRef: setTaskDropZoneRef } = useDroppable({
    id: `task-dropzone-${column.id}`,
    data: {
      type: "task-dropzone",
      columnId: column.id,
    },
  });

  useEffect(() => {
    if (!isTitleEditing) {
      return;
    }

    setDraftTitle(column.title);
    setTitleError(null);

    requestAnimationFrame(() => {
      inputRef.current?.focus();
      inputRef.current?.select();
    });
  }, [isTitleEditing, column.title]);

  const handleStartEditing = () => {
    setDraftTitle(column.title);
    setTitleError(null);
    onStartTitleEditing?.();
  };

  const handleCancel = () => {
    setDraftTitle(column.title);
    setTitleError(null);
    onCancelTitleEditing?.();
  };

  const handleSave = async () => {
    if (isSavingRef.current || isSavingTitle) {
      return;
    }

    const normalizedTitle = draftTitle.trim();

    if (normalizedTitle === column.title.trim()) {
      setTitleError(null);
      onCancelTitleEditing?.();
      return;
    }

    if (normalizedTitle.length < 2) {
      setTitleError("Min 2 characters.");
      return;
    }

    if (normalizedTitle.length > 32) {
      setTitleError("Max 32 characters.");
      return;
    }

    try {
      isSavingRef.current = true;
      setTitleError(null);

      await onSaveTitle?.(normalizedTitle);
    } finally {
      isSavingRef.current = false;
    }
  };

  return (
    <section
      className={`${styles.column} ${
        isRecentlyCreated ? styles.columnEnter : ""
      }`}
    >
      <div className={styles.columnHeader}>
        <div className={styles.columnTitleBlock}>
          {isTitleEditing ? (
            <>
              <input
                ref={inputRef}
                value={draftTitle}
                className={styles.titleInput}
                disabled={isSavingTitle}
                onChange={(event) => setDraftTitle(event.target.value)}
                onBlur={handleSave}
                onKeyDown={(event) => {
                  if (event.key === "Enter") {
                    event.preventDefault();
                    inputRef.current?.blur();
                  }

                  if (event.key === "Escape") {
                    event.preventDefault();
                    handleCancel();
                  }
                }}
              />

              {titleError && <p className={styles.titleError}>{titleError}</p>}
            </>
          ) : (
            <>
              <>
                <div className={styles.titleRow}>
                  <button
                    type="button"
                    className={styles.dragHandle}
                    aria-label="Drag column"
                    title="Drag column"
                    {...dragHandleAttributes}
                    {...dragHandleListeners}
                  >
                    <span />
                  </button>

                  <div className={styles.titleTextRow}>
                    <h2
                      className={styles.columnTitle}
                      title="Double click to rename"
                      onDoubleClick={handleStartEditing}
                    >
                      {column.title}
                    </h2>

                    <button
                      type="button"
                      className={styles.editTitleButton}
                      aria-label="Edit column title"
                      onClick={handleStartEditing}
                    >
                      <img src={editIcon} alt="" aria-hidden="true" />
                    </button>
                  </div>
                </div>

                <span className={styles.taskCount}>
                  {column.tasks.length} tasks
                </span>
              </>
            </>
          )}
        </div>

        <div className={styles.columnControls}>
          <div className={styles.finalControl}>
            <span className={styles.finalLabel}>Final</span>

            <button
              type="button"
              className={`${styles.finalSwitch} ${
                column.isFinal ? styles.finalSwitchActive : ""
              }`}
              aria-label={
                column.isFinal
                  ? "Mark column as not final"
                  : "Mark column as final"
              }
              title={column.isFinal ? "Final column" : "Mark as final"}
              aria-pressed={column.isFinal}
              onClick={onToggleFinal}
            >
              <span />
            </button>
          </div>

          <button
            type="button"
            className={styles.deleteColumnButton}
            aria-label="Delete column"
            title="Delete column"
            onClick={onDeleteColumn}
          >
            <img src={binIcon} alt="" aria-hidden="true" />
          </button>
        </div>
      </div>

      <SortableContext
        items={column.tasks.map((task) => task.id)}
        strategy={verticalListSortingStrategy}
      >
        <div
          ref={setTaskDropZoneRef}
          className={`${styles.taskList} ${
            isTaskDragging ? styles.taskListDragging : ""
          }`}
        >
          {column.tasks.map((task) => (
            <SortableTaskCard
              key={task.id}
              task={task}
              columnId={column.id}
              isCompleted={column.isFinal}
              onClick={() => onTaskClick?.(task.id)}
            />
          ))}

          <button
            type="button"
            className={styles.addTaskButton}
            onClick={onAddTask}
          >
            + Add task
          </button>
        </div>
      </SortableContext>
    </section>
  );
};

export default BoardColumn;
