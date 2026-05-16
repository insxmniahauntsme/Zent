import { useMutation, useQueryClient } from "@tanstack/react-query";
import { teamsApi } from "../api/teamsApi";

export const useCreateTeam = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: teamsApi.createTeam,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["teams"] });
    },
  });
};
