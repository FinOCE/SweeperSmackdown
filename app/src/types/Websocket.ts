import { Api } from "./Api"

export namespace Websocket {
  export type Message<T = unknown> = {
    eventName: string
    userId: string
    data?: T
  }

  export namespace Request {
    export type OnMoveData = {
      lobbyId: string
      reveals?: number[]
      flagAdd?: number
      flagRemove?: number
    }
  }

  export namespace Response {
    export type CreatedLobby = {
      eventName: "LOBBY_CREATED"
      userId: "SYSTEM"

      /** The ID of the lobby created */
      data: string
    }

    export type UpdateLobbyStatus = {
      eventName: "LOBBY_STATUS_UPDATE"
      userId: "SYSTEM"

      data: Api.LobbyOrchestratorStatus
    }

    export type JoinLobby = {
      eventName: "LOBBY_JOIN"
      userId: string

      /** The ID of the lobby joined */
      data: string
    }

    export type LeaveLobby = {
      eventName: "LOBBY_LEAVE"
      userId: string

      /** The ID of the lobby left */
      data: string
    }

    export type AddPlayer = {
      eventName: "PLAYER_ADD"
      userId: string

      data: Api.Player
    }

    export type RemovePlayer = {
      eventName: "PLAYER_REMOVE"
      userId: string
    }

    export type UpdatePlayer = {
      eventName: "PLAYER_UPDATE"
      userId: string

      data: Api.Player
    }

    export type UpdateLobbySettings = {
      eventName: "LOBBY_UPDATE_SETTINGS"
      userId: "SYSTEM"

      data: Api.GameSettings
    }

    export type UpdateLobbySettingsFailed = {
      eventName: "LOBBY_UPDATE_SETTINGS_FAILED"
      userId: "SYSTEM"

      /** The correct current game settings */
      data: Api.GameSettings
    }

    export type UpdateConfigureState = {
      eventName: "LOBBY_STATE_UPDATE"
      userId: "SYSTEM"

      data: Api.Enums.EGameSettingsStateMachineState
    }

    export type UpdateConfigureStateFailed = {
      eventName: "LOBBY_STATE_UPDATE_FAILED"
      userId: "SYSTEM"

      /** The correct current configure state */
      data: Api.Enums.EGameSettingsStateMachineState
    }

    export type UpdateLobbyHost = {
      eventName: "LOBBY_HOST_UPDATE"
      userId: "SYSTEM"

      /** The ID of the new host */
      data: string
    }

    export type UpdateLobbyHostManaged = {
      eventName: "LOBBY_HOST_MANAGED_UPDATE"
      userId: "SYSTEM"

      /** Whether or not the lobby is now host managed */
      data: boolean
    }

    export type CreateBoard = {
      eventName: "BOARD_CREATE"
      userId: string

      /** The updated state of the board and whether it was created due to reset */
      data: {
        gameState: string
        reset: boolean
      }
    }

    export type MakeMove = {
      eventName: "MOVE_ADD"
      userId: string

      data: Request.OnMoveData
    }

    export type RejectMove = {
      eventName: "MOVE_REJECT"
      userId: string

      data: Request.OnMoveData
    }

    export type UpdatePlayerState = {
      eventName: "PLAYER_STATE_UPDATE"
      userId: string

      data: Api.PlayerState
    }

    export type GameWon = {
      eventName: "GAME_WON"
      userId: string
    }
  }
}
