import Board from "@/features/game/components/Board";
import TurnStatus from "@/features/game/components/TurnStatus";
import { TurnPhase, type GameCardDto } from "@/features/game/types/game.types";
import type { CardDto } from "@/shared/types/card.types";
import { useMemo, useState } from "react";

const ZONE_FIELD = 0;
const ZONE_HAND = 1;
const ZONE_GRAVEYARD = 3;
const TOP_ROW_MAX_INDEX = 4;
const CARD_TYPE_MONSTER = 0;
const CARD_TYPE_SPELL = 1;
const CARD_TYPE_TRAP = 2;

const createCard = (
  id: string,
  name: string,
  type: number,
  attack: number | null,
  defense: number | null,
  level: number | null
): CardDto => ({
  id,
  name,
  type,
  attack,
  defense,
  level,
  effectId: null,
  image: "",
  description: `${name} debug card`,
});

const p1MonsterA = createCard("m1", "Crimson Whelp", CARD_TYPE_MONSTER, 1400, 1000, 4);
const p1MonsterB = createCard("m2", "Titan Ram", CARD_TYPE_MONSTER, 1800, 1200, 4);
const p1Spell = createCard("s1", "Flare Veil", CARD_TYPE_SPELL, null, null, null);
const p1Trap = createCard("t1", "Mirror Snare", CARD_TYPE_TRAP, null, null, null);
const p1SetMonster = createCard("m4", "Shadow Cub", CARD_TYPE_MONSTER, 900, 1200, 3);
const p2MonsterA = createCard("m3", "Azure Sentinel", CARD_TYPE_MONSTER, 1600, 1800, 4);
const p2MonsterB = createCard("m5", "Iron Hydra", CARD_TYPE_MONSTER, 1900, 1000, 4);
const p2Spell = createCard("s2", "Blue Current", CARD_TYPE_SPELL, null, null, null);
const p2Trap = createCard("t2", "Needle Cage", CARD_TYPE_TRAP, null, null, null);

const initialCards: GameCardDto[] = [
  {
    id: "g-1",
    playerId: "p1",
    zone: ZONE_HAND,
    deckOrder: null,
    isFaceDown: false,
    fieldIndex: null,
    defensePosition: false,
    card: p1MonsterA,
  },
  {
    id: "g-2",
    playerId: "p1",
    zone: ZONE_HAND,
    deckOrder: null,
    isFaceDown: false,
    fieldIndex: null,
    defensePosition: false,
    card: p1MonsterB,
  },
  {
    id: "g-3",
    playerId: "p1",
    zone: ZONE_HAND,
    deckOrder: null,
    isFaceDown: false,
    fieldIndex: null,
    defensePosition: false,
    card: p1Spell,
  },
  {
    id: "g-4",
    playerId: "p1",
    zone: ZONE_HAND,
    deckOrder: null,
    isFaceDown: false,
    fieldIndex: null,
    defensePosition: false,
    card: p1Trap,
  },
  {
    id: "g-6",
    playerId: "p1",
    zone: ZONE_FIELD,
    deckOrder: null,
    isFaceDown: true,
    fieldIndex: 0,
    defensePosition: true,
    card: p1SetMonster,
  },
  {
    id: "g-5",
    playerId: "p2",
    zone: ZONE_FIELD,
    deckOrder: null,
    isFaceDown: false,
    fieldIndex: 2,
    defensePosition: false,
    card: p2MonsterA,
  },
  {
    id: "g-7",
    playerId: "p2",
    zone: ZONE_HAND,
    deckOrder: null,
    isFaceDown: false,
    fieldIndex: null,
    defensePosition: false,
    card: p2MonsterB,
  },
  {
    id: "g-8",
    playerId: "p2",
    zone: ZONE_HAND,
    deckOrder: null,
    isFaceDown: false,
    fieldIndex: null,
    defensePosition: false,
    card: p2Spell,
  },
  {
    id: "g-9",
    playerId: "p2",
    zone: ZONE_HAND,
    deckOrder: null,
    isFaceDown: false,
    fieldIndex: null,
    defensePosition: false,
    card: p2Trap,
  },
];

const DebugGamePage = () => {
  const [cards, setCards] = useState<GameCardDto[]>(initialCards);
  const [hoveredCard, setHoveredCard] = useState<CardDto | null>(null);
  const [selectedHandCardId, setSelectedHandCardId] = useState<string | null>(null);
  const [placementPositionHover, setPlacementPositionHover] = useState<{
    fieldIndex: number;
    defensePosition: boolean;
  } | null>(null);

  const viewerPlayerId = "p1";
  const activePlayerId = "p1";
  const playerIds = ["p1", "p2"];

  const selectedHandCard = useMemo(
    () =>
      selectedHandCardId
        ? (cards.find(
            (card) => card.id === selectedHandCardId && card.playerId === viewerPlayerId && card.zone === ZONE_HAND
          ) ?? null)
        : null,
    [cards, selectedHandCardId]
  );

  const canPlaceCardAtFieldIndex = (card: GameCardDto | null, fieldIndex: number) => {
    const cardType = card?.card?.type;
    if (cardType === undefined || cardType === null) return false;

    if (cardType === CARD_TYPE_MONSTER) return fieldIndex <= TOP_ROW_MAX_INDEX;
    if (cardType === CARD_TYPE_SPELL || cardType === CARD_TYPE_TRAP) return fieldIndex > TOP_ROW_MAX_INDEX;
    return false;
  };

  const placeSelectedCard = (fieldIndex: number, isFaceDown: boolean) => {
    if (!selectedHandCard) return;
    if (!canPlaceCardAtFieldIndex(selectedHandCard, fieldIndex)) return;
    const shouldStartInDefense =
      selectedHandCard.card?.type === CARD_TYPE_MONSTER &&
      placementPositionHover?.fieldIndex === fieldIndex &&
      placementPositionHover.defensePosition;

    setCards((prev) =>
      prev.map((card) =>
        card.id === selectedHandCard.id
          ? {
              ...card,
              zone: ZONE_FIELD,
              fieldIndex,
              isFaceDown,
              defensePosition: shouldStartInDefense,
            }
          : card
      )
    );
    setSelectedHandCardId(null);
    setPlacementPositionHover(null);
  };

  return (
    <main className="relative min-h-screen w-full text-zinc-100 box-border">
      <div className="pointer-events-none absolute inset-0 backdrop-blur-[2px]" />
      <div className="pointer-events-none absolute inset-0 bg-linear-to-br from-[#4b1812]/20 via-transparent to-[#091b33]/25" />
      <div className="pointer-events-none absolute inset-0 bg-black/30" />

      <div className="relative h-screen w-full pl-2 pr-3">
        <div className="relative h-full w-full -ml-3">
          <div className="pointer-events-none absolute top-3 left-3 z-20">
            <TurnStatus status={{ activePlayerId: "debug-user", phase: TurnPhase.Main1 }} />
          </div>
          <div className="absolute top-3 right-3 z-20 rounded-md border border-white/30 bg-black/45 px-3 py-2 text-[11px] leading-tight text-white/90">
            <div className="font-semibold text-white">Debug Mode (no backend)</div>
            <div className="mt-1 flex items-center gap-2">
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
          </div>

          <Board
            cards={cards}
            playerIds={playerIds}
            viewerPlayerId={viewerPlayerId}
            activePlayerId={activePlayerId}
            playerSummaries={{
              p1: { username: "debug-user", lifePoints: 8000 },
              p2: { username: "debug-opponent", lifePoints: 8000 },
            }}
            hoveredCard={hoveredCard}
            onHoverCardChange={setHoveredCard}
            onFieldClick={(playerId, fieldIndex, card) => {
              if (playerId !== viewerPlayerId) return;
              if (!selectedHandCard || card !== null) return;
              placeSelectedCard(fieldIndex, false);
            }}
            onFieldRightClick={(playerId, fieldIndex, card) => {
              if (playerId !== viewerPlayerId) return;
              if (selectedHandCard && card === null) {
                placeSelectedCard(fieldIndex, true);
                return;
              }
              if (!selectedHandCard && card?.isFaceDown && card.zone === ZONE_FIELD) {
                setCards((prev) =>
                  prev.map((prevCard) =>
                    prevCard.id === card.id ? { ...prevCard, isFaceDown: false } : prevCard
                  )
                );
              }
            }}
            onFieldPositionClick={(playerId, _fieldIndex, card, targetDefensePosition) => {
              if (playerId !== viewerPlayerId) return;
              if (selectedHandCard) return;
              if (card.defensePosition === targetDefensePosition) return;

              setCards((prev) =>
                prev.map((prevCard) =>
                  prevCard.id === card.id ? { ...prevCard, defensePosition: targetDefensePosition } : prevCard
                )
              );
            }}
            onFieldPlacementPositionHover={(playerId, fieldIndex, targetDefensePosition) => {
              if (playerId !== viewerPlayerId || !selectedHandCard) return;
              setPlacementPositionHover({ fieldIndex, defensePosition: targetDefensePosition });
            }}
            onHandCardClick={(card) => {
              if (card.playerId !== viewerPlayerId || card.zone !== ZONE_HAND) return;
              setPlacementPositionHover(null);
              setSelectedHandCardId((prev) => (prev === card.id ? null : card.id));
            }}
            onGraveyardClick={(playerId) => {
              if (playerId !== viewerPlayerId || !selectedHandCard) return;
              setCards((prev) =>
                prev.map((card) =>
                  card.id === selectedHandCard.id
                    ? { ...card, zone: ZONE_GRAVEYARD, fieldIndex: null, isFaceDown: false, defensePosition: false }
                    : card
                )
              );
              setSelectedHandCardId(null);
            }}
            isFieldClickable={(playerId, fieldIndex, card) =>
              playerId === viewerPlayerId &&
              Boolean(selectedHandCard) &&
              card === null &&
              canPlaceCardAtFieldIndex(selectedHandCard, fieldIndex)
            }
            isFieldRightClickable={(playerId, fieldIndex, card) =>
              playerId === viewerPlayerId &&
              Boolean(selectedHandCard) &&
              card === null &&
              canPlaceCardAtFieldIndex(selectedHandCard, fieldIndex)
            }
            canClickFieldPosition={(playerId, _fieldIndex, card, targetDefensePosition) =>
              playerId === viewerPlayerId &&
              !selectedHandCard &&
              card.zone === ZONE_FIELD &&
              card.playerId === viewerPlayerId &&
              card.defensePosition !== targetDefensePosition
            }
            canHoverFieldPlacementPosition={(playerId, fieldIndex, targetDefensePosition) =>
              playerId === viewerPlayerId &&
              Boolean(selectedHandCard) &&
              canPlaceCardAtFieldIndex(selectedHandCard, fieldIndex) &&
              (
                selectedHandCard?.card?.type === CARD_TYPE_MONSTER || targetDefensePosition === false
              )
            }
            isHandCardClickable={(card) => card.playerId === viewerPlayerId && card.zone === ZONE_HAND}
            isGraveyardClickable={(playerId) => playerId === viewerPlayerId && Boolean(selectedHandCard)}
            selectedHandCardId={selectedHandCardId}
          />
        </div>
      </div>
    </main>
  );
};

export default DebugGamePage;
