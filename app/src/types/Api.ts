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
    hostId: string
    userIds: string[]
    wins: Record<string, number>
    settings: Api.GameSettings
  }

  export type User = {
    userId: string
    lobbyId: string
  }

  export type VoteSingle = {
    lobbyId: string
    userId: string
    choice: string
  }

  export type VoteGroup = {
    lobbyId: string
    requiredVotes: number
    choices: string[]
    votes: Record<string, string[]>
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

    export type UserGet = User

    export type UserPut = User

    export type VoteGetAll = VoteGroup

    export type VoteGet = VoteSingle

    export type VotePut = VoteSingle
  }
}
