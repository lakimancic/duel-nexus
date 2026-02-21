import CardBack from "@/assets/images/card_back.png";

interface DeckProps {
  playerId: string;
  deckWidth: number;
  deckHeight: number;
  onDeckClick?: (playerId: string) => void;
  isDeckClickable?: (playerId: string) => boolean;
}

const Deck = ({
  playerId,
  deckWidth,
  deckHeight,
  onDeckClick,
  isDeckClickable,
}: DeckProps) => {
  return (
    <button
      type="button"
      className="rounded-md border border-white/30 bg-center bg-cover"
      style={{
        width: `${deckWidth}px`,
        height: `${deckHeight}px`,
        backgroundImage: `url(${CardBack})`,
        cursor: isDeckClickable?.(playerId) ? "pointer" : "default",
      }}
      onClick={() => onDeckClick?.(playerId)}
    />
  );
};

export default Deck;
