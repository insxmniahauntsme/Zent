import { useMutation, useQueryClient } from "@tanstack/react-query";

import { boardsApi } from "../api/boardsApi";

interface DeleteColumnVariables {
  boardId: string;
  columnId: string;
}

export const useDeleteColumn = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({ boardId, columnId }: DeleteColumnVariables) => {
      await boardsApi.deleteColumn(boardId, columnId);
    },

    onSuccess: async (_, variables) => {
      await queryClient.invalidateQueries({
        queryKey: ["board", variables.boardId],
      });
    },
  });
};
