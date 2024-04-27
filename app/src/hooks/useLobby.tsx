import React, { createContext, ReactNode, useContext, useState } from "react"
import { Api } from "../types/Api"
import { useApi } from "./useApi"
import { LobbyWithoutNested } from "../types/Lobby"
import { useEmbeddedAppSdk } from "./useEmbeddAppSdk"

type TLobbyContext = {
  lobby: LobbyWithoutNested | null
  setLobby: (lobby: LobbyWithoutNested) => void
  create(lobbyId: string): Promise<void>
  join(lobbyId: string): Promise<void>
  leave(): Promise<void>
  settings: Api.GameSettings | null
  setSettings: (settings: Api.GameSettings) => void
  scores: Record<string, number>
  setScores: (scores: Record<string, number>) => void
  wins: Record<string, number>
  setWins: (wins: Record<string, number>) => void
}

const LobbyContext = createContext<TLobbyContext>({
  lobby: null,
  setLobby: () => {},
  create: async () => {},
  join: async () => {},
  leave: async () => {},
  settings: null,
  setSettings: () => {},
  scores: {},
  setScores: () => {},
  wins: {},
  setWins: () => {}
})
export const useLobby = () => useContext(LobbyContext)

export function LobbyProvider(props: { children?: ReactNode }) {
  const { api } = useApi()
  const { user } = useEmbeddedAppSdk()

  const [lobby, setLobby] = useState<LobbyWithoutNested | null>(null)
  const [settings, setSettings] = useState<Api.GameSettings | null>(null)
  const [scores, setScores] = useState<Record<string, number>>({})
  const [wins, setWins] = useState<Record<string, number>>({})

  async function create(lobbyId: string) {
    if (!user) throw new Error("Cannot create lobby because user is not set")

    await api.lobbyGet(lobbyId).catch(err => {
      if (err === "403") throw new Error("Lobby already exists")
      // if 404, proceed (this is good)
    })

    const [lobby] = await api.lobbyPut(lobbyId)
    await api.userPut(lobbyId, user.id)

    setLobby({ ...lobby, settings: undefined, scores: undefined, wins: undefined } as LobbyWithoutNested)
    setSettings(lobby.settings)
    setScores(lobby.scores)
    setWins(lobby.wins)
  }

  async function join(lobbyId: string) {
    if (!user) throw new Error("Cannot join lobby because user is not set")

    const addedUser = await api.userPut(lobbyId, user.id).catch(() => {})
    if (!addedUser) throw new Error("Failed to join lobby")

    const [lobby] = await api.lobbyGet(lobbyId)

    setLobby({ ...lobby, settings: undefined, scores: undefined, wins: undefined } as LobbyWithoutNested)
    setSettings(lobby.settings)
    setScores(lobby.scores)
    setWins(lobby.wins)
  }

  async function leave() {
    if (!lobby || !user) throw new Error("Cannot leave lobby because user or lobby is not set")

    await api.userDelete(lobby.lobbyId, user.id)

    setLobby(null)
    setSettings(null)
  }

  return (
    <LobbyContext.Provider
      value={{
        lobby,
        setLobby,
        create,
        join,
        leave,
        settings,
        setSettings,
        scores,
        setScores,
        wins,
        setWins
      }}
    >
      {props.children}
    </LobbyContext.Provider>
  )
}
