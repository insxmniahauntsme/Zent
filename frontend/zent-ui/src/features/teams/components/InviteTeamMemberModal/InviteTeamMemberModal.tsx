import { useState } from "react";

import type { TeamMemberDto } from "@/features/teams/model/types";
import InviteTeamMemberSelector, {
  type SelectedInviteMember,
} from "../InviteTeamMemberSelector/InviteTeamMemberSelector";

import styles from "./InviteTeamMemberModal.module.css";

interface InviteTeamMemberModalProps {
  teamId: string;
  currentMembers: TeamMemberDto[];
  isSubmitting: boolean;
  submitError: string | null;
  onClose: () => void;
  onSubmit: (members: SelectedInviteMember[]) => Promise<void>;
}

const InviteTeamMemberModal = ({
  currentMembers,
  isSubmitting,
  submitError,
  onClose,
  onSubmit,
}: InviteTeamMemberModalProps) => {
  const [selectedMembers, setSelectedMembers] = useState<
    SelectedInviteMember[]
  >([]);

  const canSubmit = selectedMembers.length > 0 && !isSubmitting;

  return (
    <div className={styles.overlay}>
      <div className={styles.modal}>
        <div className={styles.header}>
          <div>
            <span className={styles.eyebrow}>Invite members</span>
            <h2>Add people to team</h2>
            <p>Search users and choose their role in this team.</p>
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

        <InviteTeamMemberSelector
          currentMembers={currentMembers}
          selectedMembers={selectedMembers}
          onChange={setSelectedMembers}
        />

        {submitError && <p className={styles.error}>{submitError}</p>}

        <div className={styles.footer}>
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
            disabled={!canSubmit}
            onClick={() => onSubmit(selectedMembers)}
          >
            {isSubmitting ? "Adding..." : "Add members"}
          </button>
        </div>
      </div>
    </div>
  );
};

export default InviteTeamMemberModal;
