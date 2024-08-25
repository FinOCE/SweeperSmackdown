import React, { useEffect, useState } from "react"
import "./Entrypoint.scss"
import { useNavigation } from "../hooks/useNavigation"
import { useEmbeddedAppSdk } from "../hooks/useEmbeddAppSdk"
import { useWebsocket } from "../hooks/useWebsocket"
import { Bomb } from "../components/ui/icons/Bomb"
import { Text } from "../components/ui/Text"

type EntrypointProps = {}

export function Entrypoint({}: EntrypointProps) {
  const { sdk, user } = useEmbeddedAppSdk()
  const { ws, connectionFailed } = useWebsocket()
  const { navigate } = useNavigation()

  const [loading, setLoading] = useState(true)

  useEffect(() => {
    function handleNavigation() {
      if (!loading) navigate("MainMenu", {})
    }

    if (sdk && user && ws) {
      setLoading(false)
      document.addEventListener("click", handleNavigation)
    }

    return () => document.removeEventListener("click", handleNavigation)
  }, [sdk, user, ws, loading])

  function getStatusText(loading: boolean, connectionFailed: boolean) {
    if (connectionFailed) return "Unable to connect... Please try again later"
    if (loading) return "Loading... Please wait a moment"
    return "Press anywhere to Start"
  }

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
        <Text type="normal">{getStatusText(loading, connectionFailed)}</Text>
      </div>
    </div>
  )
}
