import React from "react"
import "./GameCelebration.scss"
import { useWebsocket } from "../hooks/useWebsocket"
import { Loading } from "../components/Loading"
import { Websocket } from "../types/Websocket"
import { isEvent } from "../utils/isEvent"
import { useNavigation } from "../hooks/useNavigation"
import { useLobby } from "../hooks/useLobby"
import { useEmbeddedAppSdk } from "../hooks/useEmbeddAppSdk"
import { Text } from "../components/ui/Text"
import { Box } from "../components/ui/Box"
import { Settings } from "../components/ui/controls/Settings"
import { ButtonList } from "../components/ui/ButtonList"
import { getDisplayDetails } from "../utils/getDisplayDetails"
import { ProfilePicture } from "../components/ui/users/ProfilePicture"

export function GameCelebration() {
  const { user, participants } = useEmbeddedAppSdk()
  const { leave, wins, scores } = useLobby()
  const { manager: ws } = useWebsocket()
  const { navigate } = useNavigation()

  if (!ws || !user || !participants) return <Loading hide />

  const leaderboard = Object.entries({ ...wins })
    .sort((a, b) => b[1] - a[1])
    .map(([id]) => id)

  ws.register("group-message", e => {
    const data = e.message.data as Websocket.Message
    if (!isEvent<Websocket.Response.LobbyStart>("LOBBY_START", data)) return

    navigate("GameConfigure")
  })

  async function leaveParty() {
    await leave()
    navigate("MainMenu")
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
                  <tr>
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

      <Settings>
        <ButtonList>
          <Box onClick={leaveParty}>
            <Text type="big">Leave Party</Text>
          </Box>
        </ButtonList>
      </Settings>
    </div>
  )
}
