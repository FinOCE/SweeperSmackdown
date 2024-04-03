import React, { useEffect, useState } from "react"
import "./MainMenu.scss"
import { useNavigation } from "../hooks/useNavigation"
import { useWebsocket } from "../hooks/useWebsocket"
import { useLobby } from "../hooks/useLobby"
import { useUser } from "../hooks/useUser"
import { Loading } from "../components/Loading"
import { Page } from "../components/ui/Page"
import { Text } from "../components/ui/Text"
import { RollingBackground } from "../components/ui/RollingBackground"
import { Bomb } from "../components/ui/icons/Bomb"
import { Box } from "../components/ui/Box"
import { useEmbeddedAppSdk } from "../hooks/useEmbeddAppSdk"
import { useOrigin } from "../hooks/useOrigin"

export function MainMenu() {
  const { origin } = useOrigin()
  const sdk = useEmbeddedAppSdk()
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
  if (!user || !ws || !sdk) return <Loading hide />

  // Render screen
  return (
    <Page fade>
      <RollingBackground>
        <div className="dev-preview-content">
          <div className="dev-preview-title">
            <Bomb color="yellow" />
            <div>
              <Text type="title">Sweeper</Text>
              <br />
              <Text type="title">Smackdown</Text>
            </div>
          </div>

          <div className="dev-preview-menu">
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
          </div>
        </div>
      </RollingBackground>
    </Page>
  )
}
