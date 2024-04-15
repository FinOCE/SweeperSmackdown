import React from "react"
import "./BoardPreview.scss"
import { Api } from "../../types/Api"
import { State } from "../../utils/State"

type BoardPreviewProps = {
  userId: string
  settings: Api.GameSettings
  state: Uint8Array
}

export function BoardPreview(props: BoardPreviewProps) {
  return (
    <div className="game-active-competitor-board-container">
      <table cellPadding={0} cellSpacing={0}>
        <tbody>
          {Array.from({ length: 7 }).map((_, y) => (
            <tr key={`${props.userId}y${y}`}>
              {Array.from({ length: 7 }).map((_, x) => {
                const tileIndexHeight = props.settings.height / 7
                const tileIndexWidth = props.settings.width / 7

                const indices: number[] = []

                for (let k = y * tileIndexHeight; k < (y + 1) * tileIndexHeight; k++)
                  for (let j = x * tileIndexWidth; j < (x + 1) * tileIndexWidth; j++)
                    indices.push(Math.floor(j) + Math.floor(k) * props.settings.width)

                const isRevealed =
                  indices.filter(i => State.isRevealed(props.state[i]) || State.isFlagged(props.state[i])).length >
                  indices.length / 2

                return (
                  <td key={`${props.userId}y${y}x${x}`}>
                    <div className={`game-active-competitor-tile-${isRevealed ? "revealed" : "hidden"}`} />
                  </td>
                )
              })}
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  )
}
