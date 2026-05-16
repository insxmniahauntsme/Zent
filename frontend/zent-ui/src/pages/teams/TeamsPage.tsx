import { useMemo, useState } from "react";
import styles from "./TeamsPage.module.css";

import TeamsEmptyState from "@/features/teams/components/TeamsEmptyState/TeamsEmptyState";
import TeamCard from "@/features/teams/components/TeamCard/TeamCard";
import CreateTeamModal from "@/features/teams/components/CreateTeamModal/CreateTeamModal";

import searchIcon from "@/assets/icons/search-icon.svg";
import { useCreateTeam } from "@/features/teams/hooks/useCreateTeam";
import axios from "axios";
import { useTeams } from "@/features/teams/hooks/useTeams";

type TeamsTab = "all" | "my" | "archived";

const TeamsPage = () => {
  const [activeTab, setActiveTab] = useState<TeamsTab>("all");
  const [search, setSearch] = useState("");

  const [isCreateModalOpen, setIsCreateModalOpen] = useState(false);

  const [createTeamError, setCreateTeamError] = useState<string | null>(null);

  const createTeamMutation = useCreateTeam();

  const createTeamModal = isCreateModalOpen && (
    <CreateTeamModal
      isSubmitting={createTeamMutation.isPending}
      submitError={createTeamError}
      onClose={() => {
        setCreateTeamError(null);
        setIsCreateModalOpen(false);
      }}
      onSubmit={async (data) => {
        try {
          setCreateTeamError(null);
          await createTeamMutation.mutateAsync(data);
          setIsCreateModalOpen(false);
        } catch (error) {
          if (axios.isAxiosError(error)) {
            const code = error.response?.data?.code;

            if (code === "TeamAlreadyExists") {
              setCreateTeamError("You already have a team with this name.");
              return;
            }

            if (code === "UserNotFound") {
              setCreateTeamError("One or more selected users were not found.");
              return;
            }
          }

          setCreateTeamError("Failed to create team. Please try again.");
        }
      }}
    />
  );

  const { data: teams = [], isLoading, isError } = useTeams();

  const searchedTeams = useMemo(() => {
    const normalizedSearch = search.trim().toLowerCase();

    if (!normalizedSearch) return teams;

    return teams.filter((team) =>
      team.name.toLowerCase().includes(normalizedSearch),
    );
  }, [teams, search]);

  const allTeamsCount = searchedTeams.length;

  const myTeamsCount = searchedTeams.filter(
    (team) => team.role === "Owner",
  ).length;

  const archivedTeamsCount = 0;

  const visibleTeams = useMemo(() => {
    let result = teams;

    if (activeTab === "my") {
      result = result.filter((team) => team.role === "Owner");
    }

    if (activeTab === "archived") {
      result = [];
    }

    if (search.trim()) {
      result = result.filter((team) =>
        team.name.toLowerCase().includes(search.trim().toLowerCase()),
      );
    }

    return result;
  }, [teams, activeTab, search]);

  if (isLoading) {
    return <div className={styles.state}>Loading teams...</div>;
  }

  if (isError) {
    return <div className={styles.state}>Failed to load teams.</div>;
  }

  if (teams.length === 0) {
    return (
      <>
        <TeamsEmptyState
          onCreateTeam={() => {
            setCreateTeamError(null);
            setIsCreateModalOpen(true);
          }}
        />

        {createTeamModal}
      </>
    );
  }

  return (
    <section className={styles.page}>
      <div className={styles.container}>
        <div className={styles.header}>
          <div>
            <h1 className={styles.title}>Teams</h1>
            <p className={styles.subtitle}>
              Manage and switch between your workspaces.
            </p>
          </div>

          <button
            type="button"
            className={styles.createButton}
            onClick={() => {
              setCreateTeamError(null);
              setIsCreateModalOpen(true);
            }}
          >
            + Create Team
          </button>
        </div>

        <div className={styles.tabs}>
          <button
            type="button"
            className={`${styles.tab} ${activeTab === "all" ? styles.activeTab : ""}`}
            onClick={() => setActiveTab("all")}
          >
            All teams <span>{allTeamsCount}</span>
          </button>

          <button
            type="button"
            className={`${styles.tab} ${activeTab === "my" ? styles.activeTab : ""}`}
            onClick={() => setActiveTab("my")}
          >
            My teams <span>{myTeamsCount}</span>
          </button>

          <button
            type="button"
            className={`${styles.tab} ${activeTab === "archived" ? styles.activeTab : ""}`}
            onClick={() => setActiveTab("archived")}
          >
            Archived <span>{archivedTeamsCount}</span>
          </button>
        </div>

        <div className={styles.toolbar}>
          <div className={styles.searchbar}>
            <img src={searchIcon} className={styles.searchIcon} alt="" />
            <input
              className={styles.searchInput}
              type="text"
              placeholder="Search teams..."
              value={search}
              onChange={(event) => setSearch(event.target.value)}
            />
          </div>
        </div>

        {visibleTeams.length === 0 ? (
          <div className={styles.noResults}>
            No teams found for the selected filters.
          </div>
        ) : (
          <div className={styles.grid}>
            {visibleTeams.map((team) => (
              <TeamCard key={team.id} team={team} />
            ))}
          </div>
        )}
      </div>
      {createTeamModal}
    </section>
  );
};

export default TeamsPage;
