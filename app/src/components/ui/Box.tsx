import React, { ReactNode } from "react"
import "./Box.scss"

type BoxProps = {
  children?: ReactNode
  onClick?: () => void
  disabled?: boolean
  important?: boolean
}

export function Box(props: BoxProps) {
  return (
    <div
      className={`box-outer ${props.important ? "box-important" : ""} ${props.onClick ? "box-button" : ""} ${
        props.disabled ? "box-disabled" : ""
      }`}
      onClick={props.onClick}
    >
      <div className="box-border-highlight-container">
        <div className="box-border-highlight" />
      </div>
      <div className="box-inner">{props.children}</div>
    </div>
  )
}
