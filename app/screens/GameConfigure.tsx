import { useApi } from "../hooks/useApi"
import { useGameInfo } from "../hooks/useGameInfo"
import { useNavigation } from "../hooks/useNavigation"
import { useWebsocket } from "../hooks/useWebsocket"
import { Websocket } from "../types/Websocket"
import "./GameConfigure.scss"

export function GameConfigure() {
  const { navigate } = useNavigation()
  const { lobbyId, setLobbyId } = useGameInfo()
  const ws = useWebsocket()
  const api = useApi()

  ws.register("group-message", e => {
    const data = e.message.data as Websocket.Response.UserJoin
    console.log(data)
  })

  async function leaveLobby() {
    await api.userDelete().catch(() => {})
    setLobbyId(null)
    navigate("MainMenu")
  }

  return (
    <div>
      <p>Welcome to lobby {lobbyId}</p>
      <input type="button" onClick={leaveLobby} value="Leave Lobby" />
    </div>
  )
}
