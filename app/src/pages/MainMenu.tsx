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
import { Api } from "../types/Api"

export function MainMenu() {
  const { origin } = useOrigin()
  const { sdk, user } = useEmbeddedAppSdk()
  const { lobby, createLobby, joinLobby } = useLobby()
  const { navigate } = useNavigation()

  const [lobbyId, setLobbyId] = useState("")
  const [error, setError] = useState<string>()
  const [redirecting, setRedirecting] = useState(false)

  useEffect(() => {
    if (!error) return
    alert(error)
  }, [error])

  // Go to lobby if already in one
  useEffect(() => {
    if (!redirecting || !lobby) return

    switch (lobby.state) {
      case Api.Enums.ELobbyState.Init:
      case Api.Enums.ELobbyState.Configure:
        navigate("GameConfigure")
        return
      case Api.Enums.ELobbyState.Play:
        navigate("GameActive")
        return
      case Api.Enums.ELobbyState.Won:
      case Api.Enums.ELobbyState.Celebrate:
        navigate("GameCelebration")
        return
    }
  }, [lobby, redirecting])

  async function joinOrCreate(id: string) {
    try {
      await joinLobby(id)
    } catch (err) {
      try {
        await createLobby(id)
      } catch (err) {
        setError("Failed to create or join the channel lobby. Please try again later.")
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
      setError("Failed to create a lobby. Please try again later.")
    }
  }

  async function join(id: string) {
    try {
      await joinLobby(id)
      setRedirecting(true)
    } catch (err) {
      setError("Could not find the lobby.")
    }
  }

  async function bugReport() {
    if (!sdk) return

    sdk.commands.openExternalLink({ url: "https://discord.gg/g7HWZatVPy" })
  }

  // Show loading if not ready
  if (!user || !sdk) return <Loading hide />

  // Render screen
  return (
    <div className="main-menu-content">
      <ButtonList>
        {/* origin === "discord" && (
          <>
            <Box onClick={() => joinOrCreate(sdk.instanceId)} important>
              <Text type="big">Play In Discord Call</Text>
            </Box>
            <br />
          </>
        )*/}

        <Box onClick={() => create(Math.floor(Math.random() * 100000).toString())}>
          <Text type="big">Create Party</Text>
        </Box>
        <div className="main-menu-join-container">
          <input
            type="text"
            placeholder="Enter Party Code Here"
            value={lobbyId}
            onChange={e => setLobbyId(e.currentTarget.value)}
            onKeyUp={e => (e.key === "Enter" && lobbyId.length !== 0 ? join(lobbyId) : undefined)}
          />
          <Box onClick={() => (lobbyId.length !== 0 ? join(lobbyId) : undefined)} disabled={lobbyId.length === 0}>
            <Text type="big">Join Party</Text>
          </Box>
        </div>
        <br />
        <Box onClick={bugReport}>
          <Text type="big">Found a Bug? Report it Here</Text>
        </Box>
      </ButtonList>
    </div>
  )
}
