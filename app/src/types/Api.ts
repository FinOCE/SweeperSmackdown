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

  export type BoardDictionary = Record<string, string>

  export namespace Enums {
    export enum ELobbyState {
      Init = 0,
      Configure = 1,
      Play = 2,
      Won = 3,
      Celebrate = 4
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

    export type UserGet = User

    export type UserPut = User

    export type VoteGetAll = VoteGroup

    export type VoteGet = VoteSingle

    export type VotePut = VoteSingle

    export type BoardGet = BoardDictionary

    export type BoardGetAll = BoardDictionary
  }
}
