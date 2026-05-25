import { useQuery } from "@tanstack/react-query";

import { tasksApi } from "../api/tasksApi";

export const useTaskDetails = (taskId: string | undefined) => {
  return useQuery({
    queryKey: ["task", taskId],
    queryFn: () => tasksApi.getTaskDetails(taskId!),
    enabled: Boolean(taskId),
  });
};
