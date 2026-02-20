import { createEnumStore } from "./createEnumStore";
import { enumsApi } from "../api/enums.api";

export const useCardZonesStore =
  createEnumStore(enumsApi.cardZones);
