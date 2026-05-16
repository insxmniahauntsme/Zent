import { httpClient } from "@/shared/api/httpClient";
import type {
  AddProjectRequest,
  AddProjectResponse,
  GetProjectBoardsResponse,
  GetProjectResponse,
  ProjectBoardDto,
  ProjectDto,
  ProjectMemberDto,
  ProjectsResponse,
} from "../model/types";
import type { GetProjectMembersResponse } from "@/features/tasks/model/types";

export const projectsApi = {
  async getTeamProjects(teamId: string): Promise<ProjectDto[]> {
    const response = await httpClient.get<ProjectsResponse>(
      `/teams/${teamId}/projects`,
    );

    return response.data.projects;
  },

  async createProject(
    teamId: string,
    data: AddProjectRequest,
  ): Promise<AddProjectResponse> {
    const response = await httpClient.post<AddProjectResponse>(
      `/teams/${teamId}/projects`,
      data,
    );

    return response.data;
  },

  async getProject(projectId: string): Promise<ProjectDto> {
    const response = await httpClient.get<GetProjectResponse>(
      `/projects/${projectId}`,
    );

    return response.data.project;
  },

  async getProjectBoards(projectId: string): Promise<ProjectBoardDto[]> {
    const response = await httpClient.get<GetProjectBoardsResponse>(
      `/projects/${projectId}/boards`,
    );

    return response.data.boards;
  },

  async getProjectMembers(projectId: string): Promise<ProjectMemberDto[]> {
    const response = await httpClient.get<GetProjectMembersResponse>(
      `/projects/${projectId}/members`,
    );

    return response.data.members ?? [];
  },
};
