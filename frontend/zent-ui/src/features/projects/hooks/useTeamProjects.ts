import { useQuery } from "@tanstack/react-query";
import { projectsApi } from "../api/projectsApi";

export const useTeamProjects = (teamId: string | undefined) => {
  return useQuery({
    queryKey: ["team", teamId, "projects"],
    queryFn: () => projectsApi.getTeamProjects(teamId!),
    enabled: !!teamId,
  });
};
