import React, { useEffect, useState } from "react"
import "./GameConfigure.scss"
import { useApi } from "../hooks/useApi"
import { useNavigation } from "../hooks/useNavigation"
import { useWebsocket } from "../hooks/useWebsocket"
import { Websocket } from "../types/Websocket"
import { isEvent } from "../utils/isEvent"
import { Api } from "../types/Api"
import { SliderInput } from "../components/SliderInput"
import { useLobby } from "../hooks/useLobby"
import { Loading } from "../components/Loading"
import { LobbyWithoutNested } from "../types/Lobby"
import { Text } from "../components/ui/Text"
import { Box } from "../components/ui/Box"
import { ButtonList } from "../components/ui/ButtonList"
import { useCountdown } from "../hooks/useCountdown"
import { useEmbeddedAppSdk } from "../hooks/useEmbeddAppSdk"
import { UserList } from "../components/pages/GameConfigure/UserList"
import { isGuid } from "../utils/isGuid"
import { Settings } from "../components/ui/controls/Settings"

export function GameConfigure() {
  const { api } = useApi()
  const { participants, user } = useEmbeddedAppSdk()
  const { lobby, setLobby, leave, settings, setSettings } = useLobby()
  const ws = useWebsocket()
  const { navigate } = useNavigation()
  const { countdown, start, stop } = useCountdown(() => navigate("GameActive"))

  let [vote, setVote] = useState<Api.VoteGroup>()

  // Fetch current vote on load
  useEffect(() => {
    if (!user || !lobby) return

    api.voteGetAll(lobby.lobbyId).then(([votes]) => setVote(votes))
  }, [user, lobby?.lobbyId])

  // Create function to handle translatin between game settings and payloads
  type LocalSettings = Required<Omit<Api.Request.LobbyPatch, "hostId">>

  function settingsToPayload(settings: Api.GameSettings | null): LocalSettings {
    const base = {
      mode: 0,
      height: 16,
      width: 16,
      mines: 40,
      lives: 0,
      timeLimit: 0,
      boardCount: 0,
      shareBoards: false
    }

    return {
      mode: settings?.mode ?? base.mode,
      height: settings?.height ?? base.height,
      width: settings?.width ?? base.width,
      mines: settings?.mines ?? base.mines,
      lives: settings?.lives ?? base.lives,
      timeLimit: settings?.timeLimit ?? base.timeLimit,
      boardCount: settings?.boardCount ?? base.boardCount,
      shareBoards: settings?.seed ? settings.seed !== 0 : base.shareBoards
    }
  }

  function change(changes: { [K in keyof LocalSettings]?: LocalSettings[K] }) {
    setChanges(prev => ({ ...prev, ...changes }))
    setLocalSettings(prev => ({ ...prev, ...changes }))
  }

  // Setup local state
  const [localSettings, setLocalSettings] = useState<LocalSettings>(settingsToPayload(settings))
  const [changes, setChanges] = useState<Api.Request.LobbyPatch>({})
  const [votePending, setVotePending] = useState(false)

  // Update local state whenever server state changes
  useEffect(() => {
    if (!settings) return

    setLocalSettings(settingsToPayload(settings))
  }, [settings])

  // Update server state whenever local state changes
  useEffect(() => {
    if (!lobby) return

    const timer = setTimeout(() => {
      if (Object.keys(changes).length > 0) {
        api.lobbyPatch(lobby.lobbyId, changes)
        setChanges({})
      }
    }, 500)

    return () => clearTimeout(timer)
  }, [changes])

  // Show loading if not ready
  if (!participants || !user || !ws || !lobby || !settings || !vote) return <Loading />

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

    setLobby({ ...data.data, settings: undefined, scores: undefined, wins: undefined } as LobbyWithoutNested)
    setSettings(data.data.settings)
  })

  ws.register("group-message", e => {
    const data = e.message.data as Websocket.Message
    if (!isEvent<Websocket.Response.TimerStart>("TIMER_START", data)) return

    start(data.data.expiry - Date.now())
  })

  ws.register("group-message", e => {
    const data = e.message.data as Websocket.Message
    if (!isEvent<Websocket.Response.TimerReset>("TIMER_RESET", data)) return

    stop()
  })

  const isReady = vote?.votes?.READY?.includes(user.id) ?? false

  // Setup UI functions
  async function voteStart() {
    if (!lobby || !user) return

    setVotePending(true)
    await api.votePut(lobby.lobbyId, user.id, "READY").catch(() => {})
    setVotePending(false)
  }

  async function voteCancel() {
    if (!lobby || !user) return

    setVotePending(true)
    await api.voteDelete(lobby.lobbyId, user.id, lobby?.hostId === user.id).catch(() => {})
    setVotePending(false)
  }

  async function voteForce() {
    if (!lobby || !user) return

    setVotePending(true)
    await api.votePut(lobby.lobbyId, user.id, "READY", true).catch(() => {})
    setVotePending(false)
  }

  async function leaveLobby() {
    if (!lobby || !user) return

    await leave()
    navigate("MainMenu")
  }

  // Render page
  return (
    <div id="game-configure">
      {!isGuid(lobby.lobbyId) && (
        <div id="game-configure-header">
          <Text type="title">Party {lobby.lobbyId}</Text>
        </div>
      )}

      <div id="game-configure-participants">
        <UserList />
      </div>

      <ButtonList>
        <fieldset>
          <legend>
            <Text>Mode</Text>
          </legend>

          <div className="game-configure-selector-label-container">
            <input
              type="radio"
              name="mode"
              id="mode-0"
              value="0"
              checked={localSettings.mode === 0}
              onChange={() => change({ mode: 0 })}
            />
            <label htmlFor="mode-0">
              <Text type="small">Classic</Text>
            </label>
          </div>
        </fieldset>

        <fieldset>
          <legend>
            <Text>Height</Text>
          </legend>

          <SliderInput
            name="height"
            id="height"
            min={7}
            max={32}
            value={localSettings.height}
            onChange={e =>
              change({
                height: e,
                mines: Math.floor(e * localSettings.width * 0.15625)
              })
            }
          />
        </fieldset>

        <fieldset>
          <legend>
            <Text>Width</Text>
          </legend>

          <SliderInput
            name="width"
            id="width"
            min={7}
            max={32}
            value={localSettings.width}
            onChange={e =>
              change({
                width: e,
                mines: Math.floor(localSettings.height * e * 0.15625)
              })
            }
          />
        </fieldset>

        <fieldset>
          <legend>
            <Text>Lives</Text>
          </legend>

          <SliderInput
            name="lives"
            id="lives"
            min={0}
            max={10}
            value={localSettings.lives}
            onChange={e => change({ lives: e })}
            display={v => (v !== 0 ? String(v) : "Unlimited")}
          />
        </fieldset>

        <fieldset>
          <legend>
            <Text>Time Limit</Text>
          </legend>

          <SliderInput
            name="timeLimit"
            id="timeLimit"
            min={0}
            max={600}
            step={10}
            value={localSettings.timeLimit}
            onChange={e => change({ timeLimit: e })}
            display={v => (v !== 0 ? `${v}s` : "Unlimited")}
          />
        </fieldset>

        <fieldset>
          <legend>
            <Text>Board Count</Text>
          </legend>

          <SliderInput
            name="boardCount"
            id="boardCount"
            min={0}
            max={25}
            value={localSettings.boardCount}
            onChange={e => change({ boardCount: e })}
            display={v => (v !== 0 ? String(v) : "Unlimited")}
          />
        </fieldset>

        <fieldset>
          <legend>
            <Text>Options</Text>
          </legend>

          <div className="game-configure-selector-label-container">
            <input
              type="checkbox"
              name="shareBoards"
              id="shareBoards"
              checked={localSettings.shareBoards}
              onChange={e => change({ shareBoards: e.target.checked })}
            />
            <label htmlFor="shareBoards">
              <Text type="small">Share Boards</Text>
            </label>
          </div>
        </fieldset>
      </ButtonList>

      <br />

      <ButtonList>
        {countdown && (
          <div className="game-configure-countdown-container">
            <Text type="title">Starting in {countdown}</Text>
          </div>
        )}

        <Box onClick={isReady ? voteCancel : voteStart} disabled={votePending}>
          <Text type="big">
            {isReady
              ? `Cancel Vote (${vote.votes.READY.length}/${vote.requiredVotes})`
              : `Vote Start (${vote.votes.READY.length}/${vote.requiredVotes})`}
          </Text>
        </Box>
        {isReady && (
          <Box onClick={voteForce} disabled={user.id !== lobby.hostId}>
            <Text type="big">Force Countdown</Text>
          </Box>
        )}
      </ButtonList>

      <Settings>
        <ButtonList>
          <Box onClick={leaveLobby}>
            <Text type="big">Leave Party</Text>
          </Box>
        </ButtonList>
      </Settings>
    </div>
  )
}
