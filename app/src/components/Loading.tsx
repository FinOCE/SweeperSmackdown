import React from "react"
import "./Loading.scss"
import { Text } from "./ui/Text"

export function Loading(props: { hide?: boolean }) {
  return props.hide === true ? (
    <></>
  ) : (
    <div className="loading">
      <Text type="title">Loading...</Text>
    </div>
  )
}
