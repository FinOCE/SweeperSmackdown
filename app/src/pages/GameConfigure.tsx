import React, { ChangeEvent, useCallback, useEffect, useState } from "react"
import { useApi } from "../hooks/useApi"
import { useGameInfo } from "../hooks/useGameInfo"
import { useNavigation } from "../hooks/useNavigation"
import { useWebsocket } from "../hooks/useWebsocket"
import { Websocket } from "../types/Websocket"
import { isEvent } from "../utils/isEvent"
import { Api } from "../types/Api"
import { SliderInput } from "../components/SliderInput"

type Difficulty = "easy" | "normal" | "hard" | "hell"

export function GameConfigure() {
  const { navigate } = useNavigation()
  const { lobby, setLobby, userId } = useGameInfo()
  const ws = useWebsocket()
  const api = useApi()

  let [vote, setVote] = useState<Api.VoteGroup>()

  function getInitialVote(attempts: number = 0) {
    // Vote is created in orchestrator and may not exist instantly. This function
    // loops every few moments trying to get the vote until it exists to try work
    // around this. A proper solution on the backend would be ideal, but for now
    // this is fine.

    // TODO: Fix api so the vote is created on first load as well

    if (attempts > 5) throw new Error("Vailed to get initial vote")

    setTimeout(
      () =>
        api
          .voteGetAll()
          .then(setVote)
          .catch(_ => getInitialVote(++attempts)),
      500 * attempts
    )
  }

  useEffect(() => {
    if (!lobby || !userId) return

    getInitialVote()
  }, [lobby, userId])

  let [expiry, setExpiry] = useState<number | null>(null)
  let [countdown, setCountdown] = useState<number | null>(null)

  useEffect(() => {
    if (!expiry) setCountdown(null)

    const interval = expiry ? setInterval(() => setCountdown(Math.ceil((expiry - Date.now()) / 1000)), 100) : undefined
    const timeout = expiry ? setTimeout(() => navigate("GameActive"), expiry - Date.now()) : undefined

    return () => {
      clearInterval(interval)
      clearTimeout(timeout)
    }
  }, [expiry])

  useEffect(() => {
    if (!countdown) return

    // TODO: Play ticking down sound here (this triggers each second)
  }, [countdown])

  // Register websocket events
  ws.clear()

  ws.register("group-message", e => {
    const data = e.message.data as Websocket.Message
    if (!isEvent<Websocket.Response.VoteStateUpdate>("VOTE_STATE_UPDATE", data)) return

    setVote(data.data)
  })

  ws.register("group-message", e => {
    const data = e.message.data as Websocket.Message
    if (!isEvent<Websocket.Response.LobbyUpdate>("LOBBY_UPDATE", data)) return

    setLobby(data.data)
  })

  ws.register("group-message", e => {
    const data = e.message.data as Websocket.Message
    if (!isEvent<Websocket.Response.TimerStart>("TIMER_START", data)) return

    setExpiry(data.data.expiry)
  })

  ws.register("group-message", e => {
    const data = e.message.data as Websocket.Message
    if (!isEvent<Websocket.Response.TimerReset>("TIMER_RESET", data)) return

    setExpiry(null)
  })

  // Setup UI state
  const [settings, setSettings] = useState<Api.GameSettings>({
    mode: 0,
    height: 16,
    width: 16,
    mines: 40,
    lives: 0,
    timeLimit: 0,
    boardCount: 0,
    shareBoards: false
  })

  useEffect(() => {
    if (!lobby) return

    const timer = setTimeout(() => api.lobbyPatch(settings), 500)
    return () => clearTimeout(timer)
  }, [settings])

  const changeSetting = useCallback(
    (
      e: ChangeEvent<HTMLInputElement>,
      key: keyof Api.GameSettings,
      callback: (value: ChangeEvent<HTMLInputElement>) => any
    ) => setSettings(prev => ({ ...prev, [key]: callback(e) })),
    []
  )

  const [difficulty, setDifficulty] = useState<Difficulty>("normal")

  useEffect(() => {
    const ratio: Record<Difficulty, number> = {
      easy: 0.078125,
      normal: 0.15625,
      hard: 0.234375,
      hell: 0.3125
    }
    setSettings(prev => ({ ...prev, mines: Math.floor(prev.height * prev.width * ratio[difficulty]) }))
  }, [difficulty])

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
    await api.voteDelete(lobby?.hostId === userId)
    setVotePending(false)
  }

  async function voteForce() {
    setVotePending(true)
    await api.votePut("READY", true)
    setVotePending(false)
  }

  async function leaveLobby() {
    await api.userDelete()
    setLobby(null)
    navigate("MainMenu")
  }

  // Prevent rendering until TODO
  if (!lobby || !vote) return <div>Loading...</div>

  // Render page
  return (
    <div>
      <p>Welcome to lobby {lobby.lobbyId}</p>
      <p>Members: {lobby.userIds.join(", ")}</p>
      {countdown && <p>Starting in {countdown}</p>}

      <section id="controls">
        <fieldset>
          <legend>Mode</legend>

          <div>
            <input
              type="radio"
              name="mode"
              id="mode-0"
              value="0"
              checked={settings.mode === 0}
              onChange={e => changeSetting(e, "mode", Number)}
            />
            <label htmlFor="mode-0">Classic</label>
          </div>
        </fieldset>

        <fieldset>
          <legend>Difficulty</legend>

          <div>
            <input
              type="radio"
              name="difficulty"
              id="difficulty-easy"
              value="easy"
              checked={difficulty === "easy"}
              onChange={e => setDifficulty(e.target.value as Difficulty)}
            />
            <label htmlFor="mode-easy">Easy</label>
          </div>

          <div>
            <input
              type="radio"
              name="difficulty"
              id="difficulty-normal"
              value="normal"
              checked={difficulty === "normal"}
              onChange={e => setDifficulty(e.target.value as Difficulty)}
            />
            <label htmlFor="mode-normal">Normal</label>
          </div>

          <div>
            <input
              type="radio"
              name="difficulty"
              id="difficulty-hard"
              value="hard"
              checked={difficulty === "hard"}
              onChange={e => setDifficulty(e.target.value as Difficulty)}
            />
            <label htmlFor="mode-hard">Hard</label>
          </div>

          <div>
            <input
              type="radio"
              name="difficulty"
              id="difficulty-hell"
              value="hell"
              checked={difficulty === "hell"}
              onChange={e => setDifficulty(e.target.value as Difficulty)}
            />
            <label htmlFor="mode-hell">Hell</label>
          </div>
        </fieldset>

        <fieldset>
          <legend>Height</legend>

          <SliderInput
            name="height"
            id="height"
            min={9}
            max={100}
            value={settings.height}
            onChange={e => setSettings(prev => ({ ...prev, height: e }))}
          />
        </fieldset>

        <fieldset>
          <legend>Width</legend>

          <SliderInput
            name="width"
            id="width"
            min={9}
            max={100}
            value={settings.width}
            onChange={e => setSettings(prev => ({ ...prev, width: e }))}
          />
        </fieldset>

        <fieldset>
          <legend>Lives</legend>

          <SliderInput
            name="lives"
            id="lives"
            min={0}
            max={10}
            value={settings.lives}
            onChange={e => setSettings(prev => ({ ...prev, lives: e }))}
            display={v => (v !== 0 ? String(v) : "Unlimited")}
          />
        </fieldset>

        <fieldset>
          <legend>Time Limit</legend>

          <SliderInput
            name="timeLimit"
            id="timeLimit"
            min={0}
            max={600}
            step={10}
            value={settings.timeLimit}
            onChange={e => setSettings(prev => ({ ...prev, timeLimit: e }))}
            display={v => (v !== 0 ? `${v}s` : "Unlimited")}
          />
        </fieldset>

        <fieldset>
          <legend>Board Count</legend>

          <SliderInput
            name="boardCount"
            id="boardCount"
            min={0}
            max={25}
            value={settings.boardCount}
            onChange={e => setSettings(prev => ({ ...prev, boardCount: e }))}
            display={v => (v !== 0 ? String(v) : "Unlimited")}
          />
        </fieldset>

        <fieldset>
          <legend>Options</legend>

          <div>
            <input
              type="checkbox"
              name="shareBoards"
              id="shareBoards"
              checked={settings.shareBoards}
              onChange={e => changeSetting(e, "shareBoards", e => e.target.checked)}
            />
            <label htmlFor="shareBoards">Share Boards</label>
          </div>
        </fieldset>
      </section>

      <br />

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
      {isReady && (
        <input type="button" onClick={voteForce} value="Force Countdown" disabled={userId !== lobby.hostId} />
      )}
      <input type="button" onClick={leaveLobby} value="Leave Lobby" />
    </div>
  )
}
