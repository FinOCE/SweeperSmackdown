import { useGameInfo } from "../hooks/useGameInfo"
import { useWebsocket } from "../hooks/useWebsocket"
import { Websocket } from "../types/Websocket"
import "./GameConfigure.scss"

export function GameConfigure() {
  const { lobbyId } = useGameInfo()
  const ws = useWebsocket()

  ws.register("group-message", e => {
    const data = e.message.data as Websocket.Response.UserJoin
    console.log(data)
  })

  return <div>Welcome to lobby {lobbyId}</div>
}
