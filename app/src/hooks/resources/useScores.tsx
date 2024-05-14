import React, { createContext, useContext, useState } from "react"
import { useApi } from "../useApi"
import { useWebsocket } from "../useWebsocket"

type TScores = Record<string, number>

type TScoreContext = {
  scores: TScores | null
}

const ScoreContext = createContext<TScoreContext>({ scores: null })
export const useScores = () => useContext(ScoreContext)

export function ScoreProvider(props: { children?: React.ReactNode }) {
  const { api } = useApi()
  const { ws } = useWebsocket()

  const [scores, setScores] = useState<TScores | null>(null)

  return <ScoreContext.Provider value={{ scores }}>{props.children}</ScoreContext.Provider>
}
