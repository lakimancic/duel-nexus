import { useEffect } from "react";
import { useCardTypesStore } from "./cardTypes.store";
import { useEffectTypesStore } from "./effectTypes.store";
import { useCardZonesStore } from "./cardZones.store";

export function useAdminEnums() {
  const loadCardTypes = useCardTypesStore(s => s.load);
  const loadEffectTypes = useEffectTypesStore(s => s.load);
  const loadCardZones = useCardZonesStore(s => s.load);

  useEffect(() => {
    loadCardTypes();
    loadEffectTypes();
    loadCardZones();
  }, [
    loadCardTypes,
    loadEffectTypes,
    loadCardZones,
  ]);
}
