import { create } from "zustand";

type PrivateMessagesState = {
  unreadCount: number;
  incrementUnread: () => void;
  resetUnread: () => void;
};

export const usePrivateMessagesStore = create<PrivateMessagesState>((set) => ({
  unreadCount: 0,
  incrementUnread: () =>
    set((state) => ({
      unreadCount: state.unreadCount + 1,
    })),
  resetUnread: () => set({ unreadCount: 0 }),
}));
