import { useQuery } from "@tanstack/react-query";
import { teamsApi } from "../api/teamsApi";

export const useTeams = () => {
  return useQuery({
    queryKey: ["teams"],
    queryFn: teamsApi.getMyTeams,
    staleTime: 1000 * 60,
  });
};
