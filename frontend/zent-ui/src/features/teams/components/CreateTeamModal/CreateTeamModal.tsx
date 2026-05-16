import { useState } from "react";
import styles from "./CreateTeamModal.module.css";
import type { TeamMemberRoleEntry } from "../../model/types";
import closeIcon from "@/assets/icons/close-icon.svg";
import MemberSelector, {
  type SelectedTeamMember,
} from "../TeamMemberSelector/TeamMemberSelector";

interface CreateTeamModalProps {
  onClose: () => void;
  onSubmit: (data: { name: string; members: TeamMemberRoleEntry[] }) => void;
  isSubmitting?: boolean;
  submitError?: string | null;
}

const CreateTeamModal = ({
  onClose,
  onSubmit,
  isSubmitting = false,
  submitError,
}: CreateTeamModalProps) => {
  const [name, setName] = useState("");
  const [members, setMembers] = useState<SelectedTeamMember[]>([]);

  const isTouched = name.length > 0;
  const isValid = name.trim().length >= 2;

  const handleSubmit = (event: React.FormEvent) => {
    event.preventDefault();

    if (!name.trim()) return;

    onSubmit({
      name: name.trim(),
      members: members.map((member) => ({
        userId: member.userId,
        role: member.role,
      })),
    });
  };

  return (
    <div className={styles.backdrop}>
      <div className={styles.modal}>
        <div className={styles.header}>
          <div>
            <h2>Create team</h2>
            <p>Create a workspace for projects, boards and tasks.</p>
          </div>

          <button
            type="button"
            className={styles.closeButton}
            onClick={onClose}
          >
            <img src={closeIcon} alt="" />
          </button>
        </div>

        <form className={styles.form} onSubmit={handleSubmit}>
          <div className={styles.field}>
            <label htmlFor="teamName">Team name</label>
            <input
              id="teamName"
              type="text"
              placeholder="e.g. Design Team"
              value={name}
              onChange={(event) => setName(event.target.value)}
            />
            {isTouched && !isValid && (
              <span className={styles.error}>
                Team name must be at least 2 characters
              </span>
            )}
          </div>

          <MemberSelector selectedMembers={members} onChange={setMembers} />

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
              {isSubmitting ? "Creating..." : "Create team"}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default CreateTeamModal;
