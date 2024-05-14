import React, { createContext, SetStateAction, useContext, useEffect, useState } from "react"
import { useWebsocket } from "../useWebsocket"
import { useEmbeddedAppSdk } from "../useEmbeddAppSdk"
import { isEvent } from "../../utils/isEvent"
import { Websocket } from "../../types/Websocket"
import { OnGroupDataMessageArgs } from "@azure/web-pubsub-client"
import { Api } from "../../types/Api"

type TLobbyDataContext = {
  lobbyData: Api.Lobby | null
  setLobbyData: (value: SetStateAction<Api.Lobby | null>) => void
}

const LobbyDataContext = createContext<TLobbyDataContext>({
  lobbyData: null,
  setLobbyData: () => {}
})
export const useLobbyData = () => useContext(LobbyDataContext)

export function LobbyDataProvider(props: { children?: React.ReactNode }) {
  const { ws } = useWebsocket()
  const { user } = useEmbeddedAppSdk()

  const [lobbyData, setLobbyData] = useState<Api.Lobby | null>(null)

  useEffect(() => {
    if (!ws) return

    function onLobbyUpdate(e: OnGroupDataMessageArgs) {
      if (!lobbyData) return

      const data = e.message.data as Websocket.Message
      if (!isEvent<Websocket.Response.LobbyUpdate>("LOBBY_UPDATE", data)) return

      setLobbyData(data.data)
    }

    function onUserLeave(e: OnGroupDataMessageArgs) {
      if (!lobbyData || !user) return

      const data = e.message.data as Websocket.Message
      if (!isEvent<Websocket.Response.UserLeave>("USER_LEAVE", data)) return

      if (data.userId === user.id) setLobbyData(null)
    }

    ws.on("group-message", onLobbyUpdate)
    ws.on("group-message", onUserLeave)

    return () => {
      ws.off("group-message", onLobbyUpdate)
      ws.off("group-message", onUserLeave)
    }
  }, [ws])

  return <LobbyDataContext.Provider value={{ lobbyData, setLobbyData }}>{props.children}</LobbyDataContext.Provider>
}
