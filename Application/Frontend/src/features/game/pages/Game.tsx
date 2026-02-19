import Board from "@/features/game/components/Board";
import type { GameCardDto } from "@/features/game/types/game.types";
import MonsterImage from "@/assets/images/monster_card.png";
import SpellImage from "@/assets/images/spell_card.png";
import TrapImage from "@/assets/images/trap_card.png";

const demoCards: GameCardDto[] = [
  {
    playerId: "Player-1",
    zone: 0,
    isFaceDown: false,
    fieldIndex: 0,
    defensePosition: false,
    card: {
      id: "c-1",
      name: "Dragon Knight",
      image: MonsterImage,
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
    zone: 1,
    isFaceDown: true,
    fieldIndex: 6,
    defensePosition: true,
    card: {
      id: "c-2",
      name: "Hidden Guard",
      image: TrapImage,
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
    zone: 0,
    isFaceDown: false,
    fieldIndex: 2,
    defensePosition: true,
    card: {
      id: "c-3",
      name: "Stone Turtle",
      image: MonsterImage,
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
    zone: 0,
    isFaceDown: false,
    fieldIndex: 4,
    defensePosition: false,
    card: {
      id: "c-4",
      name: "Arcane Surge",
      image: SpellImage,
      description: "Spell sample for mixed card types.",
      type: 1,
      effectId: "e-4",
      attack: null,
      defense: null,
      level: null,
    },
  },
  {
    playerId: "Player-4",
    zone: 0,
    isFaceDown: false,
    fieldIndex: 9,
    defensePosition: false,
    card: {
      id: "c-5",
      name: "Night Watch",
      image: MonsterImage,
      description: "Bottom-row field sample.",
      type: 0,
      effectId: null,
      attack: 1700,
      defense: 1200,
      level: 4,
    },
  },
];

const GamePage = () => {
  return (
    <main className="relative min-h-screen w-full text-zinc-100 box-border">
      <div className="pointer-events-none absolute inset-0 backdrop-blur-[2px]" />
      <div className="pointer-events-none absolute inset-0 bg-linear-to-br from-[#4b1812]/20 via-transparent to-[#091b33]/25" />

      <div className="relative">
        <Board cards={demoCards} />
      </div>
    </main>
  );
};

export default GamePage;
