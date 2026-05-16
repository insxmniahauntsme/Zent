import { useState } from "react";
import styles from "./CreateProjectModal.module.css";
import ProjectMemberSelector, {
  type SelectedProjectMember,
} from "../ProjectMemberSelector/ProjectMemberSelector";

interface CreateProjectModalProps {
  teamId: string;
  onClose: () => void;
  onSubmit: (data: {
    name: string;
    description?: string;
    client?: string;
    members: string[];
  }) => void;
  isSubmitting?: boolean;
  submitError?: string | null;
}

const CreateProjectModal = ({
  teamId,
  onClose,
  onSubmit,
  isSubmitting = false,
  submitError,
}: CreateProjectModalProps) => {
  const [name, setName] = useState("");
  const [description, setDescription] = useState("");
  const [client, setClient] = useState("");
  const [members, setMembers] = useState<SelectedProjectMember[]>([]);

  const isValid = name.trim().length >= 2;

  const handleSubmit = (event: React.FormEvent) => {
    event.preventDefault();

    if (!isValid) return;

    onSubmit({
      name: name.trim(),
      description: description.trim() || undefined,
      client: client.trim() || undefined,
      members: members.map((member) => member.userId),
    });
  };

  return (
    <div className={styles.backdrop}>
      <div className={styles.modal}>
        <div className={styles.header}>
          <div>
            <h2>Create project</h2>
            <p>Add a new project inside this team workspace.</p>
          </div>

          <button
            type="button"
            className={styles.closeButton}
            onClick={onClose}
          >
            ×
          </button>
        </div>

        <form className={styles.form} onSubmit={handleSubmit}>
          <div className={styles.field}>
            <label htmlFor="projectName">Project name</label>
            <input
              id="projectName"
              type="text"
              placeholder="e.g. Zent Platform"
              maxLength={20}
              value={name}
              onChange={(event) => setName(event.target.value)}
            />

            {name.length > 0 && !isValid && (
              <span className={styles.error}>
                Project name must be at least 2 characters.
              </span>
            )}
          </div>

          <div className={styles.field}>
            <label htmlFor="projectDescription">Description</label>
            <textarea
              id="projectDescription"
              placeholder="Shortly describe what this project is about..."
              maxLength={128}
              value={description}
              onChange={(event) => setDescription(event.target.value)}
            />
          </div>

          <div className={styles.field}>
            <label htmlFor="projectClient">Client</label>
            <input
              id="projectClient"
              type="text"
              placeholder="e.g. TNTU"
              maxLength={32}
              value={client}
              onChange={(event) => setClient(event.target.value)}
            />
          </div>

          <ProjectMemberSelector
            teamId={teamId}
            selectedMembers={members}
            onChange={setMembers}
          />

          {submitError && (
            <div className={styles.submitError}>{submitError}</div>
          )}

          <div className={styles.actions}>
            <button
              type="button"
              className={styles.cancelButton}
              onClick={onClose}
            >
              Cancel
            </button>

            <button
              type="submit"
              className={styles.createButton}
              disabled={isSubmitting || !isValid}
            >
              {isSubmitting ? "Creating..." : "Create project"}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default CreateProjectModal;
