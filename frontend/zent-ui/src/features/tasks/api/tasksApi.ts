import { httpClient } from "@/shared/api/httpClient";
import type {
  AddTaskAssigneeRequest,
  AddTaskRequest,
  MoveTaskRequest,
  TaskDetailsDto,
  TaskDetailsResponse,
} from "@/features/tasks/model/types";

export const tasksApi = {
  async getTaskDetails(taskId: string): Promise<TaskDetailsDto> {
    const response = await httpClient.get<TaskDetailsResponse>(
      `/tasks/${taskId}`,
    );

    return response.data.taskDetails;
  },

  async addTask(
    boardId: string,
    columnId: string,
    data: AddTaskRequest,
  ): Promise<string> {
    const response = await httpClient.post<string>(
      `/boards/${boardId}/columns/${columnId}/tasks`,
      data,
    );

    return response.data;
  },

  async moveTask(
    boardId: string,
    taskId: string,
    data: MoveTaskRequest,
  ): Promise<void> {
    await httpClient.patch(`/boards/${boardId}/tasks/${taskId}/move`, data);
  },

  async addTaskAssignee(
    boardId: string,
    taskId: string,
    data: AddTaskAssigneeRequest,
  ): Promise<void> {
    await httpClient.patch(
      `/boards/${boardId}/tasks/${taskId}/assignee/`,
      data,
    );
  },

  async removeTaskAssignee(boardId: string, taskId: string): Promise<void> {
    await httpClient.delete(
      `/boards/${boardId}/tasks/${taskId}/assignee/remove`,
    );
  },
};
