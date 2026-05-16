import { httpClient } from "@/shared/api/httpClient";
import type {
  LoginRequestDto,
  RegisterRequestDto,
  AuthResponseDto,
} from "../model/types";

export const authApi = {
  login: async (payload: LoginRequestDto) => {
    const response = await httpClient.post<AuthResponseDto>(
      "/auth/login",
      payload,
    );
    return response.data;
  },

  register: async (payload: RegisterRequestDto) => {
    const response = await httpClient.post<AuthResponseDto>(
      "/auth/register",
      payload,
    );
    return response.data;
  },
};
