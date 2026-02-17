import { useCallback, useEffect, useMemo, useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import Chat from "@/shared/components/Chat";
import { gameHub } from "@/shared/realtime/gameHub";
import type { MessageDto } from "@/shared/types/message.types";
import { chatApi } from "../api/chat.api";
import { usePrivateChat, type ConversationListItem } from "../hooks/usePrivateChat";
import { useAuthStore } from "@/features/auth/store/auth.store";

const formatDate = (isoDate: string | null) => {
  if (!isoDate) return "";
  const parsedDate = new Date(isoDate);
  if (Number.isNaN(parsedDate.getTime())) return "";
  return parsedDate.toLocaleString();
};

const getPreview = (content: string | null, maxLength = 34) => {
  if (!content) return "No messages yet";
  if (content.length <= maxLength) return content;
  return `${content.slice(0, maxLength - 3)}...`;
};

const sortConversations = (items: ConversationListItem[]) =>
  [...items].sort((a, b) => {
    const aTime = a.lastMessageSentAt ? new Date(a.lastMessageSentAt).getTime() : 0;
    const bTime = b.lastMessageSentAt ? new Date(b.lastMessageSentAt).getTime() : 0;

    if (aTime !== bTime) return bTime - aTime;
    return a.user.username.localeCompare(b.user.username);
  });

type PrivateMessagesNavigationState = {
  targetUserId?: string;
  targetUsername?: string;
};

const PrivateMessagesPage = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const myUserId = useAuthStore((state) => state.userId);
  const navState = (location.state as PrivateMessagesNavigationState | null) ?? null;

  const [isLoadingConversations, setIsLoadingConversations] = useState(false);
  const [selectedUserId, setSelectedUserId] = useState<string | null>(null);
  const [conversations, setConversations] = useState<ConversationListItem[]>([]);

  const selectedConversation = useMemo(
    () => conversations.find((conversation) => conversation.user.id === selectedUserId) ?? null,
    [conversations, selectedUserId]
  );

  const chat = usePrivateChat(selectedUserId);

  useEffect(() => {
    let disposed = false;

    const fetchConversations = async () => {
      setIsLoadingConversations(true);
      try {
        const response = await chatApi.getPrivateConversations();
        if (disposed) return;
        const mapped = response.data.map((conversation) => ({
          user: conversation.user,
          lastMessageContent: conversation.lastMessageContent,
          lastMessageSentAt: conversation.lastMessageSentAt,
        }));

        const shouldAddDraftTarget = Boolean(
          navState?.targetUserId &&
          navState.targetUsername &&
          myUserId &&
          navState.targetUserId !== myUserId &&
          !mapped.some((conversation) => conversation.user.id === navState.targetUserId)
        );

        const withDraft = shouldAddDraftTarget
          ? [
              ...mapped,
              {
                user: {
                  id: navState!.targetUserId!,
                  username: navState!.targetUsername!,
                },
                lastMessageContent: null,
                lastMessageSentAt: null,
              },
            ]
          : mapped;

        setConversations(sortConversations(withDraft));
      } finally {
        if (!disposed) setIsLoadingConversations(false);
      }
    };

    void fetchConversations();

    return () => {
      disposed = true;
    };
  }, [myUserId, navState?.targetUserId, navState?.targetUsername]);

  useEffect(() => {
    if (
      navState?.targetUserId &&
      conversations.some((conversation) => conversation.user.id === navState.targetUserId) &&
      selectedUserId !== navState.targetUserId
    ) {
      setSelectedUserId(navState.targetUserId);
      return;
    }

    if (selectedUserId && conversations.some((conversation) => conversation.user.id === selectedUserId)) {
      return;
    }

    if (conversations.length > 0) {
      setSelectedUserId(conversations[0].user.id);
      return;
    }

    setSelectedUserId(null);
  }, [conversations, navState?.targetUserId, selectedUserId]);

  const upsertConversation = useCallback((message: MessageDto) => {
    if (!myUserId) return;

    const peerUserId = message.senderId === myUserId ? message.receiverId : message.senderId;
    if (!peerUserId) return;

    setConversations((current) => {
      const existing = current.find((conversation) => conversation.user.id === peerUserId);
      const peerUser = existing?.user ?? (message.senderId === peerUserId ? message.sender : null);

      if (!peerUser) return current;

      const updatedConversation: ConversationListItem = {
        user: peerUser,
        lastMessageContent: message.content,
        lastMessageSentAt: message.sentAt,
      };

      const filtered = current.filter((conversation) => conversation.user.id !== peerUserId);
      return sortConversations([updatedConversation, ...filtered]);
    });
  }, [myUserId]);

  useEffect(() => {
    const privateMessageHandler = (message: MessageDto) => {
      upsertConversation(message);
    };

    gameHub.onPrivateChat(privateMessageHandler);

    return () => {
      gameHub.offPrivateChat(privateMessageHandler);
    };
  }, [upsertConversation]);

  return (
    <div className="h-screen w-full p-4 md:p-6">
      <div className="mx-auto h-full max-w-6xl rounded-xl border border-white/20 bg-[#091b33]/80 p-3 md:p-4">
        <div className="mb-3 flex items-center justify-between border-b border-white/20 pb-3">
          <h1 className="text-lg md:text-xl font-bold text-white">Private Messages</h1>
          <button
            type="button"
            onClick={() => navigate("/lobby")}
            className="rounded-md border border-amber-300/40 bg-[#4b1812]/70 px-3 py-1.5 text-sm text-amber-100
            hover:text-white hover:border-amber-200 transition-colors cursor-pointer"
          >
            Back to lobby
          </button>
        </div>

        <div className="h-[calc(100%-4.25rem)] flex flex-col md:flex-row gap-3">
          <div className="md:w-80 rounded-lg border border-white/20 bg-[#4b1812]/70 overflow-hidden">
            <div className="border-b border-white/20 px-3 py-2 text-sm font-semibold text-amber-100">
              Players
            </div>
            <div className="h-[13rem] md:h-[calc(100%-2.2rem)] overflow-y-auto">
              {isLoadingConversations ? (
                <p className="px-3 py-3 text-sm text-white/70">Loading players...</p>
              ) : conversations.length === 0 ? (
                <p className="px-3 py-3 text-sm text-white/70">No players available.</p>
              ) : (
                conversations.map((conversation) => (
                  <button
                    key={conversation.user.id}
                    type="button"
                    onClick={() => setSelectedUserId(conversation.user.id)}
                    className={`w-full border-b border-white/10 px-3 py-2 text-left transition-colors cursor-pointer
                    ${selectedUserId === conversation.user.id ? "bg-white/12" : "hover:bg-white/8"}`}
                  >
                    <p className="text-sm font-semibold text-amber-100">{conversation.user.username}</p>
                    <p className="text-xs text-white/70">{getPreview(conversation.lastMessageContent)}</p>
                    <p className="mt-1 text-[11px] text-white/50">{formatDate(conversation.lastMessageSentAt)}</p>
                  </button>
                ))
              )}
            </div>
          </div>

          <div className="flex-1 rounded-lg border border-white/20 bg-[#4b1812]/70">
            {selectedConversation ? (
              <>
                <div className="border-b border-white/20 px-4 py-2 text-sm font-semibold text-amber-100">
                  Chat with {selectedConversation.user.username}
                </div>
                <div className="h-[calc(100%-2.2rem)]">
                  <Chat
                    messages={chat.messages}
                    isLoading={chat.isLoading}
                    isSending={chat.isSending}
                    isLoadingMore={chat.isLoadingMore}
                    canSeeMore={chat.canSeeMore}
                    onSeeMore={chat.onSeeMore}
                    onSend={chat.onSend}
                  />
                </div>
              </>
            ) : (
              <div className="h-full w-full flex items-center justify-center text-white/70 text-sm">
                Select a player to start chatting.
              </div>
            )}
          </div>
        </div>
      </div>
    </div>
  );
};

export default PrivateMessagesPage;
