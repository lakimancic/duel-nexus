import { HubConnection } from "@microsoft/signalr";
import { createConnection } from "./connection";
import type { MessageDto } from "../types/message.types";
import type { ShortUserDto } from "../types/user.types";
import type { GameStartedEventDto } from "@/features/friendly/types/friendly.types";

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

  onGameRoomStarted(handler: (event: GameStartedEventDto) => void) {
    this.connection.on("game-room:game:started", handler);
  }

  offGameRoomStarted(handler: (...args: any[]) => void) {
    this.connection.off("game-room:game:started", handler);
  }

  // ========== Game methods / subscriptions ==========

  joinGame(gameId: string) {
    return this.ensureConnected().then(() =>
      this.connection.invoke("game:join", gameId)
    );
  }

  leaveGame(gameId: string) {
    return this.ensureConnected().then(() =>
      this.connection.invoke("game:leave", gameId)
    );
  }

  drawCard(gameId: string) {
    return this.ensureConnected().then(() =>
      this.connection.invoke("game:action:draw", gameId)
    );
  }

  skipDraw(gameId: string) {
    return this.ensureConnected().then(() =>
      this.connection.invoke("game:action:draw:skip", gameId)
    );
  }

  placeCard(gameId: string, gameCardId: string, fieldIndex: number, faceDown: boolean) {
    return this.ensureConnected().then(() =>
      this.connection.invoke("game:action:place", gameId, gameCardId, fieldIndex, faceDown)
    );
  }

  nextPhase(gameId: string) {
    return this.ensureConnected().then(() =>
      this.connection.invoke("game:action:next", gameId)
    );
  }

  sendCardToGraveyard(gameId: string, gameCardId: string) {
    return this.ensureConnected().then(() =>
      this.connection.invoke("game:action:grave", gameId, gameCardId)
    );
  }

  toggleDefensePosition(gameId: string, gameCardId: string) {
    return this.ensureConnected().then(() =>
      this.connection.invoke("game:action:toggle-defense", gameId, gameCardId)
    );
  }

  revealCard(gameId: string, gameCardId: string) {
    return this.ensureConnected().then(() =>
      this.connection.invoke("game:action:reveal", gameId, gameCardId)
    );
  }

  onDrawResult(handler: (...args: any[]) => void) {
    this.connection.on("game:draw:result", handler);
  }

  offDrawResult(handler: (...args: any[]) => void) {
    this.connection.off("game:draw:result", handler);
  }

  onPlayerDrew(handler: (...args: any[]) => void) {
    this.connection.on("game:player:drew", handler);
  }

  offPlayerDrew(handler: (...args: any[]) => void) {
    this.connection.off("game:player:drew", handler);
  }

  onSkipDrawResult(handler: (...args: any[]) => void) {
    this.connection.on("game:draw:skip:result", handler);
  }

  offSkipDrawResult(handler: (...args: any[]) => void) {
    this.connection.off("game:draw:skip:result", handler);
  }

  onPlayerSkippedDraw(handler: (...args: any[]) => void) {
    this.connection.on("game:player:draw:skipped", handler);
  }

  offPlayerSkippedDraw(handler: (...args: any[]) => void) {
    this.connection.off("game:player:draw:skipped", handler);
  }

  onPlaceResult(handler: (...args: any[]) => void) {
    this.connection.on("game:place:result", handler);
  }

  offPlaceResult(handler: (...args: any[]) => void) {
    this.connection.off("game:place:result", handler);
  }

  onPlayerPlaced(handler: (...args: any[]) => void) {
    this.connection.on("game:player:placed", handler);
  }

  offPlayerPlaced(handler: (...args: any[]) => void) {
    this.connection.off("game:player:placed", handler);
  }

  onNextResult(handler: (...args: any[]) => void) {
    this.connection.on("game:next:result", handler);
  }

  offNextResult(handler: (...args: any[]) => void) {
    this.connection.off("game:next:result", handler);
  }

  onPhaseAdvanced(handler: (...args: any[]) => void) {
    this.connection.on("game:phase:advanced", handler);
  }

  offPhaseAdvanced(handler: (...args: any[]) => void) {
    this.connection.off("game:phase:advanced", handler);
  }

  onGraveResult(handler: (...args: any[]) => void) {
    this.connection.on("game:grave:result", handler);
  }

  offGraveResult(handler: (...args: any[]) => void) {
    this.connection.off("game:grave:result", handler);
  }

  onToggleDefenseResult(handler: (...args: any[]) => void) {
    this.connection.on("game:toggle:defense:result", handler);
  }

  offToggleDefenseResult(handler: (...args: any[]) => void) {
    this.connection.off("game:toggle:defense:result", handler);
  }

  onPlayerCardUpdated(handler: (...args: any[]) => void) {
    this.connection.on("game:player:card:updated", handler);
  }

  offPlayerCardUpdated(handler: (...args: any[]) => void) {
    this.connection.off("game:player:card:updated", handler);
  }

  onRevealResult(handler: (...args: any[]) => void) {
    this.connection.on("game:reveal:result", handler);
  }

  offRevealResult(handler: (...args: any[]) => void) {
    this.connection.off("game:reveal:result", handler);
  }
}

export const gameHub = new GameHubClient();
