import { useQuery } from "@tanstack/react-query";

import { projectsApi } from "../api/projectsApi";

export const useProjectMembers = (projectId?: string) => {
  return useQuery({
    queryKey: ["projectMembers", projectId],
    queryFn: () => projectsApi.getProjectMembers(projectId!),
    enabled: Boolean(projectId),
  });
};
