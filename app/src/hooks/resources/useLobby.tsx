import React, { createContext, useContext, useEffect, useState } from "react"
import { useApi } from "../useApi"
import { useEmbeddedAppSdk } from "../useEmbeddAppSdk"
import { useLobbyData } from "../data/useLobbyData"
import { Api } from "../../types/Api"

type TLobby = {
  id: string
  hostId: string
  state: Api.Enums.ELobbyState
}

type TLobbyContext = {
  lobby: TLobby | null
  createLobby: (lobbyId?: string) => Promise<void>
  joinLobby: (lobbyId: string) => Promise<void>
  leaveLobby: () => Promise<void>
  lockLobby: () => Promise<void>
  unlockLobby: () => Promise<void>
  confirmLobby: () => Promise<void>
}

const LobbyContext = createContext<TLobbyContext>({
  lobby: null,
  async createLobby(lobbyId) {},
  async joinLobby(lobbyId) {},
  async leaveLobby() {},
  async lockLobby() {},
  async unlockLobby() {},
  async confirmLobby() {}
})
export const useLobby = () => useContext(LobbyContext)

export function LobbyProvider(props: { children?: React.ReactNode }) {
  const { api } = useApi()
  const { user } = useEmbeddedAppSdk()
  const { lobbyData, setLobbyData } = useLobbyData()

  const [lobby, setLobby] = useState<TLobby | null>(null)

  useEffect(() => {
    setLobby(lobbyData ? { id: lobbyData.lobbyId, hostId: lobbyData.hostId, state: lobbyData.state } : null)
  }, [lobbyData])

  async function createLobby(lobbyId?: string) {
    if (!user) throw new Error("No user")

    let lobby: Api.Lobby
    if (lobbyId) {
      const [lobbyPutError, lobbyRes] = await api
        .lobbyPut(lobbyId)
        .then(([data, status]) => {
          if (status === 200) throw "200"
          else return data
        })
        .then(data => [null, data] as const)
        .catch((err: Error) => [err, null] as const)

      if (lobbyPutError) throw new Error("Lobby already exists")
      lobby = lobbyRes
    } else {
      const [lobbyPutError, lobbyRes] = await api
        .lobbyPost()
        .then(([data]) => [null, data] as const)
        .catch((err: Error) => [err, null] as const)

      if (lobbyPutError) throw new Error("Failed to create lobby")
      lobby = lobbyRes
    }

    const [userPutError] = await api
      .lobbyUserPut(lobby.lobbyId, user.id)
      .then(([data]) => [null, data] as const)
      .catch((err: Error) => [err, null] as const)

    if (userPutError) throw new Error("Failed to join lobby")

    setLobbyData(lobby)
  }

  async function joinLobby(lobbyId: string) {
    if (!user) throw new Error("No user")

    const [userPutError] = await api
      .lobbyUserPut(lobbyId, user.id)
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
      .lobbyUserDelete(lobby.id, user.id)
      .then(([data]) => [null, data] as const)
      .catch((err: Error) => [err, null] as const)

    if (userDeleteError) throw new Error("Failed to leave lobby")

    setLobbyData(null)
  }

  async function lockLobby() {
    if (!lobby) throw new Error("No lobby")

    const [err] = await api
      .lobbyLock(lobby.id)
      .then(([data]) => [null, data] as const)
      .catch((err: Error) => [err, null] as const)

    if (err) throw new Error("Failed to lock lobby")

    setLobbyData(d => ({ ...d!, state: Api.Enums.ELobbyState.ConfigureLocked }))
  }

  async function unlockLobby() {
    if (!lobby) throw new Error("No lobby")

    const [err] = await api
      .lobbyUnlock(lobby.id)
      .then(([data]) => [null, data] as const)
      .catch((err: Error) => [err, null] as const)

    if (err) throw new Error("Failed to unlock lobby")

    setLobbyData(d => ({ ...d!, state: Api.Enums.ELobbyState.ConfigureUnlocked }))
  }

  async function confirmLobby() {
    if (!lobby) throw new Error("No lobby")

    const [err] = await api
      .lobbyConfirm(lobby.id)
      .then(([data]) => [null, data] as const)
      .catch((err: Error) => [err, null] as const)

    if (err) throw new Error("Failed to confirm lobby")

    // TODO: Optimistically update to confirmation lobby state once branch with that change is merged in
    //       Alternatively, just handle on frontend by forcing into some new state without all the buttons or something
  }

  return (
    <LobbyContext.Provider value={{ lobby, createLobby, joinLobby, leaveLobby, lockLobby, unlockLobby, confirmLobby }}>
      {props.children}
    </LobbyContext.Provider>
  )
}
