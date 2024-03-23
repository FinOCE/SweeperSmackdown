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

  export namespace Response {
    export type Negotiate = {
      baseUrl: string
      url: string
      accessToken: string
    }

    export type LobbyPut = {
      lobbyId: string
      userIds: string[]
      wins: Record<string, number>
      settings: Api.GameSettings
    }

    export type UserPut = {
      userId: string
      lobbyId: string
    }
  }
}
