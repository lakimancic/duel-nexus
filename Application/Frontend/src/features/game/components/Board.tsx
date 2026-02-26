import { useEffect, useMemo, useRef, useState } from "react";
import Card from "@/shared/components/Card";
import type { GameCardDto } from "@/features/game/types/game.types";
import { useCardZonesStore } from "@/shared/enums/cardZones.store";
import type { CardDto } from "@/shared/types/card.types";
import Field from "./Field";
import Deck from "./Deck";
import Graveyard from "./Graveyard";
import Hand from "./Hand";
import { getImageUrl } from "@/shared/api/httpClient";

interface BoardProps {
  cards: GameCardDto[];
  playerIds?: string[];
  viewerPlayerId?: string;
  hoveredCard?: CardDto | null;
  onHoverCardChange?: (card: CardDto | null) => void;
  onDeckClick?: (playerId: string) => void;
  onFieldClick?: (playerId: string, fieldIndex: number, card: GameCardDto | null) => void;
  onGraveyardClick?: (playerId: string) => void;
  onHandCardClick?: (card: GameCardDto) => void;
  isDeckClickable?: (playerId: string) => boolean;
  isFieldClickable?: (playerId: string, fieldIndex: number, card: GameCardDto | null) => boolean;
  isGraveyardClickable?: (playerId: string) => boolean;
  isHandCardClickable?: (card: GameCardDto) => boolean;
  selectedHandCardId?: string | null;
}

interface Metrics {
  width: number;
  height: number;
  field: number;
  gap: number;
  deckWidth: number;
  deckHeight: number;
  sideGap: number;
  zoneWidth: number;
  zoneHeight: number;
  zoneDiagonal: number;
  radius: number;
}

const MIN_FIELD = 30;
const MAX_FIELD = 110;
const OVERLAP_SAFETY_FACTOR = 1.08;

const createPlayerFieldMap = (cards: GameCardDto[]) =>
  cards.reduce<Record<string, Record<number, GameCardDto>>>((acc, card) => {
    if (card.fieldIndex === null || card.fieldIndex < 0 || card.fieldIndex > 9) return acc;
    if (!acc[card.playerId]) acc[card.playerId] = {};
    acc[card.playerId][card.fieldIndex] = card;
    return acc;
  }, {});

const createPlayerCardsMap = (cards: GameCardDto[]) =>
  cards.reduce<Record<string, GameCardDto[]>>((acc, card) => {
    if (!acc[card.playerId]) acc[card.playerId] = [];
    acc[card.playerId].push(card);
    return acc;
  }, {});

const buildZoneMetrics = (field: number) => {
  const gap = Math.max(6, Math.round(field * 0.12));
  const sideGap = Math.max(4, Math.round(field * 0.1));
  const deckWidth = Math.round(field * 0.72);
  const deckHeight = Math.round(deckWidth * 1.45);
  const zoneWidth = 5 * field + 4 * gap + sideGap + deckWidth;
  const zoneHeight = Math.max(2 * field + gap, 2 * deckHeight + gap);
  const zoneDiagonal = Math.hypot(zoneWidth, zoneHeight);

  return { gap, sideGap, deckWidth, deckHeight, zoneWidth, zoneHeight, zoneDiagonal };
};

const projectedHalfExtent = (
  halfWidth: number,
  halfHeight: number,
  zoneRotation: number,
  dirX: number,
  dirY: number
) => {
  const axisXx = Math.cos(zoneRotation);
  const axisXy = Math.sin(zoneRotation);
  const axisYx = -Math.sin(zoneRotation);
  const axisYy = Math.cos(zoneRotation);

  const projOnX = Math.abs(dirX * axisXx + dirY * axisXy);
  const projOnY = Math.abs(dirX * axisYx + dirY * axisYy);

  return halfWidth * projOnX + halfHeight * projOnY;
};

const computeMinimumPolygonRadius = (
  playersCount: number,
  zoneWidth: number,
  zoneHeight: number,
  margin: number
) => {
  if (playersCount <= 1) return 0;

  const halfWidth = zoneWidth / 2;
  const halfHeight = zoneHeight / 2;
  const step = (2 * Math.PI) / playersCount;
  const startAngle = Math.PI / 2;

  let minRadius = 0;

  for (let i = 0; i < playersCount; i += 1) {
    const thetaI = startAngle + i * step;
    const piX = Math.cos(thetaI);
    const piY = Math.sin(thetaI);
    const rotationI = thetaI - Math.PI / 2;

    for (let j = i + 1; j < playersCount; j += 1) {
      const thetaJ = startAngle + j * step;
      const pjX = Math.cos(thetaJ);
      const pjY = Math.sin(thetaJ);
      const rotationJ = thetaJ - Math.PI / 2;

      const dx = pjX - piX;
      const dy = pjY - piY;
      const centersDistanceOnUnitCircle = Math.hypot(dx, dy);
      if (centersDistanceOnUnitCircle < 1e-6) continue;

      const dirX = dx / centersDistanceOnUnitCircle;
      const dirY = dy / centersDistanceOnUnitCircle;

      const extentI = projectedHalfExtent(halfWidth, halfHeight, rotationI, dirX, dirY);
      const extentJ = projectedHalfExtent(halfWidth, halfHeight, rotationJ, dirX, dirY);

      const pairRequiredRadius =
        ((extentI + extentJ + margin) / centersDistanceOnUnitCircle) * OVERLAP_SAFETY_FACTOR;

      if (pairRequiredRadius > minRadius) minRadius = pairRequiredRadius;
    }
  }

  return minRadius;
};

const computeBoardMetrics = (width: number, height: number, playersCount: number): Metrics => {
  const safeWidth = Math.max(320, width);
  const safeHeight = Math.max(320, height);
  const playerCount = Math.max(1, playersCount);
  const pagePadding = 5;

  const canFit = (field: number) => {
    const zone = buildZoneMetrics(field);
    const minRadius = computeMinimumPolygonRadius(
      playerCount,
      zone.zoneWidth,
      zone.zoneHeight,
      Math.max(6, field * 0.1)
    );
    const maxRadius = Math.min(safeWidth, safeHeight) / 2 - zone.zoneDiagonal / 2 - pagePadding;

    return {
      ok: maxRadius >= minRadius,
      minRadius,
      maxRadius,
      zone,
    };
  };

  let low = MIN_FIELD;
  let high = Math.min(MAX_FIELD, Math.floor(Math.min(safeWidth, safeHeight) / 3));
  let best = MIN_FIELD;

  for (let i = 0; i < 18; i += 1) {
    const mid = (low + high) / 2;
    if (canFit(mid).ok) {
      best = mid;
      low = mid;
    } else {
      high = mid;
    }
  }

  const field = Math.max(MIN_FIELD, Math.floor(best));
  const fit = canFit(field);
  const nearTouchPadding = Math.max(4, field * 0.06);
  const radius =
    playerCount > 1
      ? Math.max(
          0,
          Math.min(fit.maxRadius, fit.minRadius + nearTouchPadding)
        )
      : 0;

  return {
    width: safeWidth,
    height: safeHeight,
    field,
    radius,
    ...fit.zone,
  };
};

const getPlayerCoordinates = (
  playersCount: number,
  index: number,
  radius: number,
  width: number,
  height: number
) => {
  const step = (2 * Math.PI) / playersCount;
  const angle = Math.PI / 2 + index * step;

  return {
    x: width / 2 + radius * Math.cos(angle),
    y: height / 2 + radius * Math.sin(angle),
    rotation: (angle * 180) / Math.PI - 90,
  };
};

const Board = ({
  cards,
  playerIds: orderedPlayerIds,
  viewerPlayerId,
  hoveredCard,
  onHoverCardChange,
  onDeckClick,
  onFieldClick,
  onGraveyardClick,
  onHandCardClick,
  isDeckClickable,
  isFieldClickable,
  isGraveyardClickable,
  isHandCardClickable,
  selectedHandCardId,
}: BoardProps) => {
  const loadCardZones = useCardZonesStore((s) => s.load);
  const cardZones = useCardZonesStore((s) => s.items);

  useEffect(() => {
    void loadCardZones();
  }, [loadCardZones]);

  const handZoneValue = useMemo(
    () => cardZones.find((zone) => zone.name.toLowerCase().includes("hand"))?.value ?? null,
    [cardZones]
  );
  const fieldZoneValue = useMemo(
    () =>
      cardZones.find((zone) => {
        const name = zone.name.toLowerCase();
        return name.includes("field") || name.includes("board");
      })?.value ?? null,
    [cardZones]
  );
  const graveyardZoneValue = useMemo(
    () =>
      cardZones.find((zone) => {
        const name = zone.name.toLowerCase();
        return name.includes("grave") || name.includes("cemetery");
      })?.value ?? 3,
    [cardZones]
  );

  const fieldCards = useMemo(
    () =>
      cards.filter(
        (card) =>
          card.fieldIndex !== null && (fieldZoneValue === null || card.zone === fieldZoneValue)
      ),
    [cards, fieldZoneValue]
  );
  const handCards = useMemo(
    () =>
      cards.filter((card) => {
        if (handZoneValue !== null) return card.zone === handZoneValue;
        return card.fieldIndex === null;
      }),
    [cards, handZoneValue]
  );
  const graveyardCards = useMemo(
    () => cards.filter((card) => card.zone === graveyardZoneValue),
    [cards, graveyardZoneValue]
  );

  const playerIds = useMemo(
    () =>
      orderedPlayerIds && orderedPlayerIds.length > 0
        ? orderedPlayerIds
        : Array.from(new Set(cards.map((card) => card.playerId))),
    [cards, orderedPlayerIds]
  );
  const displayedPlayerIds = useMemo(() => {
    if (!viewerPlayerId) return playerIds;

    const viewerIndex = playerIds.indexOf(viewerPlayerId);
    if (viewerIndex < 0) return playerIds;
    if (viewerIndex === 0) return playerIds;

    return [...playerIds.slice(viewerIndex), ...playerIds.slice(0, viewerIndex)];
  }, [playerIds, viewerPlayerId]);
  const playerFieldMap = useMemo(() => createPlayerFieldMap(fieldCards), [fieldCards]);
  const playerHandMap = useMemo(() => createPlayerCardsMap(handCards), [handCards]);
  const playerGraveyardMap = useMemo(() => createPlayerCardsMap(graveyardCards), [graveyardCards]);

  const boardRef = useRef<HTMLDivElement>(null);
  const [boardSize, setBoardSize] = useState({ width: 1200, height: 800 });

  useEffect(() => {
    const element = boardRef.current;
    if (!element) return;

    const observer = new ResizeObserver((entries) => {
      const entry = entries[0];
      const { width, height } = entry.contentRect;
      setBoardSize({ width, height });
    });

    observer.observe(element);
    return () => observer.disconnect();
  }, []);

  const metrics = useMemo(
    () => computeBoardMetrics(boardSize.width, boardSize.height, displayedPlayerIds.length),
    [boardSize.height, boardSize.width, displayedPlayerIds.length]
  );

  if (displayedPlayerIds.length === 0) {
    return (
      <div className="rounded-xl border border-white/20 bg-black/25 p-6 text-white/80">
        Add at least one player card to render the board.
      </div>
    );
  }

  return (
    <div
      ref={boardRef}
      className="relative h-screen w-full overflow-hidden rounded-2xl border border-white/20 bg-black/25"
      onMouseLeave={() => onHoverCardChange?.(null)}
    >
      <div className="pointer-events-none absolute inset-0 backdrop-blur-[3px]" />
      <div className="relative h-full w-full">
        {displayedPlayerIds.map((playerId, index) => {
          const topGraveyardCard = playerGraveyardMap[playerId]?.[0] ?? null;
          const position = getPlayerCoordinates(
            displayedPlayerIds.length,
            index,
            metrics.radius,
            metrics.width,
            metrics.height
          );

          return (
            <section
              key={playerId}
              className="absolute"
              style={{
                left: `${position.x}px`,
                top: `${position.y}px`,
                transform: `translate(-50%, -50%) rotate(${position.rotation}deg)`,
              }}
            >
              <div className="flex items-center" style={{ gap: `${metrics.sideGap}px` }}>
                <Field
                  playerId={playerId}
                  fieldCards={playerFieldMap[playerId]}
                  fieldSize={metrics.field}
                  gap={metrics.gap}
                  viewerPlayerId={viewerPlayerId}
                  onHoverCardChange={onHoverCardChange}
                  onFieldClick={onFieldClick}
                  isFieldClickable={isFieldClickable}
                />

                <div className="flex flex-col" style={{ gap: `${metrics.gap}px` }}>
                  <Graveyard
                    playerId={playerId}
                    topGraveyardCard={topGraveyardCard}
                    deckWidth={metrics.deckWidth}
                    deckHeight={metrics.deckHeight}
                    viewerPlayerId={viewerPlayerId}
                    onHoverCardChange={onHoverCardChange}
                    onGraveyardClick={onGraveyardClick}
                    isGraveyardClickable={isGraveyardClickable}
                  />
                  <Deck
                    playerId={playerId}
                    deckWidth={metrics.deckWidth}
                    deckHeight={metrics.deckHeight}
                    onDeckClick={onDeckClick}
                    isDeckClickable={isDeckClickable}
                  />
                </div>
              </div>

              <Hand
                playerId={playerId}
                cards={playerHandMap[playerId] ?? []}
                fieldSize={metrics.field}
                zoneHeight={metrics.zoneHeight}
                viewerPlayerId={viewerPlayerId}
                onHoverCardChange={onHoverCardChange}
                onHandCardClick={onHandCardClick}
                isHandCardClickable={isHandCardClickable}
                selectedHandCardId={selectedHandCardId}
              />
            </section>
          );
        })}
      </div>

      <aside className="absolute right-3 bottom-3 z-20 w-[300px] rounded-xl border border-white/20 bg-black/45 p-2">
        <div className="flex min-h-[250px] items-center justify-center rounded border border-white/20 bg-black/35 p-2">
          {hoveredCard ? (
            <Card
              name={hoveredCard.name}
              description={hoveredCard.description}
              type={hoveredCard.type}
              attack={hoveredCard.attack}
              defense={hoveredCard.defense}
              level={hoveredCard.level}
              src={getImageUrl(hoveredCard.image)}
              hasEffect={Boolean(hoveredCard.effectId)}
              hidden={false}
              className="text-black"
              style={{ width: "100%", maxWidth: "280px", height: "auto", maxHeight: "95%" }}
              draggable={false}
            />
          ) : (
            <p className="text-center text-sm text-white/55 px-4">
              Hover a visible card to preview it here.
            </p>
          )}
        </div>
      </aside>
    </div>
  );
};

export default Board;
