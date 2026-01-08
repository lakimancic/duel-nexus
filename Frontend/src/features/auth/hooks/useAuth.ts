import { authApi } from "../api/auth.api";
import { useAuthStore } from "../store/auth.store";
import type { LoginRequest, RegisterRequest } from "../types/auth.types";

export const useAuth = () => {
  const setAuth = useAuthStore((state) => state.setAuth);
  const clearAuth = useAuthStore((state) => state.clearAuth);

  const login = async (request: LoginRequest) => {
    const result = await authApi.login(request);
    setAuth({
      token: result.token,
      userId: result.userId,
      role: result.role,
    });
  };

  const register = async (request: RegisterRequest) => {
    const result = await authApi.register(request);
    setAuth({
      token: result.token,
      userId: result.userId,
      role: result.role,
    });
  };

  const logout = () => {
    clearAuth();
  };

  return {
    login,
    register,
    logout,
  };
};
