import React, { createContext, ReactNode, useContext, useState } from "react"
import { Api } from "../types/Api"

const baseUrl = process.env.PUBLIC_ENV__API_BASE_URL

const ApiContext = createContext<{
  setToken: (token: string) => void
  api: ReturnType<typeof getApi>
}>({
  setToken: () => {},
  api: getApi(baseUrl, null)
})
export const useApi = () => useContext(ApiContext)

export function ApiProvider(props: { children?: ReactNode }) {
  const [token, setToken] = useState<string | null>(null)

  return <ApiContext.Provider value={{ setToken, api: getApi(baseUrl, token) }}>{props.children}</ApiContext.Provider>
}

function accept(...statusCodes: number[]) {
  return (res: Response) => {
    if (!statusCodes.includes(res.status)) throw new Error("Invalid status code")
    return res
  }
}

function getApi(baseUrl: string, token: string | null) {
  return {
    lobbyGet: (lobbyId: string) =>
      fetch(baseUrl + `/lobbies/${lobbyId}`, {
        method: "GET",
        headers: { Authorization: `Bearer ${token}` }
      })
        .then(accept(200))
        .then(res => res.json())
        .then((res: Api.Response.LobbyGet) => res),

    lobbyPatch: (lobbyId: string, settings: Api.Request.LobbyPatch) =>
      fetch(baseUrl + `/lobbies/${lobbyId}`, {
        method: "PATCH",
        body: JSON.stringify(settings),
        headers: { Authorization: `Bearer ${token}`, "Content-Type": "application/json" }
      })
        .then(accept(200))
        .then(res => res.json())
        .then((res: Api.Response.LobbyPatch) => res),

    lobbyPut: (lobbyId: string) =>
      fetch(baseUrl + `/lobbies/${lobbyId}`, {
        method: "PUT",
        headers: { Authorization: `Bearer ${token}` }
      })
        .then(accept(200, 201))
        .then(res => res.json())
        .then((res: Api.Response.LobbyPut) => res),

    negotiate: async (userId: string) =>
      await fetch(baseUrl + `/negotiate?userId=${userId}`, {
        method: "POST",
        headers: { Authorization: `Bearer ${token}` }
      })
        .then(accept(200))
        .then(res => res.json())
        .then((res: Api.Response.Negotiate) => res),

    token: async (code: string, mocked: boolean) =>
      fetch(baseUrl + "/token", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ code, mocked })
      })
        .then(accept(200))
        .then(res => res.json())
        .then((res: Api.Response.Token) => res),

    login: async (accessToken: string, mocked: boolean) =>
      fetch(baseUrl + "/login", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ accessToken, mocked })
      })
        .then(accept(200))
        .then(res => res.json())
        .then((res: Api.Response.Login) => res),

    userDelete: (lobbyId: string, userId: string) =>
      fetch(baseUrl + `/lobbies/${lobbyId}/users/${userId}`, {
        method: "DELETE",
        headers: { Authorization: `Bearer ${token}` }
      })
        .then(accept(204, 404))
        .then(() => {}),

    userGet: (lobbyId: string, userId: string) =>
      fetch(baseUrl + `/lobbies/${lobbyId}/users/${userId}`, {
        method: "PUT",
        headers: { Authorization: `Bearer ${token}` }
      })
        .then(accept(200))
        .then(res => res.json())
        .then((res: Api.Response.UserGet) => res),

    userPut: (lobbyId: string, userId: string) =>
      fetch(baseUrl + `/lobbies/${lobbyId}/users/${userId}`, {
        method: "PUT",
        headers: { Authorization: `Bearer ${token}` }
      })
        .then(accept(200, 201))
        .then(res => res.json())
        .then((res: Api.Response.UserPut) => res),

    voteDelete: (lobbyId: string, userId: string, force?: boolean) =>
      fetch(baseUrl + `/lobbies/${lobbyId}/votes/${userId}`, {
        method: "DELETE",
        body: JSON.stringify({ force }),
        headers: { Authorization: `Bearer ${token}`, "Content-Type": "application/json" }
      })
        .then(accept(204, 404))
        .then(() => {}),

    voteGetAll: (lobbyId: string) =>
      fetch(baseUrl + `/lobbies/${lobbyId}/votes`, {
        method: "GET",
        headers: { Authorization: `Bearer ${token}` }
      })
        .then(accept(200))
        .then(res => res.json())
        .then((res: Api.Response.VoteGetAll) => res),

    voteGet: (lobbyId: string, userId: string) =>
      fetch(baseUrl + `/lobbies/${lobbyId}/votes/${userId}`, {
        method: "GET",
        headers: { Authorization: `Bearer ${token}` }
      })
        .then(accept(200))
        .then(res => res.json())
        .then((res: Api.Response.VoteGet) => res),

    votePut: (lobbyId: string, userId: string, choice: string, force?: boolean) =>
      fetch(baseUrl + `/lobbies/${lobbyId}/votes/${userId}`, {
        method: "PUT",
        body: JSON.stringify({ choice, force }),
        headers: { Authorization: `Bearer ${token}`, "Content-Type": "application/json" }
      })
        .then(accept(200, 201))
        .then(res => res.json())
        .then((res: Api.Response.VotePut) => res),

    boardReset: (lobbyId: string, userId: string) =>
      fetch(baseUrl + `/lobbies/${lobbyId}/boards/${userId}/reset`, {
        method: "POST",
        headers: { Authorization: `Bearer ${token}` }
      })
        .then(accept(204))
        .then(() => {}),

    boardSkip: (lobbyId: string, userId: string) =>
      fetch(baseUrl + `/lobbies/${lobbyId}/boards/${userId}/skip`, {
        method: "POST",
        headers: { Authorization: `Bearer ${token}` }
      })
        .then(accept(204))
        .then(() => {}),

    boardSolution: (lobbyId: string, userId: string, gameState: Uint8Array) =>
      fetch(baseUrl + `/lobbies/${lobbyId}/boards/${userId}/solution`, {
        method: "POST",
        body: JSON.stringify({ gameState: new TextDecoder("utf-8").decode(gameState) }),
        headers: { Authorization: `Bearer ${token}`, "Content-Type": "application/json" }
      })
        .then(accept(204))
        .then(() => {})
  }
}
