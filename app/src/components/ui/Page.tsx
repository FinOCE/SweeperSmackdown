import React, { ReactNode } from "react"
import "./Page.scss"

export function Page(props: { children?: ReactNode; fade?: boolean }) {
  return (
    <div className={`page ${props.fade ? "page-fade" : ""}`}>
      <div className="page-content">{props.children}</div>
    </div>
  )
}
