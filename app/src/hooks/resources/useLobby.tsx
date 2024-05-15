import React, { createContext, useContext, useEffect, useState } from "react"
import { useApi } from "../useApi"
import { useWebsocket } from "../useWebsocket"
import { useEmbeddedAppSdk } from "../useEmbeddAppSdk"
import { useLobbyData } from "../data/useLobbyData"

type TLobby = {
  id: string
  hostId: string
}

type TLobbyContext = {
  lobby: TLobby | null
  createLobby: (lobbyId?: string) => Promise<void>
  joinLobby: (lobbyId: string) => Promise<void>
  leaveLobby: () => Promise<void>
}

const LobbyContext = createContext<TLobbyContext>({
  lobby: null,
  async createLobby(lobbyId) {},
  async joinLobby(lobbyId) {},
  async leaveLobby() {}
})
export const useLobby = () => useContext(LobbyContext)

export function LobbyProvider(props: { children?: React.ReactNode }) {
  const { api } = useApi()
  const { ws } = useWebsocket()
  const { user } = useEmbeddedAppSdk()
  const { lobbyData, setLobbyData } = useLobbyData()

  const [lobby, setLobby] = useState<TLobby | null>(null)

  useEffect(() => {
    setLobby(lobbyData ? { id: lobbyData.lobbyId, hostId: lobbyData.hostId } : null)
  }, [lobbyData])

  async function createLobby(lobbyId?: string) {
    if (!user) throw new Error("No user")
    if (!lobbyId) throw new Error("Cannot create lobby without an ID") // TODO

    const [lobbyPutError, lobby] = await api
      .lobbyPut(lobbyId)
      .then(([data, status]) => {
        if (status === 200) throw "200"
        else return data
      })
      .then(data => [null, data] as const)
      .catch((err: Error) => [err, null] as const)

    if (lobbyPutError) throw new Error("Lobby already exists")

    const [userPutError] = await api
      .userPut(lobbyId, user.id)
      .then(([data]) => [null, data] as const)
      .catch((err: Error) => [err, null] as const)

    if (userPutError) throw new Error("Failed to join lobby")

    setLobbyData(lobby)
  }

  async function joinLobby(lobbyId: string) {
    if (!user) throw new Error("No user")

    const [userPutError] = await api
      .userPut(lobbyId, user.id)
      .then(([data]) => [null, data] as const)
      .catch((err: Error) => [err, null] as const)

    if (userPutError) throw new Error("Failed to join lobby")

    const [lobbyGetError, lobby] = await api
      .lobbyGet(lobbyId)
      .then(([data]) => [null, data] as const)
      .catch((err: Error) => [err, null] as const)

    if (lobbyGetError) throw new Error("Failed to find lobby")

    setLobbyData(lobby)
  }

  async function leaveLobby() {
    if (!user) throw new Error("No user")
    if (!lobby) throw new Error("No lobby")

    const [userDeleteError] = await api
      .userDelete(lobby.id, user.id)
      .then(([data]) => [null, data] as const)
      .catch((err: Error) => [err, null] as const)

    if (userDeleteError) throw new Error("Failed to leave lobby")

    setLobbyData(null)
  }

  return (
    <LobbyContext.Provider value={{ lobby, createLobby, joinLobby, leaveLobby }}>
      {props.children}
    </LobbyContext.Provider>
  )
}
