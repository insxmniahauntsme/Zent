import { useEffect, useMemo, useState } from "react";
import styles from "./ProjectMemberSelector.module.css";
import closeIcon from "@/assets/icons/close-icon.svg";
import { teamsApi } from "@/features/teams/api/teamsApi";
import type { TeamMemberDto } from "@/features/teams/model/types";
import { useDebounce } from "@/shared/hooks/useDebounce";

export interface SelectedProjectMember {
  userId: string;
  firstName: string;
  lastName: string;
  email: string;
}

interface ProjectMemberSelectorProps {
  teamId: string;
  selectedMembers: SelectedProjectMember[];
  onChange: (members: SelectedProjectMember[]) => void;
}

const isValidSearchQuery = (value: string) => {
  const query = value.trim();

  if (query.length < 3) return false;
  if (query.startsWith("@")) return false;

  return true;
};

const ProjectMemberSelector = ({
  teamId,
  selectedMembers,
  onChange,
}: ProjectMemberSelectorProps) => {
  const [search, setSearch] = useState("");
  const [members, setMembers] = useState<TeamMemberDto[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [hasSearched, setHasSearched] = useState(false);

  const debouncedSearch = useDebounce(search, 350);
  const normalizedSearch = debouncedSearch.trim();
  const canSearch = isValidSearchQuery(normalizedSearch);

  useEffect(() => {
    if (!canSearch) {
      setMembers([]);
      setHasSearched(false);
      setIsLoading(false);
      return;
    }

    let isCancelled = false;

    const loadMembers = async () => {
      try {
        setIsLoading(true);
        setHasSearched(false);

        const result = await teamsApi.searchTeamMembers(
          teamId,
          normalizedSearch,
        );

        if (!isCancelled) {
          setMembers(result);
          setHasSearched(true);
        }
      } catch {
        if (!isCancelled) {
          setMembers([]);
          setHasSearched(true);
        }
      } finally {
        if (!isCancelled) {
          setIsLoading(false);
        }
      }
    };

    loadMembers();

    return () => {
      isCancelled = true;
    };
  }, [teamId, normalizedSearch, canSearch]);

  const visibleMembers = useMemo(() => {
    return members.filter(
      (member) =>
        !selectedMembers.some(
          (selectedMember) => selectedMember.userId === member.userId,
        ),
    );
  }, [members, selectedMembers]);

  const shouldShowDropdown = canSearch && (isLoading || hasSearched);

  const handleSelectMember = (member: TeamMemberDto) => {
    onChange([
      ...selectedMembers,
      {
        userId: member.userId,
        firstName: member.firstName,
        lastName: member.lastName,
        email: member.email,
      },
    ]);

    setSearch("");
    setMembers([]);
    setHasSearched(false);
  };

  const handleRemoveMember = (userId: string) => {
    onChange(selectedMembers.filter((member) => member.userId !== userId));
  };

  return (
    <div className={styles.selector}>
      <label className={styles.label}>Project members</label>

      <div className={styles.searchWrapper}>
        <input
          className={styles.searchInput}
          type="text"
          placeholder="Search team members..."
          value={search}
          onChange={(event) => setSearch(event.target.value)}
        />

        {shouldShowDropdown && (
          <div className={styles.dropdown}>
            {isLoading ? (
              <div className={styles.dropdownState}>Searching...</div>
            ) : visibleMembers.length === 0 ? (
              <div className={styles.dropdownState}>No team members found.</div>
            ) : (
              visibleMembers.map((member) => (
                <button
                  key={member.userId}
                  type="button"
                  className={styles.memberOption}
                  onClick={() => handleSelectMember(member)}
                >
                  <div className={styles.avatar}>
                    {member.firstName[0]}
                    {member.lastName[0]}
                  </div>

                  <div className={styles.memberInfo}>
                    <span className={styles.memberName}>
                      {member.firstName} {member.lastName}
                    </span>
                    <span className={styles.memberEmail}>{member.email}</span>
                  </div>

                  <span className={styles.memberRole}>{member.role}</span>
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

              <div className={styles.memberInfo}>
                <span className={styles.memberName}>
                  {member.firstName} {member.lastName}
                </span>
                <span className={styles.memberEmail}>{member.email}</span>
              </div>

              <button
                type="button"
                className={styles.removeButton}
                onClick={() => handleRemoveMember(member.userId)}
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

export default ProjectMemberSelector;
