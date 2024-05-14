import React, { createContext, useContext, useEffect, useState } from "react"
import { useWebsocket } from "../useWebsocket"
import { useLobby } from "./useLobby"
import { OnGroupDataMessageArgs } from "@azure/web-pubsub-client"
import { Websocket } from "../../types/Websocket"
import { isEvent } from "../../utils/isEvent"

type TWins = Record<string, number>

type TWinContext = {
  wins: TWins | null
}

const WinContext = createContext<TWinContext>({ wins: null })
export const useScores = () => useContext(WinContext)

export function WinProvider(props: { children?: React.ReactNode }) {
  const { ws } = useWebsocket()
  const { lobby } = useLobby()

  const [wins, setWins] = useState<TWins | null>(null)

  useEffect(() => {
    if (!lobby) setWins(null)
  }, [lobby])

  useEffect(() => {
    if (!ws) return

    function onLobbyUpdate(e: OnGroupDataMessageArgs) {
      if (!lobby) return

      const data = e.message.data as Websocket.Message
      if (!isEvent<Websocket.Response.LobbyUpdate>("LOBBY_UPDATE", data)) return

      setWins(data.data.wins)
    }

    ws.on("group-message", onLobbyUpdate)

    return () => {
      ws.off("group-message", onLobbyUpdate)
    }
  }, [ws])

  return <WinContext.Provider value={{ wins }}>{props.children}</WinContext.Provider>
}
