import React, { createContext, Dispatch, JSX, SetStateAction, useContext, useEffect, useState } from "react"
import { Api } from "../types/Api"

type GameInfo = {
  userId: string | null
  setUserId: Dispatch<SetStateAction<string | null>>
  lobbyId: string | null
  lobby: Api.Lobby | null
  setLobby: Dispatch<SetStateAction<Api.Lobby | null>>
}

const GameInfoContext = createContext<GameInfo>({
  userId: null,
  setUserId() {},
  lobbyId: null,
  lobby: null,
  setLobby() {}
})

export const useGameInfo = () => useContext(GameInfoContext)

type GameInfoProviderProps = {
  children?: JSX.Element | JSX.Element[]
}

export function GameInfoProvider({ children }: GameInfoProviderProps) {
  const [userId, setUserId] = useState<string | null>(null)
  const [lobby, setLobby] = useState<Api.Lobby | null>(null)

  useEffect(() => {
    setUserId(crypto.randomUUID())
    // TODO: Get game info from discord sdk
  }, [])

  return (
    <GameInfoContext.Provider value={{ userId, setUserId, lobbyId: lobby?.lobbyId ?? null, lobby, setLobby }}>
      {children}
    </GameInfoContext.Provider>
  )
}
