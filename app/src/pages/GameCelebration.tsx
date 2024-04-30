import React from "react"
import "./GameCelebration.scss"
import { useWebsocket } from "../hooks/useWebsocket"
import { Loading } from "../components/Loading"
import { Websocket } from "../types/Websocket"
import { isEvent } from "../utils/isEvent"
import { useNavigation } from "../hooks/useNavigation"
import { useLobby } from "../hooks/useLobby"
import _ from "lodash"
import { useEmbeddedAppSdk } from "../hooks/useEmbeddAppSdk"
import { Text } from "../components/ui/Text"
import { Box } from "../components/ui/Box"
import { UserList } from "../components/pages/GameConfigure/UserList"
import { Settings } from "../components/ui/controls/Settings"
import { ButtonList } from "../components/ui/ButtonList"

export function GameCelebration() {
  const { user, participants } = useEmbeddedAppSdk()
  const { leave, wins } = useLobby()
  const ws = useWebsocket()
  const { navigate } = useNavigation()

  if (!ws || !user || !participants) return <Loading hide />

  const leaderboard = Object.entries({ ...wins })
    // .concat([
    //   ["1111", 3],
    //   ["2222", 2],
    //   ["3333", 1],
    //   ["4444", 1],
    //   ["5555", 5],
    //   ["6666", 5]
    // ])
    .sort((a, b) => b[1] - a[1])
    .map(([id, wins]) => ({ id, wins }))

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
      <div id="game-celebration-podium">
        {leaderboard
          .filter((_, i) => i <= 2)
          .map((player, i) => (
            <Box innerClass={`game-celebration-podium-winner-${i + 1}`}>
              <div>
                <Text type="big">{i + 1}</Text>
              </div>
              <div>
                <Text type="big">{player.id}</Text>
                <Text type="unset-color">{player.wins}</Text>
              </div>
            </Box>
          ))}
      </div>
      <table
        cellPadding={0}
        cellSpacing={0}
        //</div>id="game-celebration-remaining"
      >
        <tbody>
          {leaderboard
            .filter((_, i) => i > 2)
            .map((player, i) => (
              <tr>
                <td>
                  <Text type="normal">{i + 1}</Text>
                </td>
                <td>
                  <Text type="normal">{player.id}</Text>
                </td>
                <td>
                  <Text type="normal">{player.wins}</Text>
                </td>
              </tr>
            ))}
        </tbody>
      </table>

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
