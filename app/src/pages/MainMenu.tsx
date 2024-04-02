import React, { useEffect, useState } from "react"
import { useNavigation } from "../hooks/useNavigation"
import { useWebsocket } from "../hooks/useWebsocket"
import { useLobby } from "../hooks/useLobby"
import { useUser } from "../hooks/useUser"
import { Loading } from "../components/Loading"

export function MainMenu() {
  const user = useUser()
  const { lobby, create, join } = useLobby()
  const ws = useWebsocket()
  const { navigate } = useNavigation()

  const [lobbyId, setLobbyId] = useState("")
  const [error, setError] = useState<string>()

  // Go to lobby if already in one
  useEffect(() => {
    if (lobby) navigate("GameConfigure")
  }, [lobby])

  // Show loading if not ready
  if (!user || !ws) return <Loading />

  // Render screen
  return (
    <div>
      <input type="text" value={lobbyId} onChange={e => setLobbyId(e.currentTarget.value)} />
      <input
        type="button"
        onClick={() => create(lobbyId).catch(() => setError("Could not create lobby"))}
        value="Create Lobby"
      />
      <input
        type="button"
        onClick={() => join(lobbyId).catch(() => setError("Could not join lobby"))}
        value="Join Lobby"
      />
      {error && <p>{error}</p>}
    </div>
  )
}
