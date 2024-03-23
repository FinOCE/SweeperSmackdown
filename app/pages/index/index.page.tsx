import { GameInfoProvider } from "../../hooks/useGameInfo"
import { WebsocketProvider } from "../../hooks/useWebsocket"
import MainMenu from "../../screens/MainMenu"
import "./style.scss"

export function Page() {
  // TODO: Setup navigation between screens without page reload

  return (
    <GameInfoProvider>
      <WebsocketProvider>
        <MainMenu />
      </WebsocketProvider>
    </GameInfoProvider>
  )
}
