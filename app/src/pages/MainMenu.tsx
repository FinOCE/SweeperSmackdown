import React, { useEffect, useState } from "react"
import "./MainMenu.scss"
import { useNavigation } from "../hooks/useNavigation"
import { useWebsocket } from "../hooks/useWebsocket"
import { Loading } from "../components/Loading"
import { Text } from "../components/ui/Text"
import { Box } from "../components/ui/Box"
import { useEmbeddedAppSdk } from "../hooks/useEmbeddAppSdk"
import { useOrigin } from "../hooks/useOrigin"
import { ButtonList } from "../components/ui/ButtonList"
import { useLobby } from "../hooks/resources/useLobby"

export function MainMenu() {
  const { origin } = useOrigin()
  const { sdk, user } = useEmbeddedAppSdk()
  const { lobby, createLobby, joinLobby } = useLobby()
  const { navigate } = useNavigation()

  const [lobbyId, setLobbyId] = useState("")
  const [error, setError] = useState<string>()
  const [redirecting, setRedirecting] = useState(false)

  // Go to lobby if already in one
  useEffect(() => {
    if (!redirecting || !lobby) return
    navigate("GameConfigure")
  }, [lobby, redirecting])

  async function joinOrCreate(id: string) {
    try {
      await joinLobby(id)
    } catch (err) {
      try {
        await createLobby(id)
      } catch (err) {
        setError("Could not create or join lobby")
        return
      }
    }

    setRedirecting(true)
  }

  async function create(id: string) {
    try {
      await createLobby(id)
      setRedirecting(true)
    } catch (err) {
      setError("Could not create lobby")
    }
  }

  async function join(id: string) {
    try {
      await joinLobby(id)
      setRedirecting(true)
    } catch (err) {
      setError("Could not join lobby")
    }
  }

  // Show loading if not ready
  if (!user || !sdk) return <Loading hide />

  // Render screen
  return (
    <div className="main-menu-content">
      <ButtonList>
        {origin === "discord" && (
          <>
            <Box onClick={() => joinOrCreate(sdk.instanceId)} important>
              <Text type="big">Play In Discord Call</Text>
            </Box>
            <br />
          </>
        )}

        <Box onClick={() => create(Math.floor(Math.random() * 100000).toString())}>
          <Text type="big">Create Party</Text>
        </Box>
        <div className="main-menu-join-container">
          <input
            type="text"
            placeholder="Enter Party Code Here"
            value={lobbyId}
            onChange={e => setLobbyId(e.currentTarget.value)}
          />
          <Box onClick={() => join(lobbyId)} disabled={lobbyId.length === 0}>
            <Text type="big">Join Party</Text>
          </Box>
        </div>
        {error && <p>{error}</p>}
      </ButtonList>
    </div>
  )
}
