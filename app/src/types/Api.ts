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
    settings: Api.GameSettings
    state: Enums.ELobbyState
  }

  export type User = {
    userId: string
    lobbyId: string
  }

  export type BoardDictionary = Record<string, string>

  export namespace Enums {
    export enum ELobbyState {
      Init = 0,
      ConfigureUnlocked = 1,
      ConfigureLocked = 2,
      Play = 3,
      Won = 4,
      Celebrate = 5
    }
  }

  export namespace Request {
    export type LobbyPatch = {
      hostId?: string
      mode?: number
      height?: number
      width?: number
      mines?: number
      lives?: number
      timeLimit?: number
      boardCount?: number
      shareBoards?: boolean
    }
  }

  export namespace Response {
    export type LobbyGet = Lobby

    export type LobbyPatch = Lobby

    export type LobbyPut = Lobby

    export type Negotiate = {
      baseUrl: string
      url: string
      accessToken: string
    }

    export type Token = {
      accessToken: string
    }

    export type Login = {
      bearerToken: string
    }

    export type LobbyUserGet = User

    export type LobbyUserPut = User

    export type BoardGet = BoardDictionary

    export type BoardGetAll = BoardDictionary
  }
}
