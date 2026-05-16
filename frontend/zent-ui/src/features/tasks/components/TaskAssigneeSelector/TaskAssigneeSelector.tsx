import { useMemo, useState } from "react";

import type { ProjectMemberDto } from "@/features/projects/model/types";

import styles from "./TaskAssigneeSelector.module.css";

interface TaskAssigneeSelectorProps {
  assignees: ProjectMemberDto[];
  selectedAssignee: ProjectMemberDto | null;
  onChange: (assignee: ProjectMemberDto | null) => void;
  disabled?: boolean;
  isLoading?: boolean;
}

const getInitials = (firstName?: string, lastName?: string) => {
  const first = firstName?.trim()[0] ?? "";
  const last = lastName?.trim()[0] ?? "";

  const initials = `${first}${last}`.trim();

  return initials.length > 0 ? initials.toUpperCase() : "U";
};

const getFullName = (assignee: ProjectMemberDto) => {
  return `${assignee.firstName} ${assignee.lastName}`.trim();
};

const TaskAssigneeSelector = ({
  assignees = [],
  selectedAssignee,
  onChange,
  disabled = false,
  isLoading = false,
}: TaskAssigneeSelectorProps) => {
  const [search, setSearch] = useState("");
  const [isDropdownOpen, setIsDropdownOpen] = useState(false);

  const filteredAssignees = useMemo(() => {
    const query = search.trim().toLowerCase();

    return assignees.filter((assignee) => {
      if (selectedAssignee?.userId === assignee.userId) {
        return false;
      }

      if (!query) {
        return true;
      }

      const fullName = getFullName(assignee).toLowerCase();
      const email = assignee.email.toLowerCase();
      const role = String(assignee.role).toLowerCase();
      const specialization = assignee.specialization
        ? String(assignee.specialization).toLowerCase()
        : "";

      return (
        fullName.includes(query) ||
        email.includes(query) ||
        role.includes(query) ||
        specialization.includes(query)
      );
    });
  }, [assignees, search, selectedAssignee]);

  const shouldShowDropdown = isDropdownOpen && !selectedAssignee && !disabled;

  const handleSelectAssignee = (assignee: ProjectMemberDto) => {
    onChange(assignee);
    setSearch("");
    setIsDropdownOpen(false);
  };

  const handleRemoveAssignee = () => {
    onChange(null);
    setSearch("");
    setIsDropdownOpen(false);
  };

  return (
    <div className={styles.selector}>
      <span className={styles.label}>Assignee</span>

      {selectedAssignee ? (
        <div className={styles.selectedItem}>
          <div className={styles.avatar}>
            {getInitials(selectedAssignee.firstName, selectedAssignee.lastName)}
          </div>

          <div className={styles.userInfo}>
            <span className={styles.userName}>
              {getFullName(selectedAssignee)}
            </span>

            <span className={styles.userEmail}>{selectedAssignee.email}</span>
          </div>

          {selectedAssignee.specialization && (
            <span className={styles.memberRole}>
              {selectedAssignee.specialization}
            </span>
          )}

          <button
            type="button"
            className={styles.removeButton}
            onClick={handleRemoveAssignee}
            disabled={disabled}
            aria-label="Remove assignee"
          >
            ×
          </button>
        </div>
      ) : (
        <div className={styles.searchWrapper}>
          <input
            type="text"
            className={styles.searchInput}
            placeholder="Select project member..."
            value={search}
            disabled={disabled}
            onFocus={() => setIsDropdownOpen(true)}
            onBlur={() => {
              window.setTimeout(() => {
                setIsDropdownOpen(false);
              }, 120);
            }}
            onChange={(event) => {
              setSearch(event.target.value);
              setIsDropdownOpen(true);
            }}
          />

          {shouldShowDropdown && (
            <div className={styles.dropdown}>
              {isLoading ? (
                <div className={styles.dropdownState}>Loading members...</div>
              ) : assignees.length === 0 ? (
                <div className={styles.dropdownState}>
                  No project members available.
                </div>
              ) : filteredAssignees.length === 0 ? (
                <div className={styles.dropdownState}>No matching members.</div>
              ) : (
                filteredAssignees.map((assignee) => (
                  <button
                    key={assignee.userId}
                    type="button"
                    className={styles.userOption}
                    onMouseDown={(event) => event.preventDefault()}
                    onClick={() => handleSelectAssignee(assignee)}
                  >
                    <div className={styles.avatar}>
                      {getInitials(assignee.firstName, assignee.lastName)}
                    </div>

                    <div className={styles.userInfo}>
                      <span className={styles.userName}>
                        {getFullName(assignee)}
                      </span>

                      <span className={styles.userEmail}>{assignee.email}</span>
                    </div>

                    {assignee.specialization && (
                      <span className={styles.memberRole}>
                        {assignee.specialization}
                      </span>
                    )}
                  </button>
                ))
              )}
            </div>
          )}
        </div>
      )}
    </div>
  );
};

export default TaskAssigneeSelector;
