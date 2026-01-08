import { httpClient } from "../../../shared/api/httpClient";
import type {
  LoginRequest,
  RegisterRequest,
  AuthResponse,
} from "../types/auth.types";

export const authApi = {
  login: async (request: LoginRequest) => {
    const response = await httpClient.post<AuthResponse>(
      "/auth/login",
      request
    );
    return response.data;
  },

  register: async (request: RegisterRequest) => {
    const response = await httpClient.post<AuthResponse>(
      "/auth/register",
      request
    );
    return response.data;
  },
};
