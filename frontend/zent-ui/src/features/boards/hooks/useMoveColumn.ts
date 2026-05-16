import { useMutation, useQueryClient } from "@tanstack/react-query";

import { boardsApi } from "../api/boardsApi";
import type { MoveColumnRequest } from "../model/types";

interface MoveColumnVariables {
  boardId: string;
  columnId: string;
  data: MoveColumnRequest;
}

export const useMoveColumn = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({ boardId, columnId, data }: MoveColumnVariables) => {
      await boardsApi.moveColumn(boardId, columnId, data);
    },

    onSuccess: async (_, variables) => {
      await queryClient.invalidateQueries({
        queryKey: ["board", variables.boardId],
      });
    },
  });
};
