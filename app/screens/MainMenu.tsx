import { useState } from "preact/hooks"
import { useNavigation } from "../hooks/useNavigation"
import { useWebsocket } from "../hooks/useWebsocket"
import "./MainMenu.scss"
import { useGameInfo } from "../hooks/useGameInfo"
import { Websocket } from "../types/Websocket"
import { useApi } from "../hooks/useApi"

export function MainMenu() {
  const { navigate } = useNavigation()
  const { userId, setLobbyId } = useGameInfo()
  const ws = useWebsocket()
  const api = useApi()

  // Load until websocket is connected
  const [loading, setLoading] = useState(true)
  ws.register("connected", () => setLoading(false))

  if (loading) return <div>Loading...</div>

  // Choose lobby and handle navigation after joining
  const [localLobbyId, setLocalLobbyId] = useState("")
  const [error, setError] = useState<string>()

  async function joinLobby() {
    const lobby = await api.lobbyGet(localLobbyId).catch(() => setError("Lobby not found"))
    if (!lobby) return

    const user = await api.userPut(lobby.lobbyId).catch(() => setError("Failed to join lobby"))
    if (!user) return

    setLobbyId(user.lobbyId)
  }

  async function createLobby() {
    const existingLlobby = await api.lobbyGet(localLobbyId).catch(() => {})
    if (existingLlobby) return setError("Lobby already exists")

    const lobby = await api.lobbyPut(localLobbyId).catch(() => setError("Failed to create lobby"))
    if (!lobby) return

    const user = await api.userPut(lobby.lobbyId).catch(() => setError("Created lobby but failed to join"))
    if (!user) return

    setLobbyId(user.lobbyId)
  }

  ws.register("group-message", e => {
    const data = e.message.data as Websocket.Response.UserJoin
    if (data.userId === userId) navigate("GameConfigure")
  })

  // Render screen
  return (
    <div>
      <input type="text" value={localLobbyId} onChange={e => setLocalLobbyId(e.currentTarget.value)} />
      <input type="button" onClick={joinLobby} value="Join Lobby" />
      <input type="button" onClick={createLobby} value="Create Lobby" />
      {error && <p>{error}</p>}
    </div>
  )
}
