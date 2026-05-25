import { useMutation, useQueryClient } from "@tanstack/react-query";

import { tasksApi } from "../api/tasksApi";

interface RemoveTaskAssigneeVariables {
  boardId: string;
  taskId: string;
}

export const useRemoveTaskAssignee = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ boardId, taskId }: RemoveTaskAssigneeVariables) => {
      return tasksApi.removeTaskAssignee(boardId, taskId);
    },

    onSuccess: async (_, variables) => {
      await Promise.all([
        queryClient.invalidateQueries({
          queryKey: ["task", variables.taskId],
        }),

        queryClient.invalidateQueries({
          queryKey: ["board", variables.boardId],
        }),
      ]);
    },
  });
};
