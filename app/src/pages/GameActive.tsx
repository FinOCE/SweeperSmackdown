import React, { useEffect, useState } from "react"
import { useGameInfo } from "../hooks/useGameInfo"
import { useWebsocket } from "../hooks/useWebsocket"
import { Websocket } from "../types/Websocket"
import { State } from "../utils/State"
import { useApi } from "../hooks/useApi"
import { isEvent } from "../utils/isEvent"

export function GameActive() {
  const { lobby, lobbyId, userId } = useGameInfo()
  const ws = useWebsocket()
  const api = useApi()

  const [localInitialState, setLocalInitialState] = useState<Uint8Array>()
  const [localGameState, setLocalGameState] = useState<Uint8Array>()
  const [lost, setLost] = useState(false)
  const [won, setWon] = useState(false)

  useEffect(() => {
    if (!localGameState) return

    if (State.isCompleted(localGameState)) setWon(true)
  }, [localGameState])

  ws.clear()

  ws.register("group-message", e => {
    const data = e.message.data as Websocket.Message
    if (!isEvent<Websocket.Response.BoardCreate>("BOARD_CREATE", data)) return

    if (data.userId !== userId) return // TODO: Handle other users to see their progress

    const gameState = new TextEncoder().encode(data.data)
    setLocalInitialState(gameState)
    setLocalGameState(gameState)
    setLost(false)
    setWon(false)
  })

  if (!lobby || !localGameState) return <div>Loading...</div>

  const i = (x: number, y: number) => y * lobby.settings.width + x

  function render(state: number) {
    if (State.isFlagged(state)) return "F"
    else if (!State.isRevealed(state)) return "  "
    else if (State.isBomb(state)) return "B"
    else if (State.getAdjacentBombCount(state) > 0) return String(State.getAdjacentBombCount(state))
    else return "  "
  }

  function makeMove(i: number, flagging: boolean) {
    if (!localGameState || !lobby || lost) return

    const state = localGameState[i]

    if (flagging) {
      // Prevent flagging a revealed tile
      if (State.isRevealed(state)) return

      // Determine if flag is to be added or removed
      const isFlagged = State.isFlagged(state)
      const newState = isFlagged ? State.removeFlag(state) : State.flag(state)

      setLocalGameState(prev => prev!.map((state, j) => (i === j ? newState : state)))

      // Notify other users of move
      ws.sendToLobby<Websocket.Response.MoveAdd>(lobbyId!, {
        eventName: "MOVE_ADD",
        userId: userId!,
        data: isFlagged ? { flagRemove: i } : { flagAdd: i }
      })
    } else {
      // Prevent clicking on a flagged or revealed tile
      if (State.isFlagged(state)) return
      if (State.isRevealed(state)) return

      // Lose game if bomb
      if (State.isBomb(state)) setLost(true)

      // Calculate tiles to reveal
      const reveals = [i]

      if (State.isEmpty(state)) {
        const travelled: number[] = []

        const spread = (index: number) => {
          if (travelled.includes(index)) return

          travelled.push(index)
          reveals.push(index)

          if (State.isEmpty(localGameState[index])) {
            const width = lobby.settings.width
            const height = lobby.settings.height

            if (index % width !== 0) spread(index - 1)
            if (index % width !== width - 1) spread(index + 1)
            if (index >= width) spread(index - width)
            if (index < width * (height - 1)) spread(index + width)
            if (index % width !== 0 && index >= width) spread(index - width - 1)
            if (index % width !== width - 1 && index >= width) spread(index - width + 1)
            if (index % width !== 0 && index < width * (height - 1)) spread(index + width - 1)
            if (index % width !== width - 1 && index < width * (height - 1)) spread(index + width + 1)
          }
        }

        spread(i)
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

  async function reset() {
    await api.boardReset()
    setLocalGameState(localInitialState)
    setLost(false)
  }

  async function skip() {
    await api.boardSkip()
    setLost(false)
  }

  async function solve() {
    await api.boardSolution(localGameState!)
  }

  return (
    <div>
      <table>
        <tbody>
          {Array.from({ length: lobby.settings.height }).map((_, y) => (
            <tr key={`y${y}`}>
              {Array.from({ length: lobby.settings.width })
                .map((_, x) => localGameState[i(x, y)])
                .map((state, x) => (
                  <td key={i(x, y)}>
                    <input
                      type="button"
                      value={render(state)}
                      onClick={() => makeMove(i(x, y), false)}
                      onContextMenu={e => (e.preventDefault(), makeMove(i(x, y), true))}
                      disabled={lost || State.isRevealed(state)}
                    />
                  </td>
                ))}
            </tr>
          ))}
        </tbody>
      </table>

      <div>
        <input type="button" value="Reset" onClick={reset} />
        <input type="button" value="Skip" onClick={skip} />
        <input type="button" value="Solve" onClick={solve} /> {/* disabled={!won} */}
      </div>
    </div>
  )
}
