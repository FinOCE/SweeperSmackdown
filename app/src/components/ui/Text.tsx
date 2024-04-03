import React from "react"
import "./Text.scss"

type TextProps = {
  children?: string | number | (string | number)[]
  type?: "title" | "big" | "normal" | "unset-color" | "small"
}

export function Text(props: TextProps) {
  return <span className={`text-${props.type ?? "normal"}`}>{props.children}</span>
}
