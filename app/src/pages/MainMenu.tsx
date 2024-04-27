import React, { useEffect, useState } from "react"
import "./MainMenu.scss"
import { useNavigation } from "../hooks/useNavigation"
import { useWebsocket } from "../hooks/useWebsocket"
import { useLobby } from "../hooks/useLobby"
import { Loading } from "../components/Loading"
import { Text } from "../components/ui/Text"
import { Box } from "../components/ui/Box"
import { useEmbeddedAppSdk } from "../hooks/useEmbeddAppSdk"
import { useOrigin } from "../hooks/useOrigin"
import { ButtonList } from "../components/ui/ButtonList"

export function MainMenu() {
  const { origin } = useOrigin()
  const { sdk, user } = useEmbeddedAppSdk()
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
  if (!user || !ws || !sdk) return <Loading hide />

  // Render screen
  return (
    <div className="main-menu-content">
      <ButtonList>
        {origin === "discord" && (
          <>
            <Box
              onClick={() =>
                join(sdk.instanceId).catch(() =>
                  create(sdk.instanceId).catch(() => setError("Could not create or join lobby"))
                )
              }
              important
            >
              <Text type="big">Play In Discord Call</Text>
            </Box>
            <br />
          </>
        )}

        <Box
          onClick={() =>
            create(Math.floor(Math.random() * 100000).toString()).catch(() => setError("Could not create lobby"))
          }
        >
          <Text type="big">Create Party</Text>
        </Box>
        <div className="main-menu-join-container">
          <input
            type="text"
            placeholder="Enter Party Code Here"
            value={lobbyId}
            onChange={e => setLobbyId(e.currentTarget.value)}
          />
          <Box onClick={() => join(lobbyId).catch(() => setError("Could not join lobby"))}>
            <Text type="big">Join Party</Text>
          </Box>
        </div>
        {error && <p>{error}</p>}
      </ButtonList>
    </div>
  )
}
