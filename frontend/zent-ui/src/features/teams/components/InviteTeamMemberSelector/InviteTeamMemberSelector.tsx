import { useEffect, useMemo, useState } from "react";

import closeIcon from "@/assets/icons/close-icon.svg";
import { usersApi } from "@/features/users/api/usersApi";

import type { TeamMemberDto, TeamRole } from "@/features/teams/model/types";

import type { UserSearchDto } from "@/features/users/model/types";

import styles from "./InviteTeamMemberSelector.module.css";

type InviteRole = Exclude<TeamRole, "Owner">;

export interface SelectedInviteMember {
  userId: string;
  firstName: string;
  lastName: string;
  email: string;
  role: InviteRole;
}

interface InviteTeamMemberSelectorProps {
  currentMembers: TeamMemberDto[];
  selectedMembers: SelectedInviteMember[];
  onChange: (members: SelectedInviteMember[]) => void;
}

const InviteTeamMemberSelector = ({
  currentMembers,
  selectedMembers,
  onChange,
}: InviteTeamMemberSelectorProps) => {
  const [search, setSearch] = useState("");
  const [users, setUsers] = useState<UserSearchDto[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [hasSearched, setHasSearched] = useState(false);

  const trimmedSearch = search.trim();

  useEffect(() => {
    if (trimmedSearch.length < 2) {
      setUsers([]);
      setHasSearched(false);
      return;
    }

    const timeoutId = window.setTimeout(async () => {
      try {
        setIsLoading(true);
        setHasSearched(false);

        const result = await usersApi.searchUsers(trimmedSearch);

        setUsers(result);
        setHasSearched(true);
      } catch {
        setUsers([]);
        setHasSearched(true);
      } finally {
        setIsLoading(false);
      }
    }, 350);

    return () => {
      window.clearTimeout(timeoutId);
    };
  }, [trimmedSearch]);

  const visibleUsers = useMemo(() => {
    const currentMemberIds = new Set(
      currentMembers.map((member) => member.userId),
    );

    const selectedMemberIds = new Set(
      selectedMembers.map((member) => member.userId),
    );

    return users.filter(
      (user) =>
        !currentMemberIds.has(user.userId) &&
        !selectedMemberIds.has(user.userId),
    );
  }, [users, currentMembers, selectedMembers]);

  const shouldShowDropdown =
    trimmedSearch.length >= 2 && (isLoading || hasSearched);

  const handleSelectUser = (user: UserSearchDto) => {
    onChange([
      ...selectedMembers,
      {
        userId: user.userId,
        firstName: user.firstName,
        lastName: user.lastName,
        email: user.email,
        role: "Member",
      },
    ]);

    setSearch("");
    setUsers([]);
    setHasSearched(false);
  };

  const handleRemoveUser = (userId: string) => {
    onChange(selectedMembers.filter((member) => member.userId !== userId));
  };

  const handleToggleRole = (userId: string) => {
    onChange(
      selectedMembers.map((member) =>
        member.userId === userId
          ? {
              ...member,
              role: member.role === "Admin" ? "Member" : "Admin",
            }
          : member,
      ),
    );
  };

  return (
    <div className={styles.selector}>
      <label className={styles.label}>Members</label>

      <div className={styles.searchWrapper}>
        <input
          type="text"
          className={styles.searchInput}
          placeholder="Search users by name or email..."
          value={search}
          onChange={(event) => setSearch(event.target.value)}
        />

        {shouldShowDropdown && (
          <div className={styles.dropdown}>
            {isLoading ? (
              <div className={styles.dropdownState}>Searching...</div>
            ) : visibleUsers.length === 0 ? (
              <div className={styles.dropdownState}>No users found.</div>
            ) : (
              visibleUsers.map((user) => (
                <button
                  key={user.userId}
                  type="button"
                  className={styles.userOption}
                  onClick={() => handleSelectUser(user)}
                >
                  <div className={styles.avatar}>
                    {getInitials(user.firstName, user.lastName)}
                  </div>

                  <div className={styles.userInfo}>
                    <span className={styles.userName}>
                      {user.firstName} {user.lastName}
                    </span>

                    <span className={styles.userEmail}>{user.email}</span>
                  </div>
                </button>
              ))
            )}
          </div>
        )}
      </div>

      {selectedMembers.length > 0 && (
        <div className={styles.selectedList}>
          {selectedMembers.map((member) => (
            <div key={member.userId} className={styles.selectedItem}>
              <div className={styles.avatar}>
                {getInitials(member.firstName, member.lastName)}
              </div>

              <div className={styles.userInfo}>
                <span className={styles.userName}>
                  {member.firstName} {member.lastName}
                </span>

                <span className={styles.userEmail}>{member.email}</span>
              </div>

              <div className={styles.roleControl}>
                <span className={styles.roleLabel}>{member.role}</span>

                <button
                  type="button"
                  className={`${styles.switch} ${
                    member.role === "Admin" ? styles.switchOn : ""
                  }`}
                  onClick={() => handleToggleRole(member.userId)}
                >
                  <span />
                </button>
              </div>

              <button
                type="button"
                className={styles.removeButton}
                onClick={() => handleRemoveUser(member.userId)}
              >
                <img src={closeIcon} alt="" />
              </button>
            </div>
          ))}
        </div>
      )}
    </div>
  );
};

const getInitials = (firstName: string, lastName: string) => {
  return `${firstName[0] ?? ""}${lastName[0] ?? ""}`.toUpperCase();
};

export default InviteTeamMemberSelector;
