import { useState } from "react";
import styles from "./CreateBoardModal.module.css";

interface CreateBoardModalProps {
  isSubmitting: boolean;
  submitError: string | null;
  onClose: () => void;
  onSubmit: (data: {
    name: string;
    description?: string | null;
  }) => Promise<void>;
}

const CreateBoardModal = ({
  isSubmitting,
  submitError,
  onClose,
  onSubmit,
}: CreateBoardModalProps) => {
  const [name, setName] = useState("");
  const [description, setDescription] = useState("");
  const [localError, setLocalError] = useState<string | null>(null);

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    const normalizedName = name.trim();
    const normalizedDescription = description.trim();

    if (normalizedName.length < 2) {
      setLocalError("Board name must contain at least 2 characters.");
      return;
    }

    if (normalizedName.length > 40) {
      setLocalError("Board name must be less than 40 characters.");
      return;
    }

    if (normalizedDescription.length > 128) {
      setLocalError("Description must be less than 128 characters.");
      return;
    }

    setLocalError(null);

    await onSubmit({
      name: normalizedName,
      description: normalizedDescription || null,
    });
  };

  return (
    <div className={styles.backdrop}>
      <div className={styles.modal}>
        <div className={styles.header}>
          <div>
            <h2>Create board</h2>
            <p>Add a new workflow board to this project.</p>
          </div>

          <button
            type="button"
            className={styles.closeButton}
            onClick={onClose}
            disabled={isSubmitting}
          >
            ×
          </button>
        </div>

        <form className={styles.form} onSubmit={handleSubmit}>
          <label className={styles.field}>
            <span>Board name</span>
            <input
              type="text"
              placeholder="Development board"
              value={name}
              onChange={(event) => setName(event.target.value)}
              disabled={isSubmitting}
            />
          </label>

          <label className={styles.field}>
            <span>Description</span>
            <textarea
              placeholder="Optional short description..."
              value={description}
              onChange={(event) => setDescription(event.target.value)}
              disabled={isSubmitting}
            />
          </label>

          {(localError || submitError) && (
            <p className={styles.error}>{localError || submitError}</p>
          )}

          <div className={styles.actions}>
            <button
              type="button"
              className={styles.cancelButton}
              onClick={onClose}
              disabled={isSubmitting}
            >
              Cancel
            </button>

            <button
              type="submit"
              className={styles.submitButton}
              disabled={isSubmitting}
            >
              {isSubmitting ? "Creating..." : "Create board"}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default CreateBoardModal;
