import { useCallback, useEffect, useMemo, useRef, useState } from "react";
import { gameHub } from "@/shared/realtime/gameHub";
import type { MessageDto } from "@/shared/types/message.types";
import { friendlyApi } from "../api/friendly.api";
import type { GameRoomChatState } from "../types/friendly.types";

const PAGE_SIZE = 20;

const getMessageKey = (message: MessageDto) =>
  `${message.sender.id}|${message.sentAt}|${message.content}`;

export const useGameRoomChat = (roomId: string): GameRoomChatState => {
  const messageKeysRef = useRef(new Set<string>());
  const nextPageRef = useRef(1);

  const [messages, setMessages] = useState<MessageDto[]>([]);
  const [totalCount, setTotalCount] = useState<number | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [isLoadingMore, setIsLoadingMore] = useState(false);
  const [isSending, setIsSending] = useState(false);

  const addMessages = useCallback(
    (incoming: MessageDto[], placement: "prepend" | "append") => {
      const unique: MessageDto[] = [];

      for (const message of incoming) {
        const key = getMessageKey(message);
        if (messageKeysRef.current.has(key)) continue;
        messageKeysRef.current.add(key);
        unique.push(message);
      }

      if (unique.length === 0) return;

      setMessages((current) =>
        placement === "prepend" ? [...unique, ...current] : [...current, ...unique]
      );
    },
    []
  );

  const fetchPage = useCallback(
    async (page: number, placement: "prepend" | "append") => {
      const response = await friendlyApi.getGameRoomMessages(roomId, {
        page,
        pageSize: PAGE_SIZE,
      });
      const fetchedMessages = [...response.data.items].reverse();
      setTotalCount(response.data.totalCount);
      addMessages(fetchedMessages, placement);
      nextPageRef.current = page + 1;
    },
    [addMessages, roomId]
  );

  useEffect(() => {
    if (!roomId) return;

    let disposed = false;

    messageKeysRef.current = new Set<string>();
    nextPageRef.current = 1;
    setMessages([]);
    setTotalCount(null);

    const fetchInitial = async () => {
      setIsLoading(true);
      try {
        await fetchPage(1, "append");
      } finally {
        if (!disposed) setIsLoading(false);
      }
    };

    const roomMessageHandler = (message: MessageDto) => {
      addMessages([message], "append");
      setTotalCount((current) => (current === null ? current : current + 1));
    };

    void fetchInitial();
    gameHub.onGameRoomChat(roomMessageHandler);

    return () => {
      disposed = true;
      gameHub.offGameRoomChat(roomMessageHandler);
    };
  }, [addMessages, fetchPage, roomId]);

  const onSeeMore = useCallback(async () => {
    if (
      isLoadingMore ||
      isLoading ||
      (totalCount !== null && messages.length >= totalCount)
    ) {
      return;
    }

    setIsLoadingMore(true);
    try {
      await fetchPage(nextPageRef.current, "prepend");
    } finally {
      setIsLoadingMore(false);
    }
  }, [fetchPage, isLoading, isLoadingMore, messages.length, totalCount]);

  const onSend = useCallback(
    async (content: string) => {
      setIsSending(true);
      try {
        await gameHub.sendGameRoomChat(roomId, content);
      } finally {
        setIsSending(false);
      }
    },
    [roomId]
  );

  const canSeeMore = useMemo(() => {
    if (isLoading || totalCount === null) return false;
    return messages.length < totalCount;
  }, [isLoading, messages.length, totalCount]);

  return {
    messages,
    isLoading,
    isLoadingMore,
    isSending,
    canSeeMore,
    onSeeMore,
    onSend,
  };
};
