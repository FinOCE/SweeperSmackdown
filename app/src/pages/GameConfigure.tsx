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

  // Register websocket events
  useEffect(() => {
    ws.register("group-message", e => {
      const data = e.message.data as Websocket.Message
      if (!isEvent<Websocket.Response.VoteStateUpdate>("VOTE_STATE_UPDATE", data)) return

      setVote(data.data)
    })
  })

  // Setup UI state
  const [votePending, setVotePending] = useState(false)

  const isReady = vote?.votes?.READY?.includes(userId!) ?? false

  // Setup UI functions
  async function voteStart() {
    setVotePending(true)
    await api.votePut("READY")
    setVotePending(false)
  }

  async function voteCancel() {
    setVotePending(true)
    await api.voteDelete()
    setVotePending(false)
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
        disabled={votePending}
      />
      <input type="button" onClick={voteForce} value="Force Countdown" disabled={userId !== lobby.hostId} />
      <input type="button" onClick={leaveLobby} value="Leave Lobby" />
    </div>
  )
}
