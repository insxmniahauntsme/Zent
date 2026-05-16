import { useQuery } from "@tanstack/react-query";
import { teamsApi } from "../api/teamsApi";

export const useTeam = (teamId: string | undefined) => {
  return useQuery({
    queryKey: ["team", teamId],
    queryFn: () => teamsApi.getTeam(teamId!),
    enabled: !!teamId,
  });
};
