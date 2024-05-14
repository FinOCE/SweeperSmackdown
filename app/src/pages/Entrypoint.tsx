import React, { useEffect, useState } from "react"
import "./Entrypoint.scss"
import { useNavigation } from "../hooks/useNavigation"
import { useEmbeddedAppSdk } from "../hooks/useEmbeddAppSdk"
import { useWebsocket } from "../hooks/useWebsocket"
import { Bomb } from "../components/ui/icons/Bomb"
import { Text } from "../components/ui/Text"

export function Entrypoint() {
  const { sdk, user } = useEmbeddedAppSdk()
  const { manager: ws } = useWebsocket()
  const { navigate } = useNavigation()

  const [loading, setLoading] = useState(true)

  useEffect(() => {
    if (sdk && user && ws) setLoading(false)
  }, [sdk, user, ws])

  useEffect(() => {
    function handleNavigation() {
      if (!loading) navigate("MainMenu")
    }

    document.addEventListener("click", handleNavigation)
    return () => document.removeEventListener("click", handleNavigation)
  }, [loading])

  return (
    <div>
      <div className="entrypoint-title">
        <Bomb color="yellow" />
        <div>
          <Text type="title">Sweeper</Text>
          <br />
          <Text type="title">Smackdown</Text>
        </div>
      </div>
      <div className="entrypoint-start-text">
        {loading ? (
          <Text type="normal">Loading... Please wait a moment</Text>
        ) : (
          <Text type="normal">Press anywhere to Start</Text>
        )}
      </div>
    </div>
  )
}
