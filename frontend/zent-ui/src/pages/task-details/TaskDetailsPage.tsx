import { useNavigate, useParams } from "react-router-dom";

import { useTaskDetails } from "@/features/tasks/hooks/useTaskDetails";

import styles from "./TaskDetailsPage.module.css";

type DueStatus = "none" | "safe" | "soon" | "urgent" | "overdue";

const priorityClassNames = {
  Low: styles.priorityLow,
  Medium: styles.priorityMedium,
  High: styles.priorityHigh,
  Critical: styles.priorityCritical,
};

const dueClassNames: Record<DueStatus, string> = {
  none: styles.dueNone,
  safe: styles.dueSafe,
  soon: styles.dueSoon,
  urgent: styles.dueUrgent,
  overdue: styles.dueOverdue,
};

const TaskDetailsPage = () => {
  const { taskId } = useParams<{ taskId: string }>();
  const navigate = useNavigate();

  const { data: task, isLoading, isError } = useTaskDetails(taskId);

  if (isLoading) {
    return <section className={styles.page}>Loading task...</section>;
  }

  if (isError || !task) {
    return <section className={styles.page}>Failed to load task.</section>;
  }

  const dueInfo = getDueInfo(task.untilDate);

  const assigneeName = task.assignee
    ? `${task.assignee.firstName} ${task.assignee.lastName}`.trim()
    : "Unassigned";

  return (
    <section className={styles.page}>
      <button
        type="button"
        className={styles.backButton}
        onClick={() => navigate(-1)}
      >
        ← Back
      </button>

      <div className={styles.layout}>
        <main className={styles.main}>
          <div className={styles.titleBlock}>
            <div className={styles.breadcrumbs}>
              <span>{task.projectName}</span>
              <span>/</span>
              <span>{task.boardName}</span>
              <span>/</span>
              <span>{task.columnTitle}</span>
            </div>

            <h1>{task.title}</h1>

            <div className={styles.metaLine}>
              <span
                className={`${styles.priorityBadge} ${
                  priorityClassNames[task.priority]
                }`}
              >
                {task.priority}
              </span>

              <span
                className={`${styles.dueDate} ${dueClassNames[dueInfo.status]}`}
              >
                {task.untilDate ? formatDate(task.untilDate) : "No deadline"}
              </span>
            </div>
          </div>

          <section className={styles.contentCard}>
            <h2>Description</h2>

            {task.description ? (
              <p className={styles.description}>{task.description}</p>
            ) : (
              <p className={styles.emptyText}>No description yet.</p>
            )}
          </section>

          <section className={styles.contentCard}>
            <div className={styles.sectionHeader}>
              <h2>Attachments</h2>
              <span>Coming soon</span>
            </div>

            <p className={styles.emptyText}>
              Attachments will appear here after we add file upload logic.
            </p>
          </section>

          <section className={styles.contentCard}>
            <div className={styles.sectionHeader}>
              <h2>Comments</h2>
              <span>Coming soon</span>
            </div>

            <p className={styles.emptyText}>
              Comments will appear here after we add task discussion logic.
            </p>
          </section>
        </main>

        <aside className={styles.sidebar}>
          <section className={styles.sideCard}>
            <h2>Task info</h2>

            <InfoRow label="Status" value={task.columnTitle} />
            <InfoRow label="Assignee" value={assigneeName} />
            <InfoRow label="Due" value={dueInfo.label} />
            <InfoRow label="Created" value={formatDateTime(task.createdAt)} />
          </section>

          <section className={styles.sideCard}>
            <h2>Assignee</h2>

            {task.assignee ? (
              <div className={styles.assignee}>
                <div className={styles.avatar}>
                  {getInitials(task.assignee.firstName, task.assignee.lastName)}
                </div>

                <div>
                  <strong>{assigneeName}</strong>
                  <span>{task.assignee.email}</span>
                </div>
              </div>
            ) : (
              <p className={styles.emptyText}>Nobody assigned.</p>
            )}
          </section>
        </aside>
      </div>
    </section>
  );
};

interface InfoRowProps {
  label: string;
  value: string;
}

const InfoRow = ({ label, value }: InfoRowProps) => {
  return (
    <div className={styles.infoRow}>
      <span>{label}</span>
      <strong>{value}</strong>
    </div>
  );
};

const getInitials = (firstName?: string, lastName?: string) => {
  const first = firstName?.trim()[0] ?? "";
  const last = lastName?.trim()[0] ?? "";

  const initials = `${first}${last}`.trim();

  return initials.length > 0 ? initials.toUpperCase() : "?";
};

const parseDateOnly = (value: string) => {
  const [year, month, day] = value.split("-").map(Number);
  return new Date(year, month - 1, day);
};

const startOfToday = () => {
  const now = new Date();
  return new Date(now.getFullYear(), now.getMonth(), now.getDate());
};

const getDueInfo = (
  value: string | null | undefined,
): { status: DueStatus; label: string } => {
  if (!value) {
    return {
      status: "none",
      label: "No deadline",
    };
  }

  const dueDate = parseDateOnly(value);
  const today = startOfToday();

  const diffMs = dueDate.getTime() - today.getTime();
  const diffDays = Math.round(diffMs / 86_400_000);

  if (diffDays < 0) {
    return {
      status: "overdue",
      label: `${Math.abs(diffDays)}d overdue`,
    };
  }

  if (diffDays === 0) {
    return {
      status: "urgent",
      label: "Due today",
    };
  }

  if (diffDays === 1) {
    return {
      status: "urgent",
      label: "Due tomorrow",
    };
  }

  if (diffDays < 4) {
    return {
      status: "soon",
      label: `${diffDays} days left`,
    };
  }

  return {
    status: "safe",
    label: `${diffDays} days left`,
  };
};

const formatDate = (value: string) => {
  return new Intl.DateTimeFormat("en", {
    month: "short",
    day: "numeric",
    year: "numeric",
  }).format(parseDateOnly(value));
};

const formatDateTime = (value: string) => {
  return new Intl.DateTimeFormat("en", {
    month: "short",
    day: "numeric",
    year: "numeric",
  }).format(new Date(value));
};

export default TaskDetailsPage;
