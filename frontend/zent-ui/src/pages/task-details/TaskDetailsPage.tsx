import { useMemo } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { useQuery } from "@tanstack/react-query";

import { tasksApi } from "@/features/tasks/api/tasksApi";
import type { TaskPriority } from "@/features/tasks/model/types";

import styles from "./TaskDetailsPage.module.css";
import { useAddTaskAssignee } from "@/features/tasks/hooks/useAddTaskAssignee";
import { useRemoveTaskAssignee } from "@/features/tasks/hooks/useRemoveTaskAssignee";
import TaskAssigneeControl from "@/features/tasks/components/TaskAssigneeControl/TaskAssigneeControl";
import { useProjectMembers } from "@/features/projects/hooks/useProjectMembers";

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

const dueClassNames: Record<DueStatus, string> = {
  none: styles.dueNone,
  safe: styles.dueSafe,
  soon: styles.dueSoon,
  urgent: styles.dueUrgent,
  overdue: styles.dueOverdue,
};

const TaskDetailsPage = () => {
  const navigate = useNavigate();
  const { taskId } = useParams<{ taskId: string }>();

  const addTaskAssigneeMutation = useAddTaskAssignee();
  const removeTaskAssigneeMutation = useRemoveTaskAssignee();

  const {
    data: task,
    isLoading,
    isError,
  } = useQuery({
    queryKey: ["task", taskId],
    queryFn: () => tasksApi.getTaskDetails(taskId!),
    enabled: Boolean(taskId),
  });

  const { data: projectMembers = [], isLoading: areProjectMembersLoading } =
    useProjectMembers(task?.projectId);

  const dueInfo = useMemo(() => {
    return getDueInfo(task?.untilDate);
  }, [task?.untilDate]);

  const isAssigneeSaving =
    addTaskAssigneeMutation.isPending || removeTaskAssigneeMutation.isPending;

  if (isLoading) {
    return (
      <main className={styles.page}>
        <div className={styles.loadingState}>
          <div className={styles.loadingDot} />
          <span>Loading task...</span>
        </div>
      </main>
    );
  }

  if (isError || !task) {
    return (
      <main className={styles.page}>
        <div className={styles.errorState}>
          <h1>Task not found</h1>
          <p>Something went wrong while loading this task.</p>

          <button
            type="button"
            className={styles.backButton}
            onClick={() => navigate(-1)}
          >
            ← Back
          </button>
        </div>
      </main>
    );
  }

  const priority = isTaskPriority(task.priority) ? task.priority : "Medium";

  return (
    <main className={styles.page}>
      <button
        type="button"
        className={styles.backButton}
        onClick={() => navigate(-1)}
      >
        ← Back to board
      </button>

      <div className={styles.shell}>
        <div className={styles.main}>
          <section className={styles.hero}>
            <div className={styles.breadcrumbs}>
              <span>{task.projectName}</span>
              <span>/</span>
              <span>{task.boardName}</span>
              <span>/</span>
              <span>{task.columnTitle}</span>
            </div>

            <h1>{task.title}</h1>

            <div className={styles.heroMeta}>
              <span
                className={`${styles.priorityBadge} ${
                  priorityClassNames[priority]
                }`}
              >
                {priorityLabels[priority]}
              </span>

              <span
                className={`${styles.duePill} ${dueClassNames[dueInfo.status]}`}
              >
                {task.untilDate
                  ? formatFullDate(task.untilDate)
                  : "No deadline"}
              </span>
            </div>
          </section>

          <section className={styles.section}>
            <div className={styles.sectionTitleRow}>
              <h2>Description</h2>
            </div>

            {task.description ? (
              <p className={styles.description}>{task.description}</p>
            ) : (
              <div className={styles.emptyBlock}>
                <strong>No description yet</strong>
                <span>Add task details, requirements or notes here later.</span>
              </div>
            )}
          </section>

          <section className={styles.section}>
            <div className={styles.sectionTitleRow}>
              <h2>Attachments</h2>
              <span className={styles.soonBadge}>Coming soon</span>
            </div>

            <div className={styles.emptyBlock}>
              <strong>No attachments</strong>
              <span>Files, screenshots and documents will appear here.</span>
            </div>
          </section>

          <section className={styles.section}>
            <div className={styles.sectionTitleRow}>
              <h2>Comments</h2>
              <span className={styles.soonBadge}>Coming soon</span>
            </div>

            <div className={styles.commentPlaceholder}>
              <div className={styles.commentAvatar}>VR</div>

              <div className={styles.commentInput}>Write a comment...</div>
            </div>
          </section>
        </div>

        <aside className={styles.sidebar}>
          <section className={styles.propertiesPanel}>
            <h2>Task info</h2>

            <InfoRow label="Status" value={task.columnTitle} />
            <InfoRow label="Priority" value={priorityLabels[priority]} />
            <InfoRow label="Due" value={dueInfo.label} />
            <InfoRow label="Created" value={formatFullDate(task.createdAt)} />
          </section>

          <section className={styles.propertiesPanel}>
            <h2>Assignee</h2>

            <TaskAssigneeControl
              assignee={task.assignee}
              members={projectMembers}
              isLoadingMembers={areProjectMembersLoading}
              isSaving={isAssigneeSaving}
              onAssign={async (assigneeId) => {
                await addTaskAssigneeMutation.mutateAsync({
                  boardId: task.boardId,
                  taskId: task.id,
                  assigneeId,
                });
              }}
              onRemove={async () => {
                await removeTaskAssigneeMutation.mutateAsync({
                  boardId: task.boardId,
                  taskId: task.id,
                });
              }}
            />
          </section>
        </aside>
      </div>
    </main>
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

const isTaskPriority = (value: unknown): value is TaskPriority => {
  return (
    value === "Low" ||
    value === "Medium" ||
    value === "High" ||
    value === "Critical"
  );
};

const parseDateOnly = (value: string) => {
  const normalizedValue = value.includes("T") ? value.split("T")[0] : value;
  const [year, month, day] = normalizedValue.split("-").map(Number);

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

const formatFullDate = (value: string) => {
  const date = value.includes("T") ? new Date(value) : parseDateOnly(value);

  return new Intl.DateTimeFormat("en", {
    month: "short",
    day: "numeric",
    year: "numeric",
  }).format(date);
};

export default TaskDetailsPage;
