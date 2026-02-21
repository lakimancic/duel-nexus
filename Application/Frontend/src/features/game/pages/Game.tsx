import Board from "@/features/game/components/Board";
import TurnStatus from "@/features/game/components/TurnStatus";
import {
  TurnPhase,
  type GameCardDto,
  type GameTurnStatus,
} from "@/features/game/types/game.types";
import { useState } from "react";
import type { CardDto } from "@/shared/types/card.types";
import Card from "@/shared/components/Card";

const ZONE_FIELD = 0;
const ZONE_DECK = 2;
const ZONE_GRAVEYARD = 3;
const ZONE_HAND = 4;
const VIEWER_PLAYER_ID = "Player-1";

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
    isFaceDown: false,
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
      id: "d-7",
      name: "Dark Magician",
      image: "",
      description: "Extra draw test card.",
      type: 0,
      effectId: null,
      attack: 2500,
      defense: 2100,
      level: 7,
    },
  },
  {
    playerId: "Player-1",
    zone: ZONE_DECK,
    isFaceDown: false,
    fieldIndex: null,
    defensePosition: false,
    card: {
      id: "d-8",
      name: "Raigeki",
      image: "",
      description: "Extra draw test card.",
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
    playerId: "Player-1",
    zone: ZONE_HAND,
    isFaceDown: false,
    fieldIndex: null,
    defensePosition: false,
    card: {
      id: "h-1",
      name: "Summoned Skull",
      image: "",
      description: "Main1 test card already in hand.",
      type: 0,
      effectId: null,
      attack: 2500,
      defense: 1200,
      level: 6,
    },
  },
  {
    playerId: "Player-1",
    zone: ZONE_HAND,
    isFaceDown: false,
    fieldIndex: null,
    defensePosition: false,
    card: {
      id: "h-2",
      name: "Swords of Revealing Light",
      image: "",
      description: "Main1 test card for graveyard/deck actions.",
      type: 1,
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
  const [gameCards, setGameCards] = useState<GameCardDto[]>(demoCards);
  const [pendingDrawCard, setPendingDrawCard] = useState<GameCardDto | null>(null);
  const [pendingDrawReveal, setPendingDrawReveal] = useState(false);
  const [selectedHandCardId, setSelectedHandCardId] = useState<string | null>(null);
  const [pendingPlacementCard, setPendingPlacementCard] = useState<GameCardDto | null>(null);
  const [lastGraveyardCardId, setLastGraveyardCardId] = useState<string | null>(null);
  const [turnStatus, setTurnStatus] = useState<GameTurnStatus>({
    activePlayerId: VIEWER_PLAYER_ID,
    phase: TurnPhase.Draw,
  });
  const isDrawPhase = Number(turnStatus.phase) === TurnPhase.Draw;
  const isMain1Phase = Number(turnStatus.phase) === TurnPhase.Main1;
  const isBattlePhase = Number(turnStatus.phase) === TurnPhase.Battle;
  const isViewerActivePlayer =
    turnStatus.activePlayerId === VIEWER_PLAYER_ID;

  const visibleDeckCards = gameCards.filter(
    (card) =>
      card.playerId === VIEWER_PLAYER_ID &&
      card.zone === ZONE_DECK &&
      card.fieldIndex === null &&
      card.card
  );
  const drawPileCards = gameCards.filter(
    (card) =>
      card.playerId === VIEWER_PLAYER_ID &&
      card.zone === ZONE_HAND &&
      card.fieldIndex === null &&
      card.card
  );
  const selectedHandCard =
    visibleDeckCards.find((card) => card.card?.id === selectedHandCardId) ?? null;
  const graveyardCards = gameCards.filter(
    (card) => card.playerId === VIEWER_PLAYER_ID && card.zone === ZONE_GRAVEYARD
  );
  const lastGraveyardCard =
    graveyardCards.find((card) => card.card?.id === lastGraveyardCardId) ??
    (graveyardCards.length > 0 ? graveyardCards[graveyardCards.length - 1] : null);
  const fieldCards = gameCards.filter(
    (card) =>
      card.playerId === VIEWER_PLAYER_ID &&
      card.zone === ZONE_FIELD &&
      card.fieldIndex !== null &&
      card.card
  );

  const moveCardToZone = (
    cardId: string,
    zone: number,
    fieldIndex: number | null = null,
    defensePosition = false
  ) => {
    setGameCards((prevCards) => {
      const index = prevCards.findIndex((card) => card.card?.id === cardId);
      if (index === -1) return prevCards;

      const updatedCard: GameCardDto = {
        ...prevCards[index],
        zone,
        fieldIndex,
        isFaceDown: false,
        defensePosition,
      };

      const remainingCards = prevCards.filter((_, i) => i !== index);
      if (zone === ZONE_DECK || zone === ZONE_GRAVEYARD) {
        return [updatedCard, ...remainingCards];
      }

      return [...remainingCards, updatedCard];
    });

    if (zone === ZONE_GRAVEYARD) {
      setLastGraveyardCardId(cardId);
    }
  };

  const handleDeckClick = (playerId: string) => {
    if (!isDrawPhase) return;
    if (playerId !== VIEWER_PLAYER_ID || playerId !== turnStatus.activePlayerId) return;
    if (pendingDrawCard) return;

    setGameCards((prevCards) => {
      const topDeckIndex = [...prevCards]
        .map((card, index) => ({ card, index }))
        .reverse()
        .find(({ card }) => card.playerId === playerId && card.zone === ZONE_HAND && card.fieldIndex === null && card.card)
        ?.index;

      if (topDeckIndex === undefined) return prevCards;

      const drawnCard = prevCards[topDeckIndex];
      setPendingDrawCard({
        ...drawnCard,
        zone: ZONE_DECK,
        fieldIndex: null,
        isFaceDown: false,
        defensePosition: false,
      });
      setPendingDrawReveal(false);
      setTimeout(() => setPendingDrawReveal(true), 20);
      setTurnStatus((prev) => ({ ...prev, phase: TurnPhase.Main1 }));

      return prevCards.filter((_, index) => index !== topDeckIndex);
    });
  };

  const handleDrawnCardToHand = () => {
    if (!pendingDrawCard?.card) return;
    setGameCards((prevCards) => [
      { ...pendingDrawCard, zone: ZONE_DECK, fieldIndex: null, isFaceDown: false },
      ...prevCards,
    ]);
    setPendingDrawCard(null);
    setPendingDrawReveal(false);
  };

  const handleDrawnCardToField = () => {
    if (!pendingDrawCard?.card) return;
    setPendingPlacementCard({ ...pendingDrawCard, zone: ZONE_FIELD, fieldIndex: null });
    setPendingDrawCard(null);
    setPendingDrawReveal(false);
  };

  const handleDrawnCardToGraveyard = () => {
    if (!pendingDrawCard?.card) return;
    setGameCards((prevCards) => [
      { ...pendingDrawCard, zone: ZONE_GRAVEYARD, fieldIndex: null, isFaceDown: false },
      ...prevCards,
    ]);
    setLastGraveyardCardId(pendingDrawCard.card.id);
    setPendingDrawCard(null);
    setPendingDrawReveal(false);
  };

  const handleSelectedCardToField = () => {
    if (!selectedHandCard?.card || !isMain1Phase || !isViewerActivePlayer) return;
    setPendingPlacementCard({ ...selectedHandCard, zone: ZONE_FIELD, fieldIndex: null });
    setSelectedHandCardId(null);
  };

  const handleSelectedCardToGraveyard = () => {
    if (!selectedHandCard?.card || !isMain1Phase || !isViewerActivePlayer) return;
    moveCardToZone(selectedHandCard.card.id, ZONE_GRAVEYARD);
    setSelectedHandCardId(null);
  };

  const handleEndMain1 = () => {
    if (!isMain1Phase) return;
    setPendingDrawCard(null);
    setPendingDrawReveal(false);
    setPendingPlacementCard(null);
    setSelectedHandCardId(null);
    setTurnStatus((prev) => ({ ...prev, phase: TurnPhase.Battle }));
  };

  const handleFieldClick = (playerId: string, fieldIndex: number, card: GameCardDto | null) => {
    if (!isMain1Phase) return;
    if (playerId !== VIEWER_PLAYER_ID || playerId !== turnStatus.activePlayerId) return;

    if (pendingPlacementCard) {
      if (card !== null) return;

      setGameCards((prevCards) => {
        const pendingCardId = pendingPlacementCard.card?.id;
        if (!pendingCardId) return prevCards;

        const existingCardIndex = prevCards.findIndex(
          (existingCard) => existingCard.card?.id === pendingCardId
        );

        const updatedCard: GameCardDto = {
          ...(existingCardIndex >= 0 ? prevCards[existingCardIndex] : pendingPlacementCard),
          playerId,
          zone: ZONE_FIELD,
          fieldIndex,
          isFaceDown: false,
          defensePosition: false,
        };

        if (existingCardIndex >= 0) {
          return prevCards.map((existingCard, index) =>
            index === existingCardIndex ? updatedCard : existingCard
          );
        }

        return [...prevCards, updatedCard];
      });
      setPendingPlacementCard(null);
      return;
    }

    if (!card || card.playerId !== playerId || card.zone !== ZONE_FIELD) return;

    setGameCards((prevCards) =>
      prevCards.map((existingCard) => {
        const sameCard =
          existingCard.playerId === card.playerId &&
          existingCard.zone === card.zone &&
          existingCard.fieldIndex === card.fieldIndex &&
          existingCard.card?.id === card.card?.id;

        if (!sameCard) return existingCard;
        return { ...existingCard, defensePosition: !existingCard.defensePosition };
      })
    );
  };

  return (
    <main className="relative min-h-screen w-full text-zinc-100 box-border">
      <div className="pointer-events-none absolute inset-0 backdrop-blur-[2px]" />
      <div className="pointer-events-none absolute inset-0 bg-linear-to-br from-[#4b1812]/20 via-transparent to-[#091b33]/25" />
      <div className="pointer-events-none absolute inset-0 bg-black/30" />

      <div className="relative h-screen w-full flex items-center pl-2 pr-3 gap-3">
        <div className="relative h-full flex-1 -ml-3">
          <div className="pointer-events-none absolute top-3 left-3 z-20">
            <TurnStatus status={turnStatus} />
          </div>
          <Board
            cards={gameCards}
            viewerPlayerId={VIEWER_PLAYER_ID}
            onHoverCardChange={setHoveredCard}
            onDeckClick={handleDeckClick}
            onFieldClick={handleFieldClick}
            isDeckClickable={(playerId) =>
              isDrawPhase &&
              !pendingDrawCard &&
              drawPileCards.length > 0 &&
              playerId === turnStatus.activePlayerId &&
              playerId === VIEWER_PLAYER_ID
            }
            isFieldClickable={(playerId, _fieldIndex, card) =>
              isMain1Phase &&
              playerId === turnStatus.activePlayerId &&
              playerId === VIEWER_PLAYER_ID &&
              (pendingPlacementCard ? card === null : card !== null)
            }
          />
        </div>

        <aside className="h-[96%] w-[360px] shrink-0 rounded-xl border border-white/20 bg-black/45 p-3">
          <div className="h-full w-full overflow-auto rounded-lg border border-white/15 bg-black/30 p-2">
            <div className="rounded border border-white/20 bg-black/35 p-2 text-xs">
              <p className="text-white/80">Deck : {drawPileCards.length}</p>
              <p className="text-white/80">Hand : {visibleDeckCards.length}</p>
              <p className="text-white/80">Field : {fieldCards.length}</p>
              <p className="text-white/80">Graveyard : {graveyardCards.length}</p>
            </div>

            {pendingDrawCard?.card ? (
              <div className="mt-2 rounded border border-white/20 bg-black/40 p-2">
                <p className="mb-1 text-[10px] text-white/70">Izvucena karta (iz deck-a)</p>
                <div style={{ perspective: "900px" }}>
                  <div
                    style={{
                      position: "relative",
                      width: "150px",
                      transformStyle: "preserve-3d",
                      transition: "transform 420ms ease",
                      transform: pendingDrawReveal ? "rotateY(180deg)" : "rotateY(0deg)",
                    }}
                  >
                    <Card
                      name={pendingDrawCard.card.name}
                      description={pendingDrawCard.card.description}
                      type={pendingDrawCard.card.type}
                      attack={pendingDrawCard.card.attack}
                      defense={pendingDrawCard.card.defense}
                      level={pendingDrawCard.card.level}
                      src={pendingDrawCard.card.image}
                      hasEffect={Boolean(pendingDrawCard.card.effectId)}
                      hidden={true}
                      className="text-black"
                      style={{
                        width: "100%",
                        height: "auto",
                        backfaceVisibility: "hidden",
                        WebkitBackfaceVisibility: "hidden",
                      }}
                      draggable={false}
                    />
                    <Card
                      name={pendingDrawCard.card.name}
                      description={pendingDrawCard.card.description}
                      type={pendingDrawCard.card.type}
                      attack={pendingDrawCard.card.attack}
                      defense={pendingDrawCard.card.defense}
                      level={pendingDrawCard.card.level}
                      src={pendingDrawCard.card.image}
                      hasEffect={Boolean(pendingDrawCard.card.effectId)}
                      hidden={false}
                      className="text-black absolute inset-0"
                      style={{
                        width: "100%",
                        height: "100%",
                        transform: "rotateY(180deg)",
                        backfaceVisibility: "hidden",
                        WebkitBackfaceVisibility: "hidden",
                      }}
                      draggable={false}
                    />
                  </div>
                </div>
                <div className="mt-2 grid grid-cols-2 gap-1">
                  <button
                    type="button"
                    className="rounded border border-cyan-200/50 bg-cyan-300/20 px-2 py-1 text-[10px]"
                    onClick={handleDrawnCardToField}
                  >
                    To Field
                  </button>
                  <button
                    type="button"
                    className="rounded border border-white/30 bg-white/10 px-2 py-1 text-[10px]"
                    onClick={handleDrawnCardToHand}
                  >
                    To Hand
                  </button>
                  <button
                    type="button"
                    className="rounded border border-white/30 bg-white/10 px-2 py-1 text-[10px]"
                    onClick={handleDrawnCardToGraveyard}
                  >
                    To Graveyard
                  </button>
                </div>
              </div>
            ) : null}

            <div className="mt-2 rounded border border-white/20 bg-black/35 p-2 text-xs">
              <p className="text-white/75">Hand</p>
              <div className="mt-2 max-h-[150px] space-y-1 overflow-auto pr-1">
                {visibleDeckCards.map((card) => (
                  <button
                    key={card.card!.id}
                    type="button"
                    className={`w-full rounded border px-2 py-1 text-left ${
                      selectedHandCardId === card.card?.id
                        ? "border-cyan-200/70 bg-cyan-300/20"
                        : "border-white/25 bg-white/5"
                    }`}
                    onClick={() => setSelectedHandCardId(card.card?.id ?? null)}
                    disabled={!isMain1Phase || !isViewerActivePlayer || Boolean(pendingPlacementCard)}
                  >
                    {card.card?.name}
                  </button>
                ))}
                {visibleDeckCards.length === 0 ? <p className="text-white/55">Spil je prazan.</p> : null}
              </div>
              <div className="mt-2 grid grid-cols-1 gap-1">
                <button
                  type="button"
                  className="rounded border border-cyan-200/50 bg-cyan-300/20 px-2 py-1 text-[10px] disabled:opacity-40"
                  onClick={handleSelectedCardToField}
                  disabled={!selectedHandCard || !isMain1Phase || !isViewerActivePlayer}
                >
                  To Filed
                </button>
                <button
                  type="button"
                  className="rounded border border-white/30 bg-white/10 px-2 py-1 text-[10px] disabled:opacity-40"
                  onClick={handleSelectedCardToGraveyard}
                  disabled={!selectedHandCard || !isMain1Phase || !isViewerActivePlayer}
                >
                  To Graveyard
                </button>
              </div>
            </div>

            <div className="mt-2 rounded border border-white/20 bg-black/35 p-2 text-xs">
              <p className="text-white/75">Groblje</p>
              <div className="mt-1 space-y-1">
                {lastGraveyardCard ? (
                  <p className="rounded border border-white/20 bg-white/5 px-2 py-1">
                    Last: {lastGraveyardCard.card?.name}
                  </p>
                ) : (
                  <p className="text-white/55">Groblje je prazno.</p>
                )}
              </div>
              <p className="mt-2 text-[10px] text-white/65">All cards in graveyard ({graveyardCards.length})</p>
              <div className="mt-1 max-h-[90px] space-y-1 overflow-auto pr-1">
                {graveyardCards.map((card) => (
                  <p key={card.card?.id} className="rounded border border-white/20 bg-white/5 px-2 py-1">
                    {card.card?.name}
                  </p>
                ))}
              </div>
            </div>

            <button
              type="button"
              className="mt-2 w-full rounded border border-amber-200/50 bg-amber-300/20 px-2 py-1 text-[10px] disabled:opacity-40"
              onClick={handleEndMain1}
              disabled={!isMain1Phase || Boolean(pendingPlacementCard) || Boolean(pendingDrawCard)}
            >
              Next
            </button>
            {pendingPlacementCard ? (
              <p className="mt-2 text-[10px] text-cyan-100">Klikni prazno svoje polje za postavljanje.</p>
            ) : null}
            {isBattlePhase ? (
              <p className="mt-2 text-[10px] text-amber-100">Battle faza aktivna (trenutno bez akcija).</p>
            ) : null}

            <div className="mt-2 flex items-center justify-center rounded border border-white/20 bg-black/35 p-2">
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
          </div>
        </aside>
      </div>
    </main>
  );
};

export default GamePage;
