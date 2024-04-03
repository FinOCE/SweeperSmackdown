import React from "react"
import "./Bomb.scss"

type BombProps = {
  color: "off-bg" | "yellow"
}

export function Bomb(props: BombProps) {
  return (
    <div className={`bomb bomb-${props.color}`}>
      <div className="bomb-line-vertical" />
      <div className="bomb-line-horizontal" />
      <div className="bomb-line-diagonal-up" />
      <div className="bomb-line-diagonal-down" />
    </div>
  )
}
