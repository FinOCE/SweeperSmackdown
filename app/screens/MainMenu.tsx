import { useApi } from "../hooks/useApi"
import { useWebsocket } from "../hooks/useWebsocket"
import { Websocket } from "../types/Websocket"
import "./MainMenu.scss"

export default function MainMenu() {
  const api = useApi()
  const ws = useWebsocket()

  ws.register("connected", () => api.lobbyPut().then(() => api.userPut()))

  ws.register("group-message", e => {
    const data = e.message.data as Websocket.Response.UserJoin
    console.log(data)
  })

  return <div>Hello world</div>
}
