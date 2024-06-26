import React, { useEffect, useState } from "react"
import "./GameConfigure.scss"
import { useApi } from "../hooks/useApi"
import { useNavigation } from "../hooks/useNavigation"
import { Api } from "../types/Api"
import { SliderInput } from "../components/SliderInput"
import { Loading } from "../components/Loading"
import { Text } from "../components/ui/Text"
import { Box } from "../components/ui/Box"
import { ButtonList } from "../components/ui/ButtonList"
import { useCountdown } from "../hooks/useCountdown"
import { useEmbeddedAppSdk } from "../hooks/useEmbeddAppSdk"
import { isGuid } from "../utils/isGuid"
import { Settings } from "../components/ui/controls/Settings"
import { getDisplayDetails } from "../utils/getDisplayDetails"
import { ProfilePicture } from "../components/ui/users/ProfilePicture"
import { useLobby } from "../hooks/resources/useLobby"
import { useSettings } from "../hooks/resources/useSettings"
import { useWins } from "../hooks/resources/useWins"
import { useScores } from "../hooks/resources/useScores"
import { useMembers } from "../hooks/resources/useMembers"
import { useWebsocket } from "../hooks/useWebsocket"
import { isEvent } from "../utils/isEvent"
import { Websocket } from "../types/Websocket"
import { OnGroupDataMessageArgs } from "@azure/web-pubsub-client"
import { useDelay } from "../hooks/useDelay"

type GameConfigureProps = {
  lobbyId: string
}

export function GameConfigure({ lobbyId }: GameConfigureProps) {
  const { api } = useApi()
  const { participants, user } = useEmbeddedAppSdk()
  const { ws } = useWebsocket()
  const { lobby, leaveLobby, lockLobby, unlockLobby, confirmLobby } = useLobby()
  const { settings } = useSettings()
  const { wins } = useWins()
  const { scores } = useScores()
  const { members } = useMembers()
  const { navigate } = useNavigation()
  const { countdown, start, stop } = useCountdown(() => {
    setCountdownCompleted(true)
    setCountdownExpiry(null)
  })

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
  const [countdownExpiry, setCountdownExpiry] = useState<Date | null>(null)
  const [countdownCompleted, setCountdownCompleted] = useState(false)

  // Update local state whenever server state changes
  useEffect(() => {
    if (!settings) return

    setLocalSettings(settingsToPayload(settings))
  }, [settings])

  // Update server state whenever local state changes
  useDelay(
    () => {
      if (Object.keys(changes).length === 0) return

      api.lobbyPatch(lobbyId, changes)
      setChanges({})
    },
    500,
    [changes]
  )

  // Handle countdown timer
  useEffect(() => {
    if (!ws) return

    function onGameStarting(e: OnGroupDataMessageArgs) {
      const data = e.message.data as Websocket.Message
      if (!isEvent<Websocket.Response.GameStarting>("GAME_STARTING", data)) return

      setCountdownExpiry(new Date(data.data))
    }

    function onLobbyDelete(e: OnGroupDataMessageArgs) {
      const data = e.message.data as Websocket.Message
      if (!isEvent<Websocket.Response.LobbyDelete>("LOBBY_DELETE", data)) return

      alert("Your lobby has been closed due to inactivity") // TODO: Send proper alert (also shared in other game pages)
      navigate("MainMenu", {})
    }

    ws.on("group-message", onGameStarting)
    ws.on("group-message", onLobbyDelete)

    return () => {
      ws.off("group-message", onGameStarting)
      ws.off("group-message", onLobbyDelete)
    }
  }, [ws])

  useEffect(() => {
    if (!countdownExpiry) return

    start(countdownExpiry.getTime() - Date.now())
    return () => stop()
  }, [countdownExpiry])

  useEffect(() => {
    if (!lobby || !user) return

    if (lobby.state === Api.Enums.ELobbyState.Play) navigate("GameActive", { lobbyId, userId: user.id })
  }, [lobby])

  // Show loading if not ready
  if (!participants || !user || !lobby || !settings || !wins || !scores) return <Loading />

  // Setup UI functions
  async function lock() {
    try {
      await lockLobby()
    } catch (err) {
      // TODO: Handle error
    }
  }

  async function unlock() {
    try {
      await unlockLobby()
    } catch (err) {
      // TODO: Handle error
    }
  }

  async function confirm() {
    try {
      await confirmLobby()
    } catch (err) {
      // TODO: Handle error
    }
  }

  async function leave() {
    await leaveLobby()
    navigate("MainMenu", {})
  }

  // Render page
  return (
    <div id="game-configure">
      {!isGuid(lobbyId) && (
        <div id="game-configure-header">
          <Text type="title">Party {lobbyId}</Text>
        </div>
      )}

      <div id="game-configure-participants">
        <div className="game-configure-participants-list">
          {(members ?? [])
            .map(member => getDisplayDetails(member.id, user, participants, wins, scores))
            .map(({ id, displayName, avatarUrl }) => (
              <ProfilePicture {...{ id, displayName, avatarUrl }} key={id} />
            ))}
        </div>
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
            onChange={e => change({ height: e })}
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
            onChange={e => change({ width: e })}
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
        {countdown ? (
          <div className="game-configure-countdown-container">
            <Text type="title">Starting in {countdown}</Text>
          </div>
        ) : countdownCompleted ? (
          <>{/* This state is when countdown has been triggered then completed to show nothing here */}</>
        ) : lobby.hostId === user.id ? (
          <>
            <Box onClick={lock} disabled={lobby.state !== Api.Enums.ELobbyState.ConfigureUnlocked}>
              <Text type="big">Ready</Text>
            </Box>

            <Box onClick={unlock} disabled={lobby.state !== Api.Enums.ELobbyState.ConfigureLocked}>
              <Text type="big">Unready</Text>
            </Box>

            <Box onClick={confirm} disabled={lobby.state !== Api.Enums.ELobbyState.ConfigureLocked}>
              <Text type="big">Confirm</Text>
            </Box>

            {/* TODO: Clean up above to only show when needed and neater */}
          </>
        ) : (
          <Text type="title">Waiting for host to start...</Text>

          /* TODO: Add fun button to tell host to hurry up! */
        )}
      </ButtonList>

      <Settings>
        <ButtonList>
          <Box onClick={leave}>
            <Text type="big">Leave Party</Text>
          </Box>
        </ButtonList>
      </Settings>
    </div>
  )
}
