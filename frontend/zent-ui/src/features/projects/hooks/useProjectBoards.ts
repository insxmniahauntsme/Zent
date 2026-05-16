import { useQuery } from "@tanstack/react-query";
import { projectsApi } from "../api/projectsApi";

export const useProjectBoards = (projectId: string | undefined) => {
  return useQuery({
    queryKey: ["project", projectId, "boards"],
    queryFn: () => projectsApi.getProjectBoards(projectId!),
    enabled: !!projectId,
  });
};
