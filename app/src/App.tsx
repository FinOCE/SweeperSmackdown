import React from "react"
import "./App.scss"
import { LobbyProvider } from "./hooks/useLobby"
import { WebsocketProvider } from "./hooks/useWebsocket"
import { NavigationProvider } from "./hooks/useNavigation"
import { EmbeddedAppSdkProvider } from "./hooks/useEmbeddAppSdk"
import { ApiProvider } from "./hooks/useApi"
import { UserProvider } from "./hooks/useUser"

export function App() {
  return (
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
  )
}
