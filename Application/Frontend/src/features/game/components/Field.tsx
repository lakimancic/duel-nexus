import Card from "@/shared/components/Card";
import type { GameCardDto } from "@/features/game/types/game.types";
import type { CardDto } from "@/shared/types/card.types";

interface FieldProps {
  playerId: string;
  fieldCards: Record<number, GameCardDto> | undefined;
  fieldSize: number;
  gap: number;
  viewerPlayerId?: string;
  onHoverCardChange?: (card: CardDto | null) => void;
  onFieldClick?: (playerId: string, fieldIndex: number, card: GameCardDto | null) => void;
  isFieldClickable?: (playerId: string, fieldIndex: number, card: GameCardDto | null) => boolean;
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

const Field = ({
  playerId,
  fieldCards,
  fieldSize,
  gap,
  viewerPlayerId,
  onHoverCardChange,
  onFieldClick,
  isFieldClickable,
}: FieldProps) => {
  return (
    <div
      className="grid grid-cols-5 rounded-xl border border-white/20 bg-black/30 p-2"
      style={{ gap: `${gap}px` }}
    >
      {Array.from({ length: 10 }).map((_, fieldIndex) => (
        <CardField
          key={`${playerId}-${fieldIndex}`}
          playerId={playerId}
          fieldIndex={fieldIndex}
          card={fieldCards?.[fieldIndex] ?? null}
          fieldSize={fieldSize}
          viewerPlayerId={viewerPlayerId}
          onHoverCardChange={onHoverCardChange}
          onFieldClick={onFieldClick}
          isClickable={isFieldClickable?.(playerId, fieldIndex, fieldCards?.[fieldIndex] ?? null)}
        />
      ))}
    </div>
  );
};

export default Field;
