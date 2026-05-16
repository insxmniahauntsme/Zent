import type { Specialization, TeamRole } from "@/features/teams/model/types";

export interface ProjectDto {
  id: string;
  teamId: string;
  name: string;
  description?: string;
  client?: string;
  membersCount: number;
  boardsCount: number;
}

export interface ProjectsResponse {
  projects: ProjectDto[];
}

export interface AddProjectRequest {
  name: string;
  description?: string;
  client?: string;
  memberIds?: string[];
}

export interface AddProjectResponse {
  id: string;
}

export interface GetProjectResponse {
  project: ProjectDto;
}

export interface ProjectBoardDto {
  id: string;
  name: string;
}

export interface GetProjectBoardsResponse {
  boards: ProjectBoardDto[];
}

export interface ProjectMemberDto {
  userId: string;
  firstName: string;
  lastName: string;
  email: string;
  role: TeamRole;
  specialization: Specialization;
}
