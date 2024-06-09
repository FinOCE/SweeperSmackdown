import React, { useEffect, useState } from "react"
import "./GameCelebration.scss"
import { useWebsocket } from "../hooks/useWebsocket"
import { Loading } from "../components/Loading"
import { Websocket } from "../types/Websocket"
import { isEvent } from "../utils/isEvent"
import { useNavigation } from "../hooks/useNavigation"
import { useEmbeddedAppSdk } from "../hooks/useEmbeddAppSdk"
import { Text } from "../components/ui/Text"
import { Box } from "../components/ui/Box"
import { Settings } from "../components/ui/controls/Settings"
import { ButtonList } from "../components/ui/ButtonList"
import { getDisplayDetails } from "../utils/getDisplayDetails"
import { ProfilePicture } from "../components/ui/users/ProfilePicture"
import { useLobby } from "../hooks/resources/useLobby"
import { useWins } from "../hooks/resources/useWins"
import { useScores } from "../hooks/resources/useScores"
import { OnGroupDataMessageArgs } from "@azure/web-pubsub-client"
import { useCountdown } from "../hooks/useCountdown"
import { Api } from "../types/Api"

type GameCelebrationProps = {
  lobbyId: string
}

export function GameCelebration({ lobbyId }: GameCelebrationProps) {
  const { user, participants } = useEmbeddedAppSdk()
  const { lobby, leaveLobby } = useLobby()
  const { wins } = useWins()
  const { scores } = useScores()
  const { ws } = useWebsocket()
  const { navigate } = useNavigation()
  const { countdown, start, stop } = useCountdown(() => {})

  const [countdownExpiry, setCountdownExpiry] = useState<Date | null>(null)

  // Handle countdown timer
  useEffect(() => {
    if (!ws) return

    function onGameCelebrationStarting(e: OnGroupDataMessageArgs) {
      const data = e.message.data as Websocket.Message
      if (!isEvent<Websocket.Response.GameCelebrationStarting>("GAME_CELEBRATION_STARTING", data)) return

      setCountdownExpiry(data.data)
    }

    ws.on("group-message", onGameCelebrationStarting)
    return () => ws.off("group-message", onGameCelebrationStarting)
  }, [ws])

  useEffect(() => {
    if (!countdownExpiry) return

    start(countdownExpiry.getTime() - Date.now())
    return () => stop()
  }, [countdownExpiry])

  useEffect(() => {
    if (!lobby) return

    if (lobby.state !== Api.Enums.ELobbyState.Celebrate) navigate("GameConfigure", { lobbyId })
  }, [lobby])

  // Load page until ready
  if (!user || !participants || !wins || !scores) return <Loading hide />

  const leaderboard = Object.entries({ ...wins })
    .sort((a, b) => b[1] - a[1])
    .map(([id]) => id)

  async function leave() {
    await leaveLobby()
    navigate("MainMenu", {})
  }

  return (
    <div id="game-celebration">
      <div id="game-celebration-leaderboard-wrapper">
        <div id="game-celebration-podium">
          {leaderboard
            .filter((_, i) => i <= 2)
            .map((id, i) => {
              const details = getDisplayDetails(id, user, participants, wins, scores)

              return (
                <Box
                  innerClass={`game-celebration-podium-winner-${i + 1}`}
                  type={(["gold", "silver", "bronze"] as const)[i]}
                  key={id}
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
              .map((id, i) => {
                const details = getDisplayDetails(id, user, participants, wins, scores)

                return (
                  <tr key={id}>
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
