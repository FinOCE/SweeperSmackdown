export namespace Api {
  export type GameSettings = {
    mode: number
    height: number
    width: number
    mines: number
    lives: number
    timeLimit: number
    boardCount: number
    seed: number
  }

  export type Lobby = {
    lobbyId: string
    hostId: string
    userIds: string[]
    scores: Record<string, number>
    wins: Record<string, number>
    status: Enums.ELobbyStatus
    stateExpiry: string
    settings: Api.GameSettings
  }

  export type Player = {
    id: string
    lobbyId: string
    active: boolean
    score: number
    wins: number
  }

  export type PlayerState = {
    lives: number
    disabledUntil?: string
    boardState: string
  }

  export type LobbyOrchestratorStatus = {
    status: Enums.ELobbyStatus
    statusUntil: string
  }

  export namespace Enums {
    export enum ELobbyStatus {
      Configuring,
      Starting,
      Playing,
      Concluding,
      Celebrating
    }

    export enum EGameSettingsStateMachineState {
      Unlocked,
      Locked,
      Confirmed
    }
  }

  export namespace Request {
    export type TokenPost = {
      code: string
      mocked: boolean
    }

    export type LoginPost = {
      accessToken: string
      mocked: boolean
    }

    export type LobbyPatch = {
      hostId?: string
      hostManaged?: boolean
    }

    export type GameSettingsPatch = {
      mode?: number
      height?: number
      width?: number
      mines?: number
      difficulty?: number
      lives?: number
      timeLimit?: number
      boardCount?: number
      shareBoards?: boolean
    }

    export type BoardSolutionPost = {
      gameState: string
    }
  }

  export namespace Response {
    export type BoardGetAll = Record<string, PlayerState>

    export type BoardGet = Record<string, PlayerState>

    export type LobbyUserGet = Player

    export type LobbyUserPut = Player

    export type LobbyGet = Lobby

    export type LoginPost = {
      bearerToken: string
    }

    export type NegotiatePost = {
      baseUrl: string
      url: string
      accessToken: string
    }

    export type TokenPost = {
      accessToken: string
    }
  }
}
