import { useEffect, useRef, useState } from "react";

import type { ProjectMemberDto } from "@/features/projects/model/types";
import type {
  AddTaskRequest,
  TaskPriority,
} from "@/features/tasks/model/types";

import TaskAssigneeSelector from "@/features/tasks/components/TaskAssigneeSelector/TaskAssigneeSelector";

import styles from "./CreateTaskModal.module.css";

interface CreateTaskModalProps {
  columnTitle: string;
  projectMembers?: ProjectMemberDto[];
  areProjectMembersLoading?: boolean;
  isSubmitting: boolean;
  submitError?: string | null;
  onClose: () => void;
  onSubmit: (data: AddTaskRequest) => Promise<void>;
}

const priorityOptions: TaskPriority[] = ["Low", "Medium", "High", "Critical"];

const CreateTaskModal = ({
  columnTitle,
  projectMembers = [],
  areProjectMembersLoading = false,
  isSubmitting,
  submitError,
  onClose,
  onSubmit,
}: CreateTaskModalProps) => {
  const [title, setTitle] = useState("");
  const [description, setDescription] = useState("");
  const [priority, setPriority] = useState<TaskPriority>("Medium");
  const [untilDate, setUntilDate] = useState("");
  const [assignee, setAssignee] = useState<ProjectMemberDto | null>(null);

  const [titleError, setTitleError] = useState<string | null>(null);
  const [descriptionError, setDescriptionError] = useState<string | null>(null);

  const titleInputRef = useRef<HTMLInputElement | null>(null);

  useEffect(() => {
    requestAnimationFrame(() => {
      titleInputRef.current?.focus();
    });
  }, []);

  const handleSubmit = async () => {
    const normalizedTitle = title.trim();
    const normalizedDescription = description.trim();

    if (normalizedTitle.length < 2) {
      setTitleError("Min 2 characters.");
      return;
    }

    if (normalizedTitle.length > 64) {
      setTitleError("Max 64 characters.");
      return;
    }

    if (normalizedDescription.length > 512) {
      setTitleError(null);
      setDescriptionError("Max 512 characters.");
      return;
    }

    setTitleError(null);
    setDescriptionError(null);

    await onSubmit({
      title: normalizedTitle,
      description:
        normalizedDescription.length > 0 ? normalizedDescription : null,
      priority,
      untilDate: untilDate || null,
      assigneeId: assignee?.userId ?? null,
    });
  };

  return (
    <div className={styles.backdrop} onMouseDown={onClose}>
      <section
        className={styles.modal}
        role="dialog"
        aria-modal="true"
        aria-labelledby="create-task-title"
        onMouseDown={(event) => event.stopPropagation()}
      >
        <header className={styles.header}>
          <div>
            <span className={styles.eyebrow}>New task</span>
            <h2 id="create-task-title">Create task</h2>
            <p>In column “{columnTitle}”</p>
          </div>

          <button
            type="button"
            className={styles.closeButton}
            onClick={onClose}
            disabled={isSubmitting}
            aria-label="Close modal"
          >
            ×
          </button>
        </header>

        <div className={styles.form}>
          <label className={styles.field}>
            <span>Title</span>

            <input
              ref={titleInputRef}
              value={title}
              disabled={isSubmitting}
              maxLength={64}
              placeholder="Task title"
              onChange={(event) => setTitle(event.target.value)}
              onKeyDown={async (event) => {
                if (event.key === "Enter") {
                  event.preventDefault();
                  await handleSubmit();
                }

                if (event.key === "Escape") {
                  event.preventDefault();
                  onClose();
                }
              }}
            />
          </label>

          {titleError && <p className={styles.error}>{titleError}</p>}

          <label className={styles.field}>
            <span>Description</span>

            <textarea
              value={description}
              disabled={isSubmitting}
              maxLength={512}
              placeholder="Short task description"
              onChange={(event) => setDescription(event.target.value)}
            />
          </label>

          {descriptionError && (
            <p className={styles.error}>{descriptionError}</p>
          )}

          <div className={styles.formRow}>
            <label className={styles.field}>
              <span>Priority</span>

              <select
                value={priority}
                disabled={isSubmitting}
                onChange={(event) =>
                  setPriority(event.target.value as TaskPriority)
                }
              >
                {priorityOptions.map((priorityOption) => (
                  <option key={priorityOption} value={priorityOption}>
                    {priorityOption}
                  </option>
                ))}
              </select>
            </label>

            <label className={styles.field}>
              <span>Until date</span>

              <input
                type="date"
                value={untilDate}
                disabled={isSubmitting}
                onChange={(event) => setUntilDate(event.target.value)}
              />
            </label>
          </div>

          <TaskAssigneeSelector
            assignees={projectMembers}
            selectedAssignee={assignee}
            onChange={setAssignee}
            isLoading={areProjectMembersLoading}
            disabled={isSubmitting}
          />

          {submitError && <p className={styles.error}>{submitError}</p>}
        </div>

        <footer className={styles.actions}>
          <button
            type="button"
            className={styles.cancelButton}
            onClick={onClose}
            disabled={isSubmitting}
          >
            Cancel
          </button>

          <button
            type="button"
            className={styles.submitButton}
            onClick={handleSubmit}
            disabled={isSubmitting}
          >
            {isSubmitting ? "Creating..." : "Create task"}
          </button>
        </footer>
      </section>
    </div>
  );
};

export default CreateTaskModal;
