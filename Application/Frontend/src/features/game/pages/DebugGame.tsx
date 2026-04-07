import AttackDaggerOverlay, { type AttackDaggerAnimation } from "@/features/game/components/AttackDaggerOverlay";
import Board from "@/features/game/components/Board";
import TurnStatus from "@/features/game/components/TurnStatus";
import { TurnPhase, type GameCardDto } from "@/features/game/types/game.types";
import type { CardDto } from "@/shared/types/card.types";
import { useCallback, useMemo, useState } from "react";

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
    id: "g-10",
    playerId: "p1",
    zone: ZONE_FIELD,
    deckOrder: null,
    isFaceDown: false,
    fieldIndex: 1,
    defensePosition: false,
    card: p1MonsterB,
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

interface PendingDebugAttackResolution {
  attackerCardId: string;
  defenderCardId: string | null;
  defenderPlayerId: string;
  damageToDefender: number;
  damageToAttacker: number;
  attackerDestroyed: boolean;
  defenderDestroyed: boolean;
  attackFailed: boolean;
}

const DebugGamePage = () => {
  const [cards, setCards] = useState<GameCardDto[]>(initialCards);
  const [hoveredCard, setHoveredCard] = useState<CardDto | null>(null);
  const [selectedAttackerCardId, setSelectedAttackerCardId] = useState<string | null>(null);
  const [attackedCardIds, setAttackedCardIds] = useState<string[]>([]);
  const [attackAnimation, setAttackAnimation] = useState<AttackDaggerAnimation | null>(null);
  const [pendingAttackResolution, setPendingAttackResolution] = useState<PendingDebugAttackResolution | null>(null);
  const [playerSummaries, setPlayerSummaries] = useState({
    p1: { username: "debug-user", lifePoints: 8000 },
    p2: { username: "debug-opponent", lifePoints: 8000 },
  });

  const viewerPlayerId = "p1";
  const activePlayerId = "p1";
  const playerIds = ["p1", "p2"];
  const isResolvingAttack = attackAnimation !== null || pendingAttackResolution !== null;

  const selectedAttackerCard = useMemo(
    () =>
      selectedAttackerCardId
        ? (
            cards.find(
              (card) =>
                card.id === selectedAttackerCardId &&
                card.playerId === viewerPlayerId &&
                card.zone === ZONE_FIELD &&
                card.fieldIndex !== null &&
                card.fieldIndex <= TOP_ROW_MAX_INDEX &&
                !card.isFaceDown &&
                !card.defensePosition &&
                card.card?.type === CARD_TYPE_MONSTER &&
                !attackedCardIds.includes(card.id)
            ) ?? null
          )
        : null,
    [attackedCardIds, cards, selectedAttackerCardId]
  );

  const canUseCardAsAttacker = useCallback(
    (card: GameCardDto | null) => {
      if (!card) return false;
      if (card.playerId !== viewerPlayerId) return false;
      if (card.zone !== ZONE_FIELD || card.fieldIndex === null || card.fieldIndex > TOP_ROW_MAX_INDEX) return false;
      if (card.isFaceDown || card.defensePosition) return false;
      if (card.card?.type !== CARD_TYPE_MONSTER) return false;
      return !attackedCardIds.includes(card.id);
    },
    [attackedCardIds]
  );

  const playerHasMonsterOnField = useCallback(
    (playerId: string) =>
      cards.some(
        (card) =>
          card.playerId === playerId &&
          card.zone === ZONE_FIELD &&
          card.fieldIndex !== null &&
          card.fieldIndex <= TOP_ROW_MAX_INDEX
      ),
    [cards]
  );

  const startAttackAnimation = useCallback((
    attackerCardId: string,
    defenderCardId: string | null,
    defenderPlayerId: string | null,
    attackFailed: boolean
  ) => {
    const attackerCenter = getElementCenter(`[data-game-card-id="${attackerCardId}"]`);
    const targetCenter = defenderCardId
      ? getElementCenter(`[data-game-card-id="${defenderCardId}"]`)
      : defenderPlayerId
        ? getElementCenter(`[data-player-target-id="${defenderPlayerId}"]`)
        : null;

    if (!attackerCenter || !targetCenter) return false;

    attackAnimationId += 1;
    setAttackAnimation({
      id: attackAnimationId,
      startX: attackerCenter.x,
      startY: attackerCenter.y,
      endX: targetCenter.x,
      endY: targetCenter.y,
      attackFailed,
    });
    return true;
  }, []);

  const applyDebugAttackResolution = useCallback(
    (resolution: PendingDebugAttackResolution) => {
      setPlayerSummaries((prev) => ({
        ...prev,
        [resolution.defenderPlayerId]: {
          ...prev[resolution.defenderPlayerId as keyof typeof prev],
          lifePoints: Math.max(
            0,
            prev[resolution.defenderPlayerId as keyof typeof prev].lifePoints - resolution.damageToDefender
          ),
        },
        [viewerPlayerId]: {
          ...prev[viewerPlayerId],
          lifePoints: Math.max(0, prev[viewerPlayerId].lifePoints - resolution.damageToAttacker),
        },
      }));

      setCards((prev) =>
        prev.map((card) => {
          if (resolution.attackerDestroyed && card.id === resolution.attackerCardId) {
            return {
              ...card,
              zone: ZONE_GRAVEYARD,
              fieldIndex: null,
              isFaceDown: false,
              defensePosition: false,
            };
          }

          if (resolution.defenderDestroyed && resolution.defenderCardId && card.id === resolution.defenderCardId) {
            return {
              ...card,
              zone: ZONE_GRAVEYARD,
              fieldIndex: null,
              isFaceDown: false,
              defensePosition: false,
            };
          }

          return card;
        })
      );

      setAttackedCardIds((prev) => [...prev, resolution.attackerCardId]);
    },
    []
  );

  const executeDebugAttack = useCallback(
    (defenderCard: GameCardDto | null, defenderPlayerId: string) => {
      if (!selectedAttackerCard) return;
      if (isResolvingAttack) return;

      const attackerAtk = selectedAttackerCard.card?.attack ?? 0;
      let attackFailed = false;
      let damageToDefender = 0;
      let damageToAttacker = 0;
      let attackerDestroyed = false;
      let defenderDestroyed = false;

      if (!defenderCard) {
        damageToDefender = Math.max(0, attackerAtk);
      } else {
        const defenderStat = defenderCard.defensePosition
          ? (defenderCard.card?.defense ?? 0)
          : (defenderCard.card?.attack ?? 0);

        if (attackerAtk > defenderStat) {
          defenderDestroyed = true;
          if (!defenderCard.defensePosition) {
            damageToDefender = attackerAtk - defenderStat;
          }
        } else if (attackerAtk < defenderStat) {
          attackerDestroyed = true;
          damageToAttacker = defenderStat - attackerAtk;
          attackFailed = true;
        } else if (!defenderCard.defensePosition) {
          attackerDestroyed = true;
          defenderDestroyed = true;
        }
      }

      const resolution: PendingDebugAttackResolution = {
        attackerCardId: selectedAttackerCard.id,
        defenderCardId: defenderCard?.id ?? null,
        defenderPlayerId,
        damageToDefender,
        damageToAttacker,
        attackerDestroyed,
        defenderDestroyed,
        attackFailed,
      };

      const animationStarted = startAttackAnimation(
        resolution.attackerCardId,
        resolution.defenderCardId,
        resolution.defenderPlayerId,
        resolution.attackFailed
      );

      setSelectedAttackerCardId(null);
      if (animationStarted) {
        setPendingAttackResolution(resolution);
        return;
      }

      applyDebugAttackResolution(resolution);
    },
    [applyDebugAttackResolution, isResolvingAttack, selectedAttackerCard, startAttackAnimation]
  );

  return (
    <main className="relative min-h-screen w-full text-zinc-100 box-border">
      <div className="pointer-events-none absolute inset-0 backdrop-blur-[2px]" />
      <div className="pointer-events-none absolute inset-0 bg-linear-to-br from-[#4b1812]/20 via-transparent to-[#091b33]/25" />
      <div className="pointer-events-none absolute inset-0 bg-black/30" />

      <div className="relative h-screen w-full pl-2 pr-3">
        <div className="relative h-full w-full -ml-3">
          <div className="pointer-events-none absolute top-3 left-3 z-20">
            <TurnStatus status={{ activePlayerId: "debug-user", phase: TurnPhase.Battle }} />
          </div>
          <div className="absolute top-3 right-3 z-20 rounded-md border border-white/30 bg-black/45 px-3 py-2 text-[11px] leading-tight text-white/90">
            <div className="font-semibold text-white">Debug Battle Mode (no backend)</div>
            <div className="mt-1 text-white/80">1. Click your attack-position monster (yellow highlight)</div>
            <div className="mt-1 text-white/80">2. Click enemy monster or enemy player panel</div>
            <div className="mt-1 text-white/80">3. Dagger animation runs with success/fail behavior</div>
            <button
              type="button"
              className="mt-2 rounded border border-cyan-200/50 bg-cyan-500/20 px-2 py-1 text-cyan-100"
              onClick={() => {
                setCards(initialCards);
                setAttackedCardIds([]);
                setSelectedAttackerCardId(null);
                setAttackAnimation(null);
                setPendingAttackResolution(null);
                setPlayerSummaries({
                  p1: { username: "debug-user", lifePoints: 8000 },
                  p2: { username: "debug-opponent", lifePoints: 8000 },
                });
              }}
            >
              Reset Demo
            </button>
          </div>

          <Board
            cards={cards}
            playerIds={playerIds}
            viewerPlayerId={viewerPlayerId}
            activePlayerId={activePlayerId}
            selectedAttackerCardId={selectedAttackerCardId}
            playerSummaries={playerSummaries}
            hoveredCard={hoveredCard}
            onHoverCardChange={setHoveredCard}
            onPlayerClick={(playerId) => {
              if (isResolvingAttack) return;
              if (!selectedAttackerCard) return;
              if (playerId === viewerPlayerId) return;
              if (playerHasMonsterOnField(playerId)) return;
              executeDebugAttack(null, playerId);
            }}
            onFieldClick={(playerId, _fieldIndex, card) => {
              if (isResolvingAttack) return;
              if (!selectedAttackerCard) {
                if (card && canUseCardAsAttacker(card)) {
                  setSelectedAttackerCardId(card.id);
                }
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

              const isEnemyMonster =
                playerId !== viewerPlayerId &&
                card !== null &&
                card.zone === ZONE_FIELD &&
                card.fieldIndex !== null &&
                card.fieldIndex <= TOP_ROW_MAX_INDEX;

              if (!isEnemyMonster) return;

              executeDebugAttack(card, playerId);
            }}
            isPlayerClickable={(playerId) =>
              Boolean(selectedAttackerCard) &&
              !isResolvingAttack &&
              playerId !== viewerPlayerId &&
              !playerHasMonsterOnField(playerId) &&
              (playerSummaries[playerId as keyof typeof playerSummaries]?.lifePoints ?? 0) > 0
            }
            isFieldClickable={(playerId, fieldIndex, card) => {
              if (isResolvingAttack) return false;
              if (!selectedAttackerCard) {
                return canUseCardAsAttacker(card);
              }

              if (canUseCardAsAttacker(card)) return true;

              return (
                playerId !== viewerPlayerId &&
                card !== null &&
                card.zone === ZONE_FIELD &&
                card.fieldIndex !== null &&
                card.fieldIndex <= TOP_ROW_MAX_INDEX &&
                fieldIndex <= TOP_ROW_MAX_INDEX
              );
            }}
          />

          <AttackDaggerOverlay
            animation={attackAnimation}
            selectedAttackerCardId={selectedAttackerCardId}
            onAnimationEnd={() => {
              setAttackAnimation(null);

              if (!pendingAttackResolution) return;

              applyDebugAttackResolution(pendingAttackResolution);
              setPendingAttackResolution(null);
            }}
          />
        </div>
      </div>
    </main>
  );
};

export default DebugGamePage;
