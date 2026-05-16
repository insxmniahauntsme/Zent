import type { BoardTaskDto } from "@/features/boards/model/types";
import type { TaskPriority } from "@/features/tasks/model/types";

import styles from "./TaskCard.module.css";

interface TaskCardProps {
  task: BoardTaskDto;
  isCompleted: boolean;
  onClick?: () => void;
}

type DueStatus = "none" | "safe" | "soon" | "urgent" | "overdue";

const priorityLabels: Record<TaskPriority, string> = {
  Low: "Low",
  Medium: "Medium",
  High: "High",
  Critical: "Critical",
};

const priorityClassNames: Record<TaskPriority, string> = {
  Low: styles.priorityLow,
  Medium: styles.priorityMedium,
  High: styles.priorityHigh,
  Critical: styles.priorityCritical,
};

const accentClassNames: Record<TaskPriority, string> = {
  Low: styles.accentLow,
  Medium: styles.accentMedium,
  High: styles.accentHigh,
  Critical: styles.accentCritical,
};

const dueClassNames: Record<DueStatus, string> = {
  none: styles.dueNone,
  safe: styles.dueSafe,
  soon: styles.dueSoon,
  urgent: styles.dueUrgent,
  overdue: styles.dueOverdue,
};

const isTaskPriority = (value: unknown): value is TaskPriority => {
  return (
    value === "Low" ||
    value === "Medium" ||
    value === "High" ||
    value === "Critical"
  );
};

const getInitials = (firstName?: string, lastName?: string) => {
  const first = firstName?.trim()[0] ?? "";
  const last = lastName?.trim()[0] ?? "";

  const initials = `${first}${last}`.trim();

  return initials.length > 0 ? initials.toUpperCase() : "?";
};

const getFullName = (assignee: NonNullable<BoardTaskDto["assignee"]>) => {
  return `${assignee.firstName} ${assignee.lastName}`.trim();
};

const TaskCard = ({ task, isCompleted, onClick }: TaskCardProps) => {
  const priority = isTaskPriority(task.priority) ? task.priority : "Medium";
  const dueInfo = getDueInfo(task.untilDate);

  const assignee = task.assignee;

  return (
    <article
      className={`${styles.taskCard} ${
        isCompleted ? styles.taskCardCompleted : ""
      }`}
      onClick={onClick}
      role={onClick ? "button" : undefined}
      tabIndex={onClick ? 0 : undefined}
      onKeyDown={(event) => {
        if (!onClick) {
          return;
        }

        if (event.key === "Enter" || event.key === " ") {
          event.preventDefault();
          onClick();
        }
      }}
    >
      <span
        className={`${styles.priorityAccent} ${
          isCompleted ? styles.accentCompleted : accentClassNames[priority]
        }`}
      />

      <div className={styles.taskTopRow}>
        <span
          className={`${styles.priorityBadge} ${
            isCompleted
              ? styles.priorityCompleted
              : priorityClassNames[priority]
          }`}
        >
          {isCompleted ? "Completed" : priorityLabels[priority]}
        </span>

        {task.untilDate && (
          <div className={`${styles.dueDate} ${dueClassNames[dueInfo.status]}`}>
            <span className={styles.clockIcon} aria-hidden="true" />
            <span>{formatDate(task.untilDate)}</span>
          </div>
        )}
      </div>

      <h3 className={styles.taskTitle}>{task.title}</h3>

      {task.description && (
        <p className={styles.taskDescription}>{task.description}</p>
      )}

      <div className={styles.taskFooter}>
        <span className={styles.daysLeft}>{dueInfo.label}</span>

        {assignee ? (
          <div
            className={styles.assigneeAvatar}
            title={`${getFullName(assignee)} · ${assignee.email}`}
            aria-label={`Assigned to ${getFullName(assignee)}`}
          >
            {getInitials(assignee.firstName, assignee.lastName)}
          </div>
        ) : (
          <div
            className={`${styles.assigneeAvatar} ${styles.assigneeAvatarEmpty}`}
            title="Unassigned"
            aria-label="Unassigned"
          >
            ?
          </div>
        )}
      </div>
    </article>
  );
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
      label: "Today",
    };
  }

  if (diffDays === 1) {
    return {
      status: "urgent",
      label: "Tomorrow",
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
  }).format(parseDateOnly(value));
};

export default TaskCard;
