import Spinner from "@/shared/components/Spinner";
import type { CardDto } from "../../types/card.types";
import CardItem from "./CardItem";

const CardsGrid = ({
  cards,
  isLoading,
  onCardClick,
}: {
  cards: CardDto[];
  isLoading: boolean;
  onCardClick: (id: string) => void;
}) => {
  if (isLoading) return (
    <div className="w-full flex flex-col items-center justify-center my-30">
      <Spinner size="size-20 text-indigo-500" />
      <h1 className="text-indigo-400 text-xl my-5">Loading Cards</h1>
    </div>
  )
  if (!cards.length) return (
    <div className="w-full flex justify-center my-30">
      <h1 className="text-indigo-400 text-4xl font-bold">No Results</h1>
    </div>
  )

  return (
    <div className="flex gap-10 w-[90%] mx-auto my-10 justify-center content-center flex-wrap">
      {cards.map((card) => (
        <CardItem
          key={card.id}
          card={card}
          onClick={() => onCardClick(card.id)}
        />
      ))}
    </div>
  );
};

export default CardsGrid;
