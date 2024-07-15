import React, { createContext, useContext, useEffect, useState } from "react"
import { useApi } from "../useApi"
import { useEmbeddedAppSdk } from "../useEmbeddAppSdk"
import { useWebsocket } from "../useWebsocket"
import { OnGroupDataMessageArgs } from "@azure/web-pubsub-client"
import { isEvent } from "../../utils/isEvent"
import { Websocket } from "../../types/Websocket"
import { Api } from "../../types/Api"

type TLobbyContext = {
  lobby:
    | {
        resolved: true
        id: string
        hostId: string
        hostManaged: boolean
        settings: Api.GameSettings
        status: Api.PreciseLobbyStatus
        players: Api.Player[]
      }
    | {
        resolved: false
        id: string | null
        hostId: string | null
        hostManaged: boolean | null
        settings: Api.GameSettings | null
        status: Api.PreciseLobbyStatus | null
        players: Api.Player[]
      }
  controls: {
    createOrJoin(lobbyId: string): Promise<void>
    create(lobbyId?: string): Promise<void>
    join(lobbyId: string): Promise<void>
    leave(): Promise<void>
    lock(): Promise<void>
    unlock(): Promise<void>
    confirm(): Promise<void>
    updateLobby(settings: Api.Request.LobbyPatch): Promise<void>
    updateSettings(settings: Api.Request.GameSettingsPatch): Promise<void>
  }
}

const LobbyContext = createContext<TLobbyContext>({
  lobby: { resolved: false, id: null, hostId: null, hostManaged: null, settings: null, status: null, players: [] },
  controls: {
    async createOrJoin() {},
    async create() {},
    async join() {},
    async leave() {},
    async lock() {},
    async unlock() {},
    async confirm() {},
    async updateLobby() {},
    async updateSettings() {}
  }
})
export const useLobby = () => useContext(LobbyContext)

export function LobbyProvider(props: { children?: React.ReactNode }) {
  const { api } = useApi()
  const { user } = useEmbeddedAppSdk()
  const { ws } = useWebsocket()

  const [lobbyId, setLobbyId] = useState<string | null>(null)
  const [hostId, setHostId] = useState<string | null>(null)
  const [hostManaged, setHostManaged] = useState<boolean | null>(null)
  const [settings, setSettings] = useState<Api.GameSettings | null>(null)
  const [status, setStatus] = useState<Api.PreciseLobbyStatus | null>(null)
  const [players, setPlayers] = useState<Api.Player[]>([])

  // Setup websocket events
  useEffect(() => {
    if (!ws) return

    function onCreatedLobby(e: OnGroupDataMessageArgs) {
      const data = e.message.data as Websocket.Message
      if (!isEvent<Websocket.Response.CreatedLobby>("LOBBY_CREATED", data)) return

      setLobbyId(data.data)
    }

    function onUpdateLobbyStatus(e: OnGroupDataMessageArgs) {
      const data = e.message.data as Websocket.Message
      if (!isEvent<Websocket.Response.UpdateLobbyStatus>("LOBBY_STATUS_UPDATE", data)) return

      setStatus(data.data)
    }

    function onJoinLobby(e: OnGroupDataMessageArgs) {
      const data = e.message.data as Websocket.Message
      if (!isEvent<Websocket.Response.JoinLobby>("LOBBY_JOIN", data)) return

      setLobbyId(data.data)
    }

    function onLeaveLobby(e: OnGroupDataMessageArgs) {
      const data = e.message.data as Websocket.Message
      if (!isEvent<Websocket.Response.LeaveLobby>("LOBBY_LEAVE", data)) return

      setLobbyId(null)
    }

    function onAddPlayer(e: OnGroupDataMessageArgs) {
      const data = e.message.data as Websocket.Message
      if (!isEvent<Websocket.Response.AddPlayer>("PLAYER_ADD", data)) return

      setPlayers(players => players.filter(p => p.id !== data.data.id).concat([data.data]))
    }

    function onRemovePlayer(e: OnGroupDataMessageArgs) {
      const data = e.message.data as Websocket.Message
      if (!isEvent<Websocket.Response.RemovePlayer>("PLAYER_REMOVE", data)) return

      setPlayers(players => players.map(p => (p.id === data.userId ? { ...p, active: false } : p)))
    }

    function onUpdatePlayer(e: OnGroupDataMessageArgs) {
      const data = e.message.data as Websocket.Message
      if (!isEvent<Websocket.Response.UpdatePlayer>("PLAYER_UPDATE", data)) return

      setPlayers(players => players.filter(p => p.id === data.data.id).concat([data.data]))
    }

    function onUpdateLobbySettings(e: OnGroupDataMessageArgs) {
      const data = e.message.data as Websocket.Message
      if (!isEvent<Websocket.Response.UpdateLobbySettings>("LOBBY_UPDATE_SETTINGS", data)) return

      setSettings(data.data)
    }

    function onUpdateLobbySettingsFailed(e: OnGroupDataMessageArgs) {
      const data = e.message.data as Websocket.Message
      if (!isEvent<Websocket.Response.UpdateLobbySettings>("LOBBY_UPDATE_SETTINGS", data)) return

      setSettings(data.data)
    }

    function onUpdateConfigureState(e: OnGroupDataMessageArgs) {
      const data = e.message.data as Websocket.Message
      if (!isEvent<Websocket.Response.UpdateConfigureState>("LOBBY_STATE_UPDATE", data)) return

      setStatus(s => ({ ...s!, configureState: data.data }))
    }

    function onUpdateConfigureStateFailed(e: OnGroupDataMessageArgs) {
      const data = e.message.data as Websocket.Message
      if (!isEvent<Websocket.Response.UpdateConfigureStateFailed>("LOBBY_STATE_UPDATE_FAILED", data)) return

      setStatus(s => ({ ...s!, configureState: data.data }))
    }

    function onUpdateLobbyHost(e: OnGroupDataMessageArgs) {
      const data = e.message.data as Websocket.Message
      if (!isEvent<Websocket.Response.UpdateLobbyHost>("LOBBY_HOST_UPDATE", data)) return

      setHostId(data.data)
    }

    function onUpdateLobbyHostManaged(e: OnGroupDataMessageArgs) {
      const data = e.message.data as Websocket.Message
      if (!isEvent<Websocket.Response.UpdateLobbyHostManaged>("LOBBY_HOST_MANAGED_UPDATE", data)) return

      setHostManaged(data.data)
    }

    function handler(e: OnGroupDataMessageArgs) {
      ;[
        onCreatedLobby,
        onUpdateLobbyStatus,
        onJoinLobby,
        onLeaveLobby,
        onAddPlayer,
        onRemovePlayer,
        onUpdatePlayer,
        onUpdateLobbySettings,
        onUpdateLobbySettingsFailed,
        onUpdateConfigureState,
        onUpdateConfigureStateFailed,
        onUpdateLobbyHost,
        onUpdateLobbyHostManaged
      ].forEach(h => h(e))
    }

    ws.on("group-message", handler)
    return () => ws.off("group-message", handler)
  }, [api, user, ws])

  // Fetch lobby when ID is set and clear when nulled
  const [initialising, setInitialising] = useState(false)

  async function initialiseLobby(lobbyId: string) {
    if (initialising) return
    setInitialising(true)

    const [err, lobby] = await api
      .lobbyGet(lobbyId)
      .then(([data]) => [null, data] as const)
      .catch((err: Error) => [err, null] as const)

    if (err) throw err

    setHostId(lobby.hostId)
    setHostManaged(lobby.hostManaged)
    setSettings(lobby.settings)
    setStatus(lobby.status)
    setPlayers(lobby.players)

    setInitialising(false)
  }

  function clearLobby() {
    setLobbyId(null)
    setHostId(null)
    setHostManaged(null)
    setSettings(null)
    setStatus(null)
    setPlayers([])
  }

  useEffect(() => {
    lobbyId ? initialiseLobby(lobbyId) : clearLobby()
  }, [lobbyId])

  // Update resolved value if all data present
  const [resolved, setResolved] = useState(false)

  useEffect(() => {
    const conditions = [
      lobbyId !== null,
      hostId !== null,
      hostManaged !== null,
      settings !== null,
      status !== null,
      players.length > 0
    ]

    setResolved(conditions.every(c => c))
  }, [lobbyId, hostId, hostManaged, settings, status, players])

  // Create lobby-related methods to interact with API
  async function createOrJoin(lobbyId: string) {
    // TODO: Implement
  }

  async function create(lobbyId?: string) {
    if (!user) throw new Error("No user")

    if (lobbyId) {
      const [lobbyPutError] = await api
        .lobbyPut(lobbyId)
        .then(([_, status]) => {
          if (status === 200) throw "200"
        })
        .then(() => [null] as const)
        .catch((err: Error) => [err] as const)

      if (lobbyPutError) throw new Error("Lobby already exists")
    } else {
      const [lobbyPostError] = await api
        .lobbyPost()
        .then(() => [null] as const)
        .catch((err: Error) => [err] as const)

      if (lobbyPostError) throw new Error("Failed to create lobby")
    }
  }

  async function join(lobbyId: string) {
    if (!user) throw new Error("No user")

    const [userPutError] = await api
      .lobbyUserPut(lobbyId, user.id)
      .then(() => [null] as const)
      .catch((err: Error) => [err] as const)

    if (userPutError) throw new Error("Failed to join lobby")
  }

  async function leave() {
    if (!user) throw new Error("No user")
    if (!lobbyId) throw new Error("No lobby")

    const [userDeleteError] = await api
      .lobbyUserDelete(lobbyId, user.id)
      .then(() => [null] as const)
      .catch((err: Error) => [err] as const)

    if (userDeleteError) throw new Error("Failed to leave lobby")
  }

  async function lock() {
    if (!user) throw new Error("No user")
    if (!lobbyId) throw new Error("No lobby")

    const [lockError] = await api
      .lobbyLock(lobbyId)
      .then(() => [null] as const)
      .catch((err: Error) => [err] as const)

    if (lockError) throw new Error("Failed to lock lobby")
  }

  async function unlock() {
    if (!user) throw new Error("No user")
    if (!lobbyId) throw new Error("No lobby")

    const [unlockError] = await api
      .lobbyUnlock(lobbyId)
      .then(() => [null] as const)
      .catch((err: Error) => [err] as const)

    if (unlockError) throw new Error("Failed to unlock lobby")
  }

  async function confirm() {
    if (!user) throw new Error("No user")
    if (!lobbyId) throw new Error("No lobby")

    const [confirmError] = await api
      .lobbyConfirm(lobbyId)
      .then(() => [null] as const)
      .catch((err: Error) => [err] as const)

    if (confirmError) throw new Error("Failed to confirm lobby")
  }

  async function updateLobby(settings: Api.Request.LobbyPatch) {
    if (!user) throw new Error("No user")
    if (!lobbyId) throw new Error("No lobby")

    const [lobbyPatchError] = await api
      .lobbyPatch(lobbyId, settings)
      .then(() => [null] as const)
      .catch((err: Error) => [err] as const)

    if (lobbyPatchError) throw new Error("Failed to update lobby")
  }

  async function updateSettings(settings: Api.Request.GameSettingsPatch) {
    if (!user) throw new Error("No user")
    if (!lobbyId) throw new Error("No lobby")

    const [lobbySettingsPatchError] = await api
      .lobbySettingsPatch(lobbyId, settings)
      .then(() => [null] as const)
      .catch((err: Error) => [err] as const)

    if (lobbySettingsPatchError) throw new Error("Failed to update settings")
  }

  // Return provider
  return (
    <LobbyContext.Provider
      value={{
        lobby: resolved
          ? {
              resolved,
              id: lobbyId!,
              hostId: hostId!,
              hostManaged: hostManaged!,
              settings: settings!,
              status: status!,
              players
            }
          : {
              resolved,
              id: lobbyId,
              hostId,
              hostManaged,
              settings,
              status,
              players
            },
        controls: {
          createOrJoin,
          create,
          join,
          leave,
          lock,
          unlock,
          confirm,
          updateLobby,
          updateSettings
        }
      }}
    >
      {props.children}
    </LobbyContext.Provider>
  )
}
