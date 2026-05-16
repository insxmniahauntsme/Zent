import { useMutation, useQueryClient } from "@tanstack/react-query";
import { boardsApi } from "../api/boardsApi";
import type { AddBoardRequest } from "../model/types";

export const useCreateBoard = (projectId: string | undefined) => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (data: AddBoardRequest) => {
      if (!projectId) {
        throw new Error("Project id is required.");
      }

      return boardsApi.addBoard(projectId, data);
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: ["project", projectId, "boards"],
      });

      await queryClient.invalidateQueries({
        queryKey: ["project", projectId],
      });
    },
  });
};
