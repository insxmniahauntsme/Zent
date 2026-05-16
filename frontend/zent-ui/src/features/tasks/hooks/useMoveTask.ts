import { useMutation, useQueryClient } from "@tanstack/react-query";

import { tasksApi } from "@/features/tasks/api/tasksApi";
import type { MoveTaskRequest } from "@/features/tasks/model/types";

interface MoveTaskVariables {
  boardId: string;
  taskId: string;
  data: MoveTaskRequest;
}

export const useMoveTask = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ boardId, taskId, data }: MoveTaskVariables) => {
      return tasksApi.moveTask(boardId, taskId, data);
    },

    onSuccess: async (_, variables) => {
      await queryClient.invalidateQueries({
        queryKey: ["board", variables.boardId],
      });
    },
  });
};
