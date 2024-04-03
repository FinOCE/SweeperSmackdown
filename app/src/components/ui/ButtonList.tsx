import React from "react"
import "./ButtonList.scss"

export function ButtonList(props: { children?: React.ReactNode; horizontal?: boolean }) {
  return <div className={props.horizontal ? "button-list-horizontal" : "button-list-vertical"}>{props.children}</div>
}
