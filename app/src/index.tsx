import React, { useEffect, useState } from "react"
import { createRoot } from "react-dom/client"
import { Loading } from "./components/Loading"
import { ApiProvider } from "./hooks/useApi"
import { EmbeddedAppSdkProvider } from "./hooks/useEmbeddAppSdk"
import { UserProvider } from "./hooks/useUser"
import { LobbyProvider } from "./hooks/useLobby"
import { WebsocketProvider } from "./hooks/useWebsocket"
import { NavigationProvider } from "./hooks/useNavigation"

function App() {
  // Check if the app is running in an iframe (on Discord)
  const [iframeId, setIframeId] = useState<string | null>(null)
  const [ready, setReady] = useState(false)

  useEffect(() => {
    if (!window?.location?.search) return

    const params = new URLSearchParams(window.location.search)
    setIframeId(params.get("iframe_id"))
    setReady(true)
  }, [window?.location?.search])

  // Don't load providers until we know if we're running in an iframe
  if (!ready) return <Loading />

  // Render app
  return (
    <ApiProvider>
      <EmbeddedAppSdkProvider iframeId={iframeId}>
        <UserProvider>
          <LobbyProvider>
            <WebsocketProvider>
              <NavigationProvider />
            </WebsocketProvider>
          </LobbyProvider>
        </UserProvider>
      </EmbeddedAppSdkProvider>
    </ApiProvider>
  )
}

const root = createRoot(document.getElementById("root")!)
root.render(<App />)
