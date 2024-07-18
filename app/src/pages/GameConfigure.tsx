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
  const { countdown, completed, start } = useCountdown()

  const [overrides, setOverrides] = useState<Partial<Api.GameSettings>>({})

  // Update server state whenever local state changes
  useDelay(
    () => {
      if (!lobby.resolved) return
      if (Object.keys(overrides).length === 0) return

      api.lobbySettingsPatch(lobbyId, {
        mode: overrides.mode,
        height: overrides.height,
        width: overrides.width,
        mines: overrides.mines,
        difficulty: overrides.difficulty,
        lives: overrides.lives,
        timeLimit: overrides.timeLimit,
        boardCount: overrides.boardCount,
        shareBoards: overrides.seed !== 0
      })

      setOverrides({})
    },
    500,
    [overrides, lobby.resolved]
  )

  // Start countdown when lobby starting
  useEffect(() => {
    if (!lobby.resolved) return
    if (lobby.status.status !== Api.Enums.ELobbyStatus.Starting || !lobby.status.statusUntil) return

    start(new Date(lobby.status.statusUntil).getTime() - Date.now())
  }, [lobby.status?.status, lobby.status?.statusUntil])

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

  // TODO: Add button to transfer host (show when hovering user in list??)

  // Show loading if not ready
  if (!participants || !user || !lobby.resolved) return <Loading />

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
              checked={(overrides.mode ?? lobby.settings.mode) === 0}
              onChange={() => setOverrides(prev => ({ ...prev, mode: 0 }))}
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
            value={overrides.height ?? lobby.settings.height}
            onChange={e => setOverrides(prev => ({ ...prev, height: e }))}
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
            value={overrides.width ?? lobby.settings.width}
            onChange={e => setOverrides(prev => ({ ...prev, width: e }))}
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
            value={overrides.lives ?? lobby.settings.lives}
            onChange={e => setOverrides(prev => ({ ...prev, lives: e }))}
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
            value={overrides.timeLimit ?? lobby.settings.timeLimit}
            onChange={e => setOverrides(prev => ({ ...prev, timeLimit: e }))}
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
            value={overrides.boardCount ?? lobby.settings.boardCount}
            onChange={e => setOverrides(prev => ({ ...prev, boardCount: e }))}
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
              checked={(overrides.seed ?? lobby.settings.seed) !== 0}
              onChange={e => setOverrides(prev => ({ ...prev, seed: e.target.checked ? 1 : 0 }))}
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
        ) : completed ? (
          <Text type="title">Starting...</Text>
        ) : user.id === lobby.hostId ? (
          <>
            {lobby.status.configureState === Api.Enums.EGameSettingsStateMachineState.Unlocked && (
              <Box onClick={lock}>
                <Text type="big">Ready</Text>
              </Box>
            )}
            {lobby.status.configureState === Api.Enums.EGameSettingsStateMachineState.Locked && (
              <ButtonList horizontal>
                <Box onClick={unlock}>
                  <Text type="big">Unready</Text>
                </Box>

                <Box onClick={confirm}>
                  <Text type="big">Confirm</Text>
                </Box>
              </ButtonList>
            )}
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
