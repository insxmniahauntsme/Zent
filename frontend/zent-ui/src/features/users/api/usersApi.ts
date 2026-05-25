import { httpClient } from "@/shared/api/httpClient";
import type {
  CurrentUserDto,
  SearchUsersResponse,
  UserSearchDto,
} from "../model/types";

export const usersApi = {
  async getCurrentUser(): Promise<CurrentUserDto> {
    const response = await httpClient.get<CurrentUserDto>("/users/me");
    return response.data;
  },

  async searchUsers(teamId: string, query: string): Promise<UserSearchDto[]> {
    const response = await httpClient.get<SearchUsersResponse>(
      `/teams/${teamId}/users`,
      {
        params: { query },
      },
    );

    return response.data.users ?? [];
  },
};
