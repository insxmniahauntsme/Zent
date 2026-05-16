import { useState } from "react";
import styles from "./BoardNavigation.module.css";
import chevronDownIcon from "@/assets/icons/chevron-down.svg";

interface BoardItem {
  id: string;
  name: string;
}

interface BoardNavigationProps {
  boards: BoardItem[];
  activeBoardId: string;
  onBoardChange: (boardId: string) => void;
  onCreateBoard: () => void;
}

const SHOULD_USE_SWITCHER_LIMIT = 4;

const BoardNavigation = ({
  boards,
  activeBoardId,
  onBoardChange,
  onCreateBoard,
}: BoardNavigationProps) => {
  const [isBoardMenuOpen, setIsBoardMenuOpen] = useState(false);

  const activeBoard = boards.find((board) => board.id === activeBoardId);
  const shouldUseSwitcher = boards.length > SHOULD_USE_SWITCHER_LIMIT;

  return (
    <div className={styles.boardNavigation}>
      {shouldUseSwitcher ? (
        <div className={styles.boardSwitcher}>
          <button
            type="button"
            className={styles.boardSwitcherButton}
            onClick={() => setIsBoardMenuOpen((prev) => !prev)}
          >
            <span>{activeBoard?.name}</span>
            <img
              src={chevronDownIcon}
              alt=""
              className={`${styles.chevronIcon} ${
                isBoardMenuOpen ? styles.chevronIconOpen : ""
              }`}
            />
          </button>

          {isBoardMenuOpen && (
            <div className={styles.boardMenu}>
              {boards.map((board) => (
                <button
                  key={board.id}
                  type="button"
                  className={`${styles.boardMenuItem} ${
                    board.id === activeBoardId ? styles.activeBoardMenuItem : ""
                  }`}
                  onClick={() => {
                    onBoardChange(board.id);
                    setIsBoardMenuOpen(false);
                  }}
                >
                  {board.name}
                </button>
              ))}
            </div>
          )}
        </div>
      ) : (
        <div className={styles.tabsList}>
          {boards.map((board) => (
            <button
              key={board.id}
              type="button"
              className={`${styles.boardTab} ${
                board.id === activeBoardId ? styles.activeBoardTab : ""
              }`}
              onClick={() => onBoardChange(board.id)}
            >
              {board.name}
            </button>
          ))}
        </div>
      )}

      <button
        type="button"
        className={styles.newBoardButton}
        onClick={onCreateBoard}
      >
        +
      </button>
    </div>
  );
};

export default BoardNavigation;
