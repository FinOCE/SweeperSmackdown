import React from "react"
import { createRoot } from "react-dom/client"
import { Loading } from "./components/Loading"
import { ApiProvider } from "./hooks/useApi"
import { EmbeddedAppSdkProvider } from "./hooks/useEmbeddAppSdk"
import { UserProvider } from "./hooks/useUser"
import { LobbyProvider } from "./hooks/useLobby"
import { WebsocketProvider } from "./hooks/useWebsocket"
import { NavigationProvider } from "./hooks/useNavigation"
import { OriginProvider } from "./hooks/useOrigin"

function App() {
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
