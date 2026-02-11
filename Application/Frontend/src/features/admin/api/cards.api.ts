import { httpClient } from "@/shared/api/httpClient";
import type { CardDto, CreateCardDto } from "../types/card.types";
import type { PagedResult, SearchQuery } from "@/shared/types/result.types";

export const cardsApi = {
  getCards: (params?: SearchQuery) =>
    httpClient.get<PagedResult<CardDto>>("/admin/cards", {
      params,
    }),

  getCard: (id: string) => httpClient.get<CardDto>(`/admin/cards/${id}`),

  createCard: (dto: CreateCardDto) =>
    httpClient.post<CardDto>("/admin/cards", dto),

  updateCard: (id: string, dto: CreateCardDto) =>
    httpClient.put<CardDto>(`/admin/cards/${id}`, dto),

  deleteCard: (id: string) => httpClient.delete(`/admin/cards/${id}`),

  uploadImage: (file: File) => {
    const formData = new FormData();
    formData.append("file", file);

    return httpClient.post<{ fileName: string }>(
      "/admin/cards/upload-image",
      formData,
      {
        headers: { "Content-Type": "multipart/form-data" },
      },
    );
  },
};
