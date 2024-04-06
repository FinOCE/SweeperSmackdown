import React from "react"
import "./Tile.scss"
import { Gameplay } from "../../types/Gameplay"
import { State } from "../../utils/State"

type TileProps = {
  index: number
  state: number
  lost: boolean
  // neighbours: Gameplay.TileNeighbours
  onLeftClick: (i: number) => void
  onRightClick: (i: number) => void
}

export function Tile(props: TileProps) {
  const classes = (() => {
    if (State.isFlagged(props.state)) {
      // return props.neighbours
      //   .map((n, i) => (i !== -1 && State.isFlagged(n) ? `tile-flagged-${i}` : ""))
      //   .filter(c => c !== "")
      //   .concat(["tile-flagged"])
      //   .join(" ")
      return "tile-flagged"
    } else if (!State.isRevealed(props.state)) {
      // return props.neighbours
      //   .map((n, i) => (i !== -1 && !State.isRevealed(n) ? `tile-unrevealed-${i}` : ""))
      //   .filter(c => c !== "")
      //   .concat(["tile-unrevealed"])
      //   .join(" ")
      return "tile-unrevealed"
    } else if (State.isBomb(props.state)) {
      return "tile-bomb"
    } else {
      return "tile-revealed"
    }
  })()

  const content = (() => {
    if (State.isFlagged(props.state)) {
      return "F"
    } else if (!State.isRevealed(props.state)) {
      return null
    } else if (State.isBomb(props.state)) {
      return "B"
    } else if (State.isEmpty(props.state)) {
      return null
    } else {
      return State.getAdjacentBombCount(props.state)
    }
  })()

  return (
    <button
      className={"tile " + classes}
      disabled={props.lost || State.isRevealed(props.state)}
      onClick={() => props.onLeftClick(props.index)}
      onContextMenu={e => (e.preventDefault(), props.onRightClick(props.index))}
    >
      {content}
    </button>
  )
}
