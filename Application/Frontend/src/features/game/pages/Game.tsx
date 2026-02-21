import Board from "@/features/game/components/Board";
import TurnStatus from "@/features/game/components/TurnStatus";
import {
  TurnPhase,
  type GameCardDto,
  type GameTurnStatus,
} from "@/features/game/types/game.types";
import { useEffect, useRef, useState } from "react";
import type { CardDto } from "@/shared/types/card.types";

const ZONE_FIELD = 0;
const ZONE_DECK = 2;
const ZONE_GRAVEYARD = 3;
const ZONE_HAND = 1;
const VIEWER_PLAYER_ID = "Player-1";
const TOP_ROW_MAX_INDEX = 4;
const CARD_TYPE_MONSTER = 0;
const CARD_TYPE_SPELL = 1;
const CARD_TYPE_TRAP = 2;
const TURN_ANNOUNCEMENT_DURATION_MS = 1000;

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

const demoCards: GameCardDto[] = [
  {
    playerId: "Player-1",
    zone: ZONE_FIELD,
    isFaceDown: false,
    fieldIndex: 0,
    defensePosition: false,
    card: {
      id: "c-1",
      name: "Dragon Knight",
      image: "",
      description: "A frontline monster used for testing board layout.",
      type: 0,
      effectId: null,
      attack: 2400,
      defense: 1800,
      level: 6,
    },
  },
  {
    playerId: "Player-1",
    zone: ZONE_FIELD,
    isFaceDown: false,
    fieldIndex: 6,
    defensePosition: true,
    card: {
      id: "c-2",
      name: "Hidden Guard",
      image: "",
      description: "Face-down card to validate hidden mode.",
      type: 2,
      effectId: "e-2",
      attack: null,
      defense: null,
      level: null,
    },
  },
  {
    playerId: "Player-2",
    zone: ZONE_FIELD,
    isFaceDown: false,
    fieldIndex: 2,
    defensePosition: true,
    card: {
      id: "c-3",
      name: "Stone Turtle",
      image: "",
      description: "Defense-position sample card.",
      type: 0,
      effectId: null,
      attack: 800,
      defense: 2500,
      level: 4,
    },
  },
  {
    playerId: "Player-3",
    zone: ZONE_FIELD,
    isFaceDown: false,
    fieldIndex: 4,
    defensePosition: false,
    card: {
      id: "c-4",
      name: "Arcane Surge",
      image: "",
      description: "Spell sample for mixed card types.",
      type: 1,
      effectId: "e-4",
      attack: null,
      defense: null,
      level: null,
    },
  },
  {
    playerId: "Player-1",
    zone: ZONE_HAND,
    isFaceDown: false,
    fieldIndex: null,
    defensePosition: false,
    card: {
      id: "d-1",
      name: "Blue-Eyes White Dragon",
      image: "",
      description: "Demo deck card for the current player hand fan.",
      type: 0,
      effectId: null,
      attack: 3000,
      defense: 2500,
      level: 8,
    },
  },
  {
    playerId: "Player-1",
    zone: ZONE_HAND,
    isFaceDown: false,
    fieldIndex: null,
    defensePosition: false,
    card: {
      id: "d-2",
      name: "Mystical Space Typhoon",
      image: "",
      description: "Demo deck card for curved fan layout.",
      type: 1,
      effectId: null,
      attack: null,
      defense: null,
      level: null,
    },
  },
  {
    playerId: "Player-1",
    zone: ZONE_HAND,
    isFaceDown: false,
    fieldIndex: null,
    defensePosition: false,
    card: {
      id: "d-7",
      name: "Dark Magician",
      image: "",
      description: "Extra draw test card.",
      type: 0,
      effectId: null,
      attack: 2500,
      defense: 2100,
      level: 7,
    },
  },
  {
    playerId: "Player-1",
    zone: ZONE_HAND,
    isFaceDown: false,
    fieldIndex: null,
    defensePosition: false,
    card: {
      id: "d-8",
      name: "Raigeki",
      image: "",
      description: "Extra draw test card.",
      type: 1,
      effectId: null,
      attack: null,
      defense: null,
      level: null,
    },
  },
  {
    playerId: "Player-1",
    zone: ZONE_HAND,
    isFaceDown: false,
    fieldIndex: null,
    defensePosition: false,
    card: {
      id: "d-3",
      name: "Mirror Force",
      image: "",
      description: "Demo deck card for curved fan layout.",
      type: 2,
      effectId: null,
      attack: null,
      defense: null,
      level: null,
    },
  },
  {
    playerId: "Player-1",
    zone: ZONE_DECK,
    isFaceDown: false,
    fieldIndex: null,
    defensePosition: false,
    card: {
      id: "h-1",
      name: "Summoned Skull",
      image: "",
      description: "Main1 test card already in hand.",
      type: 0,
      effectId: null,
      attack: 2500,
      defense: 1200,
      level: 6,
    },
  },
  {
    playerId: "Player-1",
    zone: ZONE_DECK,
    isFaceDown: false,
    fieldIndex: null,
    defensePosition: false,
    card: {
      id: "h-2",
      name: "Swords of Revealing Light",
      image: "",
      description: "Main1 test card for graveyard/deck actions.",
      type: 1,
      effectId: null,
      attack: null,
      defense: null,
      level: null,
    },
  },
  {
    playerId: "Player-2",
    zone: ZONE_DECK,
    isFaceDown: true,
    fieldIndex: null,
    defensePosition: false,
    card: {
      id: "d-4",
      name: "Opponent Card A",
      image: "",
      description: "Should be hidden from current player.",
      type: 0,
      effectId: null,
      attack: 1500,
      defense: 1500,
      level: 4,
    },
  },
  {
    playerId: "Player-2",
    zone: ZONE_DECK,
    isFaceDown: true,
    fieldIndex: null,
    defensePosition: false,
    card: {
      id: "d-5",
      name: "Opponent Card B",
      image: "",
      description: "Should be hidden from current player.",
      type: 1,
      effectId: null,
      attack: null,
      defense: null,
      level: null,
    },
  },
  {
    playerId: "Player-3",
    zone: ZONE_DECK,
    isFaceDown: true,
    fieldIndex: null,
    defensePosition: false,
    card: {
      id: "d-6",
      name: "Opponent Card C",
      image: "",
      description: "Should be hidden from current player.",
      type: 2,
      effectId: null,
      attack: null,
      defense: null,
      level: null,
    },
  },
];

const GamePage = () => {
  const [hoveredCard, setHoveredCard] = useState<CardDto | null>(null);
  const [gameCards, setGameCards] = useState<GameCardDto[]>(demoCards);
  const [selectedHandCardId, setSelectedHandCardId] = useState<string | null>(null);
  const [turnAnnouncement, setTurnAnnouncement] = useState<{
    title: string;
    subtitle?: string;
    colorClass: string;
  } | null>(null);
  const [turnStatus, setTurnStatus] = useState<GameTurnStatus>({
    activePlayerId: VIEWER_PLAYER_ID,
    phase: TurnPhase.Draw,
  });
  const previousTurnStatusRef = useRef<GameTurnStatus | null>(null);
  const announcementTimeoutRef = useRef<ReturnType<typeof setTimeout> | null>(null);

  useEffect(() => {
    return () => {
      if (announcementTimeoutRef.current) {
        clearTimeout(announcementTimeoutRef.current);
      }
    };
  }, []);

  useEffect(() => {
    const previous = previousTurnStatusRef.current;
    const currentPhase = Number(turnStatus.phase);

    if (!previous) {
      previousTurnStatusRef.current = turnStatus;
      return;
    }

    const phaseChanged = Number(previous.phase) !== currentPhase;
    const playerChanged = previous.activePlayerId !== turnStatus.activePlayerId;

    if (!phaseChanged && !playerChanged) return;

    const phaseLabel = PHASE_LABELS[currentPhase] ?? `Phase ${turnStatus.phase}`;
    const title = phaseChanged ? `${phaseLabel} Phase` : "Player Turn Changed";
    const subtitle = playerChanged ? `Player: ${turnStatus.activePlayerId}` : undefined;
    const colorClass = PHASE_COLOR_CLASSES[currentPhase] ?? "border-cyan-300/60 text-cyan-100";

    const announcementPayload = { title, subtitle, colorClass };
    const showTimer = setTimeout(() => {
      setTurnAnnouncement(announcementPayload);
    }, 0);
    previousTurnStatusRef.current = turnStatus;

    if (announcementTimeoutRef.current) {
      clearTimeout(announcementTimeoutRef.current);
    }
    announcementTimeoutRef.current = setTimeout(() => {
      setTurnAnnouncement(null);
      announcementTimeoutRef.current = null;
    }, TURN_ANNOUNCEMENT_DURATION_MS);

    return () => clearTimeout(showTimer);
  }, [turnStatus]);
  const isDrawPhase = Number(turnStatus.phase) === TurnPhase.Draw;
  const isMain1Phase = Number(turnStatus.phase) === TurnPhase.Main1;
  const isViewerActivePlayer = turnStatus.activePlayerId === VIEWER_PLAYER_ID;
  const drawPileCards = gameCards.filter(
    (card) =>
      card.playerId === VIEWER_PLAYER_ID &&
      card.zone === ZONE_DECK &&
      card.fieldIndex === null &&
      card.card
  );
  const selectableHandCards = gameCards.filter(
    (card) =>
      card.playerId === VIEWER_PLAYER_ID &&
      card.zone === ZONE_HAND &&
      card.fieldIndex === null &&
      card.card
  );
  const selectedHandCard =
    selectableHandCards.find((card) => card.card?.id === selectedHandCardId) ?? null;

  const canPlaceCardAtFieldIndex = (card: GameCardDto | null, fieldIndex: number) => {
    const cardType = card?.card?.type;
    if (cardType === undefined || cardType === null) return false;

    if (cardType === CARD_TYPE_MONSTER) return fieldIndex <= TOP_ROW_MAX_INDEX;
    if (cardType === CARD_TYPE_SPELL || cardType === CARD_TYPE_TRAP) return fieldIndex > TOP_ROW_MAX_INDEX;
    return false;
  };

  const handleDeckClick = (playerId: string) => {
    if (!isDrawPhase) return;
    if (playerId !== VIEWER_PLAYER_ID || playerId !== turnStatus.activePlayerId) return;

    setGameCards((prevCards) => {
      const topDeckIndex = [...prevCards]
        .map((card, index) => ({ card, index }))
        .reverse()
        .find(({ card }) => card.playerId === playerId && card.zone === ZONE_DECK && card.fieldIndex === null && card.card)
        ?.index;

      if (topDeckIndex === undefined) return prevCards;

      const drawnCard = prevCards[topDeckIndex];
      const updatedCard: GameCardDto = {
        ...drawnCard,
        zone: ZONE_HAND,
        fieldIndex: null,
        isFaceDown: false,
        defensePosition: false,
      };

      setTurnStatus((prev) => ({ ...prev, phase: TurnPhase.Main1 }));
      setSelectedHandCardId(null);

      const remainingCards = prevCards.filter((_, index) => index !== topDeckIndex);
      return [updatedCard, ...remainingCards];
    });
  };

  const handleHandCardClick = (card: GameCardDto) => {
    if (!isMain1Phase || !isViewerActivePlayer) return;
    if (card.playerId !== VIEWER_PLAYER_ID || card.zone !== ZONE_HAND || !card.card) return;

    setSelectedHandCardId((prev) => (prev === card.card?.id ? null : card.card?.id ?? null));
  };

  const handleGraveyardClick = (playerId: string) => {
    if (!isMain1Phase || !isViewerActivePlayer) return;
    if (playerId !== VIEWER_PLAYER_ID) return;
    if (!selectedHandCard?.card?.id) return;

    setGameCards((prevCards) =>
      prevCards.map((card) =>
        card.card?.id === selectedHandCard.card?.id
          ? {
              ...card,
              zone: ZONE_GRAVEYARD,
              fieldIndex: null,
              isFaceDown: false,
              defensePosition: false,
            }
          : card
      )
    );
    setSelectedHandCardId(null);
  };

  const handleFieldClick = (playerId: string, fieldIndex: number, card: GameCardDto | null) => {
    if (!isMain1Phase) return;
    if (playerId !== VIEWER_PLAYER_ID || playerId !== turnStatus.activePlayerId) return;

    if (selectedHandCard?.card?.id) {
      if (card !== null) return;
      if (!canPlaceCardAtFieldIndex(selectedHandCard, fieldIndex)) return;

      setGameCards((prevCards) =>
        prevCards.map((existingCard) =>
          existingCard.card?.id === selectedHandCard.card?.id
            ? {
                ...existingCard,
                zone: ZONE_FIELD,
                fieldIndex,
                isFaceDown: false,
                defensePosition: false,
              }
            : existingCard
        )
      );
      setSelectedHandCardId(null);
      return;
    }

    if (!card || card.playerId !== playerId || card.zone !== ZONE_FIELD) return;

    setGameCards((prevCards) =>
      prevCards.map((existingCard) => {
        const sameCard =
          existingCard.playerId === card.playerId &&
          existingCard.zone === card.zone &&
          existingCard.fieldIndex === card.fieldIndex &&
          existingCard.card?.id === card.card?.id;

        if (!sameCard) return existingCard;
        return { ...existingCard, defensePosition: !existingCard.defensePosition };
      })
    );
  };

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
          <Board
            cards={gameCards}
            viewerPlayerId={VIEWER_PLAYER_ID}
            hoveredCard={hoveredCard}
            onHoverCardChange={setHoveredCard}
            onDeckClick={handleDeckClick}
            onFieldClick={handleFieldClick}
            onGraveyardClick={handleGraveyardClick}
            onHandCardClick={handleHandCardClick}
            isDeckClickable={(playerId) =>
              isDrawPhase &&
              drawPileCards.length > 0 &&
              playerId === turnStatus.activePlayerId &&
              playerId === VIEWER_PLAYER_ID
            }
            isFieldClickable={(playerId, fieldIndex, card) => {
              if (
                !isMain1Phase ||
                playerId !== turnStatus.activePlayerId ||
                playerId !== VIEWER_PLAYER_ID
              ) {
                return false;
              }

              if (selectedHandCard) {
                return card === null && canPlaceCardAtFieldIndex(selectedHandCard, fieldIndex);
              }

              return card !== null;
            }}
            isGraveyardClickable={(playerId) =>
              isMain1Phase &&
              isViewerActivePlayer &&
              playerId === VIEWER_PLAYER_ID &&
              Boolean(selectedHandCard)
            }
            isHandCardClickable={(card) =>
              isMain1Phase &&
              isViewerActivePlayer &&
              card.playerId === VIEWER_PLAYER_ID &&
              card.zone === ZONE_HAND &&
              Boolean(card.card)
            }
            selectedHandCardId={selectedHandCardId}
          />
        </div>
      </div>
    </main>
  );
};

export default GamePage;
