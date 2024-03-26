import React, { createContext, Dispatch, JSX, SetStateAction, useContext, useEffect, useState } from "react"

type GameInfo = {
  userId: string | null
  setUserId: Dispatch<SetStateAction<string | null>>
  lobbyId: string | null
  setLobbyId: Dispatch<SetStateAction<string | null>>
}

const GameInfoContext = createContext<GameInfo>({
  userId: null,
  setUserId() {},
  lobbyId: null,
  setLobbyId() {}
})

export const useGameInfo = () => useContext(GameInfoContext)

type GameInfoProviderProps = {
  children?: JSX.Element | JSX.Element[]
}

export function GameInfoProvider({ children }: GameInfoProviderProps) {
  const [userId, setUserId] = useState<string | null>(null)
  const [lobbyId, setLobbyId] = useState<string | null>(null)

  useEffect(() => {
    setUserId("userId")
    // TODO: Get game info from discord sdk
  }, [])

  return (
    <GameInfoContext.Provider value={{ userId, setUserId, lobbyId, setLobbyId }}>{children}</GameInfoContext.Provider>
  )
}
