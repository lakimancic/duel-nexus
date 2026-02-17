import { useCallback, useEffect, useMemo, useState } from "react";
import { connectionApi } from "../api/connection.api";
import { gameHub } from "@/shared/realtime/gameHub";
import type { ShortUserDto } from "@/shared/types/user.types";

const sortUsers = (users: ShortUserDto[]) =>
  [...users].sort((a, b) => a.username.localeCompare(b.username));

export const useActiveUsers = () => {
  const [activeUsers, setActiveUsers] = useState<ShortUserDto[]>([]);
  const [isLoading, setIsLoading] = useState(false);

  const addUser = useCallback((user: ShortUserDto | null) => {
    if (!user) return;

    setActiveUsers((current) => {
      if (current.some((existingUser) => existingUser.id === user.id))
        return current;
      return sortUsers([...current, user]);
    });
  }, []);

  const removeUser = useCallback((userId: string | null) => {
    if (!userId) return;

    setActiveUsers((current) => current.filter((user) => user.id !== userId));
  }, []);

  useEffect(() => {
    let disposed = false;

    const fetchActiveUsers = async (showLoader = true) => {
      if (showLoader) setIsLoading(true);
      try {
        const response = await connectionApi.getOnlineUsers();
        if (disposed) return;
        setActiveUsers(sortUsers(response.data));
      } finally {
        if (!disposed && showLoader) setIsLoading(false);
      }
    };

    void fetchActiveUsers();
    const delayedSync = window.setTimeout(() => {
      void fetchActiveUsers(false);
    }, 1200);
    gameHub.onUserConnected(addUser);
    gameHub.onUserDisconnected(removeUser);

    return () => {
      disposed = true;
      window.clearTimeout(delayedSync);
      gameHub.offUserConnected(addUser);
      gameHub.offUserDisconnected(removeUser);
    };
  }, [addUser, removeUser]);

  const count = useMemo(() => activeUsers.length, [activeUsers.length]);

  return {
    activeUsers,
    isLoading,
    count,
  };
};
