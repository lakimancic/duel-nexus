import Card from "@/shared/components/Card";
import type { GameCardDto } from "@/features/game/types/game.types";
import type { CardDto } from "@/shared/types/card.types";

interface DeckFanProps {
  cards: GameCardDto[];
  fieldSize: number;
  hideCards: boolean;
  viewerPlayerId?: string;
  onHoverCardChange?: (card: CardDto | null) => void;
  onCardClick?: (card: GameCardDto) => void;
  isCardClickable?: (card: GameCardDto) => boolean;
  selectedCardId?: string | null;
}

const CARD_RATIO = 120 / 174;

const DeckFan = ({
  cards,
  fieldSize,
  hideCards,
  viewerPlayerId,
  onHoverCardChange,
  onCardClick,
  isCardClickable,
  selectedCardId,
}: DeckFanProps) => {
  if (cards.length === 0) return null;

  const shownCards = cards.slice(0, 8);
  const cardHeight = Math.max(54, Math.floor(fieldSize * 1.24));
  const cardWidth = Math.floor(cardHeight * CARD_RATIO);
  const spread = Math.max(16, Math.floor(cardWidth * 0.86));
  const rotateStep = Math.min(8, 42 / Math.max(1, shownCards.length));
  const curveDepth = Math.max(10, Math.floor(fieldSize * 0.28));
  const fontSize = Math.max(5, Math.floor(cardHeight * 0.07));
  const containerWidth = (shownCards.length - 1) * spread + cardWidth;

  return (
    <div className="relative" style={{ width: `${containerWidth}px`, height: `${cardHeight + curveDepth}px` }}>
      {shownCards.map((card, index) => {
        const center = (shownCards.length - 1) / 2;
        const offset = index - center;
        const x = offset * spread;
        const y = Math.abs(offset) * (curveDepth / Math.max(1, center + 0.5));
        const rotate = offset * rotateStep;

        return (
          <div
            key={`${card.playerId}-deck-${card.card?.id ?? index}`}
            className="absolute top-0 left-1/2 origin-bottom"
            style={{
              transform: `translateX(calc(-50% + ${x}px)) translateY(${y}px) rotate(${rotate}deg)`,
              zIndex: index + 1,
              cursor: isCardClickable?.(card) ? "pointer" : "default",
            }}
            onMouseEnter={() => {
              if (!onHoverCardChange) return;
              const canPreview = Boolean(
                card.card &&
                !hideCards &&
                (card.playerId === viewerPlayerId || !card.isFaceDown)
              );
              onHoverCardChange(canPreview ? card.card ?? null : null);
            }}
            onMouseLeave={() => onHoverCardChange?.(null)}
            onClick={() => onCardClick?.(card)}
          >
            <Card
              name={card.card?.name ?? "Card"}
              description={card.card?.description ?? ""}
              type={card.card?.type ?? 0}
              attack={card.card?.attack ?? null}
              defense={card.card?.defense ?? null}
              level={card.card?.level ?? null}
              src={card.card?.image ?? ""}
              hasEffect={Boolean(card.card?.effectId)}
              hidden={hideCards || card.isFaceDown || !card.card}
              className={`text-black ${selectedCardId === card.card?.id ? "ring-2 ring-cyan-200 ring-offset-2 ring-offset-black/60" : ""}`}
              style={{
                width: `${cardWidth}px`,
                height: `${cardHeight}px`,
                fontSize: `${fontSize}px`,
              }}
              draggable={false}
            />
          </div>
        );
      })}
    </div>
  );
};

export default DeckFan;
