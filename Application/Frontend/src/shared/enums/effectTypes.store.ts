import { createEnumStore } from "./createEnumStore";
import { enumsApi } from "../api/enums.api";

export const useEffectTypesStore =
  createEnumStore(enumsApi.effectTypes);
