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

  private async ensureConnected() {
    if (this.connection.state === "Disconnected") {
      await this.start();
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
    return this.ensureConnected().then(() =>
      this.connection.invoke("chat:global:send", content)
    );
  }

  sendGameRoomChat(gameRoomId: string, content: string) {
    return this.ensureConnected().then(() =>
      this.connection.invoke("chat:game-room:send", gameRoomId, content)
    );
  }

  sendPrivateChat(receiverId: string, content: string) {
    return this.ensureConnected().then(() =>
      this.connection.invoke("chat:private:send", receiverId, content)
    );
  }

  onGlobalChat(handler: (msg: MessageDto) => void) {
    this.connection.on("chat:global:recv", handler);
  }

  onGameRoomChat(handler: (msg: MessageDto) => void) {
    this.connection.on("chat:game-room:recv", handler);
  }

  onPrivateChat(handler: (msg: MessageDto) => void) {
    this.connection.on("chat:private:recv", handler);
  }

  offGlobalChat(handler: (...args: any[]) => void) {
    this.connection.off("chat:global:recv", handler);
  }

  offGameRoomChat(handler: (...args: any[]) => void) {
    this.connection.off("chat:game-room:recv", handler);
  }

  offPrivateChat(handler: (...args: any[]) => void) {
    this.connection.off("chat:private:recv", handler);
  }

  // ========== Game room methods / subscriptions ==========

  joinGameRoom(gameRoomId: string) {
    return this.ensureConnected().then(() =>
      this.connection.invoke("game-room:join", gameRoomId)
    );
  }

  leaveGameRoom(gameRoomId: string) {
    return this.ensureConnected().then(() =>
      this.connection.invoke("game-room:leave", gameRoomId)
    );
  }

  onGameRoomPlayersUpdated(handler: (roomId: string) => void) {
    this.connection.on("game-room:players:updated", handler);
  }

  offGameRoomPlayersUpdated(handler: (...args: any[]) => void) {
    this.connection.off("game-room:players:updated", handler);
  }

  onGameRoomCancelled(handler: (roomId: string) => void) {
    this.connection.on("game-room:cancelled", handler);
  }

  offGameRoomCancelled(handler: (...args: any[]) => void) {
    this.connection.off("game-room:cancelled", handler);
  }
}

export const gameHub = new GameHubClient();
