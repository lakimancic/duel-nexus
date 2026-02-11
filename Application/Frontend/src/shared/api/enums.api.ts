import { httpClient } from "./httpClient";
import type { EnumDto } from "../types/enum.types";

export const enumsApi = {
  effectTypes: () =>
    httpClient.get<EnumDto[]>("/admin/effects/types"),

  cardTypes: () =>
    httpClient.get<EnumDto[]>("/admin/cards/types"),
};
