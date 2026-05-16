import type { TaskPriority } from "@/features/tasks/model/types";

export interface BoardTaskDto {
  id: string;
  columnId: string;
  creatorId: string;
  assignee: BoardTaskAssigneeDto | null;
  title: string;
  description?: string | null;
  order: number;
  priority: TaskPriority;
  createdAt: string;
  untilDate: string | null;
}

export interface BoardTaskAssigneeDto {
  userId: string;
  firstName: string;
  lastName: string;
  email: string;
}

export interface BoardColumnDto {
  id: string;
  title: string;
  order: number;
  isFinal: boolean;
  tasks: BoardTaskDto[];
}

export interface BoardDto {
  id: string;
  projectId: string;
  name: string;
  description?: string;
  createdAt: string;
  updatedAt: string;
  columns: BoardColumnDto[];
}

export interface GetBoardResponse {
  board: BoardDto;
}

export interface AddBoardRequest {
  name: string;
  description?: string | null;
}

export interface AddBoardResponse {
  boardId: string;
}

export interface AddColumnRequest {
  isFinal: boolean;
  title?: string;
}

export interface UpdateColumnRequest {
  title: string;
  isFinal: boolean;
}

export interface MoveColumnRequest {
  targetOrder: number;
}
