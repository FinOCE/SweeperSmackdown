import React, { useEffect, useState } from "react"
import "./GameActive.scss"
import { useWebsocket } from "../hooks/useWebsocket"
import { Websocket } from "../types/Websocket"
import { State } from "../utils/State"
import { useApi } from "../hooks/useApi"
import { isEvent } from "../utils/isEvent"
import { Loading } from "../components/Loading"
import { ButtonList } from "../components/ui/ButtonList"
import { Box } from "../components/ui/Box"
import { Text } from "../components/ui/Text"
import { useNavigation } from "../hooks/useNavigation"
import { Board } from "../components/gameplay/Board"
import { BoardPreview } from "../components/gameplay/BoardPreview"
import { useCountdown } from "../hooks/useCountdown"
import { Settings } from "../components/ui/controls/Settings"
import { useLobby } from "../hooks/resources/useLobby"
import { useSettings } from "../hooks/resources/useSettings"
import { useScores } from "../hooks/resources/useScores"
import { useWins } from "../hooks/resources/useWins"
import { OnGroupDataMessageArgs } from "@azure/web-pubsub-client"

type GameActiveProps = {
  lobbyId: string
  userId: string
}

export function GameActive({ lobbyId, userId }: GameActiveProps) {
  const { api } = useApi()
  const { lobby, leaveLobby } = useLobby()
  const { settings } = useSettings()
  const { wins } = useWins()
  const { scores } = useScores()
  const { ws } = useWebsocket()
  const { navigate } = useNavigation()

  const [initialGameState, setInitialGameState] = useState<Uint8Array>()
  const [localGameState, setLocalGameState] = useState<Uint8Array>()
  const [lost, setLost] = useState(false)
  const { countdown, start, stop } = useCountdown(() => setLost(false))
  const [won, setWon] = useState(false)
  const [minesRemaining, setMinesRemaining] = useState<number>()

  const [pendingCompetitionMoves, setPendingCompetitionMoves] = useState<
    Record<string, { reveals?: number[]; flagAdd?: number[]; flagRemove?: number[] }>
  >({})
  const [competitionState, setCompetitionState] = useState<Record<string, Uint8Array>>()

  // Load competitor boards if not already loaded
  useEffect(() => {
    // TODO: Move below into a resource hook
    ;(async () => {
      const [err, boards] = await api
        .boardGetAll(lobbyId)
        .then(([data]) => [null, data] as const)
        .catch((err: Error) => [err, null] as const)

      if (err || !boards) return

      if (Object.keys(boards).length > 0) {
        const states = Object.entries(boards)
          .map(([id, gameState]) => ({
            userId: id,
            gameState: new TextEncoder().encode(gameState)
          }))
          .filter(({ userId: id }) => id !== userId)
          .reduce((pre, cur) => ({ ...pre, [cur.userId]: cur.gameState }), {} as Record<string, Uint8Array>)

        setCompetitionState(prev => ({ ...prev, ...states }))
      }

      if (userId in boards) {
        const gameState = new TextEncoder().encode(boards[userId])
        setInitialGameState(gameState.map(v => State.unreveal(v)))
        setLocalGameState(gameState)
        // TODO: Handle lost/won states
      }
    })()
  }, [])

  // Determine if game has been won
  useEffect(() => {
    if (!localGameState) return
    if (won) return

    if (State.isCompleted(localGameState)) setWon(true)
  }, [localGameState])

  // Keep expected mine count up to date
  useEffect(() => {
    if (!localGameState || !settings) return

    const mines = localGameState.reduce(
      (c, v) => (State.isFlagged(v) || (State.isBomb(v) && State.isRevealed(v)) ? c + 1 : c),
      0
    )
    setMinesRemaining(settings.mines - mines)
  }, [localGameState, settings])

  // Send solution to server if game has been won
  useEffect(() => {
    if (!won || !localGameState) return

    api.boardSolution(lobbyId, userId, localGameState)
  }, [won, localGameState])

  // Apply pending competitor moves
  useEffect(() => {
    if (!pendingCompetitionMoves || !competitionState) return
    if (Object.keys(pendingCompetitionMoves).length === 0) return

    for (const id in pendingCompetitionMoves) {
      if (!competitionState[id]) continue

      const state = competitionState[id]
      const { reveals, flagAdd, flagRemove } = pendingCompetitionMoves[id]

      if (reveals) for (const i of reveals) state[i] = State.reveal(state[i])
      if (flagAdd) for (const i of flagAdd) state[i] = State.flag(state[i])
      if (flagRemove) for (const i of flagRemove) state[i] = State.removeFlag(state[i])

      setCompetitionState(prev => ({ ...prev, [id]: state }))
    }

    setPendingCompetitionMoves({})
  }, [pendingCompetitionMoves])

  // Use timeout to allow moves again
  useEffect(() => {
    if (!lost) return

    start(1000 * 3)
    return stop
  }, [lost])

  // Handle websocket events
  function isReveals(data: Websocket.Response.MoveAdd["data"]): data is { lobbyId: string; reveals: number[] } {
    return "reveals" in data && data.reveals !== null
  }

  function isFlagAdd(data: Websocket.Response.MoveAdd["data"]): data is { lobbyId: string; flagAdd: number } {
    return "flagAdd" in data && data.flagAdd !== null
  }

  function isFlagRemove(data: Websocket.Response.MoveAdd["data"]): data is { lobbyId: string; flagRemove: number } {
    return "flagRemove" in data && data.flagRemove !== null
  }

  useEffect(() => {
    if (!ws) return

    function onBoardCreate(e: OnGroupDataMessageArgs) {
      const data = e.message.data as Websocket.Message
      if (!isEvent<Websocket.Response.BoardCreate>("BOARD_CREATE", data)) return

      if (data.userId === userId) {
        if (data.data.reset) return

        const gameState = new TextEncoder().encode(data.data.gameState)
        setInitialGameState(gameState)
        setLocalGameState(gameState)
        setLost(false)
        setWon(false)
      } else {
        setCompetitionState(prev => ({ ...prev, [data.userId]: new TextEncoder().encode(data.data.gameState) }))
      }
    }

    function onMoveAdd(e: OnGroupDataMessageArgs) {
      const data = e.message.data as Websocket.Message
      if (!isEvent<Websocket.Response.MoveAdd>("MOVE_ADD", data)) return

      if (data.userId === userId) return

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
    }

    function onGameWon(e: OnGroupDataMessageArgs) {
      const data = e.message.data as Websocket.Message
      if (!isEvent<Websocket.Response.GameWon>("GAME_WON", data)) return

      navigate("GameCelebration", { lobbyId })

      // TODO: Change to stop control and tell everyone the game is over on a countdown
    }

    function onUserJoin(e: OnGroupDataMessageArgs) {
      if (!competitionState) return

      const data = e.message.data as Websocket.Message
      if (!isEvent<Websocket.Response.UserJoin>("USER_JOIN", data)) return // TODO: Move below into a resource hook
      ;(async () => {
        const [err, boards] = await api
          .boardGet(lobbyId, data.userId)
          .then(([data]) => [null, data] as const)
          .catch((err: Error) => [err, null] as const)

        if (err || !boards) return

        if (competitionState[data.userId]) return

        const state = new TextEncoder().encode(boards[data.userId])
        setCompetitionState(prev => ({ ...prev, [data.userId]: state }))
      })()
    }

    function onUserLeave(e: OnGroupDataMessageArgs) {
      const data = e.message.data as Websocket.Message
      if (!isEvent<Websocket.Response.UserLeave>("USER_LEAVE", data)) return

      setCompetitionState(prev => {
        const newState = { ...prev }
        delete newState[data.userId]
        return newState
      })
    }

    function onLobbyDelete(e: OnGroupDataMessageArgs) {
      const data = e.message.data as Websocket.Message
      if (!isEvent<Websocket.Response.LobbyDelete>("LOBBY_DELETE", data)) return

      alert("Your lobby has been closed due to inactivity") // TODO: Send proper alert (also shared in other game pages)
      navigate("MainMenu", {})
    }

    ws.on("group-message", onBoardCreate)
    ws.on("group-message", onMoveAdd)
    ws.on("group-message", onGameWon)
    ws.on("group-message", onUserJoin)
    ws.on("group-message", onUserLeave)
    ws.on("group-message", onLobbyDelete)

    return () => {
      ws.off("group-message", onBoardCreate)
      ws.off("group-message", onMoveAdd)
      ws.off("group-message", onGameWon)
      ws.off("group-message", onUserJoin)
      ws.off("group-message", onUserLeave)
      ws.off("group-message", onLobbyDelete)
    }
  }, [ws, competitionState])

  // Only render once all dependencies are loaded
  if (!lobby || !settings || !wins || !scores) return <Loading />

  function notifyMoveAdd(data: Omit<Websocket.Response.MoveAdd["data"], "lobbyId">) {
    if (!ws) return

    ws.sendEvent(
      "MOVE_ADD",
      {
        eventName: "MOVE_ADD",
        userId: userId,
        data: { ...data, lobbyId: lobbyId }
      },
      "json",
      { fireAndForget: true }
    )
  }

  async function reset() {
    setLocalGameState(initialGameState)
    await api.boardReset(lobbyId, userId)
  }

  async function skip() {
    await api.boardSkip(lobbyId, userId)
  }

  async function leave() {
    await leaveLobby()
    navigate("MainMenu", {})
  }

  if (!localGameState) return <Loading />

  return (
    <div id="game-active">
      <Text type="normal">Mines Remaining: {minesRemaining ?? ""}</Text>

      <div id="game-active-boards-container">
        <div id="game-active-play-area">
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

          <div id="game-active-countdown-text">
            <Text type="normal">{countdown ? `Recover in ${countdown}` : ""}</Text>
          </div>
        </div>

        {competitionState && Object.keys(competitionState).length > 0 && (
          <div id="game-active-competitors">
            {Object.entries(competitionState).map(([userId, state]) => (
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

      <Settings>
        <ButtonList>
          {/* <Box onClick={() => setLocalGameState(localGameState.map(v => (State.isBomb(v) ? v : State.reveal(v))))}>
            <Text type="big">Solve (DEV)</Text>
          </Box> */}
          <ButtonList horizontal>
            <Box onClick={reset}>
              <Text type="big">Reset</Text>
            </Box>
            <Box onClick={skip} disabled={settings.seed !== 0}>
              <Text type="big">Skip</Text>
            </Box>
          </ButtonList>
          <Box onClick={leave}>
            <Text type="big">Leave Party</Text>
          </Box>
        </ButtonList>
      </Settings>
    </div>
  )
}
