import { GameInfoProvider } from "../../hooks/useGameInfo"
import { NavigationProvider } from "../../hooks/useNavigation"
import { WebsocketProvider } from "../../hooks/useWebsocket"
import "./style.scss"

export function Page() {
  return (
    <GameInfoProvider>
      <WebsocketProvider>
        <NavigationProvider />
      </WebsocketProvider>
    </GameInfoProvider>
  )
}
