import { useEffect, useMemo, useRef, useState } from "react";
import Card from "@/shared/components/Card";
import type { GameCardDto } from "@/features/game/types/game.types";
import CardBack from "@/assets/images/card_back.png";
import { FaSkull } from "react-icons/fa";
import { useCardZonesStore } from "@/shared/enums/cardZones.store";
import DeckFan from "./DeckFan";
import type { CardDto } from "@/shared/types/card.types";

interface BoardProps {
  cards: GameCardDto[];
  viewerPlayerId?: string;
  onHoverCardChange?: (card: CardDto | null) => void;
  onDeckClick?: (playerId: string) => void;
  onFieldClick?: (playerId: string, fieldIndex: number, card: GameCardDto | null) => void;
  isDeckClickable?: (playerId: string) => boolean;
  isFieldClickable?: (playerId: string, fieldIndex: number, card: GameCardDto | null) => boolean;
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
const CARD_RATIO = 120 / 174;
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

const CardField = ({
  playerId,
  fieldIndex,
  card,
  fieldSize,
  viewerPlayerId,
  onHoverCardChange,
  onFieldClick,
  isClickable,
}: {
  playerId: string;
  fieldIndex: number;
  card: GameCardDto | null;
  fieldSize: number;
  viewerPlayerId?: string;
  onHoverCardChange?: (card: CardDto | null) => void;
  onFieldClick?: (playerId: string, fieldIndex: number, card: GameCardDto | null) => void;
  isClickable?: boolean;
}) => {
  const innerGap = Math.max(3, Math.floor(fieldSize * 0.06));
  const verticalRectWidth = Math.max(14, Math.floor(fieldSize * 0.58));
  const verticalRectHeight = Math.max(18, Math.floor(fieldSize * 0.84));
  const horizontalRectWidth = verticalRectHeight;
  const horizontalRectHeight = verticalRectWidth;

  const cardHeight = Math.floor(fieldSize * 0.92);
  const cardWidth = Math.floor(cardHeight * CARD_RATIO);
  const cardFontSize = Math.max(4, Math.floor(cardHeight * 0.075));
  const canPreview = Boolean(
    card?.card && (card.playerId === viewerPlayerId || !card.isFaceDown)
  );

  return (
    <div
      className={`relative rounded-md border bg-black/20 ${isClickable ? "border-cyan-200/55" : "border-white/25"}`}
      style={{ width: `${fieldSize}px`, height: `${fieldSize}px` }}
      onMouseEnter={() => {
        if (!onHoverCardChange) return;
        onHoverCardChange(canPreview ? card?.card ?? null : null);
      }}
      onMouseLeave={() => onHoverCardChange?.(null)}
      onClick={() => onFieldClick?.(playerId, fieldIndex, card)}
    >
      <div className="pointer-events-none absolute inset-0 grid place-items-center">
        <div
          className={`absolute rounded-sm border ${card?.defensePosition ? "border-white/20" : "border-cyan-200/55"}`}
          style={{ width: `${verticalRectWidth}px`, height: `${verticalRectHeight}px` }}
        />
        <div
          className={`absolute rounded-sm border ${card?.defensePosition ? "border-cyan-200/55" : "border-white/20"}`}
          style={{ width: `${horizontalRectWidth}px`, height: `${horizontalRectHeight}px` }}
        />
      </div>

      {card?.card ? (
        <div className="absolute inset-0 flex items-center justify-center" style={{ padding: `${innerGap}px` }}>
          <Card
            name={card.card.name}
            description={card.card.description}
            type={card.card.type}
            attack={card.card.attack}
            defense={card.card.defense}
            level={card.card.level}
            src={card.card.image}
            hasEffect={Boolean(card.card.effectId)}
            hidden={card.isFaceDown}
            className={`${card.defensePosition ? "rotate-90" : ""} text-black`}
            style={{
              width: `${cardWidth}px`,
              height: `${cardHeight}px`,
              fontSize: `${cardFontSize}px`,
            }}
            draggable={false}
          />
        </div>
      ) : null}
    </div>
  );
};

const Board = ({
  cards,
  viewerPlayerId,
  onHoverCardChange,
  onDeckClick,
  onFieldClick,
  isDeckClickable,
  isFieldClickable,
}: BoardProps) => {
  const loadCardZones = useCardZonesStore((s) => s.load);
  const cardZones = useCardZonesStore((s) => s.items);

  useEffect(() => {
    void loadCardZones();
  }, [loadCardZones]);

  const deckZoneValue = useMemo(
    () => cardZones.find((zone) => zone.name.toLowerCase().includes("deck"))?.value ?? null,
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
  const deckCards = useMemo(
    () =>
      cards.filter((card) => {
        if (deckZoneValue !== null) return card.zone === deckZoneValue;
        return card.fieldIndex === null;
      }),
    [cards, deckZoneValue]
  );
  const graveyardCards = useMemo(
    () => cards.filter((card) => card.zone === graveyardZoneValue),
    [cards, graveyardZoneValue]
  );

  const playerIds = useMemo(() => Array.from(new Set(cards.map((card) => card.playerId))), [cards]);
  const playerFieldMap = useMemo(() => createPlayerFieldMap(fieldCards), [fieldCards]);
  const playerDeckMap = useMemo(() => createPlayerCardsMap(deckCards), [deckCards]);
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
    () => computeBoardMetrics(boardSize.width, boardSize.height, playerIds.length),
    [boardSize.height, boardSize.width, playerIds.length]
  );

  if (playerIds.length === 0) {
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
        {playerIds.map((playerId, index) => {
          const topGraveyardCard = playerGraveyardMap[playerId]?.[0] ?? null;
          const position = getPlayerCoordinates(
            playerIds.length,
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
                <div
                  className="grid grid-cols-5 rounded-xl border border-white/20 bg-black/30 p-2"
                  style={{ gap: `${metrics.gap}px` }}
                >
                  {Array.from({ length: 10 }).map((_, fieldIndex) => (
                    <CardField
                      key={`${playerId}-${fieldIndex}`}
                      playerId={playerId}
                      fieldIndex={fieldIndex}
                      card={playerFieldMap[playerId]?.[fieldIndex] ?? null}
                      fieldSize={metrics.field}
                      viewerPlayerId={viewerPlayerId}
                      onHoverCardChange={onHoverCardChange}
                      onFieldClick={onFieldClick}
                      isClickable={isFieldClickable?.(
                        playerId,
                        fieldIndex,
                        playerFieldMap[playerId]?.[fieldIndex] ?? null
                      )}
                    />
                  ))}
                </div>

                <div className="flex flex-col" style={{ gap: `${metrics.gap}px` }}>
                  <div
                    className="grid place-items-center rounded-md border border-white/30 bg-black/25 text-white/45"
                    style={{ width: `${metrics.deckWidth}px`, height: `${metrics.deckHeight}px` }}
                    onMouseEnter={() => {
                      if (!onHoverCardChange) return;
                      const canPreview = Boolean(
                        topGraveyardCard?.card &&
                        (topGraveyardCard.playerId === viewerPlayerId || !topGraveyardCard.isFaceDown)
                      );
                      onHoverCardChange(canPreview ? topGraveyardCard?.card ?? null : null);
                    }}
                    onMouseLeave={() => onHoverCardChange?.(null)}
                  >
                    {topGraveyardCard?.card ? (
                      <Card
                        name={topGraveyardCard.card.name}
                        description={topGraveyardCard.card.description}
                        type={topGraveyardCard.card.type}
                        attack={topGraveyardCard.card.attack}
                        defense={topGraveyardCard.card.defense}
                        level={topGraveyardCard.card.level}
                        src={topGraveyardCard.card.image}
                        hasEffect={Boolean(topGraveyardCard.card.effectId)}
                        hidden={topGraveyardCard.isFaceDown}
                        className={`${topGraveyardCard.defensePosition ? "rotate-90" : ""} text-black`}
                        style={{
                          width: `${metrics.deckWidth}px`,
                          height: `${metrics.deckHeight}px`,
                          fontSize: `${Math.max(4, Math.floor(metrics.deckHeight * 0.07))}px`,
                        }}
                        draggable={false}
                      />
                    ) : (
                      <FaSkull className="size-[42%]" />
                    )}
                  </div>
                  <button
                    type="button"
                    className="rounded-md border border-white/30 bg-center bg-cover"
                    style={{
                      width: `${metrics.deckWidth}px`,
                      height: `${metrics.deckHeight}px`,
                      backgroundImage: `url(${CardBack})`,
                      cursor: isDeckClickable?.(playerId) ? "pointer" : "default",
                    }}
                    onClick={() => onDeckClick?.(playerId)}
                  />
                </div>
              </div>

              <div
                className="absolute left-1/2"
                style={{
                  top: `${metrics.zoneHeight + Math.max(14, metrics.field * 0.22)}px`,
                  transform: "translateX(-50%)",
                }}
              >
                <DeckFan
                  cards={playerDeckMap[playerId] ?? []}
                  fieldSize={metrics.field}
                  hideCards={viewerPlayerId !== undefined && viewerPlayerId !== playerId}
                  viewerPlayerId={viewerPlayerId}
                  onHoverCardChange={onHoverCardChange}
                />
              </div>
            </section>
          );
        })}
      </div>
    </div>
  );
};

export default Board;
