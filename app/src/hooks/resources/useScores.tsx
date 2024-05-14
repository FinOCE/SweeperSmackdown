import React, { createContext, useContext, useEffect, useState } from "react"
import { useWebsocket } from "../useWebsocket"
import { OnGroupDataMessageArgs } from "@azure/web-pubsub-client"
import { isEvent } from "../../utils/isEvent"
import { Websocket } from "../../types/Websocket"
import { useLobby } from "./useLobby"

type TScores = Record<string, number>

type TScoreContext = {
  scores: TScores | null
}

const ScoreContext = createContext<TScoreContext>({ scores: null })
export const useScores = () => useContext(ScoreContext)

export function ScoreProvider(props: { children?: React.ReactNode }) {
  const { ws } = useWebsocket()
  const { lobby } = useLobby()

  const [scores, setScores] = useState<TScores | null>(null)

  useEffect(() => {
    if (!lobby) setScores(null)
  }, [lobby])

  useEffect(() => {
    if (!ws) return

    function onLobbyUpdate(e: OnGroupDataMessageArgs) {
      if (!lobby) return

      const data = e.message.data as Websocket.Message
      if (!isEvent<Websocket.Response.LobbyUpdate>("LOBBY_UPDATE", data)) return

      setScores(data.data.scores)
    }

    ws.on("group-message", onLobbyUpdate)

    return () => {
      ws.off("group-message", onLobbyUpdate)
    }
  }, [ws])

  return <ScoreContext.Provider value={{ scores }}>{props.children}</ScoreContext.Provider>
}
