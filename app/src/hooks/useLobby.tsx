import React, { createContext, ReactNode, useContext, useState } from "react"
import { Api } from "../types/Api"
import { useUser } from "./useUser"
import { useApi } from "./useApi"
import { LobbyWithoutSettings } from "../types/Lobby"

type TLobbyContext = {
  lobby: LobbyWithoutSettings | null
  setLobby: (lobby: LobbyWithoutSettings) => void
  create(lobbyId: string): Promise<void>
  join(lobbyId: string): Promise<void>
  leave(): Promise<void>
  settings: Api.GameSettings | null
  setSettings: (settings: Api.GameSettings) => void
}

const LobbyContext = createContext<TLobbyContext>({
  lobby: null,
  setLobby: () => {},
  create: async () => {},
  join: async () => {},
  leave: async () => {},
  settings: null,
  setSettings: () => {}
})
export const useLobby = () => useContext(LobbyContext)

export function LobbyProvider(props: { children?: ReactNode }) {
  const { api } = useApi()
  const user = useUser()

  const [lobby, setLobby] = useState<LobbyWithoutSettings | null>(null)
  const [settings, setSettings] = useState<Api.GameSettings | null>(null)

  async function create(lobbyId: string) {
    if (!user) throw new Error("Cannot create lobby because user is not set")

    await api.lobbyGet(lobbyId).catch(err => {
      if (err === "403") throw new Error("Lobby already exists")
      // if 404, proceed (this is good)
    })

    const [lobby] = await api.lobbyPut(lobbyId)
    await api.userPut(lobbyId, user.id)

    setLobby({ ...lobby, settings: undefined } as LobbyWithoutSettings)
    setSettings(lobby.settings)
  }

  async function join(lobbyId: string) {
    if (!user) throw new Error("Cannot join lobby because user is not set")

    const addedUser = await api.userPut(lobbyId, user.id).catch(() => {})
    if (!addedUser) throw new Error("Failed to join lobby")

    const [lobby] = await api.lobbyGet(lobbyId)
    const lobbyWithoutSettings = { ...lobby, settings: undefined } as LobbyWithoutSettings

    setLobby(lobbyWithoutSettings)
    setSettings(lobby.settings)
  }

  async function leave() {
    if (!lobby || !user) throw new Error("Cannot leave lobby because user or lobby is not set")

    await api.userDelete(lobby.lobbyId, user.id)

    setLobby(null)
    setSettings(null)
  }

  return (
    <LobbyContext.Provider value={{ lobby, setLobby, create, join, leave, settings, setSettings }}>
      {props.children}
    </LobbyContext.Provider>
  )
}
