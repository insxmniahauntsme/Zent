import { useState, type ReactNode } from "react";
import { NavLink } from "react-router-dom";
import styles from "./Sidebar.module.css";

export interface SidebarItem {
  label: string;
  to: string;
  icon?: string;
  end?: boolean;
}

export interface SidebarBottomAction {
  label: string;
  onClick: () => void;
}

interface SidebarProps {
  items: SidebarItem[];
  header?: ReactNode;
  bottomAction?: SidebarBottomAction;
}

const MIN_WIDTH = 220;
const MAX_WIDTH = 360;
const DEFAULT_WIDTH = 260;

const Sidebar = ({ items, header, bottomAction }: SidebarProps) => {
  const [width, setWidth] = useState(DEFAULT_WIDTH);
  const [isResizing, setIsResizing] = useState(false);

  const startResize = (event: React.MouseEvent<HTMLDivElement>) => {
    event.preventDefault();

    setIsResizing(true);

    const startX = event.clientX;
    const startWidth = width;

    document.body.style.userSelect = "none";
    document.body.style.cursor = "col-resize";

    const handleMouseMove = (moveEvent: MouseEvent) => {
      const nextWidth = startWidth + (moveEvent.clientX - startX);

      if (nextWidth < MIN_WIDTH || nextWidth > MAX_WIDTH) return;

      setWidth(nextWidth);
    };

    const handleMouseUp = () => {
      setIsResizing(false);

      document.body.style.userSelect = "";
      document.body.style.cursor = "";

      document.removeEventListener("mousemove", handleMouseMove);
      document.removeEventListener("mouseup", handleMouseUp);
    };

    document.addEventListener("mousemove", handleMouseMove);
    document.addEventListener("mouseup", handleMouseUp);
  };

  return (
    <aside className={styles.sidebar} style={{ width }}>
      <div className={styles.content}>
        <div className={styles.logo}>Zent</div>

        {header && <div className={styles.header}>{header}</div>}

        <nav className={styles.nav}>
          {items.map((item) => (
            <NavLink
              key={item.to}
              to={item.to}
              end={item.end}
              className={({ isActive }) =>
                `${styles.navItem} ${isActive ? styles.active : ""}`
              }
            >
              {item.icon && (
                <img src={item.icon} className={styles.icon} alt="" />
              )}
              <span>{item.label}</span>
            </NavLink>
          ))}
        </nav>

        {bottomAction && (
          <button
            type="button"
            className={styles.createTaskButton}
            onClick={bottomAction.onClick}
          >
            {bottomAction.label}
          </button>
        )}
      </div>

      <div
        className={`${styles.resizer} ${
          isResizing ? styles.resizerActive : ""
        }`}
        onMouseDown={startResize}
      />
    </aside>
  );
};

export default Sidebar;
