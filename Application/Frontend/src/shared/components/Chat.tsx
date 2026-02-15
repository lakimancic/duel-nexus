import { useMemo, useState } from "react";
import type { MessageDto } from "@/shared/types/message.types";

interface ChatProps {
  messages: MessageDto[];
  isLoading: boolean;
  isSending: boolean;
  isLoadingMore: boolean;
  canSeeMore: boolean;
  onSend: (content: string) => Promise<void>;
  onSeeMore: () => Promise<void>;
}

const formatTime = (isoDate: string) => {
  const date = new Date(isoDate);
  if (Number.isNaN(date.getTime())) return "";
  return date.toLocaleString();
};

const Chat = ({
  messages,
  isLoading,
  isSending,
  isLoadingMore,
  canSeeMore,
  onSend,
  onSeeMore,
}: ChatProps) => {
  const [content, setContent] = useState("");

  const isDisabled = isSending || content.trim().length === 0;

  const chatMessages = useMemo(
    () =>
      messages.map((message, index) => ({
        ...message,
        key: `${message.sender.id}-${message.sentAt}-${index}`,
      })),
    [messages]
  );

  const handleSubmit = async () => {
    const trimmed = content.trim();
    if (!trimmed) return;
    await onSend(trimmed);
    setContent("");
  };

  return (
    <div className="h-full w-full flex flex-col text-white">
      <div className="px-4 pt-4">
        <button
          type="button"
          onClick={() => void onSeeMore()}
          disabled={!canSeeMore || isLoadingMore}
          className="w-full rounded-md border border-white/25 bg-white/10 py-2 text-sm font-semibold
          disabled:opacity-50 disabled:cursor-default hover:bg-white/15 transition-colors cursor-pointer"
        >
          {isLoadingMore ? "Loading..." : "See more"}
        </button>
      </div>

      <div className="flex-1 overflow-y-auto px-4 py-3 flex flex-col justify-end gap-2">
        {isLoading && (
          <p className="text-center text-sm text-white/70 py-2">Loading chat...</p>
        )}
        {!isLoading && chatMessages.length === 0 && (
          <p className="text-center text-sm text-white/70 py-2">No messages yet.</p>
        )}
        {chatMessages.map((message) => (
          <div key={message.key} className="rounded-lg border border-white/20 bg-black/20 px-3 py-2">
            <div className="flex items-center justify-between gap-3 text-xs text-white/65">
              <span className="font-semibold text-amber-200">{message.sender.username}</span>
              <span>{formatTime(message.sentAt)}</span>
            </div>
            <p className="mt-1 wrap-break-word text-sm">{message.content}</p>
          </div>
        ))}
      </div>

      <div className="border-t border-white/20 p-4">
        <div className="flex items-center gap-2">
          <input
            value={content}
            onChange={(event) => setContent(event.target.value)}
            onKeyDown={(event) => {
              if (event.key === "Enter" && !event.shiftKey) {
                event.preventDefault();
                void handleSubmit();
              }
            }}
            placeholder="Write a message..."
            className="flex-1 rounded-md border border-white/30 bg-black/20 px-3 py-2 text-sm outline-none
            placeholder:text-white/40 focus:border-amber-300"
          />
          <button
            type="button"
            onClick={() => void handleSubmit()}
            disabled={isDisabled}
            className="rounded-md bg-amber-500/85 px-4 py-2 text-sm font-semibold text-white
            hover:bg-amber-400 transition-colors cursor-pointer disabled:opacity-50 disabled:cursor-default"
          >
            {isSending ? "Sending..." : "Send"}
          </button>
        </div>
      </div>
    </div>
  );
};

export default Chat;
