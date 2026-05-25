import { Outlet, useParams } from "react-router-dom";

import Sidebar from "@/shared/components/Sidebar/Sidebar";
import Topbar from "@/shared/components/Topbar/Topbar";
import { useTeam } from "@/features/teams/hooks/useTeam";

import styles from "./TeamWorkspaceLayout.module.css";

import projectsIcon from "@/assets/icons/projects-icon.svg";
import memberIcon from "@/assets/icons/member-icon.svg";
import settingsIcon from "@/assets/icons/settings-icon.svg";

const TeamWorkspaceLayout = () => {
  const { teamId } = useParams();

  const { data: team, isLoading, isError } = useTeam(teamId);

  if (isLoading) {
    return <div className={styles.state}>Loading workspace...</div>;
  }

  if (isError || !team) {
    return <div className={styles.state}>Failed to load workspace.</div>;
  }

  const teamRootPath = `/app/${team.id}`;

  const isProjectsActive = (pathname: string) => {
    if (pathname === teamRootPath) {
      return true;
    }

    const projectPageRegex = new RegExp(`^/app/${team.id}/projects/[^/]+/?$`);

    if (projectPageRegex.test(pathname)) {
      return true;
    }

    const taskPageRegex = new RegExp(`^/app/${team.id}/tasks/[^/]+/?$`);

    if (taskPageRegex.test(pathname)) {
      return true;
    }

    return false;
  };

  return (
    <div className={styles.layout}>
      <Sidebar
        items={[
          {
            label: "Projects",
            to: teamRootPath,
            icon: projectsIcon,
            end: true,
            activeWhen: isProjectsActive,
          },
          {
            label: "Members",
            to: `${teamRootPath}/members`,
            icon: memberIcon,
            activeWhen: (pathname) => pathname === `${teamRootPath}/members`,
          },
          {
            label: "Settings",
            to: `${teamRootPath}/settings`,
            icon: settingsIcon,
            activeWhen: (pathname) => pathname === `${teamRootPath}/settings`,
          },
        ]}
      />

      <div className={styles.contentArea}>
        <Topbar workspaceName={team.name} />

        <main className={styles.main}>
          <Outlet context={{ team }} />
        </main>
      </div>
    </div>
  );
};

export default TeamWorkspaceLayout;
