import { Api } from "./Api"

export namespace Websocket {
  export type Message<T = unknown> = {
    eventName: string
    userId: string
    data: T
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

    export type VoteAdd = {
      eventName: "VOTE_ADD"
      userId: string

      /** Choice */
      data: string
    }

    export type VoteRemove = {
      eventName: "VOTE_REMOVE"
      userId: string

      /** Choice */
      data: string
    }

    export type VoteUpdateRequirement = {
      eventName: "VOTE_UPDATE_REQUIREMENT"
      userId: string

      /** Votes required */
      data: number
    }

    export type BoardCreate = {
      eventName: "BOARD_CREATE"
      userId: string

      /** Game state, serialized */
      data: string

      // TODO: To serialize: new TextEncoder().encode(data)
    }
  }
}
