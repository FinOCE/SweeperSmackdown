import React, { useEffect, useState } from "react"
import "./GameConfigure.scss"
import { useApi } from "../hooks/useApi"
import { useNavigation } from "../hooks/useNavigation"
import { useWebsocket } from "../hooks/useWebsocket"
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
import { useVotes } from "../hooks/resources/useVotes"
import { useMembers } from "../hooks/resources/useMembers"

export function GameConfigure() {
  const { api } = useApi()
  const { participants, user } = useEmbeddedAppSdk()
  const { lobby, leaveLobby } = useLobby()
  const { settings } = useSettings()
  const { wins } = useWins()
  const { scores } = useScores()
  const { votes, requiredVotes, countdownExpiry, fetchVotes, clearCountdownExpiry } = useVotes()
  const { members } = useMembers()
  const { navigate } = useNavigation()
  const { countdown, start, stop } = useCountdown(() => {
    clearCountdownExpiry()
    navigate("GameActive")
  })

  // Fetch current vote on load
  useEffect(() => {
    if (!user || !lobby) return

    fetchVotes().catch((err: Error) => {
      alert("Failed to join the party. Please try again later.")
      console.error(err)
      navigate("MainMenu")
    })
  }, [user, lobby?.id])

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
        api.lobbyPatch(lobby.id, changes)
        setChanges({})
      }
    }, 500)

    return () => clearTimeout(timer)
  }, [changes])

  // Handle countdown timer
  useEffect(() => {
    if (!countdownExpiry) return

    start(countdownExpiry - Date.now())
    return () => stop()
  }, [countdownExpiry])

  // Show loading if not ready
  if (!participants || !user || !lobby || !settings || !votes || !wins || !scores) return <Loading />

  // Setup UI functions
  const isReady = votes?.READY?.includes(user.id) ?? false

  async function voteStart() {
    if (!lobby || !user) return

    setVotePending(true)
    await api.votePut(lobby.id, user.id, "READY").catch(() => {})
    setVotePending(false)
  }

  async function voteCancel() {
    if (!lobby || !user) return

    setVotePending(true)
    await api.voteDelete(lobby.id, user.id, lobby.hostId === user.id).catch(() => {})
    setVotePending(false)
  }

  async function voteForce() {
    if (!lobby || !user) return

    setVotePending(true)
    await api.votePut(lobby.id, user.id, "READY", true).catch(() => {})
    setVotePending(false)
  }

  async function leave() {
    if (!lobby || !user) return

    await leaveLobby()
    navigate("MainMenu")
  }

  // Render page
  return (
    <div id="game-configure">
      {!isGuid(lobby.id) && (
        <div id="game-configure-header">
          <Text type="title">Party {lobby.id}</Text>
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
        <div className="game-configure-countdown-container">
          <Text type="title">{countdown ? `Starting in ${countdown}` : " "}</Text>
        </div>

        <Box onClick={isReady ? voteCancel : voteStart} disabled={votePending}>
          <Text type="big">
            {isReady
              ? `Cancel Vote (${votes.READY.length}/${requiredVotes})`
              : `Vote Start (${votes.READY.length}/${requiredVotes})`}
          </Text>
        </Box>
        {/* {isReady && (
          <Box onClick={voteForce} disabled={user.id !== lobby.hostId}>
            <Text type="big">Force Countdown</Text>
          </Box>
        )} */}
        {/* Temporarily disabled force countdown due to being very buggy */}
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
