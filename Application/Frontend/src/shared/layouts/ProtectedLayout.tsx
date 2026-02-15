import { Navigate, Outlet } from "react-router-dom";
import { useEffect, useState } from "react";
import { useAuthStore } from "@/features/auth/store/auth.store";
import { gameHub } from "@/shared/realtime/gameHub";

export const ProtectedLayout = () => {
  const isAuthenticated = useAuthStore((state) => state.isAuthenticated);
  const token = useAuthStore((state) => state.token);
  const [hydrated, setHydrated] = useState(false);

  useEffect(() => {
    const timeout = setTimeout(() => setHydrated(true), 0);
    return () => clearTimeout(timeout);
  }, []);

  useEffect(() => {
    if (!hydrated || !isAuthenticated || !token) return;

    gameHub.start();

    return () => {
      gameHub.stop();
    };
  }, [hydrated, isAuthenticated, token]);

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  return <Outlet />;
};
