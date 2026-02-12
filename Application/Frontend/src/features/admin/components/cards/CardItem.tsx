import Card from "@/shared/components/Card";
import type { CardDto } from "../../types/card.types";
import { getImageUrl } from "@/shared/api/httpClient";

const CardItem = ({
  card,
  onClick,
}: {
  card: CardDto;
  onClick: () => void;
}) => (
  <div onClick={onClick}>
    <Card
      {...card}
      hasEffect={card.effectId !== null}
      hidden={false}
      src={getImageUrl(card.image)}
      className="text-[0.7rem] cursor-pointer"
    />
  </div>
);

export default CardItem;
