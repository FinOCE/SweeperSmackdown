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
  const [lobbyOverrides, setLobbyOverrides] = useState<Partial<{ hostManaged: boolean; hostId: string }>>({})

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

  useDelay(
    () => {
      if (!lobby.resolved || !user) return
      if (Object.keys(lobbyOverrides).length === 0 || lobby.hostId !== user.id) return

      api.lobbyPatch(lobbyId, {
        hostId: lobbyOverrides.hostId,
        hostManaged: lobbyOverrides.hostManaged
      })

      setLobbyOverrides({})
    },
    500,
    [lobbyOverrides, lobby.resolved, user]
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

      {/* TODO: Add way to transfer host to someone else from the list above */}

      <ButtonList>
        <fieldset>
          <legend>
            <Text>Mode</Text>
          </legend>

          {Object.entries({ Classic: 0 }).map(([name, key]) => (
            <div className="game-configure-selector-label-container" key={name}>
              <input
                type="radio"
                name="mode"
                id={`mode-${key}`}
                value={key}
                checked={(overrides.mode ?? lobby.settings.mode) === key}
                onChange={() => setOverrides(prev => ({ ...prev, mode: key }))}
              />
              <label htmlFor={`mode-${key}`}>
                <Text type="small">{name}</Text>
              </label>
            </div>
          ))}
        </fieldset>

        <fieldset>
          <legend>
            <Text>Mode</Text>
          </legend>

          {Object.entries({
            Easy: Api.Enums.EDifficulty.Easy,
            Normal: Api.Enums.EDifficulty.Normal,
            Hard: Api.Enums.EDifficulty.Hard,
            Hell: Api.Enums.EDifficulty.Hell
          }).map(([name, key]) => (
            <div className="game-configure-selector-label-container" key={name}>
              <input
                type="radio"
                name="difficulty"
                id={`difficulty-${key}`}
                value={key}
                checked={(overrides.difficulty ?? lobby.settings.difficulty) === key}
                onChange={() => setOverrides(prev => ({ ...prev, difficulty: key, mines: undefined }))}
              />
              <label htmlFor={`difficulty-${key}`}>
                <Text type="small">{name}</Text>
              </label>
            </div>
          ))}
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
            <Text>Mines</Text>
          </legend>

          <SliderInput
            name="mines"
            id="mines"
            min={Math.round(
              (overrides.width ?? lobby.settings.width) * (overrides.height ?? lobby.settings.height) * 0.078125
            )}
            max={Math.round(
              (overrides.width ?? lobby.settings.width) * (overrides.height ?? lobby.settings.height) * 0.3125
            )}
            value={overrides.mines ?? lobby.settings.mines}
            onChange={e => setOverrides(prev => ({ ...prev, mines: e, difficulty: undefined }))}
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

          <div className="game-configure-selector-label-container">
            <input
              type="checkbox"
              name="hostManaged"
              id="hostManaged"
              checked={lobbyOverrides.hostManaged ?? lobby.hostManaged}
              onChange={e => setLobbyOverrides(prev => ({ ...prev, hostManaged: e.target.checked }))}
            />
            <label htmlFor="hostManaged">
              <Text type="small">Host Managed</Text>
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
