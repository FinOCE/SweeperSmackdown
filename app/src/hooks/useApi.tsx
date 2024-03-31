import { Api } from "../types/Api"
import { useGameInfo } from "./useGameInfo"

export function useApi() {
  const gameInfo = useGameInfo()
  const baseUrl = process.env.PUBLIC_ENV__API_BASE_URL

  const headers = {
    Authorization: `Bearer ${gameInfo.userId}` // TODO: Use real bearer token
  }

  const jsonHeader = {
    "Content-Type": "application/json"
  }

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
      fetch(baseUrl + `/lobbies/${lobbyId ?? gameInfo.lobbyId}`, { method: "GET", headers })
        .then(ok)
        .then(res => res.json())
        .then((res: Api.Response.LobbyGet) => res),

    lobbyPatch: (settings: Partial<Api.GameSettings>, lobbyId?: string) =>
      fetch(baseUrl + `/lobbies/${lobbyId ?? gameInfo.lobbyId}`, {
        method: "PATCH",
        body: JSON.stringify(settings),
        headers: { ...headers, ...jsonHeader }
      })
        .then(ok)
        .then(res => res.json())
        .then((res: Api.Response.LobbyPatch) => res),

    lobbyPut: (lobbyId?: string) =>
      fetch(baseUrl + `/lobbies/${lobbyId ?? gameInfo.lobbyId}`, { method: "PUT", headers })
        .then(ok)
        .then(res => res.json())
        .then((res: Api.Response.LobbyPut) => res),

    negotiate: async (userId?: string) =>
      await fetch(baseUrl + `/negotiate?userId=${userId ?? gameInfo.userId}`, { method: "POST", headers })
        .then(ok)
        .then(res => res.json())
        .then((res: Api.Response.Negotiate) => res),

    userDelete: (lobbyId?: string, userId?: string) =>
      fetch(baseUrl + `/lobbies/${lobbyId ?? gameInfo.lobbyId}/users/${userId ?? gameInfo.userId}`, {
        method: "DELETE",
        headers
      })
        .then(okOrNotFound)
        .then(() => {}),

    userGet: (lobbyId?: string, userId?: string) =>
      fetch(baseUrl + `/lobbies/${lobbyId ?? gameInfo.lobbyId}/users/${userId ?? gameInfo.userId}`, {
        method: "PUT",
        headers
      })
        .then(ok)
        .then(res => res.json())
        .then((res: Api.Response.UserGet) => res),

    userPut: (lobbyId?: string, userId?: string) =>
      fetch(baseUrl + `/lobbies/${lobbyId ?? gameInfo.lobbyId}/users/${userId ?? gameInfo.userId}`, {
        method: "PUT",
        headers
      })
        .then(ok)
        .then(res => res.json())
        .then((res: Api.Response.UserPut) => res),

    voteDelete: (force?: boolean, lobbyId?: string, userId?: string) =>
      fetch(baseUrl + `/lobbies/${lobbyId ?? gameInfo.lobbyId}/votes/${userId ?? gameInfo.userId}`, {
        method: "DELETE",
        body: JSON.stringify({ force }),
        headers: { ...headers, ...jsonHeader }
      })
        .then(okOrNotFound)
        .then(() => {}),

    voteGetAll: (lobbyId?: string) =>
      fetch(baseUrl + `/lobbies/${lobbyId ?? gameInfo.lobbyId}/votes`, { method: "GET", headers })
        .then(ok)
        .then(res => res.json())
        .then((res: Api.Response.VoteGetAll) => res),

    voteGet: (lobbyId?: string, userId?: string) =>
      fetch(baseUrl + `/lobbies/${lobbyId ?? gameInfo.lobbyId}/votes/${userId ?? gameInfo.userId}`, {
        method: "GET",
        headers
      })
        .then(ok)
        .then(res => res.json())
        .then((res: Api.Response.VoteGet) => res),

    votePut: (choice: string, force?: boolean, lobbyId?: string, userId?: string) =>
      fetch(baseUrl + `/lobbies/${lobbyId ?? gameInfo.lobbyId}/votes/${userId ?? gameInfo.userId}`, {
        method: "PUT",
        body: JSON.stringify({ choice, force }),
        headers: { ...headers, ...jsonHeader }
      })
        .then(ok)
        .then(res => res.json())
        .then((res: Api.Response.VotePut) => res),

    boardReset: (lobbyId?: string, userId?: string) =>
      fetch(baseUrl + `/lobbies/${lobbyId ?? gameInfo.lobbyId}/boards/${userId ?? gameInfo.userId}/reset`, {
        method: "POST",
        headers
      })
        .then(ok)
        .then(() => {}),

    boardSkip: (lobbyId?: string, userId?: string) =>
      fetch(baseUrl + `/lobbies/${lobbyId ?? gameInfo.lobbyId}/boards/${userId ?? gameInfo.userId}/skip`, {
        method: "POST",
        headers
      })
        .then(ok)
        .then(() => {}),

    boardSolution: (gameState: Uint8Array, lobbyId?: string, userId?: string) =>
      fetch(baseUrl + `/lobbies/${lobbyId ?? gameInfo.lobbyId}/boards/${userId ?? gameInfo.userId}/solution`, {
        method: "POST",
        body: JSON.stringify({ gameState: new TextDecoder("utf-8").decode(gameState) }),
        headers: { ...headers, ...jsonHeader }
      })
        .then(ok)
        .then(() => {})
  }
}
