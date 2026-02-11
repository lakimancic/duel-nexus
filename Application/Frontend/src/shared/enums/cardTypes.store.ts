import { createEnumStore } from "./createEnumStore";
import { enumsApi } from "../api/enums.api";

export const useCardTypesStore =
  createEnumStore(enumsApi.cardTypes);
