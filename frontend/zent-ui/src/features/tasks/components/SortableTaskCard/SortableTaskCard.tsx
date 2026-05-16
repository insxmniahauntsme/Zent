import type { CSSProperties } from "react";
import { useSortable } from "@dnd-kit/sortable";
import { CSS } from "@dnd-kit/utilities";

import type { BoardTaskDto } from "@/features/boards/model/types";
import TaskCard from "@/features/tasks/components/TaskCard/TaskCard";

import styles from "./SortableTaskCard.module.css";

interface SortableTaskCardProps {
  task: BoardTaskDto;
  columnId: string;
  isCompleted: boolean;
  onClick?: () => void;
}

const SortableTaskCard = ({
  task,
  columnId,
  isCompleted,
  onClick,
}: SortableTaskCardProps) => {
  const {
    attributes,
    listeners,
    setNodeRef,
    transform,
    transition,
    isDragging,
  } = useSortable({
    id: task.id,
    data: {
      type: "task",
      taskId: task.id,
      columnId,
      task,
    },
  });

  const style: CSSProperties = {
    transform: CSS.Transform.toString(transform),
    transition,
  };

  return (
    <div
      ref={setNodeRef}
      style={style}
      data-task-id={task.id}
      className={`${styles.sortableTask} ${
        isDragging ? styles.sortableTaskDragging : ""
      }`}
      {...attributes}
      {...listeners}
    >
      <TaskCard task={task} isCompleted={isCompleted} onClick={onClick} />
    </div>
  );
};

export default SortableTaskCard;
