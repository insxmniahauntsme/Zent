import type { CSSProperties, ReactNode } from "react";
import type { DraggableAttributes } from "@dnd-kit/core";
import type { SyntheticListenerMap } from "@dnd-kit/core/dist/hooks/utilities";

import { useSortable } from "@dnd-kit/sortable";
import { CSS } from "@dnd-kit/utilities";

import styles from "../Board/Board.module.css";

interface SortableBoardColumnProps {
  columnId: string;
  isDragging?: boolean;
  children: (dragHandleProps: {
    attributes: DraggableAttributes;
    listeners: SyntheticListenerMap | undefined;
  }) => ReactNode;
}

const SortableBoardColumn = ({
  columnId,
  isDragging = false,
  children,
}: SortableBoardColumnProps) => {
  const { attributes, listeners, setNodeRef, transform, transition } =
    useSortable({
      id: columnId,
      data: {
        type: "column",
        columnId,
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
      className={`${styles.sortableColumn} ${
        isDragging ? styles.sortableColumnDragging : ""
      }`}
    >
      {children({ attributes, listeners })}
    </div>
  );
};

export default SortableBoardColumn;
