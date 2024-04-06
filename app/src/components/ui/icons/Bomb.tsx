import React from "react"
import "./Bomb.scss"

type BombProps = {
  color: "off-bg" | "yellow"
  size?: number
}

export function Bomb(props: BombProps) {
  return (
    <div className={`bomb bomb-${props.color}`} style={{ height: props.size ?? 50 }}>
      <div className="bomb-line-vertical" />
      <div className="bomb-line-horizontal" />
      <div className="bomb-line-diagonal-up" />
      <div className="bomb-line-diagonal-down" />
    </div>
  )
}
