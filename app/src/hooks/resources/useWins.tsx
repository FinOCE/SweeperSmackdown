import React, { createContext, useContext, useEffect, useState } from "react"
import { useLobbyData } from "../data/useLobbyData"

type TWins = Record<string, number>

type TWinContext = {
  wins: TWins | null
}

const WinContext = createContext<TWinContext>({ wins: null })
export const useScores = () => useContext(WinContext)

export function WinProvider(props: { children?: React.ReactNode }) {
  const { lobbyData } = useLobbyData()

  const [wins, setWins] = useState<TWins | null>(null)

  useEffect(() => {
    setWins(lobbyData ? lobbyData.wins : null)
  }, [lobbyData])

  return <WinContext.Provider value={{ wins }}>{props.children}</WinContext.Provider>
}
