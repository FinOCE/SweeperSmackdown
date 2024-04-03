import React, { ReactNode } from "react"
import "./Box.scss"

type BoxProps = {
  children?: ReactNode
  onClick?: () => void
  disabled?: boolean
}

export function Box(props: BoxProps) {
  return (
    <div className={`box-outer ${props.onClick ? "box-button" : ""} ${props.disabled ? "box-disabled" : ""}`}>
      <div className="box-border-highlight-container">
        <div className="box-border-highlight" />
      </div>
      <div className="box-inner">{props.children}</div>
    </div>
  )
}
