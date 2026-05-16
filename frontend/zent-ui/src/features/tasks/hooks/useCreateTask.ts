import { useMutation, useQueryClient } from "@tanstack/react-query";

import { tasksApi } from "../api/tasksApi";
import type { AddTaskRequest } from "../model/types";

interface CreateTaskVariables {
  boardId: string;
  columnId: string;
  data: AddTaskRequest;
}

export const useCreateTask = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({ boardId, columnId, data }: CreateTaskVariables) => {
      return tasksApi.addTask(boardId, columnId, data);
    },

    onSuccess: async (_, variables) => {
      await queryClient.invalidateQueries({
        queryKey: ["board", variables.boardId],
      });
    },
  });
};
