import { useMutation, useQueryClient } from "@tanstack/react-query";

import { tasksApi } from "../api/tasksApi";

interface AddTaskAssigneeVariables {
  boardId: string;
  taskId: string;
  assigneeId: string;
}

export const useAddTaskAssignee = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ boardId, taskId, assigneeId }: AddTaskAssigneeVariables) => {
      return tasksApi.addTaskAssignee(boardId, taskId, { assigneeId });
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
