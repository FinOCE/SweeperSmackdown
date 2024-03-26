import React, { useEffect, useState } from "react"
import { useApi } from "../hooks/useApi"
import { useGameInfo } from "../hooks/useGameInfo"
import { useNavigation } from "../hooks/useNavigation"
import { useWebsocket } from "../hooks/useWebsocket"
import { Websocket } from "../types/Websocket"
import { isEvent } from "../utils/isEvent"
import { Api } from "../types/Api"

export function GameConfigure() {
  const { navigate } = useNavigation()
  const { lobbyId, setLobbyId, userId } = useGameInfo()
  const ws = useWebsocket()
  const api = useApi()

  let [lobby, setLobby] = useState<Api.Lobby>()
  let [vote, setVote] = useState<Api.VoteGroup>()

  useEffect(() => {
    if (!lobbyId || !userId) return

    api.lobbyGet().then(setLobby)
    api.voteGetAll().then(setVote)
  }, [lobbyId, userId])

  const isReady = vote?.votes?.READY?.includes(userId!) ?? false

  // Register websocket events
  useEffect(() => {
    ws.register("group-message", e => {
      const data = e.message.data as Websocket.Message
      if (!isEvent<Websocket.Response.VoteAdd>("VOTE_ADD", data)) return

      setVote(v => ((v = structuredClone(v!)), v.votes.READY.push(data.userId), v))
    })

    ws.register("group-message", e => {
      const data = e.message.data as Websocket.Message
      if (!isEvent<Websocket.Response.VoteRemove>("VOTE_REMOVE", data)) return

      setVote(v => ((v = structuredClone(v!)), (v.votes.READY = v.votes.READY.filter(id => id !== data.userId)), v))
    })

    ws.register("group-message", e => {
      const data = e.message.data as Websocket.Message
      if (!isEvent<Websocket.Response.VoteUpdateRequirement>("VOTE_UPDATE_REQUIREMENT", data)) return

      setVote(v => ((v = structuredClone(v!)), (v.requiredVotes = data.data), v))
    })
  }, [])

  // Setup UI functions
  async function voteStart() {
    await api.votePut("READY")
    setVote(v => ((v = structuredClone(v!)), v.votes.READY.push(userId!), v))
  }

  async function voteCancel() {
    await api.voteDelete()
    setVote(v => ((v = structuredClone(v!)), (v.votes.READY = v.votes.READY.filter(id => id !== userId)), v))
  }

  async function voteForce() {
    // TODO: Add api endpoint to force a countdown
  }

  async function leaveLobby() {
    await api.userDelete()
    setLobbyId(null)
    navigate("MainMenu")
  }

  // Prevent rendering until TODO
  if (!lobby || !vote) return <div>Loading...</div>

  // Render page
  return (
    <div>
      <p>Welcome to lobby {lobbyId}</p>
      <input
        type="button"
        onClick={isReady ? voteCancel : voteStart}
        value={
          isReady
            ? `Cancel Vote (${vote.votes.READY.length}/${vote.requiredVotes})`
            : `Vote Start (${vote.votes.READY.length}/${vote.requiredVotes})`
        }
      />
      <input type="button" onClick={voteForce} value="Force Countdown" disabled={userId !== lobby.hostId} />
      <input type="button" onClick={leaveLobby} value="Leave Lobby" />
    </div>
  )
}
