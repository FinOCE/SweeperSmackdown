import React, { createContext, useContext, useEffect, useState } from "react"
import { useLobbyData } from "../data/useLobbyData"

type TScores = Record<string, number>

type TScoreContext = {
  scores: TScores | null
}

const ScoreContext = createContext<TScoreContext>({ scores: null })
export const useScores = () => useContext(ScoreContext)

export function ScoreProvider(props: { children?: React.ReactNode }) {
  const { lobbyData } = useLobbyData()

  const [scores, setScores] = useState<TScores | null>(null)

  useEffect(() => {
    setScores(lobbyData ? lobbyData.scores : null)
  }, [lobbyData])

  return <ScoreContext.Provider value={{ scores }}>{props.children}</ScoreContext.Provider>
}
