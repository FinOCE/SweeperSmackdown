import React from "react"
import "./ButtonList.scss"

export function ButtonList(props: { children?: React.ReactNode }) {
  return <div className="button-list">{props.children}</div>
}
