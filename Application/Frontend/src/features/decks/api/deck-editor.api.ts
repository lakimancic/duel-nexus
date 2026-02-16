import { httpClient } from "@/shared/api/httpClient";
import type {
  DeckCardDto,
  DeckDto,
  DeckLimitsDto,
  PlayerCardDto,
} from "../types/deck-editor.types";

export const deckEditorApi = {
  getLimits: () => httpClient.get<DeckLimitsDto>("/decks/limits"),

  getMyDecks: () => httpClient.get<DeckDto[]>("/decks/me"),

  createDeck: (name: string) =>
    httpClient.post<DeckDto>("/decks/me", {
      name,
    }),

  getMyCards: () => httpClient.get<PlayerCardDto[]>("/decks/me/cards"),

  getDeckCards: (deckId: string) =>
    httpClient.get<DeckCardDto[]>(`/decks/me/${deckId}/cards`),

  addCardToDeck: (deckId: string, cardId: string, quantity = 1) =>
    httpClient.post(`/decks/me/${deckId}/cards/add`, {
      cardId,
      quantity,
    }),

  removeCardFromDeck: (deckId: string, cardId: string, quantity = 1) =>
    httpClient.post(`/decks/me/${deckId}/cards/remove`, {
      cardId,
      quantity,
    }),
};
