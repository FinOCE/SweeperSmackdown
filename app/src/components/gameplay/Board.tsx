import React, { Dispatch, SetStateAction } from "react"
import { Tile } from "./Tile"
import "./Board.scss"
import { State } from "../../utils/State"
import { Separator } from "./Separator"
import { BoardStylingUtil } from "../../utils/BoardStylingUtil"
import { Websocket } from "../../types/Websocket"

type BoardProps = {
  height: number
  width: number
  localState: Uint8Array
  setLocalState: Dispatch<SetStateAction<Uint8Array | undefined>>
  lost: boolean
  setLost: Dispatch<SetStateAction<boolean>>
  notifyMoveAdd: (data: Omit<Websocket.Response.MoveAdd["data"], "lobbyId">) => void
}

export function Board(props: BoardProps) {
  function onLeftClick(i: number) {
    if (props.lost) return

    // Prevent clicking on a flagged or revealed tile
    if (State.isFlagged(props.localState[i])) return
    if (State.isRevealed(props.localState[i])) return

    // Lose game if bomb
    if (State.isBomb(props.localState[i])) props.setLost(true)

    // Calculate tiles to reveal
    const reveals = [i]

    if (State.isEmpty(props.localState[i])) {
      const travelled: number[] = []

      const spread = (index: number) => {
        if (travelled.includes(index)) return
        travelled.push(index)

        if (State.isFlagged(props.localState[index])) return

        reveals.push(index)

        if (State.isEmpty(props.localState[index])) {
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

    props.setLocalState(prev => prev!.map((state, j) => (reveals.includes(j) ? State.reveal(state) : state)))

    // // Notify other users of move
    props.notifyMoveAdd({ reveals })
  }

  function onRightClick(i: number) {
    if (props.lost) return

    // Prevent flagging a revealed tile
    if (State.isRevealed(props.localState[i])) return

    // Determine if flag is to be added or removed
    const isFlagged = State.isFlagged(props.localState[i])
    const newState = isFlagged ? State.removeFlag(props.localState[i]) : State.flag(props.localState[i])

    props.setLocalState(prev => prev!.map((state, j) => (j === i ? newState : state)))

    // Notify other users of move
    props.notifyMoveAdd(isFlagged ? { flagRemove: i } : { flagAdd: i })
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
                        const boardProps = BoardStylingUtil.getProps(
                          xi,
                          yi,
                          props.localState[topLeft],
                          props.localState[topRight],
                          props.localState[bottomLeft],
                          props.localState[bottomRight]
                        )

                        return <Separator {...boardProps} />
                      })()
                    ) : (
                      <Tile
                        index={getIndex(x, y)}
                        state={props.localState[getIndex(x, y)]}
                        lost={props.lost}
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
