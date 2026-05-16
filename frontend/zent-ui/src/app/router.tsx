import { createBrowserRouter } from "react-router-dom";

import LoginPage from "@/pages/login/LoginPage";
import RegisterPage from "@/pages/register/RegisterPage";
import AppLayout from "@/shared/layouts/AppLayout/AppLayout";
import TeamsPage from "@/pages/teams/TeamsPage";
import TeamWorkspaceLayout from "@/shared/layouts/TeamWorkspaceLayout/TeamWorkspaceLayout";
import TeamProjectsPage from "@/pages/team-projects/TeamProjectsPage";
import ProjectPage from "@/pages/project/ProjectPage";
import TaskDetailsPage from "@/pages/task-details/TaskDetailsPage";

export const router = createBrowserRouter([
  {
    path: "/login",
    element: <LoginPage />,
  },
  {
    path: "/register",
    element: <RegisterPage />,
  },
  {
    path: "/",
    element: <AppLayout />,
    children: [
      {
        index: true,
        element: <TeamsPage />,
      },
      {
        path: "teams",
        element: <TeamsPage />,
      },
    ],
  },
  {
    path: "/app/:teamId",
    element: <TeamWorkspaceLayout />,
    children: [
      {
        index: true,
        element: <TeamProjectsPage />,
      },
      {
        path: "members",
        element: <div>Members page</div>,
      },
      {
        path: "settings",
        element: <div>Team settings page</div>,
      },
      {
        path: "projects/:projectId",
        element: <ProjectPage />,
      },
      {
        path: "tasks/:taskId",
        element: <TaskDetailsPage />,
      },
    ],
  },
]);
