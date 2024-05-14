import React, { createContext, useContext, useState } from "react"
import { useApi } from "../useApi"
import { useWebsocket } from "../useWebsocket"

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

  const [lobby, setLobby] = useState<TLobby | null>(null)

  async function createLobby(lobbyId?: string) {
    // TODO
  }

  async function joinLobby(lobbyId: string) {
    // TODO
  }

  async function leaveLobby() {
    // TODO
  }

  return (
    <LobbyContext.Provider value={{ lobby, createLobby, joinLobby, leaveLobby }}>
      {props.children}
    </LobbyContext.Provider>
  )
}
