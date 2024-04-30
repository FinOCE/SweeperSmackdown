import React, { ReactNode } from "react"
import "./Box.scss"

type BoxProps = {
  children?: ReactNode
  onClick?: () => void
  disabled?: boolean
  important?: boolean
  innerClass?: string
  ignoreColorOverwrite?: boolean
  type?: "bronze" | "silver" | "gold"
}

export function Box(props: BoxProps) {
  return (
    <div
      className={`box-outer ${props.important ? "box-important" : ""} ${props.onClick ? "box-button" : ""} ${
        props.disabled ? (props.ignoreColorOverwrite ? "box-disabled-nocolor" : "box-disabled") : ""
      } ${props.type ? `box-${props.type}` : ""}`}
      onClick={props.onClick}
    >
      <div className="box-border-highlight-container">
        <div className="box-border-highlight" />
      </div>
      <div className={`box-inner ${props.innerClass ?? ""}`}>{props.children}</div>
    </div>
  )
}
