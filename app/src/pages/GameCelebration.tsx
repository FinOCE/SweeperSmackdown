import React, { useEffect, useState } from "react"
import "./GameCelebration.scss"
import { Loading } from "../components/Loading"
import { useNavigation } from "../hooks/useNavigation"
import { useEmbeddedAppSdk } from "../hooks/useEmbeddAppSdk"
import { Text } from "../components/ui/Text"
import { Box } from "../components/ui/Box"
import { Settings } from "../components/ui/controls/Settings"
import { ButtonList } from "../components/ui/ButtonList"
import { getDisplayDetails } from "../utils/getDisplayDetails"
import { ProfilePicture } from "../components/ui/users/ProfilePicture"
import { useLobby } from "../hooks/resources/useLobby"
import { useCountdown } from "../hooks/useCountdown"
import { Api } from "../types/Api"

type GameCelebrationProps = {
  lobbyId: string
}

export function GameCelebration({ lobbyId }: GameCelebrationProps) {
  const { user, participants } = useEmbeddedAppSdk()
  const { lobby, controls } = useLobby()
  const { navigate } = useNavigation()
  const { countdown, start, stop } = useCountdown(() => {})

  const [countdownExpiry, setCountdownExpiry] = useState<Date | null>(null)

  // Handle countdown timer
  useEffect(() => {
    if (!lobby.resolved) return
    if (lobby.status.status !== Api.Enums.ELobbyStatus.Celebrating || !lobby.status.statusUntil) return

    setCountdownExpiry(new Date(lobby.status.statusUntil))
  }, [lobby.status?.status, lobby.status?.statusUntil])

  useEffect(() => {
    if (!countdownExpiry) return

    start(countdownExpiry.getTime() - Date.now())
    return () => stop()
  }, [countdownExpiry])

  useEffect(() => {
    if (!lobby.resolved || !user) return
    if (lobby.status.status !== Api.Enums.ELobbyStatus.Celebrating) return

    navigate("GameConfigure", { lobbyId })
  }, [lobby.status?.status, user])

  // Load page until ready
  if (!user || !participants) return <Loading hide />

  const leaderboard = lobby.players.sort((a, b) => b.wins - a.wins)

  async function leave() {
    await controls.leave()
    navigate("MainMenu", {})
  }

  return (
    <div id="game-celebration">
      <div id="game-celebration-leaderboard-wrapper">
        <div id="game-celebration-podium">
          {leaderboard
            .filter((_, i) => i <= 2)
            .map((player, i) => {
              const details = getDisplayDetails(player, user, participants)

              return (
                <Box
                  innerClass={`game-celebration-podium-winner-${i + 1}`}
                  type={(["gold", "silver", "bronze"] as const)[i]}
                  key={player.id}
                >
                  <div>
                    <Text type="big">#{i + 1}</Text>
                  </div>
                  <div>
                    <ProfilePicture id={details.id} displayName={details.displayName} avatarUrl={details.avatarUrl} />
                  </div>
                  <div>
                    <Text type="big">{details.displayName}</Text>
                    <Text type="unset-color">{details.wins}</Text>
                  </div>
                </Box>
              )
            })}
        </div>
        <table cellPadding={0} cellSpacing={0} id="game-celebration-remaining">
          <tbody>
            {leaderboard
              .filter((_, i) => i > 2)
              .map((player, i) => {
                const details = getDisplayDetails(player, user, participants)

                return (
                  <tr key={player.id}>
                    <td>
                      <Text type="normal">#{i + 1 + 3}</Text>
                    </td>
                    <td>
                      <ProfilePicture id={details.id} displayName={details.displayName} avatarUrl={details.avatarUrl} />
                    </td>
                    <td>
                      <div className="game-celebration-remaining-stats">
                        <Text type="normal">{details.displayName}</Text>
                        <Text type="normal">{details.wins}</Text>
                      </div>
                    </td>
                  </tr>
                )
              })}
          </tbody>
        </table>
      </div>

      {countdown && (
        <div className="game-celebration-countdown-container">
          <Text type="normal">Next round will begin in {countdown}</Text>
        </div>
      )}

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
