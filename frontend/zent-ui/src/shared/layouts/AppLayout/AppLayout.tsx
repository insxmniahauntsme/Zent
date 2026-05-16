import { Outlet } from "react-router-dom";
import Sidebar from "../../components/Sidebar/Sidebar";
import Topbar from "../../components/Topbar/Topbar";
import styles from "./AppLayout.module.css";

import teamIcon from "@/assets/icons/team-icon.svg";
import settingsIcon from "@/assets/icons/settings-icon.svg";

const AppLayout = () => {
  return (
    <div className={styles.layout}>
      <Sidebar
        items={[
          { label: "Teams", to: "/teams", icon: teamIcon },
          { label: "Settings", to: "/settings", icon: settingsIcon },
        ]}
        bottomAction={{
          label: "+ Create Task",
          onClick: () => {},
        }}
      />

      <div className={styles.contentArea}>
        <Topbar />

        <main className={styles.main}>
          <Outlet />
        </main>
      </div>
    </div>
  );
};

export default AppLayout;
