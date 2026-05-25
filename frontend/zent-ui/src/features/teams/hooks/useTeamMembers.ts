import { useQuery } from "@tanstack/react-query";
import { teamsApi } from "@/features/teams/api/teamsApi";

export const useTeamMembers = (teamId: string | undefined) => {
  return useQuery({
    queryKey: ["team-members", teamId],
    queryFn: () => teamsApi.getTeamMembers(teamId!),
    enabled: Boolean(teamId),
  });
};
