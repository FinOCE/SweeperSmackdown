import React, { createContext, ReactNode, useContext, useState } from "react"
import { Api } from "../types/Api"
import { useUser } from "./useUser"
import { useApi } from "./useApi"

type LobbyWithoutSettings = Omit<Api.Lobby, "settings">

type TLobbyContext = {
  lobby: LobbyWithoutSettings | null
  create(lobbyId: string): Promise<void>
  join(lobbyId: string): Promise<void>
  leave(): Promise<void>
  settings: Api.GameSettings | null
  setSettings: (settings: Api.GameSettings) => void
}

const LobbyContext = createContext<TLobbyContext>({
  lobby: null,
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

    const existingLobby = await api.lobbyGet(lobbyId)
    if (existingLobby != null) throw new Error("Lobby already exists")

    const lobby = await api.lobbyPut(lobbyId)
    await api.userPut(lobbyId, user.id)

    const lobbyWithoutSettings = { ...lobby, settings: undefined } as LobbyWithoutSettings
    setLobby(lobbyWithoutSettings)
    setSettings(lobby.settings)
  }

  async function join(lobbyId: string) {
    if (!user) throw new Error("Cannot join lobby because user is not set")

    const lobby = await api.lobbyGet(lobbyId)
    await api.userPut(lobbyId, user.id)

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
    <LobbyContext.Provider value={{ lobby, create, join, leave, settings, setSettings }}>
      {props.children}
    </LobbyContext.Provider>
  )
}
