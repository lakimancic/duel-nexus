import Card from "@/shared/components/Card";
import type { GameCardDto } from "@/features/game/types/game.types";
import type { CardDto } from "@/shared/types/card.types";
import { FaSkull } from "react-icons/fa";

interface GraveyardProps {
  playerId: string;
  topGraveyardCard: GameCardDto | null;
  deckWidth: number;
  deckHeight: number;
  viewerPlayerId?: string;
  onHoverCardChange?: (card: CardDto | null) => void;
  onGraveyardClick?: (playerId: string) => void;
  isGraveyardClickable?: (playerId: string) => boolean;
}

const Graveyard = ({
  playerId,
  topGraveyardCard,
  deckWidth,
  deckHeight,
  viewerPlayerId,
  onHoverCardChange,
  onGraveyardClick,
  isGraveyardClickable,
}: GraveyardProps) => {
  return (
    <div
      className={`grid place-items-center rounded-md border text-white/45 ${
        isGraveyardClickable?.(playerId)
          ? "border-cyan-200/55 bg-black/35 cursor-pointer"
          : "border-white/30 bg-black/25"
      }`}
      style={{ width: `${deckWidth}px`, height: `${deckHeight}px` }}
      onMouseEnter={() => {
        if (!onHoverCardChange) return;
        const canPreview = Boolean(
          topGraveyardCard?.card &&
          (topGraveyardCard.playerId === viewerPlayerId || !topGraveyardCard.isFaceDown)
        );
        onHoverCardChange(canPreview ? topGraveyardCard?.card ?? null : null);
      }}
      onMouseLeave={() => onHoverCardChange?.(null)}
      onClick={() => onGraveyardClick?.(playerId)}
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
            width: `${deckWidth}px`,
            height: `${deckHeight}px`,
            fontSize: `${Math.max(4, Math.floor(deckHeight * 0.07))}px`,
          }}
          draggable={false}
        />
      ) : (
        <FaSkull className="size-[42%]" />
      )}
    </div>
  );
};

export default Graveyard;
