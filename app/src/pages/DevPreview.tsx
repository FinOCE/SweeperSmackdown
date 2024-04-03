import React from "react"
import "./DevPreview.scss"
import { Box } from "../components/ui/Box"
import { Text } from "../components/ui/Text"

export function DevPreview() {
  return (
    <div id="dev-preview">
      <Text type="title">Sweeper Smackdown</Text>

      <div className="menu">
        <Box onClick={() => {}}>
          <Text type="big">Join Party</Text>
        </Box>
        <Box onClick={() => {}}>
          <Text type="big">Create Party</Text>
        </Box>
      </div>
    </div>
  )
}
