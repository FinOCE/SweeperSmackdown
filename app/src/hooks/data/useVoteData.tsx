import React, { createContext, SetStateAction, useContext, useEffect, useState } from "react"
import { useWebsocket } from "../useWebsocket"
import { isEvent } from "../../utils/isEvent"
import { Websocket } from "../../types/Websocket"
import { OnGroupDataMessageArgs } from "@azure/web-pubsub-client"
import { Api } from "../../types/Api"
import { useLobbyData } from "./useLobbyData"

type TVoteDataContext = {
  voteData: Api.VoteGroup | null
  setVoteData: (votes: Api.VoteGroup) => void
  countdown: number | null
}

const VoteDataContext = createContext<TVoteDataContext>({
  voteData: null,
  setVoteData: () => {},
  countdown: null
})
export const useVoteData = () => useContext(VoteDataContext)

export function VoteDataProvider(props: { children?: React.ReactNode }) {
  const { ws } = useWebsocket()
  const { lobbyData } = useLobbyData()

  const [voteData, setVoteData] = useState<Api.VoteGroup | null>(null)
  const [countdown, setCountdown] = useState<number | null>(null)

  useEffect(() => {
    if (!lobbyData) setVoteData(null)
  }, [lobbyData])

  useEffect(() => {
    if (!ws) return

    function onVoteStateUpdate(e: OnGroupDataMessageArgs) {
      const data = e.message.data as Websocket.Message
      if (!isEvent<Websocket.Response.VoteStateUpdate>("VOTE_STATE_UPDATE", data)) return

      setVoteData(data.data)
    }

    function onTimerStart(e: OnGroupDataMessageArgs) {
      if (!voteData) return

      const data = e.message.data as Websocket.Message
      if (!isEvent<Websocket.Response.TimerStart>("TIMER_START", data)) return

      setCountdown(data.data.expiry)
    }

    function onTimerReset(e: OnGroupDataMessageArgs) {
      if (!voteData) return

      const data = e.message.data as Websocket.Message
      if (!isEvent<Websocket.Response.TimerReset>("TIMER_RESET", data)) return

      setCountdown(null)
    }

    ws.on("group-message", onVoteStateUpdate)
    ws.on("group-message", onTimerStart)
    ws.on("group-message", onTimerReset)

    return () => {
      ws.off("group-message", onVoteStateUpdate)
      ws.off("group-message", onTimerStart)
      ws.off("group-message", onTimerReset)
    }
  }, [ws])

  return (
    <VoteDataContext.Provider value={{ voteData, setVoteData, countdown }}>{props.children}</VoteDataContext.Provider>
  )
}
