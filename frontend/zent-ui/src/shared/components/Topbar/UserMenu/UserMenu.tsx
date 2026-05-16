import styles from "./UserMenu.module.css";

interface UserMenuProps {
  firstName: string;
  lastName: string;
  email: string;
  onProfileClick?: () => void;
  onSettingsClick?: () => void;
  onLogoutClick: () => void;
}

const UserMenu = ({
  firstName,
  lastName,
  email,
  onProfileClick,
  onSettingsClick,
  onLogoutClick,
}: UserMenuProps) => {
  return (
    <div className={styles.menu}>
      <div className={styles.userPreview}>
        <div className={styles.avatar}>
          {firstName[0]}
          {lastName[0]}
        </div>

        <div className={styles.userInfo}>
          <span className={styles.userName}>
            {firstName} {lastName}
          </span>
          <span className={styles.userEmail}>{email}</span>
        </div>
      </div>

      <div className={styles.divider} />

      <button type="button" className={styles.item} onClick={onProfileClick}>
        Profile
      </button>

      <button type="button" className={styles.item} onClick={onSettingsClick}>
        Account settings
      </button>

      <div className={styles.divider} />

      <button
        type="button"
        className={`${styles.item} ${styles.dangerItem}`}
        onClick={onLogoutClick}
      >
        Log out
      </button>
    </div>
  );
};

export default UserMenu;
