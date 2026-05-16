import { useQuery } from "@tanstack/react-query";
import { projectsApi } from "../api/projectsApi";

export const useProject = (projectId: string | undefined) => {
  return useQuery({
    queryKey: ["project", projectId],
    queryFn: () => projectsApi.getProject(projectId!),
    enabled: !!projectId,
  });
};
