import { create } from "zustand";
import { persist } from "zustand/middleware";
import { isTokenExpired, parseJwt } from "../utils/jwt";

type AuthState = {
  token: string | null;
  userId: string | null;
  role: string | null;
  isAuthenticated: boolean;

  setAuth: (data: { token: string; userId: string; role: string }) => void;

  clearAuth: () => void;
};

export const useAuthStore = create<AuthState>()(
  persist(
    (set) => ({
      token: null,
      userId: null,
      role: null,
      isAuthenticated: false,

      setAuth: ({ token, userId, role }) =>
        set({
          token,
          userId,
          role,
          isAuthenticated: true,
        }),

      clearAuth: () =>
        set({
          token: null,
          userId: null,
          role: null,
          isAuthenticated: false,
        }),
    }),
    {
      name: "auth-storage",
      onRehydrateStorage: () => (state) => {
        if (!state?.token) return;

        if (isTokenExpired(state.token)) {
          state.clearAuth();
          return;
        }

        const payload = parseJwt(state.token);
        if (!payload) {
          state.clearAuth();
          return;
        }

        state.isAuthenticated = true;
      },
    }
  )
);
