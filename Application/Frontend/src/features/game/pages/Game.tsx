import Board from "@/features/game/components/Board";
import TurnStatus from "@/features/game/components/TurnStatus";
import { gameApi } from "@/features/game/api/game.api";
import { TurnPhase, type GameCardDto } from "@/features/game/types/game.types";
import { useAuthStore } from "@/features/auth/store/auth.store";
import { gameHub } from "@/shared/realtime/gameHub";
import type { CardDto } from "@/shared/types/card.types";
import type { ErrorMessage } from "@/shared/types/error.types";
import { AxiosError } from "axios";
import { useCallback, useEffect, useMemo, useRef, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";

const ZONE_FIELD = 0;
const ZONE_HAND = 1;
const TOP_ROW_MAX_INDEX = 4;
const CARD_TYPE_MONSTER = 0;
const CARD_TYPE_SPELL = 1;
const CARD_TYPE_TRAP = 2;
const TURN_ANNOUNCEMENT_DURATION_MS = 1100;
const DRAW_PHASE_TIMEOUT_SECONDS = 20;
const MAIN1_PHASE_TIMEOUT_SECONDS = 60;

const PHASE_LABELS: Record<number, string> = {
  [TurnPhase.Draw]: "Draw",
  [TurnPhase.Main1]: "Main1",
  [TurnPhase.Battle]: "Battle",
  [TurnPhase.Main2]: "Main2",
  [TurnPhase.End]: "End",
};

const PHASE_COLOR_CLASSES: Record<number, string> = {
  [TurnPhase.Draw]: "text-cyan-300 drop-shadow-[0_0_18px_rgba(34,211,238,0.95)]",
  [TurnPhase.Main1]: "text-lime-300 drop-shadow-[0_0_18px_rgba(190,242,100,0.95)]",
  [TurnPhase.Battle]: "text-rose-400 drop-shadow-[0_0_18px_rgba(251,113,133,0.95)]",
  [TurnPhase.Main2]: "text-fuchsia-400 drop-shadow-[0_0_18px_rgba(232,121,249,0.95)]",
  [TurnPhase.End]: "text-yellow-300 drop-shadow-[0_0_18px_rgba(253,224,71,0.95)]",
};

const GamePage = () => {
  const { gameId } = useParams();
  const navigate = useNavigate();
  const currentUserId = useAuthStore((state) => state.userId);

  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [hoveredCard, setHoveredCard] = useState<CardDto | null>(null);
  const [cards, setCards] = useState<GameCardDto[]>([]);
  const [playerOrder, setPlayerOrder] = useState<string[]>([]);
  const [viewerPlayerId, setViewerPlayerId] = useState<string | null>(null);
  const [viewerDrawsInTurn, setViewerDrawsInTurn] = useState(0);
  const [viewerTurnEnded, setViewerTurnEnded] = useState(false);
  const [activePlayerId, setActivePlayerId] = useState<string | null>(null);
  const [activePlayerLabel, setActivePlayerLabel] = useState<string>("-");
  const [phase, setPhase] = useState<number>(0);
  const [phaseStartedAt, setPhaseStartedAt] = useState<string | null>(null);
  const [isSubmittingDrawAction, setIsSubmittingDrawAction] = useState(false);
  const [isSubmittingMainAction, setIsSubmittingMainAction] = useState(false);
  const [selectedHandCardId, setSelectedHandCardId] = useState<string | null>(null);
  const [placeFaceDown, setPlaceFaceDown] = useState(false);
  const [pendingRevealCardId, setPendingRevealCardId] = useState<string | null>(null);
  const [turnAnnouncement, setTurnAnnouncement] = useState<{
    title: string;
    subtitle?: string;
    colorClass: string;
  } | null>(null);

  const previousTurnStatusRef = useRef<{ activePlayerId: string; phase: number } | null>(null);
  const announcementTimeoutRef = useRef<ReturnType<typeof setTimeout> | null>(null);

  const safeGameId = gameId ?? "";

  const fetchGameState = useCallback(async () => {
    if (!safeGameId) return;

    const response = await gameApi.getGameState(safeGameId);
    const data = response.data;

    const mappedCards: GameCardDto[] = data.cards.map((card) => ({
      id: card.id,
      playerId: card.playerGameId,
      zone: card.zone,
      deckOrder: card.deckOrder,
      isFaceDown: card.isFaceDown,
      fieldIndex: card.fieldIndex,
      defensePosition: card.defensePosition,
      card: card.card,
    }));

    const playerNameByPlayerGameId = new Map(
      data.players.map((player) => [player.id, player.user.username])
    );
    const orderedPlayerIds = [...data.players]
      .sort((a, b) => a.index - b.index)
      .map((player) => player.id);
    const activePlayerId = data.currentTurn.activePlayerId;

    setCards(mappedCards);
    setPlayerOrder(orderedPlayerIds);
    setViewerPlayerId(data.viewerPlayerId);
    setViewerDrawsInTurn(data.viewerDrawsInTurn ?? 0);
    setViewerTurnEnded(
      data.players.find((player) => player.id === data.viewerPlayerId)?.turnEnded ?? false
    );
    setActivePlayerId(data.currentTurn.activePlayerId);
    setPhase(Number(data.currentTurn.phase));
    setPhaseStartedAt(data.currentTurn.startedAt ?? null);
    setActivePlayerLabel(
      activePlayerId ? (playerNameByPlayerGameId.get(activePlayerId) ?? activePlayerId) : "-"
    );
  }, [safeGameId]);

  useEffect(() => {
    if (!safeGameId) {
      navigate("/friendly", { replace: true });
      return;
    }

    let disposed = false;

    const load = async () => {
      setIsLoading(true);
      setError(null);
      try {
        await fetchGameState();
      } catch (err) {
        if (!disposed && err instanceof AxiosError) {
          const data = err.response?.data as ErrorMessage | undefined;
          setError(data?.error ?? "Failed to load game state.");
        }
      } finally {
        if (!disposed) setIsLoading(false);
      }
    };

    const onStateMayHaveChanged = (..._args: unknown[]) => {
      void fetchGameState();
    };

    const pollStateInterval = setInterval(() => {
      if (!disposed) {
        void fetchGameState();
      }
    }, 1000);

    void load();
    void gameHub.joinGame(safeGameId);

    gameHub.onDrawResult(onStateMayHaveChanged);
    gameHub.onPlayerDrew(onStateMayHaveChanged);
    gameHub.onSkipDrawResult(onStateMayHaveChanged);
    gameHub.onPlayerSkippedDraw(onStateMayHaveChanged);
    gameHub.onPlaceResult(onStateMayHaveChanged);
    gameHub.onPlayerPlaced(onStateMayHaveChanged);
    gameHub.onNextResult(onStateMayHaveChanged);
    gameHub.onPhaseAdvanced(onStateMayHaveChanged);
    gameHub.onGraveResult(onStateMayHaveChanged);
    gameHub.onToggleDefenseResult(onStateMayHaveChanged);
    gameHub.onRevealResult(onStateMayHaveChanged);
    gameHub.onPlayerCardUpdated(onStateMayHaveChanged);

    return () => {
      disposed = true;
      gameHub.offDrawResult(onStateMayHaveChanged);
      gameHub.offPlayerDrew(onStateMayHaveChanged);
      gameHub.offSkipDrawResult(onStateMayHaveChanged);
      gameHub.offPlayerSkippedDraw(onStateMayHaveChanged);
      gameHub.offPlaceResult(onStateMayHaveChanged);
      gameHub.offPlayerPlaced(onStateMayHaveChanged);
      gameHub.offNextResult(onStateMayHaveChanged);
      gameHub.offPhaseAdvanced(onStateMayHaveChanged);
      gameHub.offGraveResult(onStateMayHaveChanged);
      gameHub.offToggleDefenseResult(onStateMayHaveChanged);
      gameHub.offRevealResult(onStateMayHaveChanged);
      gameHub.offPlayerCardUpdated(onStateMayHaveChanged);
      void gameHub.leaveGame(safeGameId);
      clearInterval(pollStateInterval);
    };
  }, [fetchGameState, navigate, safeGameId]);

  const turnStatus = useMemo(
    () => ({ activePlayerId: activePlayerLabel, phase }),
    [activePlayerLabel, phase]
  );

  useEffect(() => {
    const previous = previousTurnStatusRef.current;
    const current = { activePlayerId: activePlayerLabel, phase: Number(phase) };

    if (!previous) {
      previousTurnStatusRef.current = current;
      return;
    }

    const phaseChanged = previous.phase !== current.phase;
    const playerChanged = previous.activePlayerId !== current.activePlayerId;
    if (!phaseChanged && !playerChanged) return;

    const phaseLabel = PHASE_LABELS[current.phase] ?? `Phase ${current.phase}`;
    const colorClass = PHASE_COLOR_CLASSES[current.phase] ?? "text-cyan-100";
    const title = `${current.activePlayerId} ${phaseLabel} Phase`;
    const subtitle = playerChanged && !phaseChanged ? "Player Turn Changed" : undefined;

    setTurnAnnouncement({ title, subtitle, colorClass });
    previousTurnStatusRef.current = current;

    if (announcementTimeoutRef.current) {
      clearTimeout(announcementTimeoutRef.current);
    }
    announcementTimeoutRef.current = setTimeout(() => {
      setTurnAnnouncement(null);
      announcementTimeoutRef.current = null;
    }, TURN_ANNOUNCEMENT_DURATION_MS);
  }, [activePlayerLabel, phase]);

  useEffect(() => {
    return () => {
      if (announcementTimeoutRef.current) {
        clearTimeout(announcementTimeoutRef.current);
      }
    };
  }, []);

  const phaseNumber = Number(phase);
  const canViewerDraw =
    Boolean(viewerPlayerId) &&
    !viewerTurnEnded &&
    phaseNumber === TurnPhase.Draw &&
    viewerDrawsInTurn < 2;
  const canViewerPlayMain1 = Boolean(viewerPlayerId) && !viewerTurnEnded && phaseNumber === TurnPhase.Main1;
  const canViewerAdvancePhase =
    Boolean(viewerPlayerId) &&
    (
      ((phaseNumber === TurnPhase.Draw || phaseNumber === TurnPhase.Main1) && !viewerTurnEnded) ||
      (phaseNumber === TurnPhase.Battle && viewerPlayerId === activePlayerId)
    );

  const phaseTimeoutSeconds = useMemo(() => {
    if (phaseNumber === TurnPhase.Draw) return DRAW_PHASE_TIMEOUT_SECONDS;
    if (phaseNumber === TurnPhase.Main1) return MAIN1_PHASE_TIMEOUT_SECONDS;
    return null;
  }, [phaseNumber]);

  const [nowMs, setNowMs] = useState(() => Date.now());

  useEffect(() => {
    const intervalId = setInterval(() => {
      setNowMs(Date.now());
    }, 250);

    return () => clearInterval(intervalId);
  }, []);

  const timeoutDisplay = useMemo(() => {
    if (!phaseTimeoutSeconds || !phaseStartedAt) return null;

    const startedMs = Date.parse(phaseStartedAt);
    if (Number.isNaN(startedMs)) return null;

    const elapsedSeconds = Math.floor((nowMs - startedMs) / 1000);
    const remainingSeconds = Math.max(0, phaseTimeoutSeconds - elapsedSeconds);
    const minutes = Math.floor(remainingSeconds / 60);
    const seconds = remainingSeconds % 60;
    const isExpired = remainingSeconds <= 0;

    return {
      label: `${String(minutes).padStart(2, "0")}:${String(seconds).padStart(2, "0")}`,
      isExpired,
    };
  }, [nowMs, phaseStartedAt, phaseTimeoutSeconds]);

  useEffect(() => {
    if (!canViewerPlayMain1) {
      setSelectedHandCardId(null);
      setPlaceFaceDown(false);
      setPendingRevealCardId(null);
    }
  }, [canViewerPlayMain1]);

  const pendingRevealCard = useMemo(
    () =>
      pendingRevealCardId
        ? (cards.find((card) => card.id === pendingRevealCardId && card.isFaceDown) ?? null)
        : null,
    [cards, pendingRevealCardId]
  );

  const selectedHandCard = useMemo(() => {
    if (!selectedHandCardId) return null;

    return (
      cards.find(
        (card) =>
          card.id === selectedHandCardId &&
          card.playerId === viewerPlayerId &&
          card.zone === ZONE_HAND &&
          Boolean(card.card)
      ) ?? null
    );
  }, [cards, selectedHandCardId, viewerPlayerId]);

  const canPlaceCardAtFieldIndex = useCallback((card: GameCardDto | null, fieldIndex: number) => {
    const cardType = card?.card?.type;
    if (cardType === undefined || cardType === null) return false;

    if (cardType === CARD_TYPE_MONSTER) return fieldIndex <= TOP_ROW_MAX_INDEX;
    if (cardType === CARD_TYPE_SPELL || cardType === CARD_TYPE_TRAP) return fieldIndex > TOP_ROW_MAX_INDEX;
    return false;
  }, []);

  const handleDrawCard = useCallback(async () => {
    if (!safeGameId || !canViewerDraw || isSubmittingDrawAction) return;

    setIsSubmittingDrawAction(true);
    setError(null);
    try {
      await gameHub.drawCard(safeGameId);
      await fetchGameState();
    } catch (err) {
      const message = err instanceof Error ? err.message : "Draw action failed.";
      setError(message);
    } finally {
      setIsSubmittingDrawAction(false);
    }
  }, [canViewerDraw, fetchGameState, isSubmittingDrawAction, safeGameId]);

  const handleNextPhase = useCallback(async () => {
    if (!safeGameId || !canViewerAdvancePhase || isSubmittingDrawAction || isSubmittingMainAction) return;
    setIsSubmittingDrawAction(true);
    setError(null);
    try {
      await gameHub.nextPhase(safeGameId);
      await fetchGameState();
    } catch (err) {
      const message = err instanceof Error ? err.message : "Next phase action failed.";
      setError(message);
    } finally {
      setIsSubmittingDrawAction(false);
    }
  }, [canViewerAdvancePhase, fetchGameState, isSubmittingDrawAction, isSubmittingMainAction, safeGameId]);

  const handleHandCardClick = useCallback(
    (card: GameCardDto) => {
      if (!canViewerPlayMain1) return;
      if (card.playerId !== viewerPlayerId || card.zone !== ZONE_HAND || !card.card) return;

      setSelectedHandCardId((prev) => (prev === card.id ? null : card.id));
    },
    [canViewerPlayMain1, viewerPlayerId]
  );

  const handleGraveyardClick = useCallback(
    (playerId: string) => {
      if (!canViewerPlayMain1 || !viewerPlayerId) return;
      if (playerId !== viewerPlayerId) return;
      if (!selectedHandCard) return;
      if (!safeGameId || isSubmittingMainAction) return;

      setIsSubmittingMainAction(true);
      setError(null);
      void gameHub
        .sendCardToGraveyard(safeGameId, selectedHandCard.id)
        .then(async () => {
          await fetchGameState();
          setSelectedHandCardId(null);
        })
        .catch((err: unknown) => {
          const message = err instanceof Error ? err.message : "Send to graveyard failed.";
          setError(message);
        })
        .finally(() => {
          setIsSubmittingMainAction(false);
        });
    },
    [
      canViewerPlayMain1,
      fetchGameState,
      isSubmittingMainAction,
      safeGameId,
      selectedHandCard,
      viewerPlayerId,
    ]
  );

  const handleFieldClick = useCallback(
    (playerId: string, fieldIndex: number, card: GameCardDto | null) => {
      if (!canViewerPlayMain1 || !viewerPlayerId) return;
      if (playerId !== viewerPlayerId) return;

      if (selectedHandCard) {
        if (card !== null) return;
        if (!canPlaceCardAtFieldIndex(selectedHandCard, fieldIndex)) return;
        if (!safeGameId || isSubmittingMainAction) return;

        setIsSubmittingMainAction(true);
        setError(null);
        void gameHub
          .placeCard(safeGameId, selectedHandCard.id, fieldIndex, placeFaceDown)
          .then(async () => {
            await fetchGameState();
            setSelectedHandCardId(null);
            setPlaceFaceDown(false);
          })
          .catch((err: unknown) => {
            const message = err instanceof Error ? err.message : "Place action failed.";
            setError(message);
          })
          .finally(() => {
            setIsSubmittingMainAction(false);
          });
        return;
      }

      if (!card || card.playerId !== playerId || card.zone !== ZONE_FIELD) return;
      if (!safeGameId || isSubmittingMainAction) return;

      setIsSubmittingMainAction(true);
      setError(null);
      void gameHub
        .toggleDefensePosition(safeGameId, card.id)
        .then(async () => {
          await fetchGameState();
          setPendingRevealCardId(card.isFaceDown ? card.id : null);
        })
        .catch((err: unknown) => {
          const message = err instanceof Error ? err.message : "Card action failed.";
          setError(message);
        })
        .finally(() => {
          setIsSubmittingMainAction(false);
        });
    },
    [
      canPlaceCardAtFieldIndex,
      canViewerPlayMain1,
      fetchGameState,
      isSubmittingMainAction,
      placeFaceDown,
      safeGameId,
      selectedHandCard,
      viewerPlayerId,
    ]
  );

  const handleRevealCard = useCallback(async () => {
    if (!safeGameId || !pendingRevealCard || isSubmittingMainAction) return;

    setIsSubmittingMainAction(true);
    setError(null);
    try {
      await gameHub.revealCard(safeGameId, pendingRevealCard.id);
      await fetchGameState();
      setPendingRevealCardId(null);
    } catch (err) {
      const message = err instanceof Error ? err.message : "Reveal action failed.";
      setError(message);
    } finally {
      setIsSubmittingMainAction(false);
    }
  }, [fetchGameState, isSubmittingMainAction, pendingRevealCard, safeGameId]);

  const showBoard = !isLoading && !error && cards.length > 0 && viewerPlayerId;
  const isViewer = Boolean(currentUserId);

  return (
    <main className="relative min-h-screen w-full text-zinc-100 box-border">
      <div className="pointer-events-none absolute inset-0 backdrop-blur-[2px]" />
      <div className="pointer-events-none absolute inset-0 bg-linear-to-br from-[#4b1812]/20 via-transparent to-[#091b33]/25" />
      <div className="pointer-events-none absolute inset-0 bg-black/30" />
      {turnAnnouncement ? (
        <div className="pointer-events-none absolute inset-0 z-40 grid place-items-center px-4">
          <div className={`text-center ${turnAnnouncement.colorClass}`}>
            <p
              className="text-5xl font-black italic tracking-[0.12em] uppercase [text-shadow:0_0_1.2rem_rgba(255,255,255,0.35)]"
              style={{ WebkitTextStroke: "1px rgba(255,255,255,0.22)" }}
            >
              {turnAnnouncement.title}
            </p>
            {turnAnnouncement.subtitle ? (
              <p className="mt-2 text-2xl font-extrabold italic tracking-[0.08em] text-white [text-shadow:0_0_0.9rem_rgba(255,255,255,0.28)]">
                {turnAnnouncement.subtitle}
              </p>
            ) : null}
          </div>
        </div>
      ) : null}

      <div className="relative h-screen w-full pl-2 pr-3">
        <div className="relative h-full w-full -ml-3">
          <div className="pointer-events-none absolute top-3 left-3 z-20">
            <TurnStatus status={turnStatus} />
          </div>
          <div className="absolute top-3 right-3 z-20 flex items-center gap-2">
            {timeoutDisplay ? (
              <div
                className={`rounded-md border px-3 py-2 text-xs font-semibold ${
                  timeoutDisplay.isExpired
                    ? "border-red-300/60 bg-red-500/20 text-red-100"
                    : "border-white/30 bg-black/45 text-white/90"
                }`}
              >
                Timeout: {timeoutDisplay.label}
              </div>
            ) : null}
            <button
              type="button"
              onClick={() => {
                void handleDrawCard();
              }}
              disabled={!canViewerDraw || isSubmittingDrawAction}
              className="rounded-md border border-cyan-200/50 bg-cyan-500/20 px-3 py-2 text-xs font-semibold text-cyan-100 disabled:cursor-default disabled:opacity-50"
            >
              Draw card
            </button>
            <button
              type="button"
              onClick={() => {
                void handleNextPhase();
              }}
              disabled={!canViewerAdvancePhase || isSubmittingDrawAction || isSubmittingMainAction}
              className="rounded-md border border-amber-200/50 bg-amber-500/20 px-3 py-2 text-xs font-semibold text-amber-100 disabled:cursor-default disabled:opacity-50"
            >
              Next
            </button>
            {canViewerPlayMain1 && pendingRevealCard ? (
              <button
                type="button"
                onClick={() => {
                  void handleRevealCard();
                }}
                disabled={isSubmittingMainAction}
                className="rounded-md border border-violet-200/50 bg-violet-500/20 px-3 py-2 text-xs font-semibold text-violet-100 disabled:cursor-default disabled:opacity-50"
              >
                Reveal
              </button>
            ) : null}
            {canViewerPlayMain1 && selectedHandCard ? (
              <label className="inline-flex items-center gap-2 rounded-md border border-white/25 bg-black/35 px-3 py-2 text-xs text-white/90">
                <input
                  type="checkbox"
                  checked={placeFaceDown}
                  onChange={(event) => setPlaceFaceDown(event.target.checked)}
                  className="size-3.5"
                />
                Set face-down
              </label>
            ) : null}
          </div>

          {isLoading && (
            <div className="grid h-full w-full place-items-center text-white/80">Loading game...</div>
          )}

          {!isLoading && error && (
            <div className="grid h-full w-full place-items-center px-4 text-red-300">{error}</div>
          )}

          {!isLoading && !error && !showBoard && (
            <div className="grid h-full w-full place-items-center px-4 text-white/70">
              Game state is empty.
            </div>
          )}

          {showBoard ? (
            <Board
              cards={cards}
              playerIds={playerOrder}
              viewerPlayerId={viewerPlayerId}
              hoveredCard={hoveredCard}
              onHoverCardChange={setHoveredCard}
              onDeckClick={(playerId) => {
                if (playerId !== viewerPlayerId) return;
                void handleDrawCard();
              }}
              onFieldClick={handleFieldClick}
              onGraveyardClick={handleGraveyardClick}
              onHandCardClick={handleHandCardClick}
              isDeckClickable={(playerId) =>
                canViewerDraw && !isSubmittingDrawAction && playerId === viewerPlayerId
              }
              isFieldClickable={(playerId, fieldIndex, card) => {
                if (
                  !canViewerPlayMain1 ||
                  !viewerPlayerId ||
                  playerId !== viewerPlayerId ||
                  isSubmittingMainAction
                ) {
                  return false;
                }

                if (selectedHandCard) {
                  return card === null && canPlaceCardAtFieldIndex(selectedHandCard, fieldIndex);
                }

                return card !== null;
              }}
              isGraveyardClickable={(playerId) =>
                canViewerPlayMain1 &&
                Boolean(selectedHandCard) &&
                Boolean(viewerPlayerId) &&
                playerId === viewerPlayerId &&
                !isSubmittingMainAction
              }
              isHandCardClickable={(card) =>
                canViewerPlayMain1 &&
                Boolean(viewerPlayerId) &&
                card.playerId === viewerPlayerId &&
                card.zone === ZONE_HAND &&
                Boolean(card.card) &&
                !isSubmittingMainAction
              }
              selectedHandCardId={selectedHandCardId}
            />
          ) : null}

          {!isViewer && (
            <div className="absolute right-3 top-3 rounded-md border border-amber-200/40 bg-black/40 px-3 py-2 text-xs text-amber-100">
              Missing user session.
            </div>
          )}
        </div>
      </div>
    </main>
  );
};

export default GamePage;
