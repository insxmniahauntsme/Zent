import { useEffect, useMemo, useRef, useState } from "react";

import type { ProjectMemberDto } from "@/features/projects/model/types";
import type { TaskAssigneeDto } from "@/features/tasks/model/types";

import styles from "./TaskAssigneeControl.module.css";

interface TaskAssigneeControlProps {
  assignee: TaskAssigneeDto | null;
  members: ProjectMemberDto[];
  isLoadingMembers: boolean;
  isSaving: boolean;
  onAssign: (userId: string) => Promise<void>;
  onRemove: () => Promise<void>;
}

const TaskAssigneeControl = ({
  assignee,
  members,
  isLoadingMembers,
  isSaving,
  onAssign,
  onRemove,
}: TaskAssigneeControlProps) => {
  const [isOpen, setIsOpen] = useState(false);
  const [search, setSearch] = useState("");

  const controlRef = useRef<HTMLDivElement | null>(null);

  useEffect(() => {
    if (!isOpen) {
      return;
    }

    const handleClickOutside = (event: MouseEvent) => {
      if (
        controlRef.current &&
        !controlRef.current.contains(event.target as Node)
      ) {
        setIsOpen(false);
        setSearch("");
      }
    };

    document.addEventListener("mousedown", handleClickOutside);

    return () => {
      document.removeEventListener("mousedown", handleClickOutside);
    };
  }, [isOpen]);

  const filteredMembers = useMemo(() => {
    const value = search.trim().toLowerCase();

    if (!value) {
      return members;
    }

    return members.filter((member) => {
      const fullName = `${member.firstName} ${member.lastName}`.toLowerCase();
      const email = member.email.toLowerCase();

      return fullName.includes(value) || email.includes(value);
    });
  }, [members, search]);

  const handleAssign = async (userId: string) => {
    await onAssign(userId);

    setIsOpen(false);
    setSearch("");
  };

  const handleRemove = async () => {
    await onRemove();

    setIsOpen(false);
    setSearch("");
  };

  return (
    <div ref={controlRef} className={styles.control}>
      <button
        type="button"
        className={styles.currentAssignee}
        onClick={() => setIsOpen((prev) => !prev)}
        disabled={isSaving}
      >
        {assignee ? (
          <>
            <div className={styles.avatar}>
              {getInitials(assignee.firstName, assignee.lastName)}
            </div>

            <div className={styles.userInfo}>
              <strong>
                {assignee.firstName} {assignee.lastName}
              </strong>
              <span>{assignee.email}</span>
            </div>
          </>
        ) : (
          <>
            <div className={styles.avatarMuted}>?</div>

            <div className={styles.userInfo}>
              <strong>Unassigned</strong>
              <span>Choose responsible person</span>
            </div>
          </>
        )}

        <span className={styles.chevron}>⌄</span>
      </button>

      {isOpen && (
        <div className={styles.dropdown}>
          <input
            value={search}
            className={styles.searchInput}
            placeholder="Search member..."
            onChange={(event) => setSearch(event.target.value)}
          />

          <div className={styles.list}>
            {isLoadingMembers ? (
              <div className={styles.state}>Loading members...</div>
            ) : filteredMembers.length === 0 ? (
              <div className={styles.state}>No members found.</div>
            ) : (
              filteredMembers.map((member) => {
                const isSelected = assignee?.userId === member.userId;

                return (
                  <button
                    key={member.userId}
                    type="button"
                    className={`${styles.option} ${
                      isSelected ? styles.optionActive : ""
                    }`}
                    disabled={isSaving}
                    onClick={() => handleAssign(member.userId)}
                  >
                    <div className={styles.avatar}>
                      {getInitials(member.firstName, member.lastName)}
                    </div>

                    <div className={styles.userInfo}>
                      <strong>
                        {member.firstName} {member.lastName}
                      </strong>
                      <span>{member.email}</span>
                    </div>

                    {isSelected && <span className={styles.check}>✓</span>}
                  </button>
                );
              })
            )}
          </div>

          {assignee && (
            <button
              type="button"
              className={styles.removeButton}
              disabled={isSaving}
              onClick={handleRemove}
            >
              Remove assignee
            </button>
          )}
        </div>
      )}
    </div>
  );
};

const getInitials = (firstName: string, lastName: string) => {
  return `${firstName[0] ?? ""}${lastName[0] ?? ""}`.toUpperCase();
};

export default TaskAssigneeControl;
