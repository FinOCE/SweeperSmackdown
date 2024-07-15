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
import { LobbyDataProvider } from "./hooks/data_old/useLobbyData"
import { LobbyProvider } from "./hooks/resources_old/useLobby"
import { MemberProvider } from "./hooks/resources_old/useMembers"
import { ScoreProvider } from "./hooks/resources_old/useScores"
import { SettingsProvider } from "./hooks/resources_old/useSettings"
import { WinProvider } from "./hooks/resources_old/useWins"

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
                <LobbyDataProvider>
                  <LobbyProvider>
                    <MemberProvider>
                      <ScoreProvider>
                        <SettingsProvider>
                          <WinProvider>
                            <NavigationProvider />
                          </WinProvider>
                        </SettingsProvider>
                      </ScoreProvider>
                    </MemberProvider>
                  </LobbyProvider>
                </LobbyDataProvider>
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
