export namespace Api {
  export type GameSettings = {
    mode: number
    height: number
    width: number
    mines: number
    lives: number
    timeLimit: number
    boardCount: number
    shareBoards: boolean
  }

  export type Lobby = {
    lobbyId: string
    userIds: string[]
    wins: Record<string, number>
    settings: Api.GameSettings
  }

  export type User = {
    userId: string
    lobbyId: string
  }

  export namespace Response {
    export type Negotiate = {
      baseUrl: string
      url: string
      accessToken: string
    }

    export type LobbyGet = Lobby

    export type LobbyPut = Lobby

    export type UserPut = User
  }
}
