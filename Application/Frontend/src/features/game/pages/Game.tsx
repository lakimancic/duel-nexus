import Board from "@/features/game/components/Board";
import AttackDaggerOverlay, { type AttackDaggerAnimation } from "@/features/game/components/AttackDaggerOverlay";
import TurnStatus from "@/features/game/components/TurnStatus";
import { gameApi } from "@/features/game/api/game.api";
import {
  type GameEffectActivationDto,
  type TrapResponseWindowEventDto,
  TurnPhase,
  type BattleAttackResultDto,
  type GameCardDto,
} from "@/features/game/types/game.types";
import { useAuthStore } from "@/features/auth/store/auth.store";
import { gameHub } from "@/shared/realtime/gameHub";
import type { CardDto } from "@/shared/types/card.types";
import type { ErrorMessage } from "@/shared/types/error.types";
import { AxiosError } from "axios";
import { useCallback, useEffect, useMemo, useRef, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";

const ZONE_FIELD = 0;
const ZONE_HAND = 1;
const FIELD_TOP_ROW_MAX_INDEX = 4;
const TOP_ROW_MAX_INDEX = 4;
const CARD_TYPE_MONSTER = 0;
const CARD_TYPE_SPELL = 1;
const CARD_TYPE_TRAP = 2;
const TURN_ANNOUNCEMENT_DURATION_MS = 1100;
const EFFECT_ANNOUNCEMENT_DURATION_MS = 5000;
const DRAW_PHASE_TIMEOUT_SECONDS = 20;
const MAIN1_PHASE_TIMEOUT_SECONDS = 60;
const MAIN2_PHASE_TIMEOUT_SECONDS = 60;
const END_PHASE_TIMEOUT_SECONDS = 10;

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

let attackAnimationId = 0;

const getElementCenter = (selector: string) => {
  const element = document.querySelector<HTMLElement>(selector);
  if (!element) return null;

  const rect = element.getBoundingClientRect();
  return {
    x: rect.left + rect.width / 2,
    y: rect.top + rect.height / 2,
  };
};

const GamePage = () => {
  const { gameId } = useParams();
  const navigate = useNavigate();
  const currentUserId = useAuthStore((state) => state.userId);

  const [isLoading, setIsLoading] = useState(true);
  const [fatalError, setFatalError] = useState<string | null>(null);
  const [hoveredCard, setHoveredCard] = useState<CardDto | null>(null);
  const [cards, setCards] = useState<GameCardDto[]>([]);
  const [playerOrder, setPlayerOrder] = useState<string[]>([]);
  const [playerSummaries, setPlayerSummaries] = useState<
    Record<string, { username: string; lifePoints: number }>
  >({});
  const [viewerPlayerId, setViewerPlayerId] = useState<string | null>(null);
  const [viewerLifePoints, setViewerLifePoints] = useState(0);
  const [viewerDrawsInTurn, setViewerDrawsInTurn] = useState(0);
  const [attackedCardIdsInTurn, setAttackedCardIdsInTurn] = useState<string[]>([]);
  const [viewerTurnEnded, setViewerTurnEnded] = useState(false);
  const [activePlayerId, setActivePlayerId] = useState<string | null>(null);
  const [activePlayerLabel, setActivePlayerLabel] = useState<string>("-");
  const [phase, setPhase] = useState<number>(0);
  const [phaseStartedAt, setPhaseStartedAt] = useState<string | null>(null);
  const [isSubmittingDrawAction, setIsSubmittingDrawAction] = useState(false);
  const [isSubmittingMainAction, setIsSubmittingMainAction] = useState(false);
  const [isSubmittingBattleAction, setIsSubmittingBattleAction] = useState(false);
  const [selectedHandCardId, setSelectedHandCardId] = useState<string | null>(null);
  const [selectedAttackerCardId, setSelectedAttackerCardId] = useState<string | null>(null);
  const [attackAnimation, setAttackAnimation] = useState<AttackDaggerAnimation | null>(null);
  const [placementPositionHover, setPlacementPositionHover] = useState<{
    fieldIndex: number;
    defensePosition: boolean;
  } | null>(null);
  const [turnAnnouncement, setTurnAnnouncement] = useState<{
    title: string;
    subtitle?: string;
    colorClass: string;
  } | null>(null);
  const [effectAnnouncement, setEffectAnnouncement] = useState<{
    title: string;
    subtitle?: string;
    colorClass: string;
  } | null>(null);
  const [trapWindow, setTrapWindow] = useState<TrapResponseWindowEventDto | null>(null);
  const [isSubmittingTrapResponse, setIsSubmittingTrapResponse] = useState(false);

  const previousTurnStatusRef = useRef<{ activePlayerId: string; phase: number } | null>(null);
  const viewerPlayerIdRef = useRef<string | null>(null);
  const playerSummariesRef = useRef<Record<string, { username: string; lifePoints: number }>>({});
  const announcementTimeoutRef = useRef<ReturnType<typeof setTimeout> | null>(null);
  const effectAnnouncementTimeoutRef = useRef<ReturnType<typeof setTimeout> | null>(null);
  const isAttackAnimationActiveRef = useRef(false);
  const pendingStateRefreshRef = useRef(false);

  const safeGameId = gameId ?? "";

  useEffect(() => {
    viewerPlayerIdRef.current = viewerPlayerId;
  }, [viewerPlayerId]);

  useEffect(() => {
    playerSummariesRef.current = playerSummaries;
  }, [playerSummaries]);

  const reportRuntimeError = useCallback((scope: string, err: unknown) => {
    console.error(`[Game:${scope}]`, err);
  }, []);

  const showEffectAnnouncement = useCallback((effect: GameEffectActivationDto) => {
    const effectTypeLabel = `Effect ${effect.effectType}`;
    const byPlayerLabel = playerSummariesRef.current[effect.activatedByPlayerGameId]?.username ?? "Unknown player";
    const sourceTypeLabel = effect.isTrap ? "Trap Activated" : "Spell Activated";
    const colorClass = effect.isTrap
      ? "text-amber-300 drop-shadow-[0_0_18px_rgba(253,224,71,0.9)]"
      : "text-sky-300 drop-shadow-[0_0_18px_rgba(125,211,252,0.9)]";

    setEffectAnnouncement({
      title: `${sourceTypeLabel}: ${effect.sourceCardName}`,
      subtitle: `${effectTypeLabel} by ${byPlayerLabel}`,
      colorClass,
    });

    if (effectAnnouncementTimeoutRef.current) {
      clearTimeout(effectAnnouncementTimeoutRef.current);
    }
    effectAnnouncementTimeoutRef.current = setTimeout(() => {
      setEffectAnnouncement(null);
      effectAnnouncementTimeoutRef.current = null;
    }, EFFECT_ANNOUNCEMENT_DURATION_MS);
  }, []);

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
    const nextPlayerSummaries = data.players.reduce<
      Record<string, { username: string; lifePoints: number }>
    >((acc, player) => {
      acc[player.id] = {
        username: player.user.username,
        lifePoints: player.lifePoints,
      };
      return acc;
    }, {});
    const orderedPlayerIds = [...data.players]
      .sort((a, b) => a.index - b.index)
      .map((player) => player.id);
    const activePlayerId = data.currentTurn.activePlayerId;

    setCards(mappedCards);
    setPlayerOrder(orderedPlayerIds);
    setPlayerSummaries(nextPlayerSummaries);
    setViewerPlayerId(data.viewerPlayerId);
    setViewerLifePoints(
      data.players.find((player) => player.id === data.viewerPlayerId)?.lifePoints ?? 0
    );
    setViewerDrawsInTurn(data.viewerDrawsInTurn ?? 0);
    setAttackedCardIdsInTurn(data.attackedCardIdsInCurrentTurn ?? []);
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
      setFatalError(null);
      try {
        await fetchGameState();
      } catch (err) {
        if (!disposed && err instanceof AxiosError) {
          const data = err.response?.data as ErrorMessage | undefined;
          setFatalError(data?.error ?? "Failed to load game state.");
        }
      } finally {
        if (!disposed) setIsLoading(false);
      }
    };

    const onStateMayHaveChanged = (..._args: unknown[]) => {
      if (isAttackAnimationActiveRef.current) {
        pendingStateRefreshRef.current = true;
        return;
      }

      void fetchGameState().catch((err: unknown) => {
        reportRuntimeError("refresh", err);
      });
    };

    const onEffectActivated = (effect: GameEffectActivationDto) => {
      showEffectAnnouncement(effect);
      onStateMayHaveChanged();
    };

    const onTrapWindow = (event: TrapResponseWindowEventDto) => {
      if (!viewerPlayerIdRef.current || event.defenderPlayerGameId !== viewerPlayerIdRef.current) return;
      setTrapWindow(event);
    };

    const pollStateInterval = setInterval(() => {
      if (!disposed) {
        if (isAttackAnimationActiveRef.current) {
          pendingStateRefreshRef.current = true;
          return;
        }

        void fetchGameState().catch((err: unknown) => {
          reportRuntimeError("poll", err);
        });
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
    gameHub.onPlayerAttacked(onStateMayHaveChanged);
    gameHub.onEffectActivated(onEffectActivated);
    gameHub.onTrapWindow(onTrapWindow);

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
      gameHub.offPlayerAttacked(onStateMayHaveChanged);
      gameHub.offEffectActivated(onEffectActivated);
      gameHub.offTrapWindow(onTrapWindow);
      void gameHub.leaveGame(safeGameId);
      clearInterval(pollStateInterval);
    };
  }, [fetchGameState, navigate, reportRuntimeError, safeGameId, showEffectAnnouncement]);

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
      if (effectAnnouncementTimeoutRef.current) {
        clearTimeout(effectAnnouncementTimeoutRef.current);
      }
    };
  }, []);

  const phaseNumber = Number(phase);
  const isViewerAlive = viewerLifePoints > 0;
  const canViewerDraw =
    Boolean(viewerPlayerId) &&
    isViewerAlive &&
    !viewerTurnEnded &&
    phaseNumber === TurnPhase.Draw &&
    viewerDrawsInTurn < 2;
  const canViewerPlayMain1 =
    Boolean(viewerPlayerId) &&
    isViewerAlive &&
    !viewerTurnEnded &&
    (phaseNumber === TurnPhase.Main1 || phaseNumber === TurnPhase.Main2);
  const canViewerBattleAttack =
    Boolean(viewerPlayerId) &&
    isViewerAlive &&
    !viewerTurnEnded &&
    phaseNumber === TurnPhase.Battle &&
    viewerPlayerId === activePlayerId;
  const canViewerAdvancePhase =
    Boolean(viewerPlayerId) &&
    isViewerAlive &&
    (
      ((phaseNumber === TurnPhase.Draw || phaseNumber === TurnPhase.Main1) && !viewerTurnEnded) ||
      (phaseNumber === TurnPhase.Main2 && !viewerTurnEnded) ||
      (phaseNumber === TurnPhase.Battle && viewerPlayerId === activePlayerId)
    );

  const phaseTimeoutSeconds = useMemo(() => {
    if (phaseNumber === TurnPhase.Draw) return DRAW_PHASE_TIMEOUT_SECONDS;
    if (phaseNumber === TurnPhase.Main1) return MAIN1_PHASE_TIMEOUT_SECONDS;
    if (phaseNumber === TurnPhase.Main2) return MAIN2_PHASE_TIMEOUT_SECONDS;
    if (phaseNumber === TurnPhase.End) return END_PHASE_TIMEOUT_SECONDS;
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
      setPlacementPositionHover(null);
    }
  }, [canViewerPlayMain1]);

  useEffect(() => {
    if (!canViewerBattleAttack) {
      setSelectedAttackerCardId(null);
    }
  }, [canViewerBattleAttack]);

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

  const selectedAttackerCard = useMemo(() => {
    if (!selectedAttackerCardId) return null;

    return (
      cards.find(
        (card) =>
          card.id === selectedAttackerCardId &&
          card.playerId === viewerPlayerId &&
          card.zone === ZONE_FIELD &&
          card.fieldIndex !== null &&
          card.fieldIndex <= FIELD_TOP_ROW_MAX_INDEX &&
          !card.isFaceDown &&
          !card.defensePosition &&
          card.card?.type === CARD_TYPE_MONSTER &&
          !attackedCardIdsInTurn.includes(card.id)
      ) ?? null
    );
  }, [attackedCardIdsInTurn, cards, selectedAttackerCardId, viewerPlayerId]);

  const playerHasMonsterOnField = useCallback(
    (playerId: string) =>
      cards.some(
        (card) =>
          card.playerId === playerId &&
          card.zone === ZONE_FIELD &&
          card.fieldIndex !== null &&
          card.fieldIndex <= FIELD_TOP_ROW_MAX_INDEX
      ),
    [cards]
  );

  const canUseCardAsAttacker = useCallback(
    (card: GameCardDto | null) => {
      if (!card) return false;
      if (!viewerPlayerId || card.playerId !== viewerPlayerId) return false;
      if (card.zone !== ZONE_FIELD || card.fieldIndex === null) return false;
      if (card.fieldIndex > FIELD_TOP_ROW_MAX_INDEX) return false;
      if (card.isFaceDown || card.defensePosition) return false;
      if (card.card?.type !== CARD_TYPE_MONSTER) return false;
      if (attackedCardIdsInTurn.includes(card.id)) return false;
      return true;
    },
    [attackedCardIdsInTurn, viewerPlayerId]
  );

  const canPlaceCardAtFieldIndex = useCallback((card: GameCardDto | null, fieldIndex: number) => {
    const cardType = card?.card?.type;
    if (cardType === undefined || cardType === null) return false;

    if (phaseNumber === TurnPhase.Main2 && cardType === CARD_TYPE_MONSTER) return false;
    if (cardType === CARD_TYPE_MONSTER) return fieldIndex <= TOP_ROW_MAX_INDEX;
    if (cardType === CARD_TYPE_SPELL || cardType === CARD_TYPE_TRAP) return fieldIndex > TOP_ROW_MAX_INDEX;
    return false;
  }, [phaseNumber]);

  const canPlaceCardFaceDown = useCallback((card: GameCardDto | null, fieldIndex: number) => {
    const cardType = card?.card?.type;
    if (cardType === undefined || cardType === null) return false;
    if (!canPlaceCardAtFieldIndex(card, fieldIndex)) return false;
    if (cardType === CARD_TYPE_SPELL) return false;
    return true;
  }, [canPlaceCardAtFieldIndex]);

  const playAttackAnimation = useCallback((result: BattleAttackResultDto) => {
    const attackerCenter = getElementCenter(`[data-game-card-id="${result.attackerCardId}"]`);
    const targetCenter = result.defenderCardId
      ? getElementCenter(`[data-game-card-id="${result.defenderCardId}"]`)
      : result.defenderPlayerGameId
        ? getElementCenter(`[data-player-target-id="${result.defenderPlayerGameId}"]`)
        : null;

    if (!attackerCenter || !targetCenter) return false;

    attackAnimationId += 1;
    isAttackAnimationActiveRef.current = true;
    setAttackAnimation({
      id: attackAnimationId,
      startX: attackerCenter.x,
      startY: attackerCenter.y,
      endX: targetCenter.x,
      endY: targetCenter.y,
      attackFailed: result.attackFailed,
    });
    return true;
  }, []);

  const handleDrawCard = useCallback(async () => {
    if (!safeGameId || !canViewerDraw || isSubmittingDrawAction) return;

    setIsSubmittingDrawAction(true);
    try {
      await gameHub.drawCard(safeGameId);
      await fetchGameState();
    } catch (err) {
      reportRuntimeError("draw", err);
    } finally {
      setIsSubmittingDrawAction(false);
    }
  }, [canViewerDraw, fetchGameState, isSubmittingDrawAction, reportRuntimeError, safeGameId]);

  const handleNextPhase = useCallback(async () => {
    if (attackAnimation !== null) return;
    if (!safeGameId || !canViewerAdvancePhase || isSubmittingDrawAction || isSubmittingMainAction || isSubmittingBattleAction) return;
    setIsSubmittingDrawAction(true);
    try {
      await gameHub.nextPhase(safeGameId);
      await fetchGameState();
    } catch (err) {
      reportRuntimeError("next-phase", err);
    } finally {
      setIsSubmittingDrawAction(false);
    }
  }, [
    canViewerAdvancePhase,
    fetchGameState,
    attackAnimation,
    isSubmittingBattleAction,
    isSubmittingDrawAction,
    isSubmittingMainAction,
    reportRuntimeError,
    safeGameId,
  ]);

  const handleHandCardClick = useCallback(
    (card: GameCardDto) => {
      if (!canViewerPlayMain1) return;
      if (card.playerId !== viewerPlayerId || card.zone !== ZONE_HAND || !card.card) return;

      setSelectedHandCardId((prev) => (prev === card.id ? null : card.id));
      setPlacementPositionHover(null);
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
      void gameHub
        .sendCardToGraveyard(safeGameId, selectedHandCard.id)
        .then(async () => {
          await fetchGameState();
          setSelectedHandCardId(null);
        })
        .catch((err: unknown) => {
          reportRuntimeError("grave", err);
        })
        .finally(() => {
          setIsSubmittingMainAction(false);
        });
    },
    [
      canViewerPlayMain1,
      fetchGameState,
      isSubmittingMainAction,
      reportRuntimeError,
      safeGameId,
      selectedHandCard,
      viewerPlayerId,
    ]
  );

  const placeSelectedHandCard = useCallback(
    async (fieldIndex: number, faceDown: boolean) => {
      if (!canViewerPlayMain1 || !selectedHandCard || !safeGameId || isSubmittingMainAction) return;
      if (!canPlaceCardAtFieldIndex(selectedHandCard, fieldIndex)) return;
      const shouldStartInDefense = faceDown && selectedHandCard.card?.type === CARD_TYPE_MONSTER
        ? true
        : (
          placementPositionHover?.fieldIndex === fieldIndex
            ? placementPositionHover.defensePosition
            : false
        );
      const isMonsterPlacement = selectedHandCard.card?.type === CARD_TYPE_MONSTER;

      setIsSubmittingMainAction(true);
      try {
        await gameHub.placeCard(safeGameId, selectedHandCard.id, fieldIndex, faceDown);
        if (!faceDown && shouldStartInDefense && isMonsterPlacement) {
          await gameHub.toggleDefensePosition(safeGameId, selectedHandCard.id);
        }
        await fetchGameState();
        setSelectedHandCardId(null);
        setPlacementPositionHover(null);
      } catch (err) {
        reportRuntimeError("place", err);
      } finally {
        setIsSubmittingMainAction(false);
      }
    },
    [
      canPlaceCardAtFieldIndex,
      canViewerPlayMain1,
      fetchGameState,
      isSubmittingMainAction,
      placementPositionHover,
      reportRuntimeError,
      safeGameId,
      selectedHandCard,
    ]
  );

  const executeBattleAttack = useCallback(
    async (defenderCardId?: string, defenderPlayerGameId?: string) => {
      if (!safeGameId || !selectedAttackerCard || !canViewerBattleAttack || isSubmittingBattleAction) return;

      setIsSubmittingBattleAction(true);
      try {
        const result = await gameHub.attack(
          safeGameId,
          selectedAttackerCard.id,
          defenderCardId,
          defenderPlayerGameId
        );
        const animationStarted = playAttackAnimation(result);
        setSelectedAttackerCardId(null);
        if (animationStarted) {
          pendingStateRefreshRef.current = true;
        } else {
          await fetchGameState();
        }
      } catch (err) {
        reportRuntimeError("attack", err);
      } finally {
        setIsSubmittingBattleAction(false);
      }
    },
    [
      canViewerBattleAttack,
      fetchGameState,
      isSubmittingBattleAction,
      playAttackAnimation,
      reportRuntimeError,
      safeGameId,
      selectedAttackerCard,
    ]
  );

  const handleFieldClick = useCallback(
    (playerId: string, fieldIndex: number, card: GameCardDto | null) => {
      if (canViewerBattleAttack) {
        if (attackAnimation !== null) return;
        if (isSubmittingBattleAction) return;

        if (!selectedAttackerCard) {
          if (!card || !canUseCardAsAttacker(card)) return;
          setSelectedAttackerCardId(card.id);
          return;
        }

        if (card?.id === selectedAttackerCard.id) {
          setSelectedAttackerCardId(null);
          return;
        }

        if (card && canUseCardAsAttacker(card)) {
          setSelectedAttackerCardId(card.id);
          return;
        }

        const isEnemyMonsterTarget =
          playerId !== viewerPlayerId &&
          card !== null &&
          card.zone === ZONE_FIELD &&
          card.fieldIndex !== null &&
          card.fieldIndex <= FIELD_TOP_ROW_MAX_INDEX;

        if (isEnemyMonsterTarget) {
          void executeBattleAttack(card.id);
        }

        return;
      }

      if (!canViewerPlayMain1 || !viewerPlayerId) return;
      if (playerId !== viewerPlayerId) return;

      if (selectedHandCard) {
        if (card !== null) return;
        void placeSelectedHandCard(fieldIndex, false);
        return;
      }
    },
    [
      attackAnimation,
      canUseCardAsAttacker,
      canViewerBattleAttack,
      canViewerPlayMain1,
      executeBattleAttack,
      isSubmittingBattleAction,
      placeSelectedHandCard,
      selectedHandCard,
      selectedAttackerCard,
      viewerPlayerId,
    ]
  );

  const handleFieldRightClick = useCallback(
    (playerId: string, fieldIndex: number, card: GameCardDto | null) => {
      if (!canViewerPlayMain1 || !viewerPlayerId) return;
      if (playerId !== viewerPlayerId) return;
      if (!safeGameId || isSubmittingMainAction) return;

      if (selectedHandCard && card === null) {
        if (!canPlaceCardFaceDown(selectedHandCard, fieldIndex)) return;
        void placeSelectedHandCard(fieldIndex, true);
        return;
      }

      if (!selectedHandCard && card?.isFaceDown && card.zone === ZONE_FIELD && card.playerId === viewerPlayerId) {
        setIsSubmittingMainAction(true);
        void gameHub
          .revealCard(safeGameId, card.id)
          .then(async () => {
            await fetchGameState();
          })
          .catch((err: unknown) => {
            reportRuntimeError("reveal", err);
          })
          .finally(() => {
            setIsSubmittingMainAction(false);
          });
      }
    },
    [
      canPlaceCardFaceDown,
      canViewerPlayMain1,
      fetchGameState,
      isSubmittingMainAction,
      placeSelectedHandCard,
      reportRuntimeError,
      safeGameId,
      selectedHandCard,
      viewerPlayerId,
    ]
  );

  const handleFieldPositionClick = useCallback(
    (
      playerId: string,
      _fieldIndex: number,
      card: GameCardDto,
      targetDefensePosition: boolean
    ) => {
      if (!canViewerPlayMain1 || !viewerPlayerId || selectedHandCard) return;
      if (playerId !== viewerPlayerId) return;
      if (card.zone !== ZONE_FIELD || card.playerId !== viewerPlayerId) return;
      if (card.card?.type !== CARD_TYPE_MONSTER) return;
      if (card.defensePosition === targetDefensePosition) return;
      if (!safeGameId || isSubmittingMainAction) return;

      setIsSubmittingMainAction(true);
      void gameHub
        .toggleDefensePosition(safeGameId, card.id)
        .then(async () => {
          await fetchGameState();
        })
        .catch((err: unknown) => {
          reportRuntimeError("toggle-defense", err);
        })
        .finally(() => {
          setIsSubmittingMainAction(false);
        });
    },
    [
      canViewerPlayMain1,
      fetchGameState,
      isSubmittingMainAction,
      reportRuntimeError,
      safeGameId,
      selectedHandCard,
      viewerPlayerId,
    ]
  );

  const handleFieldPlacementPositionHover = useCallback(
    (playerId: string, fieldIndex: number, targetDefensePosition: boolean) => {
      if (!canViewerPlayMain1 || !viewerPlayerId || !selectedHandCard) return;
      if (playerId !== viewerPlayerId) return;
      setPlacementPositionHover({ fieldIndex, defensePosition: targetDefensePosition });
    },
    [canViewerPlayMain1, selectedHandCard, viewerPlayerId]
  );

  const handlePlayerClick = useCallback(
    (playerId: string) => {
      if (!canViewerBattleAttack || !selectedAttackerCard) return;
      if (playerId === viewerPlayerId) return;
      if (isSubmittingBattleAction) return;
      if (attackAnimation !== null) return;
      if (playerHasMonsterOnField(playerId)) return;

      void executeBattleAttack(undefined, playerId);
    },
    [
      attackAnimation,
      canViewerBattleAttack,
      executeBattleAttack,
      isSubmittingBattleAction,
      playerHasMonsterOnField,
      selectedAttackerCard,
      viewerPlayerId,
    ]
  );

  const handleTrapActivation = useCallback(
    async (trapGameCardId: string) => {
      if (!safeGameId || !trapWindow || isSubmittingTrapResponse) return;

      setIsSubmittingTrapResponse(true);
      try {
        await gameHub.activateTrapResponse(safeGameId, trapWindow.windowId, trapGameCardId);
        setTrapWindow(null);
      } catch (err) {
        reportRuntimeError("trap-response", err);
      } finally {
        setIsSubmittingTrapResponse(false);
      }
    },
    [isSubmittingTrapResponse, reportRuntimeError, safeGameId, trapWindow]
  );

  useEffect(() => {
    if (!trapWindow) return;

    const remainingMs = Math.max(0, Date.parse(trapWindow.expiresAtUtc) - Date.now());
    const timeoutId = setTimeout(() => {
      setTrapWindow((current) => (current?.windowId === trapWindow.windowId ? null : current));
    }, remainingMs);

    return () => clearTimeout(timeoutId);
  }, [trapWindow]);

  const showBoard = !isLoading && !fatalError && cards.length > 0 && viewerPlayerId;
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
      {effectAnnouncement ? (
        <div className="pointer-events-none absolute inset-0 z-30 grid place-items-center px-4">
          <div className={`text-center ${effectAnnouncement.colorClass}`}>
            <p
              className="text-4xl font-black tracking-[0.1em] uppercase [text-shadow:0_0_1.1rem_rgba(255,255,255,0.3)]"
              style={{ WebkitTextStroke: "1px rgba(255,255,255,0.2)" }}
            >
              {effectAnnouncement.title}
            </p>
            {effectAnnouncement.subtitle ? (
              <p className="mt-2 text-lg font-bold tracking-[0.06em] text-white [text-shadow:0_0_0.8rem_rgba(255,255,255,0.26)]">
                {effectAnnouncement.subtitle}
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
            {trapWindow ? (
              <div className="rounded-md border border-amber-300/60 bg-black/65 px-3 py-2 text-xs text-amber-100">
                <p className="font-semibold uppercase tracking-[0.06em]">Trap response window</p>
                <p className="mt-1 text-[11px] text-amber-50/90">
                  You have {trapWindow.timeoutSeconds}s to activate a trap.
                </p>
                <div className="mt-2 flex flex-wrap gap-2">
                  {trapWindow.availableTrapCards.map((trap) => (
                    <button
                      key={trap.gameCardId}
                      type="button"
                      onClick={() => {
                        void handleTrapActivation(trap.gameCardId);
                      }}
                      disabled={isSubmittingTrapResponse}
                      className="rounded border border-amber-200/50 bg-amber-500/25 px-2 py-1 text-[10px] font-semibold text-amber-50 disabled:opacity-50"
                    >
                      {trap.cardName}
                    </button>
                  ))}
                </div>
              </div>
            ) : null}
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
              disabled={!canViewerAdvancePhase || isSubmittingDrawAction || isSubmittingMainAction || isSubmittingBattleAction || attackAnimation !== null}
              className="rounded-md border border-amber-200/50 bg-amber-500/20 px-3 py-2 text-xs font-semibold text-amber-100 disabled:cursor-default disabled:opacity-50"
            >
              Next
            </button>
            <div className="rounded-md border border-white/30 bg-black/45 px-3 py-2 text-[11px] leading-tight text-white/90">
              <div className="flex items-center gap-2">
                <span className="inline-flex h-4 w-3 rounded-sm border border-white/60">
                  <span className="h-full w-1/2 rounded-l-sm bg-cyan-300/75" />
                  <span className="h-full w-1/2 rounded-r-sm bg-transparent" />
                </span>
                Left click field holder: place selected card face-up
              </div>
              <div className="mt-1 flex items-center gap-2">
                <span className="inline-flex h-4 w-3 rounded-sm border border-white/60">
                  <span className="h-full w-1/2 rounded-l-sm bg-transparent" />
                  <span className="h-full w-1/2 rounded-r-sm bg-amber-300/75" />
                </span>
                Right click empty holder: place selected card face-down
              </div>
              <div className="mt-1 text-white/80">
                While placing: hover vertical/horizontal holder to set initial Attack/Defense
              </div>
              <div className="mt-1 text-white/80">
                Right click a face-down field card: reveal it
              </div>
              <div className="mt-1 text-white/80">
                After placed: left click vertical/horizontal holder to change Attack/Defense
              </div>
              <div className="mt-2 border-t border-white/20 pt-2 text-white/80">
                Battle: click your attack-position monster, then click enemy monster or enemy player.
              </div>
            </div>
          </div>

          {isLoading && (
            <div className="grid h-full w-full place-items-center text-white/80">Loading game...</div>
          )}

          {!isLoading && fatalError && (
            <div className="grid h-full w-full place-items-center px-4 text-red-300">{fatalError}</div>
          )}

          {!isLoading && !fatalError && !showBoard && (
            <div className="grid h-full w-full place-items-center px-4 text-white/70">
              Game state is empty.
            </div>
          )}

          {showBoard ? (
            <Board
              cards={cards}
              playerIds={playerOrder}
              viewerPlayerId={viewerPlayerId}
              activePlayerId={activePlayerId}
              selectedAttackerCardId={selectedAttackerCardId}
              playerSummaries={playerSummaries}
              hoveredCard={hoveredCard}
              onHoverCardChange={setHoveredCard}
              onPlayerClick={handlePlayerClick}
              onDeckClick={(playerId) => {
                if (playerId !== viewerPlayerId) return;
                void handleDrawCard();
              }}
              onFieldClick={handleFieldClick}
              onFieldRightClick={handleFieldRightClick}
              onFieldPositionClick={handleFieldPositionClick}
              onFieldPlacementPositionHover={handleFieldPlacementPositionHover}
              onGraveyardClick={handleGraveyardClick}
              onHandCardClick={handleHandCardClick}
              isDeckClickable={(playerId) =>
                canViewerDraw && !isSubmittingDrawAction && playerId === viewerPlayerId
              }
              isPlayerClickable={(playerId) =>
                canViewerBattleAttack &&
                Boolean(selectedAttackerCard) &&
                !isSubmittingBattleAction &&
                attackAnimation === null &&
                Boolean(viewerPlayerId) &&
                playerId !== viewerPlayerId &&
                !playerHasMonsterOnField(playerId) &&
                (playerSummaries[playerId]?.lifePoints ?? 0) > 0
              }
              isFieldClickable={(playerId, fieldIndex, card) => {
                if (canViewerBattleAttack) {
                  if (isSubmittingBattleAction) return false;
                  if (attackAnimation !== null) return false;

                  if (!selectedAttackerCard) {
                    return canUseCardAsAttacker(card);
                  }

                  if (canUseCardAsAttacker(card)) return true;

                  return (
                    Boolean(viewerPlayerId) &&
                    playerId !== viewerPlayerId &&
                    card !== null &&
                    card.zone === ZONE_FIELD &&
                    card.fieldIndex !== null &&
                    card.fieldIndex <= FIELD_TOP_ROW_MAX_INDEX &&
                    fieldIndex <= FIELD_TOP_ROW_MAX_INDEX
                  );
                }

                if (
                  !canViewerPlayMain1 ||
                  !viewerPlayerId ||
                  playerId !== viewerPlayerId ||
                  isSubmittingMainAction
                ) {
                  return false;
                }

                if (selectedHandCard) {
                  return card === null && canPlaceCardFaceDown(selectedHandCard, fieldIndex);
                }

                return false;
              }}
              isFieldRightClickable={(playerId, fieldIndex, card) => {
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

                return false;
              }}
              canClickFieldPosition={(playerId, _fieldIndex, card, targetDefensePosition) => {
                if (
                  !canViewerPlayMain1 ||
                  !viewerPlayerId ||
                  playerId !== viewerPlayerId ||
                  isSubmittingMainAction ||
                  canViewerBattleAttack ||
                  Boolean(selectedHandCard)
                ) {
                  return false;
                }
                if (card.zone !== ZONE_FIELD || card.playerId !== viewerPlayerId) return false;
                if (card.card?.type !== CARD_TYPE_MONSTER) return false;

                return card.defensePosition !== targetDefensePosition;
              }}
              canHoverFieldPlacementPosition={(playerId, fieldIndex, targetDefensePosition) => {
                if (
                  !canViewerPlayMain1 ||
                  !viewerPlayerId ||
                  !selectedHandCard ||
                  playerId !== viewerPlayerId ||
                  isSubmittingMainAction
                ) {
                  return false;
                }

                if (!canPlaceCardAtFieldIndex(selectedHandCard, fieldIndex)) return false;

                if (selectedHandCard.card?.type !== CARD_TYPE_MONSTER) {
                  return targetDefensePosition === false;
                }

                return true;
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

          <AttackDaggerOverlay
            animation={attackAnimation}
            selectedAttackerCardId={selectedAttackerCardId}
            onAnimationEnd={() => {
              setAttackAnimation(null);
              isAttackAnimationActiveRef.current = false;

              if (!pendingStateRefreshRef.current) return;

              pendingStateRefreshRef.current = false;
              void fetchGameState();
            }}
          />

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
