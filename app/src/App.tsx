import React from "react"
import "./App.scss"
import { GameInfoProvider } from "./hooks/useGameInfo"
import { WebsocketProvider } from "./hooks/useWebsocket"
import { NavigationProvider } from "./hooks/useNavigation"

export function App() {
  return (
    <GameInfoProvider>
      <WebsocketProvider>
        <NavigationProvider />
      </WebsocketProvider>
    </GameInfoProvider>
  )
}
