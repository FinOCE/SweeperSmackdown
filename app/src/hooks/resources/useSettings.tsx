import React, { createContext, useContext, useEffect, useState } from "react"
import { useApi } from "../useApi"
import { useWebsocket } from "../useWebsocket"
import { Api } from "../../types/Api"
import { OnGroupDataMessageArgs } from "@azure/web-pubsub-client"
import { Websocket } from "../../types/Websocket"
import { isEvent } from "../../utils/isEvent"
import { useLobby } from "../useLobby"

type TSettingsContext = {
  settings: Api.GameSettings | null
  updateSettings: (settings: Api.Request.LobbyPatch) => Promise<void>
}

const SettingsContext = createContext<TSettingsContext>({ settings: null, async updateSettings(settings) {} })
export const useSettings = () => useContext(SettingsContext)

export function SettingsProvider(props: { children?: React.ReactNode }) {
  const { api } = useApi()
  const { ws } = useWebsocket()
  const { lobby } = useLobby()

  const [settings, setSettings] = useState<Api.GameSettings | null>(null)

  async function updateSettings(settings: Api.Request.LobbyPatch) {
    if (!lobby) throw new Error("No lobby")

    const [err] = await api
      .lobbyPatch(lobby.lobbyId, settings)
      .then(([data]) => [null, data] as const)
      .catch((err: Error) => [err, null] as const)

    if (err) throw new Error("Failed to update settings")
  }

  useEffect(() => {
    if (!lobby) setSettings(null)
  }, [lobby])

  useEffect(() => {
    if (!ws) return

    function onLobbyUpdate(e: OnGroupDataMessageArgs) {
      if (!lobby) return

      const data = e.message.data as Websocket.Message
      if (!isEvent<Websocket.Response.LobbyUpdate>("LOBBY_UPDATE", data)) return

      setSettings(data.data.settings)
    }

    ws.on("group-message", onLobbyUpdate)

    return () => {
      ws.off("group-message", onLobbyUpdate)
    }
  }, [ws])

  return <SettingsContext.Provider value={{ settings, updateSettings }}>{props.children}</SettingsContext.Provider>
}
