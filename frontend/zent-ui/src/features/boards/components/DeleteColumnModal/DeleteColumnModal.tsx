// features/boards/components/DeleteColumnModal/DeleteColumnModal.tsx

import styles from "./DeleteColumnModal.module.css";

interface DeleteColumnModalProps {
  columnTitle: string;
  tasksCount: number;
  isDeleting: boolean;
  onClose: () => void;
  onConfirm: () => Promise<void>;
}

const DeleteColumnModal = ({
  columnTitle,
  tasksCount,
  isDeleting,
  onClose,
  onConfirm,
}: DeleteColumnModalProps) => {
  return (
    <div className={styles.backdrop} onMouseDown={onClose}>
      <section
        className={styles.modal}
        role="dialog"
        aria-modal="true"
        aria-labelledby="delete-column-title"
        onMouseDown={(event) => event.stopPropagation()}
      >
        <div className={styles.iconWrap}>
          <span>!</span>
        </div>

        <div className={styles.content}>
          <h2 id="delete-column-title">Delete column?</h2>

          <p>
            Are you sure you want to delete{" "}
            <strong>&quot;{columnTitle}&quot;</strong>?
          </p>

          {tasksCount > 0 && (
            <p className={styles.warning}>
              This column contains {tasksCount}{" "}
              {tasksCount === 1 ? "task" : "tasks"}. They will also be deleted.
            </p>
          )}
        </div>

        <div className={styles.actions}>
          <button
            type="button"
            className={styles.cancelButton}
            onClick={onClose}
            disabled={isDeleting}
          >
            Cancel
          </button>

          <button
            type="button"
            className={styles.deleteButton}
            onClick={onConfirm}
            disabled={isDeleting}
          >
            {isDeleting ? "Deleting..." : "Delete column"}
          </button>
        </div>
      </section>
    </div>
  );
};

export default DeleteColumnModal;
