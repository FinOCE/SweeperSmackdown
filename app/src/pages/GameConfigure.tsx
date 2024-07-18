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
import { useDelay } from "../hooks/useDelay"

type GameConfigureProps = {
  lobbyId: string
}

export function GameConfigure({ lobbyId }: GameConfigureProps) {
  const { api } = useApi()
  const { participants, user } = useEmbeddedAppSdk()
  const { lobby, controls } = useLobby()
  const { navigate } = useNavigation()
  const { countdown, start, stop } = useCountdown(() => {
    setCountdownCompleted(true)
    setCountdownExpiry(null)
  })

  // Create function to handle translatin between game settings and payloads
  type LocalSettings = Required<Omit<Api.Request.GameSettingsPatch, "difficulty">>

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
  const [localSettings, setLocalSettings] = useState<LocalSettings>(settingsToPayload(lobby.settings))
  const [changes, setChanges] = useState<Api.Request.LobbyPatch>({})
  const [countdownExpiry, setCountdownExpiry] = useState<Date | null>(null)
  const [countdownCompleted, setCountdownCompleted] = useState(false)

  // Update local state whenever server state changes
  useEffect(() => {
    if (!lobby.resolved) return

    setLocalSettings(settingsToPayload(lobby.settings))
  }, [lobby])

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
    if (!lobby.resolved) return
    if (lobby.status.status !== Api.Enums.ELobbyStatus.Starting || !lobby.status.statusUntil) return

    setCountdownExpiry(new Date(lobby.status.statusUntil))
  }, [lobby.status?.status, lobby.status?.statusUntil])

  useEffect(() => {
    if (!countdownExpiry) return

    start(countdownExpiry.getTime() - Date.now())
    return () => stop()
  }, [countdownExpiry])

  // Show loading if not ready
  if (!participants || !user || !lobby.resolved) return <Loading />

  // Setup UI functions
  async function lock() {
    try {
      await controls.lock()
    } catch (err) {
      // TODO: Handle error
    }
  }

  async function unlock() {
    try {
      await controls.unlock()
    } catch (err) {
      // TODO: Handle error
    }
  }

  async function confirm() {
    try {
      await controls.confirm()
    } catch (err) {
      // TODO: Handle error
    }
  }

  async function leave() {
    await controls.leave()
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
          {lobby.players
            .map(player => getDisplayDetails(player, user, participants))
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
            <Box
              onClick={lock}
              disabled={lobby.status.configureState !== Api.Enums.EGameSettingsStateMachineState.Unlocked}
            >
              <Text type="big">Ready</Text>
            </Box>

            <Box
              onClick={unlock}
              disabled={lobby.status.configureState !== Api.Enums.EGameSettingsStateMachineState.Locked}
            >
              <Text type="big">Unready</Text>
            </Box>

            <Box
              onClick={confirm}
              disabled={lobby.status.configureState !== Api.Enums.EGameSettingsStateMachineState.Locked}
            >
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
