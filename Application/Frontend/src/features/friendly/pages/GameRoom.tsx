import { AxiosError } from "axios";
import { useCallback, useEffect, useMemo, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import Logo from "@/assets/images/logo.png";
import Chat from "@/shared/components/Chat";
import { useAuthStore } from "@/features/auth/store/auth.store";
import { gameHub } from "@/shared/realtime/gameHub";
import type { ErrorMessage } from "@/shared/types/error.types";
import { friendlyApi } from "../api/friendly.api";
import { useGameRoomChat } from "../hooks/useGameRoomChat";
import type { DeckDto, GameRoomDto, GameRoomPlayerDto } from "../types/friendly.types";

type TabValue = "chat" | "players";

const GameRoomPage = () => {
  const { roomId } = useParams();
  const navigate = useNavigate();
  const currentUserId = useAuthStore((state) => state.userId);

  const [tab, setTab] = useState<TabValue>("chat");
  const [room, setRoom] = useState<GameRoomDto | null>(null);
  const [players, setPlayers] = useState<GameRoomPlayerDto[]>([]);
  const [decks, setDecks] = useState<DeckDto[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [isDeckLoading, setIsDeckLoading] = useState(false);
  const [isDeckUpdating, setIsDeckUpdating] = useState(false);
  const [isLeaving, setIsLeaving] = useState(false);
  const [isCancelling, setIsCancelling] = useState(false);
  const [isStarting, setIsStarting] = useState(false);
  const [statusMessage, setStatusMessage] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);

  const safeRoomId = roomId ?? "";
  const chat = useGameRoomChat(safeRoomId);

  const fetchRoomState = useCallback(async () => {
    if (!roomId) return;

    const [roomResponse, playersResponse] = await Promise.all([
      friendlyApi.getRoomById(roomId),
      friendlyApi.getRoomPlayers(roomId),
    ]);

    setRoom(roomResponse.data);
    setPlayers(playersResponse.data);
  }, [roomId]);

  const fetchDecks = useCallback(async () => {
    setIsDeckLoading(true);
    try {
      const response = await friendlyApi.getMyCompleteDecks();
      setDecks(response.data);
    } finally {
      setIsDeckLoading(false);
    }
  }, []);

  useEffect(() => {
    if (!roomId) {
      navigate("/friendly", { replace: true });
      return;
    }

    let disposed = false;

    const load = async () => {
      setIsLoading(true);
      setError(null);
      try {
        await Promise.all([fetchRoomState(), fetchDecks()]);
        await gameHub.joinGameRoom(roomId);
      } catch (err) {
        if (!disposed && err instanceof AxiosError) {
          const data = err.response?.data as ErrorMessage | undefined;
          setError(data?.error ?? "Failed to load room.");
        }
      } finally {
        if (!disposed) setIsLoading(false);
      }
    };

    const playersUpdatedHandler = (updatedRoomId: string) => {
      if (updatedRoomId !== roomId) return;
      void fetchRoomState();
    };

    const roomCancelledHandler = (cancelledRoomId: string) => {
      if (cancelledRoomId !== roomId) return;
      navigate("/friendly", {
        replace: true,
        state: { message: "Room was canceled." },
      });
    };

    void load();
    gameHub.onGameRoomPlayersUpdated(playersUpdatedHandler);
    gameHub.onGameRoomCancelled(roomCancelledHandler);

    return () => {
      disposed = true;
      gameHub.offGameRoomPlayersUpdated(playersUpdatedHandler);
      gameHub.offGameRoomCancelled(roomCancelledHandler);
      void gameHub.leaveGameRoom(roomId);
    };
  }, [fetchDecks, fetchRoomState, navigate, roomId]);

  const myPlayer = useMemo(
    () => players.find((player) => player.user.id === currentUserId),
    [currentUserId, players]
  );

  const isHost = room?.hostUserId === currentUserId;
  const selectedDeckId = myPlayer?.deck?.id ?? null;
  const canStart = players.length >= 2 && players.every((player) => player.isReady);

  const onSelectDeck = async (deckId: string) => {
    if (!roomId || isDeckUpdating) return;

    setError(null);
    setStatusMessage(null);
    setIsDeckUpdating(true);

    try {
      await friendlyApi.setMyDeck(roomId, deckId);
      await fetchRoomState();
    } catch (err) {
      if (err instanceof AxiosError) {
        const data = err.response?.data as ErrorMessage | undefined;
        setError(data?.error ?? "Failed to select deck.");
      }
    } finally {
      setIsDeckUpdating(false);
    }
  };

  const handleLeave = async () => {
    if (!roomId || isLeaving) return;

    setIsLeaving(true);
    try {
      const response = await friendlyApi.leaveRoom(roomId);
      navigate("/friendly", {
        replace: true,
        state: {
          message: response.data.cancelled
            ? "Room was canceled."
            : "You left the room.",
        },
      });
    } catch (err) {
      if (err instanceof AxiosError) {
        const data = err.response?.data as ErrorMessage | undefined;
        setError(data?.error ?? "Failed to leave room.");
      }
    } finally {
      setIsLeaving(false);
    }
  };

  const handleCancel = async () => {
    if (!roomId || isCancelling) return;

    setIsCancelling(true);
    try {
      await friendlyApi.cancelRoom(roomId);
      navigate("/friendly", {
        replace: true,
        state: { message: "Room was canceled." },
      });
    } catch (err) {
      if (err instanceof AxiosError) {
        const data = err.response?.data as ErrorMessage | undefined;
        setError(data?.error ?? "Failed to cancel room.");
      }
    } finally {
      setIsCancelling(false);
    }
  };

  const handleStartGame = async () => {
    if (!roomId || isStarting || !canStart) return;

    setIsStarting(true);
    setStatusMessage(null);
    try {
      const response = await friendlyApi.startRoomGame(roomId);
      setStatusMessage(`Game started. Game ID: ${response.data.gameId}`);
      await fetchRoomState();
    } catch (err) {
      if (err instanceof AxiosError) {
        const data = err.response?.data as ErrorMessage | undefined;
        setError(data?.error ?? "Failed to start game.");
      }
    } finally {
      setIsStarting(false);
    }
  };

  return (
    <div className="h-screen w-full p-4 md:p-6">
      <div className="h-full w-full grid grid-cols-1 md:grid-cols-9 gap-4 text-white">
        <div className="md:col-span-4 rounded-xl bg-[#4b1812]/85 border border-amber-300/20 p-4 flex flex-col gap-4 overflow-hidden">
          <img src={Logo} alt="logo" className="w-[70%] mx-auto" />

          <div className="rounded-lg border border-white/15 bg-black/20 p-3 text-sm">
            <p>
              <span className="text-white/70">Room code:</span>{" "}
              <span className="font-bold tracking-[0.2em]">{room?.joinCode ?? "------"}</span>
            </p>
            <p>
              <span className="text-white/70">Host:</span> {room?.hostUser.username ?? "-"}
            </p>
            <p>
              <span className="text-white/70">Players:</span> {players.length}
            </p>
          </div>

          <div className="flex-1 min-h-0 rounded-lg border border-white/15 bg-black/20 p-3 overflow-y-auto">
            <p className="text-amber-200 font-semibold mb-2">Choose Deck</p>
            {isDeckLoading && <p className="text-sm text-white/70">Loading decks...</p>}
            {!isDeckLoading && decks.length === 0 && (
              <p className="text-sm text-red-300">
                You don&apos;t have complete decks.
              </p>
            )}
            <div className="flex flex-col gap-2">
              {decks.map((deck) => {
                const selected = selectedDeckId === deck.id;
                return (
                  <button
                    key={deck.id}
                    type="button"
                    onClick={() => void onSelectDeck(deck.id)}
                    disabled={isDeckUpdating}
                    className={`text-left rounded-md border px-3 py-2 transition-colors cursor-pointer disabled:opacity-60 disabled:cursor-default ${
                      selected
                        ? "border-amber-300 bg-amber-500/25"
                        : "border-white/20 bg-white/5 hover:bg-white/10"
                    }`}
                  >
                    <span className="font-semibold">{deck.name}</span>
                    {selected && (
                      <span className="text-xs text-amber-100/90 ml-2">(Selected)</span>
                    )}
                  </button>
                );
              })}
            </div>
          </div>

          <div className="flex flex-col gap-2">
            {!isHost && (
              <button
                type="button"
                onClick={() => void handleLeave()}
                disabled={isLeaving}
                className="rounded-md bg-white/10 border border-white/25 py-2 font-semibold cursor-pointer hover:bg-white/15 disabled:opacity-60 disabled:cursor-default"
              >
                {isLeaving ? "Leaving..." : "Leave Room"}
              </button>
            )}

            {isHost && (
              <>
                <button
                  type="button"
                  onClick={() => void handleStartGame()}
                  disabled={isStarting || !canStart}
                  className="rounded-md bg-amber-500/85 py-2 font-semibold cursor-pointer hover:bg-amber-400 disabled:opacity-60 disabled:cursor-default"
                >
                  {isStarting ? "Starting..." : "Start Game"}
                </button>
                <button
                  type="button"
                  onClick={() => void handleCancel()}
                  disabled={isCancelling}
                  className="rounded-md bg-red-500/80 py-2 font-semibold cursor-pointer hover:bg-red-400 disabled:opacity-60 disabled:cursor-default"
                >
                  {isCancelling ? "Cancelling..." : "Cancel Room"}
                </button>
              </>
            )}
          </div>
        </div>

        <div className="md:col-span-5 rounded-xl bg-[#091b33]/85 border border-white/20 min-h-0 flex flex-col">
          <div className="flex border-b border-white/20">
            <button
              type="button"
              disabled={tab === "chat"}
              onClick={() => setTab("chat")}
              className="flex-1 py-3 disabled:text-white text-white/70 hover:text-white cursor-pointer"
            >
              Room Chat
            </button>
            <button
              type="button"
              disabled={tab === "players"}
              onClick={() => setTab("players")}
              className="flex-1 py-3 disabled:text-white text-white/70 hover:text-white cursor-pointer"
            >
              Players
            </button>
          </div>

          {isLoading && (
            <div className="p-4 text-sm text-white/70">Loading room...</div>
          )}

          {!isLoading && tab === "chat" && (
            <div className="flex-1 min-h-0">
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
          )}

          {!isLoading && tab === "players" && (
            <div className="flex-1 min-h-0 overflow-y-auto p-4 flex flex-col gap-2">
              {players.map((player) => (
                <div
                  key={player.id}
                  className="rounded-lg border border-white/20 bg-black/20 px-3 py-2"
                >
                  <div className="flex justify-between items-center gap-3">
                    <span className="font-semibold text-amber-100">
                      {player.user.username}
                      {player.user.id === room?.hostUserId && (
                        <span className="text-xs text-amber-200/80 ml-2">(Host)</span>
                      )}
                    </span>
                    <span
                      className={`text-xs font-semibold ${
                        player.isReady ? "text-green-300" : "text-white/60"
                      }`}
                    >
                      {player.isReady ? "Ready" : "Not ready"}
                    </span>
                  </div>
                  <p className="text-xs text-white/70 mt-1">
                    Deck: {player.deck?.name ?? "No deck selected"}
                  </p>
                </div>
              ))}
            </div>
          )}

          {(error || statusMessage) && (
            <div className="border-t border-white/20 px-4 py-3 text-sm">
              {error && <p className="text-red-300">{error}</p>}
              {statusMessage && <p className="text-emerald-300">{statusMessage}</p>}
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default GameRoomPage;
