import React, { useState } from "react"
import { Tile } from "./Tile"
import "./Board.scss"
import { State } from "../../utils/State"
import { Separator } from "./Separator"
import { BoardStylingUtil } from "../../utils/BoardStylingUtil"

type BoardProps = {
  height: number
  width: number
  state: Uint8Array
  lost: boolean
}

export function Board(props: BoardProps) {
  const [localState, setLocalState] = useState(props.state)
  const [lost, setLost] = useState(props.lost)

  function onLeftClick(i: number) {
    if (lost) return

    // Prevent clicking on a flagged or revealed tile
    if (State.isFlagged(localState[i])) return
    if (State.isRevealed(localState[i])) return

    // Lose game if bomb
    if (State.isBomb(localState[i])) setLost(true)

    // Calculate tiles to reveal
    const reveals = [i]

    if (State.isEmpty(localState[i])) {
      const travelled: number[] = []

      const spread = (index: number) => {
        if (travelled.includes(index)) return

        travelled.push(index)
        reveals.push(index)

        if (State.isEmpty(localState[index])) {
          const width = props.width
          const height = props.height

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

    setLocalState(prev => prev.map((state, j) => (reveals.includes(j) ? State.reveal(state) : state)))

    // // Notify other users of move
    // ws.sendToLobby<Websocket.Response.MoveAdd>(lobby.lobbyId, {
    //   eventName: "MOVE_ADD",
    //   userId: user.id,
    //   data: { lobbyId: lobby.lobbyId, reveals }
    // })
  }

  function onRightClick(i: number) {
    if (lost) return

    // Prevent flagging a revealed tile
    if (State.isRevealed(localState[i])) return

    // Determine if flag is to be added or removed
    const isFlagged = State.isFlagged(localState[i])
    const newState = isFlagged ? State.removeFlag(localState[i]) : State.flag(localState[i])

    setLocalState(prev => prev.map((state, j) => (j === i ? newState : state)))

    // Notify other users of move
    // ws.sendToLobby<Websocket.Response.MoveAdd>(lobby.lobbyId, {
    //   eventName: "MOVE_ADD",
    //   userId: user.id,
    //   data: isFlagged ? { lobbyId: lobby.lobbyId, flagRemove: i } : { lobbyId: lobby.lobbyId, flagAdd: i }
    // })
  }

  function getIndex(x: number, y: number) {
    return y * props.width + x
  }

  return (
    <table cellPadding={0} cellSpacing={0} className="board" onContextMenu={e => e.preventDefault()}>
      <tbody>
        {Array.from({ length: props.height * 2 })
          .map((_, y) => Math.floor(y / 2))
          .map((y, yi) => (
            <tr key={`y${yi}`}>
              {Array.from({ length: props.width * 2 })
                .map((_, x) => Math.floor(x / 2))
                .map((x, xi) => (
                  <td key={`y${yi}x${xi}`}>
                    {yi === 0 || xi === 0 ? (
                      <></>
                    ) : yi % 2 === 0 || xi % 2 === 0 ? (
                      (() => {
                        const topLeft = getIndex(x - 1, y - 1)
                        const topRight = getIndex(x, y - 1)
                        const bottomLeft = getIndex(x - 1, y)
                        const bottomRight = getIndex(x, y)
                        const props = BoardStylingUtil.getProps(
                          xi,
                          yi,
                          localState[topLeft],
                          localState[topRight],
                          localState[bottomLeft],
                          localState[bottomRight]
                        )

                        return <Separator {...props} />
                      })()
                    ) : (
                      <Tile
                        index={getIndex(x, y)}
                        state={localState[getIndex(x, y)]}
                        lost={lost}
                        onLeftClick={onLeftClick}
                        onRightClick={onRightClick}
                      />
                    )}
                  </td>
                ))}
            </tr>
          ))}
      </tbody>
    </table>
  )
}
