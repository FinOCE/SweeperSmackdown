import { Api } from "../types/Api"
import { useGameInfo } from "./useGameInfo"

export function useApi() {
  const gameInfo = useGameInfo()
  const baseUrl = import.meta.env.PUBLIC_ENV__API_BASE_URL

  function ok(res: Response): Response {
    if (res.status < 200 || res.status >= 300) throw new Error("Invalid status code")
    return res
  }

  function okOrNotFound(res: Response): Response {
    if (res.status !== 404 && (res.status < 200 || res.status >= 300)) throw new Error("Invalid status code")
    return res
  }

  return {
    lobbyGet: (lobbyId?: string) =>
      fetch(baseUrl + `/lobbies/${lobbyId ?? gameInfo.lobbyId}`, { method: "GET" })
        .then(ok)
        .then(res => res.json())
        .then((res: Api.Response.LobbyGet) => res),

    lobbyPatch: (settings: Partial<Api.GameSettings>, lobbyId?: string) =>
      fetch(baseUrl + `/lobbies/${lobbyId ?? gameInfo.lobbyId}`, { method: "PATCH", body: JSON.stringify(settings) })
        .then(ok)
        .then(res => res.json())
        .then((res: Api.Response.LobbyPatch) => res),

    lobbyPut: (lobbyId?: string) =>
      fetch(baseUrl + `/lobbies/${lobbyId ?? gameInfo.lobbyId}`, { method: "PUT" })
        .then(ok)
        .then(res => res.json())
        .then((res: Api.Response.LobbyPut) => res),

    negotiate: async (userId?: string) =>
      await fetch(baseUrl + `/negotiate?userId=${userId ?? gameInfo.userId}`, { method: "POST" })
        .then(ok)
        .then(res => res.json())
        .then((res: Api.Response.Negotiate) => res),

    userDelete: (lobbyId?: string, userId?: string) =>
      fetch(baseUrl + `/lobbies/${lobbyId ?? gameInfo.lobbyId}/users/${userId ?? gameInfo.userId}`, {
        method: "DELETE"
      })
        .then(okOrNotFound)
        .then(() => {}),

    userGet: (lobbyId?: string, userId?: string) =>
      fetch(baseUrl + `/lobbies/${lobbyId ?? gameInfo.lobbyId}/users/${userId ?? gameInfo.userId}`, { method: "PUT" })
        .then(ok)
        .then(res => res.json())
        .then((res: Api.Response.UserGet) => res),

    userPut: (lobbyId?: string, userId?: string) =>
      fetch(baseUrl + `/lobbies/${lobbyId ?? gameInfo.lobbyId}/users/${userId ?? gameInfo.userId}`, { method: "PUT" })
        .then(ok)
        .then(res => res.json())
        .then((res: Api.Response.UserPut) => res),

    voteDelete: (lobbyId?: string, userId?: string) =>
      fetch(baseUrl + `/lobbies/${lobbyId ?? gameInfo.lobbyId}/votes/${userId ?? gameInfo.userId}`, {
        method: "DELETE"
      })
        .then(okOrNotFound)
        .then(() => {}),

    voteGetAll: (lobbyId?: string) =>
      fetch(baseUrl + `/lobbies/${lobbyId ?? gameInfo.lobbyId}/votes`, { method: "GET" })
        .then(ok)
        .then(res => res.json())
        .then((res: Api.Response.VoteGetAll) => res),

    voteGet: (lobbyId?: string, userId?: string) =>
      fetch(baseUrl + `/lobbies/${lobbyId ?? gameInfo.lobbyId}/votes/${userId ?? gameInfo.userId}`, { method: "GET" })
        .then(ok)
        .then(res => res.json())
        .then((res: Api.Response.VoteGet) => res),

    votePut: (choice: string, lobbyId?: string, userId?: string) =>
      fetch(baseUrl + `/lobbies/${lobbyId ?? gameInfo.lobbyId}/votes/${userId ?? gameInfo.userId}`, {
        method: "PUT",
        body: JSON.stringify({ choice })
      })
        .then(ok)
        .then(res => res.json())
        .then((res: Api.Response.VotePut) => res)
  }
}
