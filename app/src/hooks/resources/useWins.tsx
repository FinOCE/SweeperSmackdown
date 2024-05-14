import React, { createContext, useContext, useState } from "react"
import { useApi } from "../useApi"
import { useWebsocket } from "../useWebsocket"

type TWins = Record<string, number>

type TWinContext = {
  wins: TWins | null
}

const WinContext = createContext<TWinContext>({ wins: null })
export const useScores = () => useContext(WinContext)

export function WinProvider(props: { children?: React.ReactNode }) {
  const { api } = useApi()
  const { ws } = useWebsocket()

  const [wins, setWins] = useState<TWins | null>(null)

  return <WinContext.Provider value={{ wins }}>{props.children}</WinContext.Provider>
}
