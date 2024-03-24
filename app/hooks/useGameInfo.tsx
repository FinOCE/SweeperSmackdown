import { createContext, JSX } from "preact"
import { StateUpdater, useContext, useEffect, useState } from "preact/hooks"

type GameInfo = {
  userId: string
  setUserId: (value: StateUpdater<string>) => void
  lobbyId: string
  setLobbyId: (value: StateUpdater<string>) => void
}

const GameInfoContext = createContext<GameInfo>(null)
export const useGameInfo = () => useContext(GameInfoContext)

type GameInfoProviderProps = {
  children?: JSX.Element | JSX.Element[]
}

export function GameInfoProvider({ children }: GameInfoProviderProps) {
  const [userId, setUserId] = useState<string>()
  const [lobbyId, setLobbyId] = useState<string>()

  useEffect(() => {
    setUserId("userId")
    setLobbyId("lobbyId")
    // TODO: Get game info from discord sdk
  }, [])

  return (
    <GameInfoContext.Provider value={{ userId, setUserId, lobbyId, setLobbyId }}>{children}</GameInfoContext.Provider>
  )
}
