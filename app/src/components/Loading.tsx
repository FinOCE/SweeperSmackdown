import React from "react"
import { Text } from "./ui/Text"
import { Page } from "./ui/Page"

export function Loading(props: { hide?: boolean }) {
  return <Page>{props.hide === false && <Text type="title">Loading...</Text>}</Page>
}
