import { useEffect, useMemo, useState } from "react";
import styles from "./TeamMemberSelector.module.css";
import closeIcon from "@/assets/icons/close-icon.svg";
import { usersApi } from "@/features/users/api/usersApi";
import type { UserSearchDto } from "@/features/users/model/types";
import { useDebounce } from "@/shared/hooks/useDebounce";

type TeamRole = "Admin" | "Member";

export interface SelectedTeamMember {
  userId: string;
  firstName: string;
  lastName: string;
  email: string;
  role: TeamRole;
}

interface MemberSelectorProps {
  selectedMembers: SelectedTeamMember[];
  onChange: (members: SelectedTeamMember[]) => void;
}

const isValidSearchQuery = (value: string) => {
  const query = value.trim();

  if (query.length < 3) return false;
  if (query.startsWith("@")) return false;

  return true;
};

const MemberSelector = ({ selectedMembers, onChange }: MemberSelectorProps) => {
  const [search, setSearch] = useState("");
  const [users, setUsers] = useState<UserSearchDto[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [hasSearched, setHasSearched] = useState(false);

  const debouncedSearch = useDebounce(search, 350);
  const normalizedSearch = debouncedSearch.trim();
  const canSearch = isValidSearchQuery(normalizedSearch);

  useEffect(() => {
    if (!canSearch) {
      setUsers([]);
      setHasSearched(false);
      setIsLoading(false);
      return;
    }

    let isCancelled = false;

    const loadUsers = async () => {
      try {
        setIsLoading(true);
        setHasSearched(false);

        const result = await usersApi.searchUsers(normalizedSearch);

        if (!isCancelled) {
          setUsers(result);
          setHasSearched(true);
        }
      } catch {
        if (!isCancelled) {
          setUsers([]);
          setHasSearched(true);
        }
      } finally {
        if (!isCancelled) {
          setIsLoading(false);
        }
      }
    };

    loadUsers();

    return () => {
      isCancelled = true;
    };
  }, [normalizedSearch, canSearch]);

  const visibleUsers = useMemo(() => {
    return users.filter(
      (user) => !selectedMembers.some((member) => member.userId === user.id),
    );
  }, [users, selectedMembers]);

  const shouldShowDropdown = canSearch && (isLoading || hasSearched);

  const handleSelectUser = (user: UserSearchDto) => {
    onChange([
      ...selectedMembers,
      {
        userId: user.id,
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

  const handleChangeRole = (userId: string, role: TeamRole) => {
    onChange(
      selectedMembers.map((member) =>
        member.userId === userId ? { ...member, role } : member,
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
          placeholder="Search users by email..."
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
                  key={user.id}
                  type="button"
                  className={styles.userOption}
                  onClick={() => handleSelectUser(user)}
                >
                  <div className={styles.avatar}>
                    {user.firstName[0]}
                    {user.lastName[0]}
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
                {member.firstName[0]}
                {member.lastName[0]}
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
                  onClick={() =>
                    handleChangeRole(
                      member.userId,
                      member.role === "Admin" ? "Member" : "Admin",
                    )
                  }
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

export default MemberSelector;
