import { Api } from "../types/Api"
import { useGameInfo } from "./useGameInfo"

export function useApi() {
  const gameInfo = useGameInfo()
  const baseUrl = import.meta.env.PUBLIC_ENV__API_BASE_URL

  function ok(res: Response): Response {
    if (res.status < 200 || res.status >= 300) throw new Error("Invalid status code")
    return res
  }

  return {
    negotiate: async (userId?: string) =>
      await fetch(baseUrl + `/negotiate?userId=${userId ?? gameInfo.userId}`, { method: "POST" })
        .then(ok)
        .then(res => res.json())
        .then((res: Api.Response.Negotiate) => res),

    lobbyGet: (lobbyId?: string) =>
      fetch(baseUrl + `/lobbies/${lobbyId ?? gameInfo.lobbyId}`, { method: "GET" })
        .then(ok)
        .then(res => res.json())
        .then((res: Api.Response.LobbyGet) => res),

    lobbyPut: (lobbyId?: string) =>
      fetch(baseUrl + `/lobbies/${lobbyId ?? gameInfo.lobbyId}`, { method: "PUT" })
        .then(ok)
        .then(res => res.json())
        .then((res: Api.Response.LobbyPut) => res),

    userPut: (lobbyId?: string, userId?: string) =>
      fetch(baseUrl + `/lobbies/${lobbyId ?? gameInfo.lobbyId}/users/${userId ?? gameInfo.userId}`, { method: "PUT" })
        .then(ok)
        .then(res => res.json())
        .then((res: Api.Response.UserPut) => res),

    userDelete: (lobbyId?: string, userId?: string) =>
      fetch(baseUrl + `/lobbies/${lobbyId ?? gameInfo.lobbyId}/users/${userId ?? gameInfo.userId}`, {
        method: "DELETE "
      })
        .then(ok)
        .then(() => {})
  }
}
