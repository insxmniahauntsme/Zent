import styles from "./ProjectHeader.module.css";

interface ProjectHeaderProps {
  name: string;
  description?: string;
  client?: string;
  membersCount: number;
  boardsCount: number;
  updatedAtLabel: string;
}

const ProjectHeader = ({
  name,
  description,
  client,
  membersCount,
  boardsCount,
  updatedAtLabel,
}: ProjectHeaderProps) => {
  return (
    <header className={styles.projectHeader}>
      <div>
        <div className={styles.titleRow}>
          <h1 className={styles.title}>{name}</h1>
          {client && <span className={styles.clientBadge}>{client}</span>}
        </div>

        {description && <p className={styles.description}>{description}</p>}

        <div className={styles.meta}>
          <span>{membersCount} members</span>
          <span>{boardsCount} boards</span>
          <span>{updatedAtLabel}</span>
        </div>
      </div>

      <button type="button" className={styles.secondaryButton}>
        Project settings
      </button>
    </header>
  );
};

export default ProjectHeader;
