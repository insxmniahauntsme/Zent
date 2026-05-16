import styles from "./TeamsEmptyState.module.css";
import emptyTeamsIllustration from "@/assets/images/teams-empty.svg";
import plusCircleIcon from "@/assets/icons/plus-circle-icon.svg";

interface TeamsEmptyStateProps {
  onCreateTeam: () => void;
}

const TeamsEmptyState = ({ onCreateTeam }: TeamsEmptyStateProps) => {
  return (
    <section className={styles.emptyState}>
      <img
        src={emptyTeamsIllustration}
        alt=""
        className={styles.illustration}
      />

      <div className={styles.content}>
        <h1 className={styles.title}>
          You&apos;re not part of any
          <br />
          team yet
        </h1>

        <p className={styles.description}>
          Create a new workspace to start managing your projects and
          collaborating with your colleagues.
        </p>

        <button
          type="button"
          className={styles.createButton}
          onClick={onCreateTeam}
        >
          <img src={plusCircleIcon} alt="" className={styles.buttonIcon} />
          <span>Create Team</span>
        </button>
      </div>
    </section>
  );
};

export default TeamsEmptyState;
