import { useMutation, useQueryClient } from "@tanstack/react-query";
import { boardsApi } from "../api/boardsApi";
import type { AddColumnRequest } from "../model/types";

export const useCreateColumn = (boardId?: string) => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (data: AddColumnRequest) => {
      if (!boardId) {
        throw new Error("Board id is required.");
      }

      return boardsApi.addColumn(boardId, data);
    },

    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: ["board", boardId],
      });
    },
  });
};
