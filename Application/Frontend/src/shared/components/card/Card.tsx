import type { CardProps } from "./card.types";
import SpellCard from "@/assets/images/spell_card.png";
import TrapCard from "@/assets/images/trap_card.png";
import MonsterCard from "@/assets/images/monster_card.png";
// import EffectCard from "@/assets/images/effect_card.png";
import CardBack from "@/assets/images/card_back.png";
import { FaStar } from "react-icons/fa";

const Card: React.FC<CardProps> = ({
  name,
  description,
  type,
  attack,
  defense,
  level,
  src,
  hidden,
  className,
  ...props
}) => {
  const bgUrl = hidden ? CardBack : [MonsterCard, SpellCard, TrapCard][type];

  return (
    <div
      className={`relative aspect-120/174 bg-center bg-cover select-none h-[25em] ${className ?? ""}`}
      style={
        {
          backgroundImage: `url(${bgUrl})`,
        } as React.CSSProperties
      }
      {...props}
    >
      {!hidden && (
        <>
          <img
            src={src}
            alt="card-image"
            className="w-[75%] absolute left-[12.5%] top-[18.5%]"
          />
          <h1 className="font-bold absolute top-[4.8%] left-[9.6%]">{name}</h1>
          <p
            className={`absolute text-[65%] italic text-justify top-[75.5%] left-[10%] w-[80%] text-wrap`}
          >
            {description}
          </p>
          {attack && defense && (
            <p className="text-[70%] font-bold absolute bottom-[6.5%] right-[10%] flex gap-3">
              <span>ATK/{attack}</span>
              <span>DEF/{defense}</span>
            </p>
          )}
          {level && (
            <div className="absolute top-[12.3%] text-[60%] right-[12%] flex gap-1">
              {Array.from({ length: level }).map((_, i) => (
                <FaStar
                  key={i}
                  className="bg-red-500 rounded-full text-yellow-300 size-3.5 p-[1%]"
                />
              ))}
            </div>
          )}
        </>
      )}
    </div>
  );
};

export default Card;
