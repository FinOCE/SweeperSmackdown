import React, { useEffect, useState } from "react"
import { useApi } from "../hooks/useApi"
import { useGameInfo } from "../hooks/useGameInfo"
import { useNavigation } from "../hooks/useNavigation"
import { useWebsocket } from "../hooks/useWebsocket"
import { Websocket } from "../types/Websocket"
import { isEvent } from "../utils/isEvent"

export function GameConfigure() {
  const { navigate } = useNavigation()
  const { lobbyId, setLobbyId, userId } = useGameInfo()
  const ws = useWebsocket()
  const api = useApi()

  const [hostId, setHostId] = useState("")
  const [currentVotes, setCurrentVotes] = useState<Record<string, string[]>>({})
  const [requiredVotes, setRequiredVotes] = useState(0)

  const readyVotes = (currentVotes?.READY ?? []).length

  useEffect(() => {
    if (!lobbyId) return

    api.lobbyGet().then(({ hostId }) => setHostId(hostId))
    api.voteGetAll().then(({ votes, requiredVotes }) => (setCurrentVotes(votes), setRequiredVotes(requiredVotes)))
  }, [lobbyId])

  ws.register("group-message", e => {
    const data = e.message.data as Websocket.Message
    if (!isEvent<Websocket.Response.UserJoin>("USER_JOIN", data)) return

    // TODO: Handle users joining
  })

  ws.register("group-message", e => {
    const data = e.message.data as Websocket.Message
    if (!isEvent<Websocket.Response.UserLeave>("USER_LEAVE", data)) return

    // TODO: Handle users leaving
  })

  ws.register("group-message", e => {
    const data = e.message.data as Websocket.Message
    if (!isEvent<Websocket.Response.VoteAdd>("VOTE_ADD", data)) return

    setCurrentVotes(prev => ((prev[data.data] = (prev[data.data] ?? []).concat(userId!)), prev))
  })

  ws.register("group-message", e => {
    const data = e.message.data as Websocket.Message
    if (!isEvent<Websocket.Response.VoteRemove>("VOTE_REMOVE", data)) return

    setCurrentVotes(prev => ((prev[data.data] = prev[data.data].filter(id => id !== userId)), prev))
  })

  ws.register("group-message", e => {
    const data = e.message.data as Websocket.Message
    if (!isEvent<Websocket.Response.VoteUpdateRequirement>("VOTE_UPDATE_REQUIREMENT", data)) return

    setRequiredVotes(data.data)
  })

  async function voteStart() {
    await api.votePut("READY")
  }

  async function voteForce() {
    // TODO: Add api endpoint to force a countdown
  }

  async function leaveLobby() {
    await api.userDelete()
    setLobbyId(null)
    navigate("MainMenu")
  }

  return (
    <div>
      <p>Welcome to lobby {lobbyId}</p>
      <input type="button" onClick={voteStart} value={`Vote Start (${readyVotes}/${requiredVotes})`} />
      <input type="button" onClick={voteForce} value="Force Countdown" disabled={userId === hostId} />
      <input type="button" onClick={leaveLobby} value="Leave Lobby" />
    </div>
  )
}
