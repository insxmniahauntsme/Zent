import { useEffect, useMemo, useRef, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";

import { useCurrentUser } from "@/features/users/hooks/useCurrentUser";
import { useTeams } from "@/features/teams/hooks/useTeams";
import UserMenu from "./UserMenu/UserMenu";

import styles from "./Topbar.module.css";

import searchIcon from "@/assets/icons/search-icon.svg";
import faqIcon from "@/assets/icons/faq-icon.svg";
import notificationsIcon from "@/assets/icons/notifications-icon.svg";
import chevronDownIcon from "@/assets/icons/chevron-down.svg";

interface TopbarProps {
  workspaceName?: string;
}

const Topbar = ({ workspaceName }: TopbarProps) => {
  const navigate = useNavigate();
  const { teamId } = useParams<{ teamId: string }>();

  const { data: user } = useCurrentUser();
  const { data: teams = [], isLoading: areTeamsLoading } = useTeams();

  const [isUserMenuOpen, setIsUserMenuOpen] = useState(false);
  const [isWorkspaceMenuOpen, setIsWorkspaceMenuOpen] = useState(false);

  const userMenuRef = useRef<HTMLDivElement | null>(null);
  const workspaceMenuRef = useRef<HTMLDivElement | null>(null);

  const activeTeam = useMemo(() => {
    return teams.find((team) => team.id === teamId) ?? null;
  }, [teams, teamId]);

  const currentWorkspaceName = workspaceName ?? activeTeam?.name ?? "Workspace";

  useEffect(() => {
    if (!isUserMenuOpen) {
      return;
    }

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

  useEffect(() => {
    if (!isWorkspaceMenuOpen) {
      return;
    }

    const handleClickOutside = (event: MouseEvent) => {
      if (
        workspaceMenuRef.current &&
        !workspaceMenuRef.current.contains(event.target as Node)
      ) {
        setIsWorkspaceMenuOpen(false);
      }
    };

    document.addEventListener("mousedown", handleClickOutside);

    return () => {
      document.removeEventListener("mousedown", handleClickOutside);
    };
  }, [isWorkspaceMenuOpen]);

  const handleLogout = () => {
    localStorage.removeItem("token");
    navigate("/login");
  };

  const handleWorkspaceSelect = (selectedTeamId: string) => {
    setIsWorkspaceMenuOpen(false);

    if (selectedTeamId === teamId) {
      return;
    }

    navigate(`/app/${selectedTeamId}`);
  };

  return (
    <header className={styles.topbar}>
      <div className={styles.left}>
        {workspaceName || teamId ? (
          <div className={styles.workspaceMenuWrapper} ref={workspaceMenuRef}>
            <button
              type="button"
              className={`${styles.workspaceSwitcher} ${
                isWorkspaceMenuOpen ? styles.workspaceSwitcherActive : ""
              }`}
              onClick={() => setIsWorkspaceMenuOpen((prev) => !prev)}
            >
              <div className={styles.workspaceIcon}>
                {getInitials(currentWorkspaceName)}
              </div>

              <div className={styles.workspaceText}>
                <span className={styles.workspaceEyebrow}>Workspace</span>

                <span className={styles.workspaceName}>
                  {currentWorkspaceName}
                </span>
              </div>

              <span
                className={`${styles.workspaceChevron} ${
                  isWorkspaceMenuOpen ? styles.workspaceChevronOpen : ""
                }`}
                aria-hidden="true"
              >
                <img src={chevronDownIcon} alt="" />
              </span>
            </button>

            {isWorkspaceMenuOpen && (
              <div className={styles.workspaceDropdown}>
                <div className={styles.workspaceDropdownHeader}>
                  <span>Switch workspace</span>
                </div>

                <div className={styles.workspaceList}>
                  {areTeamsLoading ? (
                    <div className={styles.workspaceState}>
                      Loading workspaces...
                    </div>
                  ) : teams.length === 0 ? (
                    <div className={styles.workspaceState}>
                      No workspaces found.
                    </div>
                  ) : (
                    teams.map((team) => {
                      const isActive = team.id === teamId;

                      return (
                        <button
                          key={team.id}
                          type="button"
                          className={`${styles.workspaceOption} ${
                            isActive ? styles.workspaceOptionActive : ""
                          }`}
                          onClick={() => handleWorkspaceSelect(team.id)}
                        >
                          <div className={styles.workspaceOptionIcon}>
                            {getInitials(team.name)}
                          </div>

                          <div className={styles.workspaceOptionInfo}>
                            <strong>{team.name}</strong>
                            <span>
                              {isActive
                                ? "Current workspace"
                                : "Open workspace"}
                            </span>
                          </div>

                          {isActive && (
                            <span className={styles.workspaceCheck}>✓</span>
                          )}
                        </button>
                      );
                    })
                  )}
                </div>

                <button
                  type="button"
                  className={styles.viewAllTeamsButton}
                  onClick={() => {
                    setIsWorkspaceMenuOpen(false);
                    navigate("/teams");
                  }}
                >
                  View all teams
                </button>
              </div>
            )}
          </div>
        ) : (
          <span className={styles.breadcrumb}>
            Teams / <strong>Zent workspace</strong>
          </span>
        )}
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

const getInitials = (value: string) => {
  const words = value.trim().split(/\s+/);

  if (words.length === 1) {
    return words[0].slice(0, 2).toUpperCase();
  }

  return `${words[0][0] ?? ""}${words[1][0] ?? ""}`.toUpperCase();
};

export default Topbar;
