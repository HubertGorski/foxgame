import { defineStore } from "pinia";
import type { User } from "@/models/User";
import { BASE_URL_PSYCH } from "@/api/Client";
import * as signalR from "@microsoft/signalr";
import { Game } from "@/models/Game";
import { plainToInstance } from "class-transformer";

interface SignalRState {
  connection: signalR.HubConnection | null;
  game: Game | null;
  publicGames: Game[];
  errorPassword: string | null;
  errorCode: string | null;
}

export const useSignalRStore = defineStore({
  id: "signalRStore",
  state: (): SignalRState => ({
    connection: null,
    game: null,
    publicGames: [],
    errorCode: null,
    errorPassword: null,
  }),

  getters: {},

  actions: {
    clearStore() {
      this.connection = null;
      this.game = null;
    },

    async connect() {
      if (this.connection) {
        return;
      }

      this.connection = new signalR.HubConnectionBuilder()
        .withUrl(BASE_URL_PSYCH)
        .withAutomaticReconnect()
        .build();

      this.connection.on("PlayerLeft", (playerId: number) => {
        if (!this.game) {
          return;
        }

        this.game.users = this.game.users.filter(
          (p: User) => p.userId !== playerId
        );
      });

      this.connection.on("GetPlayers", (joinedPlayers: User[]) => {
        if (!this.game) {
          return;
        }

        this.game.users = joinedPlayers;
      });

      this.connection.on("GetGameCode", (code: string) => {
        if (!this.game) {
          return;
        }

        this.game.code = code;
      });

      this.connection.on("RoomClosed", () => {
        this.clearStore();
      });

      this.connection.on("LoadRoom", (game: Game) => {
        this.game = plainToInstance(Game, game);
      });

      this.connection.on("ReceiveError", (error) => {
        if (error.fieldId === "code") {
          this.errorCode = error.message;
        } else {
          this.errorPassword = error.message;
        }
      });

      this.connection.on("GetPublicRooms", (games: Game[]) => {
        this.publicGames = plainToInstance(Game, games);
      });

      await this.connection.start();
    },

    async joinRoom(
      gameCode: string,
      player: User,
      password: string,
      ownerId: number | null = null
    ) {
      if (!this.connection) {
        return;
      }

      if (this.game) {
        await this.leaveRoom(player.userId);
      }

      await this.connection.invoke(
        "JoinRoom",
        gameCode,
        player,
        password,
        ownerId
      );
    },

    async createRoom(game: Game) {
      if (!this.connection) {
        return;
      }

      this.game = game;
      await this.connection.invoke("CreateRoom", game);
    },

    async goToJoinGameView() {
      if (!this.connection) {
        return;
      }

      await this.connection.invoke("GoToJoinGameView");
    },

    async editRoom(game: Game) {
      if (!this.connection) {
        return;
      }

      this.game = game;
      await this.connection.invoke("EditRoom", game);
    },

    async setStatus(playerId: number, status: boolean) {
      if (!this.connection || !this.game) {
        return;
      }

      await this.connection.invoke("SetStatus", this.game.code, playerId, status);
    },

    async leaveRoom(playerId: number) {
      if (!this.connection || !this.game) {
        return;
      }

      await this.connection.invoke("LeaveRoom", this.game.code, playerId);

      this.clearStore();
    },

    async removeRoom() {
      if (!this.connection || !this.game) {
        return;
      }

      await this.connection.invoke("RemoveRoom", this.game.code);
      this.clearStore();
    },
  },
});
