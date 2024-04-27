import React from "react"
import "./GameCelebration.scss"
import { useWebsocket } from "../hooks/useWebsocket"
import { Loading } from "../components/Loading"
import { Websocket } from "../types/Websocket"
import { isEvent } from "../utils/isEvent"
import { useNavigation } from "../hooks/useNavigation"

export function GameCelebration() {
  const ws = useWebsocket()
  const { navigate } = useNavigation()

  if (!ws) return <Loading hide />

  ws.register("group-message", e => {
    const data = e.message.data as Websocket.Message
    if (!isEvent<Websocket.Response.LobbyStart>("LOBBY_START", data)) return

    navigate("GameConfigure")
  })

  return <div>TODO</div>
}
