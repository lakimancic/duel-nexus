import { HubConnection } from "@microsoft/signalr";
import { createConnection } from "./connection";
import type { MessageDto } from "../types/message.types";
import type { ShortUserDto } from "../types/user.types";

class GameHubClient {
  private connection: HubConnection;

  constructor() {
    this.connection = createConnection("/gameHub");
  }

  async start() {
    if (this.connection.state === "Disconnected") {
      await this.connection.start();
    }
  }

  async stop() {
    if (this.connection.state !== "Disconnected") {
      await this.connection.stop();
    }
  }

  // ========== Connections subscriptions ==========

  onUserConnected(handler: (user: ShortUserDto|null) => void) {
    this.connection.on("users:connected", handler);
  }

  onUserDisconnected(handler: (userId: string|null) => void) {
    this.connection.on("users:disconnected", handler);
  }

  offUserConnected(handler: (...args: any[]) => void) {
    this.connection.off("users:connected", handler);
  }

  offUserDisconnected(handler: (...args: any[]) => void) {
    this.connection.off("users:disconnected", handler);
  }

  // ========== Chat methods / subscriptions ==========

  sendGlobalChat(content: string) {
    return this.connection.invoke("chat:global:send", content);
  }

  onGlobalChat(handler: (msg: MessageDto) => void) {
    this.connection.on("chat:global:recv", handler);
  }

  offGlobalChat(handler: (...args: any[]) => void) {
    this.connection.off("chat:global:recv", handler);
  }
}

export const gameHub = new GameHubClient();
