import * as signalR from "@microsoft/signalr";
import { useAuthStore } from "@/features/auth/store/auth.store";
import { HttpTransportType } from "@microsoft/signalr";

export function createConnection(hubUrl: string) {
  return new signalR.HubConnectionBuilder()
    .withUrl(`${import.meta.env.VITE_API_BASE_URL}${hubUrl}`, {
      accessTokenFactory: () => {
        const { token } = useAuthStore.getState();
        return token ?? "";
      },
      transport: HttpTransportType.WebSockets | HttpTransportType.LongPolling
    })
    .withAutomaticReconnect()
    .configureLogging(signalR.LogLevel.Information)
    .build();
};
