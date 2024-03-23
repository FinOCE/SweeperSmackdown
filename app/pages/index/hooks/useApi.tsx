import { Api } from "../types/Api"
import { useGameInfo } from "./useGameInfo"

export function useApi() {
  const gameInfo = useGameInfo()
  const baseUrl = import.meta.env.PUBLIC_ENV__API_BASE_URL

  return {
    negotiate: () =>
      fetch(baseUrl + `/negotiate?userId=${gameInfo.userId}`, { method: "POST" })
        .then(res => res.json())
        .then((res: Api.Response.Negotiate) => res),

    lobbyPut: () =>
      fetch(baseUrl + `/lobbies/${gameInfo.lobbyId}`, { method: "PUT" })
        .then(res => res.json())
        .then((res: Api.Response.LobbyPut) => res),

    userPut: () =>
      fetch(baseUrl + `/lobbies/${gameInfo.lobbyId}/users/${gameInfo.userId}`, { method: "PUT" })
        .then(res => res.json())
        .then((res: Api.Response.UserPut) => res)
  }
}
