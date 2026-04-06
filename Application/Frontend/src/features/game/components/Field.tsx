import { useState } from "react";
import Card from "@/shared/components/Card";
import type { GameCardDto } from "@/features/game/types/game.types";
import type { CardDto } from "@/shared/types/card.types";
import { getImageUrl } from "@/shared/api/httpClient";

interface FieldProps {
  playerId: string;
  fieldCards: Record<number, GameCardDto> | undefined;
  fieldSize: number;
  gap: number;
  viewerPlayerId?: string;
  selectedAttackerCardId?: string | null;
  onHoverCardChange?: (card: CardDto | null) => void;
  onFieldClick?: (playerId: string, fieldIndex: number, card: GameCardDto | null) => void;
  onFieldRightClick?: (playerId: string, fieldIndex: number, card: GameCardDto | null) => void;
  onFieldPositionClick?: (
    playerId: string,
    fieldIndex: number,
    card: GameCardDto,
    targetDefensePosition: boolean
  ) => void;
  onFieldPlacementPositionHover?: (
    playerId: string,
    fieldIndex: number,
    targetDefensePosition: boolean
  ) => void;
  isFieldClickable?: (playerId: string, fieldIndex: number, card: GameCardDto | null) => boolean;
  isFieldRightClickable?: (playerId: string, fieldIndex: number, card: GameCardDto | null) => boolean;
  canClickFieldPosition?: (
    playerId: string,
    fieldIndex: number,
    card: GameCardDto,
    targetDefensePosition: boolean
  ) => boolean;
  canHoverFieldPlacementPosition?: (
    playerId: string,
    fieldIndex: number,
    targetDefensePosition: boolean
  ) => boolean;
}

const CARD_RATIO = 120 / 174;

const CardField = ({
  playerId,
  fieldIndex,
  card,
  fieldSize,
  viewerPlayerId,
  onHoverCardChange,
  onFieldClick,
  onFieldRightClick,
  onFieldPositionClick,
  onFieldPlacementPositionHover,
  isClickable,
  isRightClickable,
  canClickAttackPosition,
  canClickDefensePosition,
  canHoverPlaceAttackPosition,
  canHoverPlaceDefensePosition,
  selectedAttackerCardId,
}: {
  playerId: string;
  fieldIndex: number;
  card: GameCardDto | null;
  fieldSize: number;
  viewerPlayerId?: string;
  onHoverCardChange?: (card: CardDto | null) => void;
  onFieldClick?: (playerId: string, fieldIndex: number, card: GameCardDto | null) => void;
  onFieldRightClick?: (playerId: string, fieldIndex: number, card: GameCardDto | null) => void;
  onFieldPositionClick?: (
    playerId: string,
    fieldIndex: number,
    card: GameCardDto,
    targetDefensePosition: boolean
  ) => void;
  onFieldPlacementPositionHover?: (
    playerId: string,
    fieldIndex: number,
    targetDefensePosition: boolean
  ) => void;
  isClickable?: boolean;
  isRightClickable?: boolean;
  canClickAttackPosition?: boolean;
  canClickDefensePosition?: boolean;
  canHoverPlaceAttackPosition?: boolean;
  canHoverPlaceDefensePosition?: boolean;
  selectedAttackerCardId?: string | null;
}) => {
  const [isHovered, setIsHovered] = useState(false);
  const innerGap = Math.max(3, Math.floor(fieldSize * 0.06));
  const verticalRectWidth = Math.max(14, Math.floor(fieldSize * 0.58));
  const verticalRectHeight = Math.max(18, Math.floor(fieldSize * 0.84));
  const horizontalRectWidth = verticalRectHeight;
  const horizontalRectHeight = verticalRectWidth;

  const cardHeight = Math.floor(fieldSize * 0.92);
  const cardWidth = Math.floor(cardHeight * CARD_RATIO);
  const cardFontSize = Math.max(4, Math.floor(cardHeight * 0.075));
  const showPlacementPositionPickers = Boolean(
    !card && (canHoverPlaceAttackPosition || canHoverPlaceDefensePosition)
  );
  const canPreview = Boolean(
    card?.card && (card.playerId === viewerPlayerId || !card.isFaceDown)
  );

  return (
    <div
      className={`relative rounded-md border bg-black/20 ${
        card?.id === selectedAttackerCardId
          ? "border-amber-200/90 bg-amber-400/10 shadow-[0_0_0_1px_rgba(252,211,77,0.85),0_0_16px_rgba(245,158,11,0.25)]"
          : isHovered
          ? "border-cyan-100/90 bg-cyan-500/10 shadow-[0_0_0_1px_rgba(165,243,252,0.7),0_0_16px_rgba(34,211,238,0.18)]"
          : isClickable || isRightClickable
            ? "border-cyan-200/55"
            : "border-white/25"
      }`}
      style={{ width: `${fieldSize}px`, height: `${fieldSize}px` }}
      data-field-slot={`${playerId}:${fieldIndex}`}
      onMouseEnter={() => {
        setIsHovered(true);
        if (!onHoverCardChange) return;
        onHoverCardChange(canPreview ? card?.card ?? null : null);
      }}
      onMouseLeave={() => {
        setIsHovered(false);
        onHoverCardChange?.(null);
      }}
      onClick={() => onFieldClick?.(playerId, fieldIndex, card)}
      onContextMenu={(event) => {
        event.preventDefault();
        onFieldRightClick?.(playerId, fieldIndex, card);
      }}
    >
      <div className="pointer-events-none absolute inset-0 grid place-items-center">
        <div
          className={`absolute z-10 rounded-sm border ${
            card?.defensePosition ? "border-white/20" : "border-cyan-200/55"
          }`}
          style={{ width: `${verticalRectWidth}px`, height: `${verticalRectHeight}px` }}
        />
        <div
          className={`absolute rounded-sm border ${
            card?.defensePosition ? "border-cyan-200/55" : "border-white/20"
          }`}
          style={{ width: `${horizontalRectWidth}px`, height: `${horizontalRectHeight}px` }}
        />
      </div>
      {card || showPlacementPositionPickers ? (
        <>
          <div
            className={`absolute left-1/2 top-1/2 z-10 -translate-x-1/2 -translate-y-1/2 rounded-sm ${
              card
                ? (canClickAttackPosition ? "cursor-pointer" : "")
                : (
                    canHoverPlaceAttackPosition
                      ? "cursor-pointer border border-cyan-200/40 bg-cyan-300/8 hover:bg-cyan-300/20"
                      : ""
                  )
            }`}
            style={{ width: `${verticalRectWidth}px`, height: `${verticalRectHeight}px` }}
            onMouseEnter={() => {
              if (card || !canHoverPlaceAttackPosition || !onFieldPlacementPositionHover) return;
              onFieldPlacementPositionHover(playerId, fieldIndex, false);
            }}
            onClick={(event) => {
              if (!card) return;
              if (!canClickAttackPosition || !onFieldPositionClick) return;
              event.stopPropagation();
              onFieldPositionClick(playerId, fieldIndex, card, false);
            }}
          />
          <div
            className={`absolute left-1/2 top-1/2 z-[9] -translate-x-1/2 -translate-y-1/2 rounded-sm ${
              card
                ? (canClickDefensePosition ? "cursor-pointer" : "")
                : (
                    canHoverPlaceDefensePosition
                      ? "cursor-pointer border border-cyan-200/40 bg-cyan-300/8 hover:bg-cyan-300/20"
                      : ""
                  )
            }`}
            style={{ width: `${horizontalRectWidth}px`, height: `${horizontalRectHeight}px` }}
            onMouseEnter={() => {
              if (card || !canHoverPlaceDefensePosition || !onFieldPlacementPositionHover) return;
              onFieldPlacementPositionHover(playerId, fieldIndex, true);
            }}
            onClick={(event) => {
              if (!card) return;
              if (!canClickDefensePosition || !onFieldPositionClick) return;
              event.stopPropagation();
              onFieldPositionClick(playerId, fieldIndex, card, true);
            }}
          />
        </>
      ) : null}

      {card ? (
        <div
          className="pointer-events-none absolute inset-0 flex items-center justify-center"
          style={{ padding: `${innerGap}px` }}
          data-game-card-id={card.id}
        >
          <Card
            name={card.card?.name ?? "Card"}
            description={card.card?.description ?? ""}
            type={card.card?.type ?? 0}
            attack={card.card?.attack ?? null}
            defense={card.card?.defense ?? null}
            level={card.card?.level ?? null}
            src={getImageUrl(card.card?.image ?? "")}
            hasEffect={Boolean(card.card?.effectId)}
            hidden={card.isFaceDown || !card.card}
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

const Field = ({
  playerId,
  fieldCards,
  fieldSize,
  gap,
  viewerPlayerId,
  selectedAttackerCardId,
  onHoverCardChange,
  onFieldClick,
  onFieldRightClick,
  onFieldPositionClick,
  onFieldPlacementPositionHover,
  isFieldClickable,
  isFieldRightClickable,
  canClickFieldPosition,
  canHoverFieldPlacementPosition,
}: FieldProps) => {
  return (
    <div
      className="grid grid-cols-5 rounded-xl border border-white/20 bg-black/30 p-2"
      style={{ gap: `${gap}px` }}
    >
      {Array.from({ length: 10 }).map((_, fieldIndex) => {
        const slotCard = fieldCards?.[fieldIndex] ?? null;

        return (
          <CardField
            key={`${playerId}-${fieldIndex}`}
            playerId={playerId}
            fieldIndex={fieldIndex}
            card={slotCard}
            fieldSize={fieldSize}
            viewerPlayerId={viewerPlayerId}
            selectedAttackerCardId={selectedAttackerCardId}
            onHoverCardChange={onHoverCardChange}
            onFieldClick={onFieldClick}
            onFieldRightClick={onFieldRightClick}
            onFieldPositionClick={onFieldPositionClick}
            onFieldPlacementPositionHover={onFieldPlacementPositionHover}
            isClickable={isFieldClickable?.(playerId, fieldIndex, slotCard)}
            isRightClickable={isFieldRightClickable?.(playerId, fieldIndex, slotCard)}
            canClickAttackPosition={Boolean(
              slotCard && canClickFieldPosition?.(playerId, fieldIndex, slotCard, false)
            )}
            canClickDefensePosition={Boolean(
              slotCard && canClickFieldPosition?.(playerId, fieldIndex, slotCard, true)
            )}
            canHoverPlaceAttackPosition={Boolean(
              !slotCard && canHoverFieldPlacementPosition?.(playerId, fieldIndex, false)
            )}
            canHoverPlaceDefensePosition={Boolean(
              !slotCard && canHoverFieldPlacementPosition?.(playerId, fieldIndex, true)
            )}
          />
        );
      })}
    </div>
  );
};

export default Field;
