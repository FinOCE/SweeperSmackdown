import React from "react"
import "./Flag.scss"

type FlagProps = {
  color: "off-bg"
  size?: number
}

export function Flag(props: FlagProps) {
  return (
    <div className={`flag flag-${props.color}`} style={{ height: props.size ?? 50 }}>
      <div className="flag-pole" />
      <div className="flag-cloth" />
    </div>
  )
}
