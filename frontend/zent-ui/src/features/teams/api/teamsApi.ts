import { httpClient } from "@/shared/api/httpClient";
import type {
  AddTeamRequest,
  GetTeamResponse,
  TeamDto,
  TeamMemberDto,
  TeamsResponse,
  UserTeamDto,
} from "../model/types";

export const teamsApi = {
  async getMyTeams(): Promise<UserTeamDto[]> {
    const response = await httpClient.get<TeamsResponse>("/teams/my");
    return response.data.teams;
  },

  async getTeam(teamId: string): Promise<TeamDto> {
    const response = await httpClient.get<GetTeamResponse>(`/teams/${teamId}`);
    return response.data.team;
  },

  async createTeam(data: AddTeamRequest): Promise<{ id: string }> {
    const response = await httpClient.post("/teams", data);
    return response.data;
  },

  async searchTeamMembers(
    teamId: string,
    query: string,
  ): Promise<TeamMemberDto[]> {
    const response = await httpClient.get<{ members: TeamMemberDto[] }>(
      `/teams/${teamId}/members/search`,
      {
        params: { query },
      },
    );

    return response.data.members;
  },
};
