import { AxiosError } from "axios";
import { useCallback, useEffect, useMemo, useState } from "react";
import { useNavigate } from "react-router-dom";
import Logo from "@/assets/images/logo.png";
import Card from "@/shared/components/Card";
import { getImageUrl } from "@/shared/api/httpClient";
import type { ErrorMessage } from "@/shared/types/error.types";
import { deckEditorApi } from "../api/deck-editor.api";
import type {
  DeckCardDto,
  DeckDto,
  DeckLimitsDto,
  PlayerCardDto,
} from "../types/deck-editor.types";

const defaultLimits: DeckLimitsDto = {
  maxDecks: 5,
  maxDeckSize: 40,
};

const DeckEditorPage = () => {
  const navigate = useNavigate();

  const [limits, setLimits] = useState<DeckLimitsDto>(defaultLimits);
  const [decks, setDecks] = useState<DeckDto[]>([]);
  const [ownedCards, setOwnedCards] = useState<PlayerCardDto[]>([]);
  const [deckCards, setDeckCards] = useState<DeckCardDto[]>([]);
  const [selectedDeckId, setSelectedDeckId] = useState<string | null>(null);
  const [newDeckName, setNewDeckName] = useState("");

  const [isLoading, setIsLoading] = useState(false);
  const [isCreatingDeck, setIsCreatingDeck] = useState(false);
  const [isUpdatingDeck, setIsUpdatingDeck] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const fetchDecks = useCallback(async () => {
    const response = await deckEditorApi.getMyDecks();
    setDecks(response.data);
    return response.data;
  }, []);

  const fetchDeckCards = useCallback(async (deckId: string | null) => {
    if (!deckId) {
      setDeckCards([]);
      return;
    }

    const response = await deckEditorApi.getDeckCards(deckId);
    setDeckCards(response.data);
  }, []);

  const fetchInitial = useCallback(async () => {
    setIsLoading(true);
    setError(null);

    try {
      const [limitsResponse, decksResponse, cardsResponse] = await Promise.all([
        deckEditorApi.getLimits(),
        deckEditorApi.getMyDecks(),
        deckEditorApi.getMyCards(),
      ]);

      setLimits(limitsResponse.data);
      setDecks(decksResponse.data);
      setOwnedCards(cardsResponse.data);

      if (decksResponse.data.length > 0) {
        const firstDeckId = decksResponse.data[0].id;
        setSelectedDeckId(firstDeckId);
        const deckCardsResponse = await deckEditorApi.getDeckCards(firstDeckId);
        setDeckCards(deckCardsResponse.data);
      }
    } catch (err) {
      if (err instanceof AxiosError) {
        const data = err.response?.data as ErrorMessage | undefined;
        setError(data?.error ?? "Failed to load deck editor.");
      }
    } finally {
      setIsLoading(false);
    }
  }, []);

  useEffect(() => {
    void fetchInitial();
  }, [fetchInitial]);

  const deckCardQuantityById = useMemo(() => {
    const map = new Map<string, number>();
    for (const deckCard of deckCards) {
      map.set(deckCard.card.id, deckCard.quantity);
    }
    return map;
  }, [deckCards]);

  const deckSize = useMemo(
    () => deckCards.reduce((sum, card) => sum + card.quantity, 0),
    [deckCards]
  );

  const selectedDeck = useMemo(
    () => decks.find((deck) => deck.id === selectedDeckId) ?? null,
    [decks, selectedDeckId]
  );

  const createDeck = async () => {
    const normalizedName = newDeckName.trim();
    if (!normalizedName) {
      setError("Deck name is required.");
      return;
    }

    setError(null);
    setIsCreatingDeck(true);

    try {
      const response = await deckEditorApi.createDeck(normalizedName);
      const updatedDecks = await fetchDecks();
      setSelectedDeckId(response.data.id);
      await fetchDeckCards(response.data.id);
      setDecks(updatedDecks);
      setNewDeckName("");
    } catch (err) {
      if (err instanceof AxiosError) {
        const data = err.response?.data as ErrorMessage | undefined;
        setError(data?.error ?? "Failed to create deck.");
      }
    } finally {
      setIsCreatingDeck(false);
    }
  };

  const selectDeck = async (deckId: string) => {
    setSelectedDeckId(deckId);
    setError(null);

    try {
      await fetchDeckCards(deckId);
    } catch (err) {
      if (err instanceof AxiosError) {
        const data = err.response?.data as ErrorMessage | undefined;
        setError(data?.error ?? "Failed to load selected deck.");
      }
    }
  };

  const addCard = async (cardId: string) => {
    if (!selectedDeckId || isUpdatingDeck) return;

    setError(null);
    setIsUpdatingDeck(true);
    try {
      await deckEditorApi.addCardToDeck(selectedDeckId, cardId, 1);
      await Promise.all([fetchDeckCards(selectedDeckId), fetchDecks()]);
    } catch (err) {
      if (err instanceof AxiosError) {
        const data = err.response?.data as ErrorMessage | undefined;
        setError(data?.error ?? "Failed to add card.");
      }
    } finally {
      setIsUpdatingDeck(false);
    }
  };

  const removeCard = async (cardId: string) => {
    if (!selectedDeckId || isUpdatingDeck) return;

    setError(null);
    setIsUpdatingDeck(true);
    try {
      await deckEditorApi.removeCardFromDeck(selectedDeckId, cardId, 1);
      await Promise.all([fetchDeckCards(selectedDeckId), fetchDecks()]);
    } catch (err) {
      if (err instanceof AxiosError) {
        const data = err.response?.data as ErrorMessage | undefined;
        setError(data?.error ?? "Failed to remove card.");
      }
    } finally {
      setIsUpdatingDeck(false);
    }
  };

  return (
    <div className="h-screen w-full p-4 md:p-6">
      <div className="h-full w-full grid grid-cols-1 md:grid-cols-10 gap-4 text-white">
        <div className="md:col-span-3 rounded-xl bg-[#4b1812]/85 border border-amber-300/20 p-4 flex flex-col gap-4">
          <img src={Logo} alt="logo" className="w-[75%] mx-auto" />

          <div className="rounded-lg border border-white/15 bg-black/20 p-3 text-sm">
            <p>
              Decks: <span className="font-semibold">{decks.length}</span> / {limits.maxDecks}
            </p>
            <p>
              Selected size: <span className="font-semibold">{deckSize}</span> / {limits.maxDeckSize}
            </p>
            <p>
              Status: <span className="font-semibold">{selectedDeck?.isComplete ? "Complete" : "Incomplete"}</span>
            </p>
          </div>

          <div className="flex gap-2">
            <input
              value={newDeckName}
              onChange={(event) => setNewDeckName(event.target.value)}
              placeholder="New deck name"
              className="flex-1 rounded-md border border-amber-300/40 bg-black/20 px-3 py-2 text-sm
              placeholder:text-white/40 outline-none focus:border-amber-300"
            />
            <button
              type="button"
              onClick={() => void createDeck()}
              disabled={isCreatingDeck || decks.length >= limits.maxDecks}
              className="rounded-md bg-amber-500/85 px-3 py-2 text-sm font-semibold cursor-pointer hover:bg-amber-400
              disabled:opacity-60 disabled:cursor-default"
            >
              {isCreatingDeck ? "..." : "Create"}
            </button>
          </div>

          <div className="flex-1 min-h-0 overflow-y-auto rounded-lg border border-white/15 bg-black/20 p-2 flex flex-col gap-2">
            {decks.map((deck) => (
              <button
                key={deck.id}
                type="button"
                onClick={() => void selectDeck(deck.id)}
                className={`rounded-md border px-3 py-2 text-left cursor-pointer transition-colors ${
                  deck.id === selectedDeckId
                    ? "border-amber-300 bg-amber-500/25"
                    : "border-white/20 bg-white/5 hover:bg-white/10"
                }`}
              >
                <p className="font-semibold">{deck.name}</p>
                <p className="text-xs text-white/70">{deck.isComplete ? "Complete" : "Incomplete"}</p>
              </button>
            ))}
            {decks.length === 0 && (
              <p className="text-sm text-white/70 p-2">You have no decks yet.</p>
            )}
          </div>

          <button
            type="button"
            onClick={() => navigate("/lobby")}
            className="text-sm text-amber-100/80 hover:text-amber-100 cursor-pointer"
          >
            Back to lobby
          </button>
        </div>

        <div className="md:col-span-7 rounded-xl bg-[#091b33]/85 border border-white/20 p-4 flex flex-col gap-3 min-h-0">
          <div>
            <p className="text-lg font-semibold text-amber-200">Owned Cards</p>
            <p className="text-xs text-white/70">
              Add cards from your collection. Available = Owned - In deck.
            </p>
          </div>

          {isLoading && <p className="text-sm text-white/70">Loading...</p>}

          {!isLoading && !selectedDeckId && (
            <p className="text-sm text-white/70">Create or select a deck to start editing.</p>
          )}

          {!isLoading && selectedDeckId && (
            <div className="flex-1 min-h-0 overflow-x-auto overflow-y-hidden rounded-lg border border-white/15 bg-black/20 p-3">
              <div className="flex gap-4 min-w-max">
                {ownedCards.map((ownedCard) => {
                  const inDeck = deckCardQuantityById.get(ownedCard.card.id) ?? 0;
                  const available = Math.max(ownedCard.quantity - inDeck, 0);

                  return (
                    <div
                      key={ownedCard.id}
                      className="shrink-0 rounded-lg border border-white/20 bg-black/30 p-2"
                    >
                      <Card
                        name={ownedCard.card.name}
                        description={ownedCard.card.description}
                        type={ownedCard.card.type}
                        src={getImageUrl(ownedCard.card.image)}
                        hasEffect={Boolean(ownedCard.card.effectId)}
                        hidden={false}
                        attack={ownedCard.card.attack}
                        defense={ownedCard.card.defense}
                        level={ownedCard.card.level}
                        className="text-black"
                      />

                      <div className="text-md space-y-2 py-2">
                        <p>Owned: {ownedCard.quantity}</p>
                        <p>In deck: {inDeck}</p>
                        <p>Available: {available}</p>
                      </div>

                      <div className="flex gap-2 mt-auto">
                        <button
                          type="button"
                          onClick={() => void addCard(ownedCard.card.id)}
                          disabled={isUpdatingDeck || available <= 0 || deckSize >= limits.maxDeckSize}
                          className="flex-1 rounded-md bg-emerald-500/80 py-1 text-md font-semibold cursor-pointer hover:bg-emerald-400
                          disabled:opacity-60 disabled:cursor-default"
                        >
                          +1
                        </button>
                        <button
                          type="button"
                          onClick={() => void removeCard(ownedCard.card.id)}
                          disabled={isUpdatingDeck || inDeck <= 0}
                          className="flex-1 rounded-md bg-red-500/80 py-1 text-md font-semibold cursor-pointer hover:bg-red-400
                          disabled:opacity-60 disabled:cursor-default"
                        >
                          -1
                        </button>
                      </div>
                    </div>
                  );
                })}
              </div>
            </div>
          )}

          {error && <p className="text-sm text-red-300">{error}</p>}
        </div>
      </div>
    </div>
  );
};

export default DeckEditorPage;
