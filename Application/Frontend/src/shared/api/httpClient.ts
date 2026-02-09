import axios from "axios";
import { useAuthStore } from "@/features/auth/store/auth.store";

export const httpClient = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL,
});

httpClient.interceptors.request.use((config) => {
  const token = useAuthStore.getState().token;
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export const backImageUrl = (url: string) => `${import.meta.env.VITE_API_BASE_URL}/admin/cards/image/${url}`;
