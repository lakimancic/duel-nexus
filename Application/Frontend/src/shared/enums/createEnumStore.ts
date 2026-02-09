import { create } from "zustand";
import type { EnumDto } from "../types/enum.types";

interface EnumState {
  items: EnumDto[];
  loading: boolean;
  loaded: boolean;
  load: () => Promise<void>;
}

export function createEnumStore(loader: () => Promise<{ data: EnumDto[] }>) {
  return create<EnumState>((set, get) => ({
    items: [],
    loading: false,
    loaded: false,

    load: async () => {
      if (get().loaded) return;

      set({ loading: true });
      const { data } = await loader();
      set({
        items: data,
        loading: false,
        loaded: true,
      });
    },
  }));
}
