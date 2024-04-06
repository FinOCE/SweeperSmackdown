import React from "react"
import "./DevPreview.scss"
import { Board } from "../components/gameplay/Board"

export function DevPreview() {
  const height = 5
  const width = 5
  const lost = false
  const state = encodedState

  return <Board {...{ height, width, lost, state }} />
}

const encodedState = new Uint8Array("4 4 4 4 4 4 4 4 4 4 4 4 4 4 4 4 4 4 4 4 4 4 4 4 4".split(" ").map(p => Number(p)))

// const encodedState = new Uint8Array(
//   "8 36 36 8 36 4 0 4 4 4 4 8 8 4 0 0 36 16 12 12 4 4 0 4 36 4 4 36 36 4 0 0 4 8 36 8 4 0 4 8 12 8 8 8 8 4 0 0 4 8 16 36 8 0 4 36 8 36 4 4 4 4 0 0 12 36 16 36 12 4 4 4 8 4 4 4 36 4 0 0 36 36 12 8 36 4 0 0 4 4 4 4 4 4 0 0 8 8 4 4 4 4 0 0 4 36 4 0 4 4 4 0 0 0 0 0 0 0 0 0 4 4 4 0 4 36 8 4 0 0 0 4 4 4 0 0 0 0 0 4 8 12 12 36 0 0 0 4 36 8 8 8 4 0 4 8 36 8 36 8 0 0 0 4 4 8 36 36 12 8 8 36 8 8 4 4 0 0 0 0 0 4 16 36 36 12 36 8 4 0 4 4 4 4 8 4 8 4 16 36 24 36 8 4 0 0 4 36 8 36 12 36 8 36 12 36 36 8 4 0 0 4 8 8 8 36 16 8 12 4 8 8 8 4 0 0 0 4 36 4 4 4 8 36 4 0 0 0 0 0 0 0 0 4 4 4"
//     .split(" ")
//     .map(p => Number(p))
// )
