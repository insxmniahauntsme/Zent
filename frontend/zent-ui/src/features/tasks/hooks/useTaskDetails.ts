import { useQuery } from "@tanstack/react-query";

import { tasksApi } from "@/features/tasks/api/tasksApi";

export const useTaskDetails = (taskId?: string) => {
  return useQuery({
    queryKey: ["task", taskId],
    queryFn: () => tasksApi.getTaskDetails(taskId!),
    enabled: Boolean(taskId),
  });
};
