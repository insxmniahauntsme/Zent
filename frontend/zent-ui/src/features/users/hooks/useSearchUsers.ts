import { useQuery } from "@tanstack/react-query";
import { usersApi } from "../api/usersApi";

export const useSearchUsers = (query: string) => {
  return useQuery({
    queryKey: ["users", "search", query],
    queryFn: () => usersApi.searchUsers(query),
    enabled: query.length >= 3,
  });
};
