import Board from "@/features/game/components/Board";
import type { GameCardDto } from "@/features/game/types/game.types";
import { useState } from "react";
import type { CardDto } from "@/shared/types/card.types";
import Card from "@/shared/components/Card";

const ZONE_FIELD = 0;
const ZONE_DECK = 2;

const demoCards: GameCardDto[] = [
  {
    playerId: "Player-1",
    zone: ZONE_FIELD,
    isFaceDown: false,
    fieldIndex: 0,
    defensePosition: false,
    card: {
      id: "c-1",
      name: "Dragon Knight",
      image: "",
      description: "A frontline monster used for testing board layout.",
      type: 0,
      effectId: null,
      attack: 2400,
      defense: 1800,
      level: 6,
    },
  },
  {
    playerId: "Player-1",
    zone: ZONE_FIELD,
    isFaceDown: true,
    fieldIndex: 6,
    defensePosition: true,
    card: {
      id: "c-2",
      name: "Hidden Guard",
      image: "",
      description: "Face-down card to validate hidden mode.",
      type: 2,
      effectId: "e-2",
      attack: null,
      defense: null,
      level: null,
    },
  },
  {
    playerId: "Player-2",
    zone: ZONE_FIELD,
    isFaceDown: false,
    fieldIndex: 2,
    defensePosition: true,
    card: {
      id: "c-3",
      name: "Stone Turtle",
      image: "",
      description: "Defense-position sample card.",
      type: 0,
      effectId: null,
      attack: 800,
      defense: 2500,
      level: 4,
    },
  },
  {
    playerId: "Player-3",
    zone: ZONE_FIELD,
    isFaceDown: false,
    fieldIndex: 4,
    defensePosition: false,
    card: {
      id: "c-4",
      name: "Arcane Surge",
      image: "",
      description: "Spell sample for mixed card types.",
      type: 1,
      effectId: "e-4",
      attack: null,
      defense: null,
      level: null,
    },
  },
  {
    playerId: "Player-1",
    zone: ZONE_DECK,
    isFaceDown: false,
    fieldIndex: null,
    defensePosition: false,
    card: {
      id: "d-1",
      name: "Blue-Eyes White Dragon",
      image: "",
      description: "Demo deck card for the current player hand fan.",
      type: 0,
      effectId: null,
      attack: 3000,
      defense: 2500,
      level: 8,
    },
  },
  {
    playerId: "Player-1",
    zone: ZONE_DECK,
    isFaceDown: false,
    fieldIndex: null,
    defensePosition: false,
    card: {
      id: "d-2",
      name: "Mystical Space Typhoon",
      image: "",
      description: "Demo deck card for curved fan layout.",
      type: 1,
      effectId: null,
      attack: null,
      defense: null,
      level: null,
    },
  },
  {
    playerId: "Player-1",
    zone: ZONE_DECK,
    isFaceDown: false,
    fieldIndex: null,
    defensePosition: false,
    card: {
      id: "d-3",
      name: "Mirror Force",
      image: "",
      description: "Demo deck card for curved fan layout.",
      type: 2,
      effectId: null,
      attack: null,
      defense: null,
      level: null,
    },
  },
  {
    playerId: "Player-2",
    zone: ZONE_DECK,
    isFaceDown: true,
    fieldIndex: null,
    defensePosition: false,
    card: {
      id: "d-4",
      name: "Opponent Card A",
      image: "",
      description: "Should be hidden from current player.",
      type: 0,
      effectId: null,
      attack: 1500,
      defense: 1500,
      level: 4,
    },
  },
  {
    playerId: "Player-2",
    zone: ZONE_DECK,
    isFaceDown: true,
    fieldIndex: null,
    defensePosition: false,
    card: {
      id: "d-5",
      name: "Opponent Card B",
      image: "",
      description: "Should be hidden from current player.",
      type: 1,
      effectId: null,
      attack: null,
      defense: null,
      level: null,
    },
  },
  {
    playerId: "Player-3",
    zone: ZONE_DECK,
    isFaceDown: true,
    fieldIndex: null,
    defensePosition: false,
    card: {
      id: "d-6",
      name: "Opponent Card C",
      image: "",
      description: "Should be hidden from current player.",
      type: 2,
      effectId: null,
      attack: null,
      defense: null,
      level: null,
    },
  },
];

const GamePage = () => {
  const [hoveredCard, setHoveredCard] = useState<CardDto | null>(null);

  return (
    <main className="relative min-h-screen w-full text-zinc-100 box-border">
      <div className="pointer-events-none absolute inset-0 backdrop-blur-[2px]" />
      <div className="pointer-events-none absolute inset-0 bg-linear-to-br from-[#4b1812]/20 via-transparent to-[#091b33]/25" />
      <div className="pointer-events-none absolute inset-0 bg-black/30" />

      <div className="relative h-screen w-full flex items-center pl-2 pr-3 gap-3">
        <div className="relative h-full flex-1 -ml-3">
          <Board
            cards={demoCards}
            viewerPlayerId="Player-1"
            onHoverCardChange={setHoveredCard}
          />
        </div>

        <aside className="h-[96%] w-[330px] shrink-0 rounded-xl border border-white/20 bg-black/45 p-3">
          <div className="h-full w-full rounded-lg border border-white/15 bg-black/30 p-2 flex items-center justify-center">
            {hoveredCard ? (
              <Card
                name={hoveredCard.name}
                description={hoveredCard.description}
                type={hoveredCard.type}
                attack={hoveredCard.attack}
                defense={hoveredCard.defense}
                level={hoveredCard.level}
                src={hoveredCard.image}
                hasEffect={Boolean(hoveredCard.effectId)}
                hidden={false}
                className="text-black"
                style={{ width: "100%", maxWidth: "280px", height: "auto", maxHeight: "95%" }}
                draggable={false}
              />
            ) : (
              <p className="text-sm text-white/55 text-center px-4">
                Hover a visible card to preview it here.
              </p>
            )}
          </div>
        </aside>
      </div>
    </main>
  );
};

export default GamePage;
