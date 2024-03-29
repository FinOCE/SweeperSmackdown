import React, { useState } from "react"
import { useGameInfo } from "../hooks/useGameInfo"
import { useWebsocket } from "../hooks/useWebsocket"
import { Websocket } from "../types/Websocket"
import { State } from "../utils/State"

export function GameActive() {
  const { lobby } = useGameInfo()
  const ws = useWebsocket()

  const [localGameState, setLocalGameState] = useState<Uint8Array>()

  ws.clear()

  ws.register("group-message", e => {
    const data = e.message.data as Websocket.Response.BoardCreate

    const gameState = new TextEncoder().encode(data.data)
    setLocalGameState(gameState)
  })

  if (!lobby || !localGameState) return <div>Loading...</div>

  const i = (x: number, y: number) => y * lobby.settings.width + x

  function render(state: number) {
    if (State.isBomb(state)) return "B"
    else if (State.getAdjacentBombCount(state) > 0) return String(State.getAdjacentBombCount(state))
    else return "  "
  }

  function makeMove(i: number) {
    // TODO: Implement sending moves over websocket
    console.log(`Clicked ${i} (${i % lobby!.settings.width},${Math.floor(i / lobby!.settings.width)})`)
  }

  return (
    <div>
      <table>
        <tbody>
          {Array.from({ length: lobby.settings.height }).map((_, y) => (
            <tr key={`y${y}`}>
              {Array.from({ length: lobby.settings.width }).map((_, x) => (
                <td key={i(x, y)}>
                  <input type="button" value={render(localGameState[i(x, y)])} onClick={() => makeMove(i(x, y))} />
                </td>
              ))}
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  )
}
