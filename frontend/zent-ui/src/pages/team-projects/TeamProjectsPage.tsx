import { useMemo, useState } from "react";
import { useOutletContext } from "react-router-dom";

import type { TeamDto } from "@/features/teams/model/types";
import { useTeamProjects } from "@/features/projects/hooks/useTeamProjects";
import { useCreateProject } from "@/features/projects/hooks/useCreateProject";
import ProjectCard from "@/features/projects/components/ProjectCard/ProjectCard";
import CreateProjectModal from "@/features/projects/components/CreateProjectModal/CreateProjectModal";

import searchIcon from "@/assets/icons/search-icon.svg";
import styles from "./TeamProjectsPage.module.css";

const TeamProjectsPage = () => {
  const { team } = useOutletContext<{ team: TeamDto }>();

  const [search, setSearch] = useState("");
  const [isCreateProjectOpen, setIsCreateProjectOpen] = useState(false);
  const [createProjectError, setCreateProjectError] = useState<string | null>(
    null,
  );

  const { data: projects = [], isLoading, isError } = useTeamProjects(team.id);

  const createProjectMutation = useCreateProject(team.id);

  const visibleProjects = useMemo(() => {
    const value = search.trim().toLowerCase();

    if (!value) return projects;

    return projects.filter(
      (project) =>
        project.name.toLowerCase().includes(value) ||
        project.description?.toLowerCase().includes(value) ||
        project.client?.toLowerCase().includes(value),
    );
  }, [projects, search]);

  return (
    <section className={styles.page}>
      <div className={styles.container}>
        <header className={styles.workspaceHeader}>
          <div>
            <p className={styles.eyebrow}>Workspace</p>
            <h1 className={styles.teamName}>{team.name}</h1>

            <div className={styles.meta}>
              <span>{team.membersCount} members</span>
              <span>{team.projectsCount} projects</span>
              <span>{team.currentUserRole}</span>
            </div>
          </div>
        </header>

        <div className={styles.divider} />

        <div className={styles.sectionHeader}>
          <div>
            <h2 className={styles.sectionTitle}>Projects</h2>
            <p className={styles.sectionSubtitle}>
              Manage boards, members and tasks inside this workspace.
            </p>
          </div>

          <button
            type="button"
            className={styles.createButton}
            onClick={() => {
              setCreateProjectError(null);
              setIsCreateProjectOpen(true);
            }}
          >
            + Create Project
          </button>
        </div>

        <div className={styles.toolbar}>
          <div className={styles.searchbar}>
            <img src={searchIcon} className={styles.searchIcon} alt="" />
            <input
              className={styles.searchInput}
              type="text"
              placeholder="Search projects..."
              value={search}
              onChange={(event) => setSearch(event.target.value)}
            />
          </div>
        </div>

        {isLoading ? (
          <div className={styles.noResults}>Loading projects...</div>
        ) : isError ? (
          <div className={styles.noResults}>Failed to load projects.</div>
        ) : projects.length === 0 ? (
          <div className={styles.emptyState}>
            <h2>No projects yet</h2>
            <p>
              Create your first project to start organizing boards and tasks.
            </p>
          </div>
        ) : visibleProjects.length === 0 ? (
          <div className={styles.noResults}>
            No projects found for this search.
          </div>
        ) : (
          <div className={styles.grid}>
            {visibleProjects.map((project) => (
              <ProjectCard
                key={project.id}
                teamId={team.id}
                project={project}
              />
            ))}
          </div>
        )}
      </div>

      {isCreateProjectOpen && (
        <CreateProjectModal
          teamId={team.id}
          isSubmitting={createProjectMutation.isPending}
          submitError={createProjectError}
          onClose={() => {
            setCreateProjectError(null);
            setIsCreateProjectOpen(false);
          }}
          onSubmit={async (data) => {
            try {
              setCreateProjectError(null);
              await createProjectMutation.mutateAsync(data);
              setIsCreateProjectOpen(false);
            } catch {
              setCreateProjectError(
                "Failed to create project. Please try again.",
              );
            }
          }}
        />
      )}
    </section>
  );
};

export default TeamProjectsPage;
