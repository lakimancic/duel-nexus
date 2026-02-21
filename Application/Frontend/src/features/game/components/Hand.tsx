import type { GameCardDto } from "@/features/game/types/game.types";
import type { CardDto } from "@/shared/types/card.types";
import DeckFan from "./DeckFan";

interface HandProps {
  playerId: string;
  cards: GameCardDto[];
  fieldSize: number;
  zoneHeight: number;
  viewerPlayerId?: string;
  onHoverCardChange?: (card: CardDto | null) => void;
  onHandCardClick?: (card: GameCardDto) => void;
  isHandCardClickable?: (card: GameCardDto) => boolean;
  selectedHandCardId?: string | null;
}

const Hand = ({
  playerId,
  cards,
  fieldSize,
  zoneHeight,
  viewerPlayerId,
  onHoverCardChange,
  onHandCardClick,
  isHandCardClickable,
  selectedHandCardId,
}: HandProps) => {
  return (
    <div
      className="absolute left-1/2"
      style={{
        top: `${zoneHeight + Math.max(14, fieldSize * 0.22)}px`,
        transform: "translateX(-50%)",
      }}
    >
      <DeckFan
        cards={cards}
        fieldSize={fieldSize}
        hideCards={viewerPlayerId !== undefined && viewerPlayerId !== playerId}
        viewerPlayerId={viewerPlayerId}
        onHoverCardChange={onHoverCardChange}
        onCardClick={onHandCardClick}
        isCardClickable={isHandCardClickable}
        selectedCardId={selectedHandCardId}
      />
    </div>
  );
};

export default Hand;
