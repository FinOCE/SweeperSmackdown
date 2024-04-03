import React from "react"
import { createRoot } from "react-dom/client"
import { ApiProvider } from "./hooks/useApi"
import { EmbeddedAppSdkProvider } from "./hooks/useEmbeddAppSdk"
import { UserProvider } from "./hooks/useUser"
import { LobbyProvider } from "./hooks/useLobby"
import { WebsocketProvider } from "./hooks/useWebsocket"
import { NavigationProvider } from "./hooks/useNavigation"
import { OriginProvider } from "./hooks/useOrigin"
import { DevPreview } from "./pages/DevPreview"

function App() {
  // Show dev preview for developing UI features without running full app
  if (process.env.PUBLIC_ENV__DEV_PREVIEW) return <DevPreview />

  // Render app
  return (
    <OriginProvider>
      <ApiProvider>
        <EmbeddedAppSdkProvider>
          <UserProvider>
            <LobbyProvider>
              <WebsocketProvider>
                <NavigationProvider />
              </WebsocketProvider>
            </LobbyProvider>
          </UserProvider>
        </EmbeddedAppSdkProvider>
      </ApiProvider>
    </OriginProvider>
  )
}

const root = createRoot(document.getElementById("root")!)
root.render(<App />)
