import { useCallback, useEffect, useMemo, useRef, useState } from "react";
import { chatApi } from "@/features/lobby/api/chat.api";
import { useAuthStore } from "@/features/auth/store/auth.store";
import { gameHub } from "@/shared/realtime/gameHub";
import type { MessageDto, PrivateConversationDto } from "@/shared/types/message.types";

const PAGE_SIZE = 20;

const getMessageKey = (message: MessageDto) =>
  `${message.senderId}|${message.receiverId}|${message.sentAt}|${message.content}`;

const belongsToThread = (message: MessageDto, myUserId: string | null, otherUserId: string | null) => {
  if (!myUserId || !otherUserId) return false;
  return (
    (message.senderId === myUserId && message.receiverId === otherUserId) ||
    (message.senderId === otherUserId && message.receiverId === myUserId)
  );
};

export type ConversationListItem = {
  user: PrivateConversationDto["user"];
  lastMessageContent: string | null;
  lastMessageSentAt: string | null;
};

export const usePrivateChat = (selectedUserId: string | null) => {
  const myUserId = useAuthStore((state) => state.userId);
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
      if (!selectedUserId) return;

      const response = await chatApi.getPrivateMessages(selectedUserId, {
        page,
        pageSize: PAGE_SIZE,
      });

      const fetchedMessages = [...response.data.items].reverse();
      setTotalCount(response.data.totalCount);
      addMessages(fetchedMessages, placement);
      nextPageRef.current = page + 1;
    },
    [addMessages, selectedUserId]
  );

  useEffect(() => {
    if (!selectedUserId) {
      messageKeysRef.current = new Set<string>();
      nextPageRef.current = 1;
      setMessages([]);
      setTotalCount(null);
      setIsLoading(false);
      setIsLoadingMore(false);
      return;
    }

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

    const privateMessageHandler = (message: MessageDto) => {
      if (!belongsToThread(message, myUserId, selectedUserId)) return;

      addMessages([message], "append");
      setTotalCount((current) => (current === null ? current : current + 1));
    };

    void fetchInitial();
    gameHub.onPrivateChat(privateMessageHandler);

    return () => {
      disposed = true;
      gameHub.offPrivateChat(privateMessageHandler);
    };
  }, [addMessages, fetchPage, myUserId, selectedUserId]);

  const onSeeMore = useCallback(async () => {
    if (
      !selectedUserId ||
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
  }, [fetchPage, isLoading, isLoadingMore, messages.length, selectedUserId, totalCount]);

  const onSend = useCallback(
    async (content: string) => {
      if (!selectedUserId) return;

      setIsSending(true);
      try {
        await gameHub.sendPrivateChat(selectedUserId, content);
      } finally {
        setIsSending(false);
      }
    },
    [selectedUserId]
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
