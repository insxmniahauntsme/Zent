import styles from "./Topbar.module.css";
import searchIcon from "@/assets/icons/search-icon.svg";
import faqIcon from "@/assets/icons/faq-icon.svg";
import notificationsIcon from "@/assets/icons/notifications-icon.svg";
import { useEffect, useRef, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useCurrentUser } from "@/features/users/hooks/useCurrentUser";
import UserMenu from "./UserMenu/UserMenu";

const Topbar = () => {
  const { data: user } = useCurrentUser();

  const [isUserMenuOpen, setIsUserMenuOpen] = useState(false);
  const userMenuRef = useRef<HTMLDivElement | null>(null);

  useEffect(() => {
    if (!isUserMenuOpen) return;

    const handleClickOutside = (event: MouseEvent) => {
      if (
        userMenuRef.current &&
        !userMenuRef.current.contains(event.target as Node)
      ) {
        setIsUserMenuOpen(false);
      }
    };

    document.addEventListener("mousedown", handleClickOutside);

    return () => {
      document.removeEventListener("mousedown", handleClickOutside);
    };
  }, [isUserMenuOpen]);

  const navigate = useNavigate();

  const handleLogout = () => {
    localStorage.removeItem("token");
    navigate("/login");
  };

  return (
    <header className={styles.topbar}>
      <div className={styles.left}>
        <span className={styles.breadcrumb}>
          Teams / <strong>Zent workspace</strong>
        </span>
      </div>

      <div className={styles.right}>
        <div className={styles.search}>
          <img src={searchIcon} className={styles.searchIcon} alt="" />
          <input type="text" placeholder="Search..." />
        </div>

        <div className={styles.actions}>
          <button type="button">
            <img src={faqIcon} className={styles.icon} alt="" />
          </button>

          <button type="button">
            <img src={notificationsIcon} className={styles.icon} alt="" />
          </button>
        </div>

        <div className={styles.userMenuWrapper} ref={userMenuRef}>
          <button
            type="button"
            className={styles.userBlock}
            onClick={() => setIsUserMenuOpen((prev) => !prev)}
          >
            <div className={styles.userInfo}>
              <span className={styles.userName}>
                {user ? `${user.firstName} ${user.lastName}` : "Loading..."}
              </span>
              <span className={styles.userEmail}>{user?.email ?? ""}</span>
            </div>

            <div className={styles.avatar}>
              {user ? `${user.firstName[0]}${user.lastName[0]}` : ""}
            </div>
          </button>

          {isUserMenuOpen && user && (
            <UserMenu
              firstName={user.firstName}
              lastName={user.lastName}
              email={user.email}
              onProfileClick={() => {}}
              onSettingsClick={() => navigate("/settings")}
              onLogoutClick={handleLogout}
            />
          )}
        </div>
      </div>
    </header>
  );
};

export default Topbar;
