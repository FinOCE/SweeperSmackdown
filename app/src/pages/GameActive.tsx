import React, { useEffect, useState } from "react"
import "./GameActive.scss"
import { useWebsocket } from "../hooks/useWebsocket"
import { Websocket } from "../types/Websocket"
import { State } from "../utils/State"
import { useApi } from "../hooks/useApi"
import { isEvent } from "../utils/isEvent"
import { useLobby } from "../hooks/useLobby"
import { Loading } from "../components/Loading"
import { ButtonList } from "../components/ui/ButtonList"
import { Box } from "../components/ui/Box"
import { Text } from "../components/ui/Text"
import { useNavigation } from "../hooks/useNavigation"
import { Board } from "../components/gameplay/Board"
import { BoardPreview } from "../components/gameplay/BoardPreview"
import { useCountdown } from "../hooks/useCountdown"
import { useEmbeddedAppSdk } from "../hooks/useEmbeddAppSdk"

export function GameActive() {
  const { api } = useApi()
  const { user } = useEmbeddedAppSdk()
  const { lobby, settings, leave, scores, setScores, wins, setWins } = useLobby()
  const ws = useWebsocket()
  const { navigate } = useNavigation()

  const [initialGameState, setInitialGameState] = useState<Uint8Array>()
  const [localGameState, setLocalGameState] = useState<Uint8Array>()
  const [lost, setLost] = useState(false)
  const { countdown, start, stop } = useCountdown(() => setLost(false))
  const [won, setWon] = useState(false)

  const [pendingCompetitionMoves, setPendingCompetitionMoves] = useState<
    Record<string, { reveals?: number[]; flagAdd?: number[]; flagRemove?: number[] }>
  >({})
  const [competitionState, setCompetitionState] = useState<Record<string, Uint8Array>>()

  // Determine if game has been won
  useEffect(() => {
    if (!localGameState) return

    if (State.isCompleted(localGameState)) setWon(true)
  }, [localGameState])

  // Send solution to server if game has been won
  useEffect(() => {
    if (!won || !lobby || !user) return

    api.boardSolution(lobby.lobbyId, user.id, localGameState!)
  }, [won, lobby, user])

  // Apply pending competitor moves
  useEffect(() => {
    if (!pendingCompetitionMoves || !competitionState) return
    if (Object.keys(pendingCompetitionMoves).length === 0) return

    for (const userId in pendingCompetitionMoves) {
      if (!competitionState[userId]) continue

      const state = competitionState[userId]
      const { reveals, flagAdd, flagRemove } = pendingCompetitionMoves[userId]

      if (reveals) for (const i of reveals) state[i] = State.reveal(state[i])
      if (flagAdd) for (const i of flagAdd) state[i] = State.flag(state[i])
      if (flagRemove) for (const i of flagRemove) state[i] = State.removeFlag(state[i])

      setCompetitionState(prev => ({ ...prev, [userId]: state }))
    }

    setPendingCompetitionMoves({})
  }, [pendingCompetitionMoves])

  // Use timeout to allow moves again
  useEffect(() => {
    if (!lost) return

    start(1000 * 3)
    return stop
  }, [lost])

  // Only render once all dependencies are loaded
  if (!ws || !user || !lobby || !settings) return <Loading />

  ws.clear()

  // TODO: Handle users joining and leaving with USER_JOIN and USER_LEAVE ws events

  ws.register("group-message", e => {
    const data = e.message.data as Websocket.Message
    if (!isEvent<Websocket.Response.BoardCreate>("BOARD_CREATE", data)) return

    if (data.userId === user.id) {
      if (data.data.reset) return

      const gameState = new TextEncoder().encode(data.data.gameState)
      setInitialGameState(gameState)
      setLocalGameState(gameState)
      setLost(false)
      setWon(false)
    } else {
      setCompetitionState(prev => ({ ...prev, [data.userId]: new TextEncoder().encode(data.data.gameState) }))
    }
  })

  function isReveals(data: Websocket.Response.MoveAdd["data"]): data is { lobbyId: string; reveals: number[] } {
    return "reveals" in data && data.reveals !== null
  }

  function isFlagAdd(data: Websocket.Response.MoveAdd["data"]): data is { lobbyId: string; flagAdd: number } {
    return "flagAdd" in data && data.flagAdd !== null
  }

  function isFlagRemove(data: Websocket.Response.MoveAdd["data"]): data is { lobbyId: string; flagRemove: number } {
    return "flagRemove" in data && data.flagRemove !== null
  }

  // Track score update
  ws.register("group-message", e => {
    const data = e.message.data as Websocket.Message
    if (!isEvent<Websocket.Response.LobbyUpdate>("LOBBY_UPDATE", data)) return

    setScores(data.data.scores)
    setWins(data.data.wins)
  })

  // Queue competitior moves into pending moves to be updated
  ws.register("group-message", e => {
    const data = e.message.data as Websocket.Message
    if (!isEvent<Websocket.Response.MoveAdd>("MOVE_ADD", data)) return

    if (data.userId === user.id) return

    if (isReveals(data.data)) {
      const { reveals } = data.data
      setPendingCompetitionMoves(prev => ({
        ...prev,
        [data.userId]: { ...prev[data.userId], reveals: [...(prev[data.userId]?.reveals ?? []), ...reveals] }
      }))
    } else if (isFlagAdd(data.data)) {
      const { flagAdd } = data.data
      setPendingCompetitionMoves(prev => ({
        ...prev,
        [data.userId]: { ...prev[data.userId], flagAdd: [...(prev[data.userId]?.flagAdd ?? []), flagAdd] }
      }))
    } else if (isFlagRemove(data.data)) {
      const { flagRemove } = data.data
      setPendingCompetitionMoves(prev => ({
        ...prev,
        [data.userId]: { ...prev[data.userId], flagRemove: [...(prev[data.userId]?.flagRemove ?? []), flagRemove] }
      }))
    }
  })

  // Handle game winner
  ws.register("group-message", e => {
    const data = e.message.data as Websocket.Message
    if (!isEvent<Websocket.Response.GameWon>("GAME_WON", data)) return

    navigate("GameCelebration")

    // TODO: Change to stop control and tell everyone the game is over on a countdown
  })

  function notifyMoveAdd(data: Omit<Websocket.Response.MoveAdd["data"], "lobbyId">) {
    if (!ws || !user || !lobby) return

    ws.sendToLobby<Websocket.Response.MoveAdd>(lobby.lobbyId, {
      eventName: "MOVE_ADD",
      userId: user.id,
      data: { ...data, lobbyId: lobby.lobbyId } as Websocket.Response.MoveAdd["data"]
    })
  }

  async function reset() {
    if (!lobby || !user) return

    setLocalGameState(initialGameState)
    await api.boardReset(lobby.lobbyId, user.id)
  }

  async function skip() {
    if (!lobby || !user) return

    await api.boardSkip(lobby.lobbyId, user.id)
  }

  async function leaveParty() {
    await leave()
    navigate("MainMenu")
  }

  if (!localGameState) return <Loading />

  return (
    <div id="game-active">
      <div id="game-active-boards-container">
        <div id="game-active-current-board-container">
          <Board
            height={settings.height}
            width={settings.width}
            localState={localGameState}
            setLocalState={setLocalGameState}
            lost={lost}
            setLost={setLost}
            notifyMoveAdd={notifyMoveAdd}
          />
        </div>
        {Object.keys(competitionState ?? {}).length > 0 && (
          <div id="game-active-competitors">
            {Object.entries(competitionState ?? {}).map(([userId, state]) => (
              <div key={userId}>
                <Text type="small">
                  {userId} - {scores[userId] ?? 0} ({wins[userId] ?? 0})
                </Text>
                <br />
                <BoardPreview {...{ userId, settings, state }} />
              </div>
            ))}
          </div>
        )}
      </div>

      {countdown && <Text type="normal">Recover in {countdown}</Text>}

      <ButtonList>
        <ButtonList horizontal>
          <Box onClick={reset}>
            <Text type="big">Reset</Text>
          </Box>
          <Box onClick={skip} disabled={settings.seed !== 0}>
            <Text type="big">Skip</Text>
          </Box>
        </ButtonList>
        <Box onClick={leaveParty}>
          <Text type="big">Leave Party</Text>
        </Box>
      </ButtonList>
    </div>
  )
}
