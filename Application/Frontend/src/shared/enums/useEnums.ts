import { useEffect } from "react";
import { useCardTypesStore } from "./cardTypes.store";
import { useEffectTypesStore } from "./effectTypes.store";

export function useAdminEnums() {
  const loadCardTypes = useCardTypesStore(s => s.load);
  const loadEffectTypes = useEffectTypesStore(s => s.load);

  useEffect(() => {
    loadCardTypes();
    loadEffectTypes();
  }, [
    loadCardTypes,
    loadEffectTypes,
  ]);
}
