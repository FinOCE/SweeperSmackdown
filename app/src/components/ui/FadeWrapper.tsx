import React, { ReactNode } from "react"
import "./FadeWrapper.scss"

type FadeWrapperProps = {
  children?: ReactNode
  animate: "in" | "out" | "off"
}

export function FadeWrapper(props: FadeWrapperProps) {
  return <div className={`fade-wrapper-${props.animate}`}>{props.children}</div>
}
