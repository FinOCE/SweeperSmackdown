import React, { useEffect } from "react"
import "./Entrypoint.scss"
import { useNavigation } from "../hooks/useNavigation"
import { useUser } from "../hooks/useUser"
import { useEmbeddedAppSdk } from "../hooks/useEmbeddAppSdk"
import { useWebsocket } from "../hooks/useWebsocket"

export function Entrypoint() {
  const sdk = useEmbeddedAppSdk()
  const user = useUser()
  const ws = useWebsocket()
  const { navigate } = useNavigation()

  useEffect(() => {
    if (sdk && user && ws) navigate("MainMenu")
  }, [sdk, user, ws])

  return <div>TODO</div>
}
