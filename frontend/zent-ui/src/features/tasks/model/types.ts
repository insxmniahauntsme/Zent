import type { ProjectMemberDto } from "@/features/projects/model/types";

export type TaskPriority = "Low" | "Medium" | "High" | "Critical";

export interface AddTaskRequest {
  title: string;
  description: string | null;
  priority: TaskPriority;
  untilDate: string | null;
  assigneeId: string | null;
}

export interface MoveTaskRequest {
  targetColumnId: string;
  targetOrder: number;
}

export interface GetProjectMembersResponse {
  members: ProjectMemberDto[];
}

export interface TaskAssigneeDto {
  userId: string;
  firstName: string;
  lastName: string;
  email: string;
}

export interface TaskDetailsDto {
  id: string;
  boardId: string;
  boardName: string;
  projectId: string;
  projectName: string;
  columnId: string;
  columnTitle: string;
  creatorId: string;
  assignee: TaskAssigneeDto | null;
  title: string;
  description: string | null;
  priority: TaskPriority;
  createdAt: string;
  untilDate: string | null;
}

export interface TaskDetailsResponse {
  taskDetails: TaskDetailsDto;
}
