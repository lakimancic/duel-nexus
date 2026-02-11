import { httpClient } from "@/shared/api/httpClient";
import type {
  InformativeResult,
  PagedResult,
  Query,
} from "@/shared/types/result.types";
import type { UpdateUserDto, UserDto } from "../types/users.types";
import type { EnumDto } from "@/shared/types/enum.types";
import type { CreatePlayerCardDto, PlayerCardDto } from "../types/card.types";

export const usersApi = {
  getUsers: (params?: Query) => httpClient.get<PagedResult<UserDto>>("/admin/users", {params}),

  getUserById: (id: string) => httpClient.get<UserDto>(`/admin/users/${id}`),

  editUser: (id: string, dto: UpdateUserDto) =>
    httpClient.put<UserDto>(`/admin/users/${id}`, dto),

  deleteUser: (id: string) =>
    httpClient.delete<InformativeResult>(`/admin/users/${id}`),

  getRoles: () => httpClient.get<EnumDto>("/admin/users/roles"),

  getUserCards: (id: string, params?: Query) =>
    httpClient.get<PagedResult<PlayerCardDto>>(`/admin/users/${id}/cards`, {params}),

  createUserCard: (id: string, dto: CreatePlayerCardDto) =>
    httpClient.post<PlayerCardDto>(`/admin/users/${id}/cards`, dto),

  updateUserCard: (userId: string, cardId: string, quantity: number) =>
    httpClient.put<PlayerCardDto>(`/admin/users/${userId}/cards/${cardId}`, {
      quantity,
    }),

  deleteUserCard: (userId: string, cardId: number) =>
    httpClient.delete<InformativeResult>(
      `/admin/users/${userId}/cards/${cardId}`,
    ),
};
