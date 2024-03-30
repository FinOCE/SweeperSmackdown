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

      /** User joined message */
      data: string
    }

    export type UserLeave = {
      eventName: "USER_LEAVE"
      userId: string

      /** User left message */
      data: string
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

      /** Game state, serialized */
      data: string
    }

    export type MoveAdd = {
      eventName: "MOVE_ADD"
      userId: string

      /** The indices of tiles that were revealed by the move */
      data: { reveals: number[] } | { flagAdd: number } | { flagRemove: number }
    }
  }
}
