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

  return (
    <div className={styles.layout}>
      <Sidebar
        items={[
          {
            label: "Projects",
            to: `/app/${team.id}`,
            icon: projectsIcon,
            end: true,
          },
          {
            label: "Members",
            to: `/app/${team.id}/members`,
            icon: memberIcon,
          },
          {
            label: "Settings",
            to: `/app/${team.id}/settings`,
            icon: settingsIcon,
          },
        ]}
      />

      <div className={styles.contentArea}>
        <Topbar />

        <main className={styles.main}>
          <Outlet context={{ team }} />
        </main>
      </div>
    </div>
  );
};

export default TeamWorkspaceLayout;
