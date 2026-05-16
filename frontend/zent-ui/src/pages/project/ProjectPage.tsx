import { useState } from "react";
import { useParams } from "react-router-dom";

import ProjectHeader from "@/features/projects/components/ProjectHeader/ProjectHeader";
import BoardNavigation from "@/features/boards/components/BoardNavigation/BoardNavigation";
import Board from "@/features/boards/components/Board/Board";

import { useProject } from "@/features/projects/hooks/useProject";
import { useProjectBoards } from "@/features/projects/hooks/useProjectBoards";
import { useBoard } from "@/features/boards/hooks/useBoard";

import styles from "./ProjectPage.module.css";
import { useCreateBoard } from "@/features/boards/hooks/useCreateBoard";
import CreateBoardModal from "@/features/boards/components/CreateBoardModal/CreateBoardModal";
import { useCreateColumn } from "@/features/boards/hooks/useCreateColumn";
import { useUpdateColumn } from "@/features/boards/hooks/useUpdateColumn";
import { useMoveColumn } from "@/features/boards/hooks/useMoveColumn";
import { useDeleteColumn } from "@/features/boards/hooks/useDeleteColumn";
import { useCreateTask } from "@/features/tasks/hooks/useCreateTask";
import { useMoveTask } from "@/features/tasks/hooks/useMoveTask";
import { useProjectMembers } from "@/features/projects/hooks/useProjectMembers";

const ProjectPage = () => {
  const { projectId } = useParams<{ projectId: string }>();
  const [selectedBoardId, setSelectedBoardId] = useState<string | undefined>();

  const [isCreateBoardOpen, setIsCreateBoardOpen] = useState(false);
  const [createBoardError, setCreateBoardError] = useState<string | null>(null);

  const createBoardMutation = useCreateBoard(projectId);

  const {
    data: project,
    isLoading: isProjectLoading,
    isError: isProjectError,
  } = useProject(projectId);

  const {
    data: boards = [],
    isLoading: areBoardsLoading,
    isError: areBoardsError,
  } = useProjectBoards(projectId);

  const { data: projectMembers = [], isLoading: areProjectMembersLoading } =
    useProjectMembers(projectId);

  const activeBoardId = selectedBoardId ?? boards[0]?.id;

  const createColumnMutation = useCreateColumn(activeBoardId);
  const updateColumnMutation = useUpdateColumn();

  const moveColumnMutation = useMoveColumn();

  const deleteColumnMutation = useDeleteColumn();

  const createTaskMutation = useCreateTask();

  const moveTaskMutation = useMoveTask();

  const {
    data: board,
    isLoading: isBoardLoading,
    isError: isBoardError,
  } = useBoard(activeBoardId);

  if (isProjectLoading) {
    return <section className={styles.page}>Loading project...</section>;
  }

  if (isProjectError || !project) {
    return <section className={styles.page}>Failed to load project.</section>;
  }

  return (
    <section className={styles.page}>
      <ProjectHeader
        name={project.name}
        description={project.description}
        client={project.client}
        membersCount={project.membersCount}
        boardsCount={project.boardsCount}
        updatedAtLabel="Updated recently"
      />

      {areBoardsLoading ? (
        <div className={styles.state}>Loading boards...</div>
      ) : areBoardsError ? (
        <div className={styles.state}>Failed to load boards.</div>
      ) : boards.length === 0 ? (
        <div className={styles.emptyState}>
          <h2>No boards yet</h2>
          <p>Create your first board to start organizing tasks.</p>
        </div>
      ) : (
        <>
          <BoardNavigation
            boards={boards}
            activeBoardId={activeBoardId}
            onBoardChange={setSelectedBoardId}
            onCreateBoard={() => {
              setCreateBoardError(null);
              setIsCreateBoardOpen(true);
            }}
          />

          {isBoardLoading ? (
            <div className={styles.state}>Loading board...</div>
          ) : isBoardError || !board ? (
            <div className={styles.state}>Failed to load board.</div>
          ) : (
            <Board
              boardId={board.id}
              columns={board.columns}
              isCreatingColumn={createColumnMutation.isPending}
              isDeletingColumn={deleteColumnMutation.isPending}
              isCreatingTask={createTaskMutation.isPending}
              projectMembers={projectMembers}
              areProjectMembersLoading={areProjectMembersLoading}
              onCreateColumn={async () => {
                const columnId = await createColumnMutation.mutateAsync({
                  isFinal: false,
                });

                return columnId;
              }}
              onUpdateColumn={async (boardId, columnId, title, isFinal) => {
                await updateColumnMutation.mutateAsync({
                  boardId,
                  columnId,
                  data: {
                    title,
                    isFinal,
                  },
                });
              }}
              onMoveColumn={async (boardId, columnId, targetOrder) => {
                await moveColumnMutation.mutateAsync({
                  boardId,
                  columnId,
                  data: {
                    targetOrder,
                  },
                });
              }}
              onDeleteColumn={async (boardId, columnId) => {
                await deleteColumnMutation.mutateAsync({
                  boardId,
                  columnId,
                });
              }}
              onCreateTask={async (boardId, columnId, data) => {
                await createTaskMutation.mutateAsync({
                  boardId,
                  columnId,
                  data,
                });
              }}
              onMoveTask={async (boardId, taskId, data) => {
                await moveTaskMutation.mutateAsync({
                  boardId,
                  taskId,
                  data,
                });
              }}
            />
          )}
        </>
      )}
      {isCreateBoardOpen && (
        <CreateBoardModal
          isSubmitting={createBoardMutation.isPending}
          submitError={createBoardError}
          onClose={() => {
            setCreateBoardError(null);
            setIsCreateBoardOpen(false);
          }}
          onSubmit={async (data) => {
            try {
              setCreateBoardError(null);
              const boardId = await createBoardMutation.mutateAsync(data);
              setSelectedBoardId(boardId);
              setIsCreateBoardOpen(false);
            } catch {
              setCreateBoardError("Failed to create board. Please try again.");
            }
          }}
        />
      )}
    </section>
  );
};

export default ProjectPage;
