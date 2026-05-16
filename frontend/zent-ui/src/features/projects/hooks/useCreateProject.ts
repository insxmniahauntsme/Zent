import { useMutation, useQueryClient } from "@tanstack/react-query";
import { projectsApi } from "../api/projectsApi";
import type { AddProjectRequest } from "../model/types";

export const useCreateProject = (teamId: string) => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (data: AddProjectRequest) =>
      projectsApi.createProject(teamId, data),

    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["team", teamId] });
      queryClient.invalidateQueries({ queryKey: ["team", teamId, "projects"] });
    },
  });
};
