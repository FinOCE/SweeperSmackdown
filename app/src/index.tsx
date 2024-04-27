import React from "react"
import "./index.scss"
import { createRoot } from "react-dom/client"
import { ApiProvider } from "./hooks/useApi"
import { EmbeddedAppSdkProvider } from "./hooks/useEmbeddAppSdk"
import { LobbyProvider } from "./hooks/useLobby"
import { WebsocketProvider } from "./hooks/useWebsocket"
import { NavigationProvider } from "./hooks/useNavigation"
import { OriginProvider } from "./hooks/useOrigin"
import { DevPreview } from "./pages/DevPreview"
import { RollingBackground } from "./components/ui/RollingBackground"
import { Page } from "./components/ui/Page"

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
              <LobbyProvider>
                <WebsocketProvider>
                  <NavigationProvider />
                </WebsocketProvider>
              </LobbyProvider>
            </EmbeddedAppSdkProvider>
          </ApiProvider>
        </OriginProvider>
      </Page>
    </RollingBackground>
  )
}

const root = createRoot(document.getElementById("root")!)
root.render(<App />)
