import React, { createContext, ReactNode, useContext, useEffect, useState } from "react"
import { Api } from "../types/Api"
import { useOrigin } from "./useOrigin"
import { Loading } from "../components/Loading"

type TApiContext = {
  hasToken: boolean
  setToken: (token: string) => void
  api: ReturnType<typeof getApi>["api"]
}

const ApiContext = createContext<TApiContext>({ hasToken: false, setToken: () => {}, api: getApi(null, null).api })
export const useApi = () => useContext(ApiContext)

export function ApiProvider(props: { children?: ReactNode }) {
  const { origin } = useOrigin()

  const [token, setToken] = useState<string | null>(null)
  const [api, setApi] = useState<ReturnType<typeof getApi> | null>(null)

  useEffect(() => {
    const baseUrl = origin === "browser" ? process.env.PUBLIC_ENV__API_BASE_URL : "/api"
    const api = getApi(baseUrl, token)

    setApi(api)
  }, [origin, token])

  if (!api) return <Loading />

  return (
    <ApiContext.Provider value={{ hasToken: api.hasToken, setToken, api: api.api }}>
      {props.children}
    </ApiContext.Provider>
  )
}

const accept =
  (...statusCodes: number[]) =>
  (res: Response) => {
    if (!statusCodes.includes(res.status)) throw new Error(res.status.toString())
    return res
  }

const result =
  (json: boolean = false) =>
  async <T extends object>(res: Response) =>
    [(json ? await res.json() : {}) as T, res.status] as const

function getApi(baseUrl: string | null, token: string | null) {
  return {
    hasToken: token !== null,
    api: {
      lobbyGet: (lobbyId: string) =>
        fetch(baseUrl + `/lobbies/${lobbyId}`, {
          method: "GET",
          headers: { Authorization: `Bearer ${token}` }
        })
          .then(accept(200))
          .then(res => result(true)<Api.Response.LobbyGet>(res)),

      lobbyPatch: (lobbyId: string, settings: Api.Request.LobbyPatch) =>
        fetch(baseUrl + `/lobbies/${lobbyId}`, {
          method: "PATCH",
          body: JSON.stringify(settings),
          headers: { Authorization: `Bearer ${token}`, "Content-Type": "application/json" }
        })
          .then(accept(200))
          .then(res => result(true)<Api.Response.LobbyPatch>(res)),

      lobbyPut: (lobbyId: string) =>
        fetch(baseUrl + `/lobbies/${lobbyId}`, {
          method: "PUT",
          headers: { Authorization: `Bearer ${token}` }
        })
          .then(accept(200, 201))
          .then(res => result(true)<Api.Response.LobbyPut>(res)),

      lobbyPost: () =>
        fetch(baseUrl + "/lobbies", {
          method: "POST",
          headers: { Authorization: `Bearer ${token}` }
        })
          .then(accept(201))
          .then(res => result(true)<Api.Response.LobbyPost>(res)),

      negotiate: async (userId: string) =>
        await fetch(baseUrl + `/negotiate?userId=${userId}`, {
          method: "POST",
          headers: { Authorization: `Bearer ${token}` }
        })
          .then(accept(200))
          .then(res => result(true)<Api.Response.Negotiate>(res)),

      token: async (code: string, mocked: boolean) =>
        fetch(baseUrl + "/token", {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({ code, mocked })
        })
          .then(accept(200))
          .then(res => result(true)<Api.Response.Token>(res)),

      login: async (accessToken: string, mocked: boolean) =>
        fetch(baseUrl + "/login", {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({ accessToken, mocked })
        })
          .then(accept(200))
          .then(res => result(true)<Api.Response.Login>(res)),

      lobbyUserDelete: (lobbyId: string, userId: string) =>
        fetch(baseUrl + `/lobbies/${lobbyId}/users/${userId}`, {
          method: "DELETE",
          headers: { Authorization: `Bearer ${token}` }
        })
          .then(accept(204, 404))
          .then(result()),

      lobbyUserGet: (lobbyId: string, userId: string) =>
        fetch(baseUrl + `/lobbies/${lobbyId}/users/${userId}`, {
          method: "PUT",
          headers: { Authorization: `Bearer ${token}` }
        })
          .then(accept(200))
          .then(res => result(true)<Api.Response.LobbyUserGet>(res)),

      lobbyUserPut: (lobbyId: string, userId: string) =>
        fetch(baseUrl + `/lobbies/${lobbyId}/users/${userId}`, {
          method: "PUT",
          headers: { Authorization: `Bearer ${token}` }
        })
          .then(accept(200, 201))
          .then(res => result(true)<Api.Response.LobbyUserPut>(res)),

      boardGet: (lobbyId: string, userId: string) =>
        fetch(baseUrl + `/lobbies/${lobbyId}/boards/${userId}`, {
          method: "GET",
          headers: { Authorization: `Bearer ${token}` }
        })
          .then(accept(200))
          .then(res => result(true)<Api.Response.BoardGet>(res)),

      boardGetAll: (lobbyId: string) =>
        fetch(baseUrl + `/lobbies/${lobbyId}/boards`, {
          method: "GET",
          headers: { Authorization: `Bearer ${token}` }
        })
          .then(accept(200))
          .then(res => result(true)<Api.Response.BoardGet>(res)),

      boardReset: (lobbyId: string, userId: string) =>
        fetch(baseUrl + `/lobbies/${lobbyId}/boards/${userId}/reset`, {
          method: "POST",
          headers: { Authorization: `Bearer ${token}` }
        })
          .then(accept(204))
          .then(result()),

      boardSkip: (lobbyId: string, userId: string) =>
        fetch(baseUrl + `/lobbies/${lobbyId}/boards/${userId}/skip`, {
          method: "POST",
          headers: { Authorization: `Bearer ${token}` }
        })
          .then(accept(204))
          .then(result()),

      boardSolution: (lobbyId: string, userId: string, gameState: Uint8Array) =>
        fetch(baseUrl + `/lobbies/${lobbyId}/boards/${userId}/solution`, {
          method: "POST",
          body: JSON.stringify({ gameState: new TextDecoder("utf-8").decode(gameState) }),
          headers: { Authorization: `Bearer ${token}`, "Content-Type": "application/json" }
        })
          .then(accept(204))
          .then(result()),

      lobbyLock: (lobbyId: string) =>
        fetch(baseUrl + `/lobbies/${lobbyId}/lock`, {
          method: "POST",
          headers: { Authorization: `Bearer ${token}` }
        })
          .then(accept(202))
          .then(result()),

      lobbyUnlock: (lobbyId: string) =>
        fetch(baseUrl + `/lobbies/${lobbyId}/unlock`, {
          method: "POST",
          headers: { Authorization: `Bearer ${token}` }
        })
          .then(accept(202))
          .then(result()),

      lobbyConfirm: (lobbyId: string) =>
        fetch(baseUrl + `/lobbies/${lobbyId}/confirm`, {
          method: "POST",
          headers: { Authorization: `Bearer ${token}` }
        })
          .then(accept(202))
          .then(result())
    }
  }
}
