import { useQuery } from "@tanstack/react-query";
import { boardsApi } from "../api/boardsApi";

export const useBoard = (boardId: string | undefined) => {
  return useQuery({
    queryKey: ["board", boardId],
    queryFn: () => boardsApi.getBoard(boardId!),
    enabled: !!boardId,
  });
};
