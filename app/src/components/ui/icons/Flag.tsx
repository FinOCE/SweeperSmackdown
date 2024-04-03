import React from "react"
import "./Flag.scss"

type FlagProps = {
  color: "off-bg"
}

export function Flag(props: FlagProps) {
  return (
    <div className={`flag flag-${props.color}`}>
      <div className="flag-pole" />
      <div className="flag-cloth" />
    </div>
  )
}
