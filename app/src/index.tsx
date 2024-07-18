import React from "react"
import "./index.scss"
import { createRoot } from "react-dom/client"
import { ApiProvider } from "./hooks/useApi"
import { EmbeddedAppSdkProvider } from "./hooks/useEmbeddAppSdk"
import { WebsocketProvider } from "./hooks/useWebsocket"
import { NavigationProvider } from "./hooks/useNavigation"
import { OriginProvider } from "./hooks/useOrigin"
import { DevPreview } from "./pages/DevPreview"
import { RollingBackground } from "./components/ui/RollingBackground"
import { Page } from "./components/ui/Page"
import { LobbyProvider } from "./hooks/resources/useLobby"

function App() {
  // Show dev preview for developing UI features without running full app
  if (process.env.PUBLIC_ENV__DEV_PREVIEW) return <DevPreview />

  // Render app
  return (
    <RollingBackground>
      <Page>
        <OriginProvider>
          <ApiProvider>
            <EmbeddedAppSdkProvider>
              <WebsocketProvider>
                <LobbyProvider>
                  <NavigationProvider />
                </LobbyProvider>
              </WebsocketProvider>
            </EmbeddedAppSdkProvider>
          </ApiProvider>
        </OriginProvider>
      </Page>
    </RollingBackground>
  )
}

const root = createRoot(document.getElementById("root")!)
root.render(<App />)
