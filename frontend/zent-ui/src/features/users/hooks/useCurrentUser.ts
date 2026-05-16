import { useQuery } from "@tanstack/react-query";
import { usersApi } from "../api/usersApi";

export const useCurrentUser = () => {
  return useQuery({
    queryKey: ["currentUser"],
    queryFn: usersApi.getCurrentUser,
    staleTime: 1000 * 60 * 5,
  });
};
