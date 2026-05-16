import styles from "./AddColumnRail.module.css";

interface AddColumnRailProps {
  isExpanded: boolean;
  disabled?: boolean;
  onClick: () => void;
}

const AddColumnRail = ({
  isExpanded,
  disabled = false,
  onClick,
}: AddColumnRailProps) => {
  return (
    <button
      type="button"
      className={`${styles.rail} ${isExpanded ? styles.expanded : ""}`}
      onClick={onClick}
      disabled={disabled}
    >
      <span>{disabled ? "Creating..." : "+ Add column"}</span>
    </button>
  );
};

export default AddColumnRail;
