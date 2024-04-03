import React from "react"
import "./DevPreview.scss"
import { Box } from "../components/ui/Box"
import { Text } from "../components/ui/Text"
import { RollingBackground } from "../components/ui/RollingBackground"
import { Bomb } from "../components/ui/icons/Bomb"

export function DevPreview() {
  return (
    <div id="dev-preview">
      <RollingBackground>
        <div className="dev-preview-content">
          <div className="dev-preview-title">
            <Bomb color="yellow" />
            <div>
              <Text type="title">Sweeper</Text>
              <br />
              <Text type="title">Smackdown</Text>
            </div>
          </div>

          <div className="dev-preview-menu">
            <Box onClick={() => {}} important>
              <Text type="big">Play In Discord Call</Text>
            </Box>
            <Box onClick={() => {}}>
              <Text type="big">Join Party</Text>
            </Box>
            <Box onClick={() => {}}>
              <Text type="big">Create Party</Text>
            </Box>
          </div>
        </div>
      </RollingBackground>
    </div>
  )
}
