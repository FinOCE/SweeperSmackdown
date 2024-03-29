import React, { useState } from "react"
import { useApi } from "../hooks/useApi"
import { useGameInfo } from "../hooks/useGameInfo"
import { useWebsocket } from "../hooks/useWebsocket"
import { Websocket } from "../types/Websocket"

export function GameActive() {
  const { lobby } = useGameInfo()
  const ws = useWebsocket()
  const api = useApi()

  const [localGameState, setLocalGameState] = useState()

  ws.register("group-message", e => {
    const data = e.message.data as Websocket.Response.BoardCreate
    console.log(data.data)
  })

  if (!lobby) return <div>Loading...</div>

  return (
    <div>
      <table>
        <tbody>
          {Array.from({ length: lobby.settings.height }).map((_, y) => (
            <tr>
              {Array.from({ length: lobby.settings.width }).map((_, x) => {
                const i = y * lobby.settings.width + x
                return <td>{i}</td>
              })}
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  )
}
