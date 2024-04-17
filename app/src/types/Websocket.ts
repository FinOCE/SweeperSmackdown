import { Api } from "./Api"

export namespace Websocket {
  export type Message<T = unknown> = {
    eventName: string
    userId: string
    data?: T
  }

  export namespace Response {
    export type LobbyUpdate = {
      eventName: "LOBBY_UPDATE"
      userId: string

      /** Updated lobby */
      data: Api.Lobby
    }

    export type UserJoin = {
      eventName: "USER_JOIN"
      userId: string
    }

    export type UserLeave = {
      eventName: "USER_LEAVE"
      userId: string
    }

    export type LobbyStart = {
      eventName: "LOBBY_START"
      userId: "SYSTEM"
    }

    export type VoteStateUpdate = {
      eventName: "VOTE_STATE_UPDATE"
      userId: string

      /** Updated vote state */
      data: Api.VoteGroup
    }

    export type TimerStart = {
      eventName: "TIMER_START"
      userId: "SYSTEM"

      /** Expiry for the timer as unix time milliseconds */
      data: { expiry: number }
    }

    export type TimerReset = {
      eventName: "TIMER_RESET"
      userId: "SYSTEM"
    }

    export type BoardCreate = {
      eventName: "BOARD_CREATE"
      userId: string

      /** Serialized game state and whether or not it was a reset */
      data: { gameState: string; reset: boolean }
    }

    export type MoveAdd = {
      eventName: "MOVE_ADD"
      userId: string

      /** The indices of tiles that were revealed by the move */
      data: { lobbyId: string } & ({ reveals: number[] } | { flagAdd: number } | { flagRemove: number })
    }

    export type GameWon = {
      eventName: "GAME_WON"
      userId: string
    }
  }
}
