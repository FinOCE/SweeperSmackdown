import React from "react"
import "./Text.scss"

type TextProps = {
  children?: string
  type: "title" | "big"
}

export function Text(props: TextProps) {
  return <span className={`text-${props.type}`}>{props.children}</span>
}
