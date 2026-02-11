import type {
  InformativeResult,
  PagedResult,
  SearchQuery,
} from "@/shared/types/result.types";
import type {
  AddCardInDeckDto,
  CreateDeckDto,
  DeckDto,
  EditDeckDto,
} from "../types/deck.types";
import { httpClient } from "@/shared/api/httpClient";

export const decksApi = {
  getDecks: (params?: SearchQuery) => httpClient.get<PagedResult<DeckDto>>("/admin/decks",{params}),

  getDeckById: (id: number) => httpClient.get<DeckDto>(`/admin/decks/${id}`),

  createDeck: (dto: CreateDeckDto) =>
    httpClient.post<DeckDto>("/admin/decks", dto),

  editDeck: (id: string, dto: EditDeckDto) =>
    httpClient.put<DeckDto>(`/admin/decks/${id}`, dto),

  deleteDeck: (id: string) =>
    httpClient.delete<InformativeResult>(`/admin/decks/${id}`),

  getCardFromDeck: (id: string) => httpClient.get(`/admin/decks/${id}`),

  addCardInDeck: (id: string, dto: AddCardInDeckDto[]) =>
    httpClient.put<InformativeResult>(`/admin/decks/${id}/cards`, dto),

  removeCardFromDeck: (id: string, idsCards: number[]) =>
    httpClient.delete<InformativeResult>(`/admin/decks/${id}/remove-cards`, {
      data: idsCards,
    }),
};
