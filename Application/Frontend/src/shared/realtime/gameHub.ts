import { HubConnection } from "@microsoft/signalr";
import { createConnection } from "./connection";
import type { MessageDto } from "../types/message.types";

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
