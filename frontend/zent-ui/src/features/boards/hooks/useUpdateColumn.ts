import { useMutation, useQueryClient } from "@tanstack/react-query";

import { boardsApi } from "../api/boardsApi";
import type { UpdateColumnRequest } from "../model/types";

interface UpdateColumnVariables {
  boardId: string;
  columnId: string;
  data: UpdateColumnRequest;
}

export const useUpdateColumn = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({ boardId, columnId, data }: UpdateColumnVariables) => {
      await boardsApi.updateColumn(boardId, columnId, data);
    },

    onSuccess: async (_, variables) => {
      await queryClient.invalidateQueries({
        queryKey: ["board", variables.boardId],
      });
    },
  });
};
