import React, { useState } from "react"
import { useGameInfo } from "../hooks/useGameInfo"
import { useWebsocket } from "../hooks/useWebsocket"
import { Websocket } from "../types/Websocket"
import { State } from "../utils/State"

export function GameActive() {
  const { lobby, lobbyId, userId } = useGameInfo()
  const ws = useWebsocket()

  const [localGameState, setLocalGameState] = useState<Uint8Array>()
  const [lost, setLost] = useState(false)

  ws.clear()

  ws.register("group-message", e => {
    const data = e.message.data as Websocket.Response.BoardCreate

    const gameState = new TextEncoder().encode(data.data)
    setLocalGameState(gameState)
  })

  if (!lobby || !localGameState) return <div>Loading...</div>

  const i = (x: number, y: number) => y * lobby.settings.width + x

  function render(state: number) {
    if (!State.isRevealed(state)) return "  "
    else if (State.isBomb(state)) return "B"
    else if (State.getAdjacentBombCount(state) > 0) return String(State.getAdjacentBombCount(state))
    else return "  "
  }

  function makeMove(i: number, flagging: boolean) {
    if (!localGameState || lost) return

    const state = localGameState[i]
    if (flagging) {
      setLocalGameState(prev => prev!.map((state, j) => (i === j ? State.flag(state) : state)))
      ws.sendToLobby<Websocket.Response.MoveAdd>(lobbyId!, {
        eventName: "MOVE_ADD",
        userId: userId!,
        data: { flags: [i] }
      })
    } else {
      // Lose game if bomb
      if (State.isBomb(state)) console.log("Lost!") // setLost(true)

      // Calculate tiles to reveal
      const reveals = [i]

      if (State.isEmpty(state)) {
        // TODO: branch out to find other tiles to reveal
      }

      setLocalGameState(prev => prev!.map((state, j) => (reveals.includes(j) ? State.reveal(state) : state)))

      // Notify other users of move
      ws.sendToLobby<Websocket.Response.MoveAdd>(lobbyId!, {
        eventName: "MOVE_ADD",
        userId: userId!,
        data: { reveals }
      })
    }
  }

  return (
    <div>
      <table>
        <tbody>
          {Array.from({ length: lobby.settings.height }).map((_, y) => (
            <tr>
              {/*  key={`y${y}`} */}
              {Array.from({ length: lobby.settings.width })
                .map((_, x) => localGameState[i(x, y)])
                .map((state, x) => (
                  <td>
                    {/*  key={i(x, y)} */}
                    <input
                      type="button"
                      value={render(state)}
                      onClick={e => makeMove(i(x, y), e.button !== 0)}
                      disabled={State.isRevealed(state)}
                    />
                  </td>
                ))}
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  )
}
