import Card from "@/shared/components/Card";
import type { GameCardDto } from "@/features/game/types/game.types";

interface BoardProps {
  cards: GameCardDto[];
}

const FIELD_SIZE_PX = 96;
const FIELD_GAP_PX = 12;
const ZONE_RADIUS_PERCENT = 36;
const CARD_RATIO = 120 / 174;

const createPlayerFieldMap = (cards: GameCardDto[]) =>
  cards.reduce<Record<string, Record<number, GameCardDto>>>((acc, card) => {
    if (card.fieldIndex === null) return acc;
    if (!acc[card.playerId]) acc[card.playerId] = {};
    acc[card.playerId][card.fieldIndex] = card;
    return acc;
  }, {});

const getPlayerCoordinates = (playersCount: number, index: number) => {
  const angleStep = (2 * Math.PI) / playersCount;
  const angle = -Math.PI / 2 + index * angleStep;

  return {
    x: 50 + ZONE_RADIUS_PERCENT * Math.cos(angle),
    y: 50 + ZONE_RADIUS_PERCENT * Math.sin(angle),
    rotation: (angle * 180) / Math.PI + 90,
  };
};

const CardField = ({ card }: { card: GameCardDto | null }) => {
  const isDefense = Boolean(card?.defensePosition);
  const cardHeight = FIELD_SIZE_PX - 18;
  const cardWidth = Math.floor(cardHeight * CARD_RATIO);

  return (
    <div className="relative rounded-lg border border-white/20 bg-black/20">
      <div className="pointer-events-none absolute inset-0 grid place-items-center">
        <div
          className={`absolute rounded-sm border`}
        />
        <div
          className={`absolute rounded-sm border`}
        />
      </div>

      {card?.card ? (
        <div className="">
          <Card
            {...card.card}
            src={card.card.image}
            hasEffect={Boolean(card.card.effectId)}
            hidden={card.isFaceDown}
            className={`${isDefense ? "rotate-90" : ""}`}
            style={{ height: `${cardHeight}px` }}
            draggable={false}
          />
        </div>
      ) : null}
    </div>
  );
};

const CardBoard = ({ cards }: BoardProps) => {
  const playerIds = Array.from(new Set(cards.map((c) => c.playerId)));
  const playerFieldMap = createPlayerFieldMap(cards);

  if (playerIds.length === 0) {
    return (
      <div className="rounded-lg border border-white/20 bg-black/30 p-6 text-zinc-300">
        No players/cards provided.
      </div>
    );
  }

  return (
    <div className="relative h-screen w-full overflow-hidden rounded-2xl border border-white/20 bg-black/25">
      <div className="pointer-events-none absolute inset-0 backdrop-blur-xs" />
      <div className="relative h-full">
        <section
          className="absolute"
          style={{
            left: `50%`,
            top: `calc(50% + 40vh)`,
            transform: `translate(-50%, -50%) rotate(0deg)`,
          }}
        >
          TEST
        </section>
        {/* {playerIds.map((playerId, index) => {
          const { x, y, rotation } = getPlayerCoordinates(playerIds.length, index);

          return (
            <section
              key={playerId}
              className="absolute"
              style={{
                left: `${x}%`,
                top: `${y}%`,
                transform: `translate(-50%, -50%) rotate(${rotation}deg)`,
              }}
            >
              <div className="flex items-center gap-4">
                <div
                  className="grid grid-cols-5 rounded-xl border border-white/20 bg-black/30 p-3 shadow-[0_0_25px_rgba(0,0,0,0.35)]"
                  style={{ gap: `${FIELD_GAP_PX}px` }}
                >
                  {Array.from({ length: 10 }).map((_, fieldIndex) => (
                    <CardField
                      key={`${playerId}-${fieldIndex}`}
                      card={playerFieldMap[playerId]?.[fieldIndex] ?? null}
                    />
                  ))}
                </div>

                <div className="flex flex-col gap-2">
                  <div className="grid h-[120px] w-[84px] place-items-center rounded-md border border-white/30 bg-black/35 text-xs font-semibold tracking-wide text-white/85">
                    Deck
                  </div>
                  <div className="grid h-[120px] w-[84px] place-items-center rounded-md border border-white/30 bg-black/35 text-xs font-semibold tracking-wide text-white/85">
                    Grave
                  </div>
                </div>
              </div>
            </section>
          );
        })} */}
      </div>
    </div>
  );
};

export default CardBoard;
