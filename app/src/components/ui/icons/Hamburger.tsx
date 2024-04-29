import React from "react"
import "./Hamburger.scss"

type HamburgerProps = {
  size?: number
}

export function Hamburger(props: HamburgerProps) {
  return (
    <div className="hamburger" style={{ height: props.size ?? 50 }}>
      <div />
      <div />
      <div />
    </div>
  )
}
