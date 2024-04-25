import React, { useEffect, useState } from "react"
import "./Tile.scss"
import { State } from "../../utils/State"
import { Flag } from "../ui/icons/Flag"
import { Bomb } from "../ui/icons/Bomb"

type TileProps = {
  index: number
  state: number
  lost: boolean
  onLeftClick: (i: number) => void
  onRightClick: (i: number) => void
}

export function Tile(props: TileProps) {
  const classes = (() => {
    if (State.isFlagged(props.state)) {
      return "tile-flagged"
    } else if (!State.isRevealed(props.state)) {
      return "tile-unrevealed"
    } else if (State.isBomb(props.state)) {
      return "tile-bomb"
    } else {
      return "tile-revealed"
    }
  })()

  const content = (() => {
    if (State.isFlagged(props.state)) {
      return <Flag color="off-bg" size={20} />
    } else if (!State.isRevealed(props.state)) {
      return null
    } else if (State.isBomb(props.state)) {
      return <Bomb color="off-bg" size={20} />
    } else if (State.isEmpty(props.state)) {
      return null
    } else {
      return State.getAdjacentBombCount(props.state)
    }
  })()

  const [isDown, setIsDown] = useState(false)

  useEffect(() => {
    if (!isDown) return

    const interval = setTimeout(() => {
      props.onRightClick(props.index)
      setIsDown(false)
    }, 500)

    return () => clearInterval(interval)
  }, [isDown])

  function onMouseDown(e: React.MouseEvent<HTMLButtonElement, MouseEvent>) {
    if (e.button !== 0) return
    setIsDown(true)
  }

  function onMouseUp() {
    if (!isDown) return
    setIsDown(false)

    props.onLeftClick(props.index)
  }

  function onMouseLeave() {
    setIsDown(false)
  }

  return (
    <button
      className={"tile " + classes}
      disabled={props.lost || State.isRevealed(props.state)}
      onMouseDown={onMouseDown}
      onMouseUp={onMouseUp}
      onMouseLeave={onMouseLeave}
      onContextMenu={e => (e.preventDefault(), props.onRightClick(props.index))}
    >
      {content}
    </button>
  )
}
