import React, { useEffect, useState } from "react"
import "./GameActive.scss"
import { useWebsocket } from "../hooks/useWebsocket"
import { Websocket } from "../types/Websocket"
import { State } from "../utils/State"
import { useApi } from "../hooks/useApi"
import { isEvent } from "../utils/isEvent"
import { useLobby } from "../hooks/useLobby"
import { useUser } from "../hooks/useUser"
import { Loading } from "../components/Loading"
import { Page } from "../components/ui/Page"
import { RollingBackground } from "../components/ui/RollingBackground"
import { ButtonList } from "../components/ui/ButtonList"
import { Box } from "../components/ui/Box"
import { Text } from "../components/ui/Text"
import { useNavigation } from "../hooks/useNavigation"

export function GameActive() {
  const { api } = useApi()
  const user = useUser()
  const { lobby, settings, leave } = useLobby()
  const ws = useWebsocket()
  const { navigate } = useNavigation()

  const [localGameState, setLocalGameState] = useState<Uint8Array>()
  const [lost, setLost] = useState(false)
  const [won, setWon] = useState(false)

  const [pendingCompetitionMoves, setPendingCompetitionMoves] = useState<
    Record<string, { reveals?: number[]; flagAdd?: number[]; flagRemove?: number[] }>
  >({})
  const [competitionState, setCompetitionState] = useState<Record<string, Uint8Array>>()

  // Determine if game has been won
  useEffect(() => {
    if (!localGameState) return

    if (State.isCompleted(localGameState)) setWon(true)
  }, [localGameState])

  // Send solution to server if game has been won
  useEffect(() => {
    if (!won || !lobby || !user) return

    api.boardSolution(lobby.lobbyId, user.id, localGameState!)
  }, [won, lobby, user])

  // Apply pending competitor moves
  useEffect(() => {
    if (!pendingCompetitionMoves || !competitionState) return
    if (Object.keys(pendingCompetitionMoves).length === 0) return

    for (const userId in pendingCompetitionMoves) {
      if (!competitionState[userId]) continue

      const state = competitionState[userId]
      const { reveals, flagAdd, flagRemove } = pendingCompetitionMoves[userId]

      if (reveals) for (const i of reveals) state[i] = State.reveal(state[i])
      if (flagAdd) for (const i of flagAdd) state[i] = State.flag(state[i])
      if (flagRemove) for (const i of flagRemove) state[i] = State.removeFlag(state[i])

      setCompetitionState(prev => ({ ...prev, [userId]: state }))
    }

    setPendingCompetitionMoves({})
  }, [pendingCompetitionMoves])

  // Only render once all dependencies are loaded
  if (!ws || !user || !lobby || !settings) return <Loading />

  ws.clear()

  ws.register("group-message", e => {
    const data = e.message.data as Websocket.Message
    if (!isEvent<Websocket.Response.BoardCreate>("BOARD_CREATE", data)) return

    if (data.userId === user.id) {
      const gameState = new TextEncoder().encode(data.data)
      setLocalGameState(gameState)
      setLost(false)
      setWon(false)
    } else {
      setCompetitionState(prev => ({ ...prev, [data.userId]: new TextEncoder().encode(data.data) }))
    }
  })

  function isReveals(data: Websocket.Response.MoveAdd["data"]): data is { lobbyId: string; reveals: number[] } {
    return "reveals" in data && data.reveals !== null
  }

  function isFlagAdd(data: Websocket.Response.MoveAdd["data"]): data is { lobbyId: string; flagAdd: number } {
    return "flagAdd" in data && data.flagAdd !== null
  }

  function isFlagRemove(data: Websocket.Response.MoveAdd["data"]): data is { lobbyId: string; flagRemove: number } {
    return "flagRemove" in data && data.flagRemove !== null
  }

  // Queue competitior moves into pending moves to be updated
  ws.register("group-message", e => {
    const data = e.message.data as Websocket.Message
    if (!isEvent<Websocket.Response.MoveAdd>("MOVE_ADD", data)) return

    if (data.userId === user.id) return

    if (isReveals(data.data)) {
      const { reveals } = data.data
      setPendingCompetitionMoves(prev => ({
        ...prev,
        [data.userId]: { ...prev[data.userId], reveals: [...(prev[data.userId]?.reveals ?? []), ...reveals] }
      }))
    } else if (isFlagAdd(data.data)) {
      const { flagAdd } = data.data
      setPendingCompetitionMoves(prev => ({
        ...prev,
        [data.userId]: { ...prev[data.userId], flagAdd: [...(prev[data.userId]?.flagAdd ?? []), flagAdd] }
      }))
    } else if (isFlagRemove(data.data)) {
      const { flagRemove } = data.data
      setPendingCompetitionMoves(prev => ({
        ...prev,
        [data.userId]: { ...prev[data.userId], flagRemove: [...(prev[data.userId]?.flagRemove ?? []), flagRemove] }
      }))
    }
  })

  const i = (x: number, y: number) => y * settings.width + x

  function render(state: number) {
    if (State.isFlagged(state)) return "F"
    else if (!State.isRevealed(state)) return "  "
    else if (State.isBomb(state)) return "B"
    else if (State.getAdjacentBombCount(state) > 0) return String(State.getAdjacentBombCount(state))
    else return "  "
  }

  function makeMove(i: number, flagging: boolean) {
    if (!localGameState || !ws || !lobby || !settings || !user || lost) return

    const state = localGameState[i]

    if (flagging) {
      // Prevent flagging a revealed tile
      if (State.isRevealed(state)) return

      // Determine if flag is to be added or removed
      const isFlagged = State.isFlagged(state)
      const newState = isFlagged ? State.removeFlag(state) : State.flag(state)

      setLocalGameState(prev => prev!.map((state, j) => (i === j ? newState : state)))

      // Notify other users of move
      ws.sendToLobby<Websocket.Response.MoveAdd>(lobby.lobbyId, {
        eventName: "MOVE_ADD",
        userId: user.id,
        data: isFlagged ? { lobbyId: lobby.lobbyId, flagRemove: i } : { lobbyId: lobby.lobbyId, flagAdd: i }
      })
    } else {
      // Prevent clicking on a flagged or revealed tile
      if (State.isFlagged(state)) return
      if (State.isRevealed(state)) return

      // Lose game if bomb
      if (State.isBomb(state)) setLost(true)

      // Calculate tiles to reveal
      const reveals = [i]

      if (State.isEmpty(state)) {
        const travelled: number[] = []

        const spread = (index: number) => {
          if (travelled.includes(index)) return

          travelled.push(index)
          reveals.push(index)

          if (State.isEmpty(localGameState[index])) {
            const width = settings.width
            const height = settings.height

            if (index % width !== 0) spread(index - 1)
            if (index % width !== width - 1) spread(index + 1)
            if (index >= width) spread(index - width)
            if (index < width * (height - 1)) spread(index + width)
            if (index % width !== 0 && index >= width) spread(index - width - 1)
            if (index % width !== width - 1 && index >= width) spread(index - width + 1)
            if (index % width !== 0 && index < width * (height - 1)) spread(index + width - 1)
            if (index % width !== width - 1 && index < width * (height - 1)) spread(index + width + 1)
          }
        }

        spread(i)
      }

      setLocalGameState(prev => prev!.map((state, j) => (reveals.includes(j) ? State.reveal(state) : state)))

      // Notify other users of move
      ws.sendToLobby<Websocket.Response.MoveAdd>(lobby.lobbyId, {
        eventName: "MOVE_ADD",
        userId: user.id,
        data: { lobbyId: lobby.lobbyId, reveals }
      })
    }
  }

  async function reset() {
    if (!lobby || !user) return

    await api.boardReset(lobby.lobbyId, user.id)
  }

  async function skip() {
    if (!lobby || !user) return

    await api.boardSkip(lobby.lobbyId, user.id)
  }

  async function leaveParty() {
    await leave()
    navigate("MainMenu")
  }

  if (!localGameState) return <Loading />

  return (
    <RollingBackground fade>
      <Page>
        <div id="game-active">
          <div id="game-active-boards-container">
            <div id="game-active-current-board-container">
              <table cellPadding={0} cellSpacing={0}>
                <tbody>
                  {Array.from({ length: settings.height }).map((_, y) => (
                    <tr key={`y${y}`}>
                      {Array.from({ length: settings.width })
                        .map((_, x) => localGameState[i(x, y)])
                        .map((state, x) => (
                          <td key={i(x, y)}>
                            <input
                              type="button"
                              value={render(state)}
                              onClick={() => makeMove(i(x, y), false)}
                              onContextMenu={e => (e.preventDefault(), makeMove(i(x, y), true))}
                              disabled={lost || State.isRevealed(state)}
                            />
                          </td>
                        ))}
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
            <div id="game-active-competitors">
              {Object.entries(competitionState ?? {}).map(([userId, state]) => (
                <div key={userId}>
                  <Text type="small">{userId}</Text>
                  <br />
                  <div className="game-active-competitor-board-container">
                    <table cellPadding={0} cellSpacing={0}>
                      <tbody>
                        {Array.from({ length: 7 }).map((_, y) => (
                          <tr key={`${userId}y${y}`}>
                            {Array.from({ length: 7 }).map((_, x) => {
                              const tileIndexHeight = settings.height / 7
                              const tileIndexWidth = settings.width / 7

                              const indices: number[] = []

                              for (let k = y * tileIndexHeight; k < (y + 1) * tileIndexHeight; k++)
                                for (let j = x * tileIndexWidth; j < (x + 1) * tileIndexWidth; j++)
                                  indices.push(i(Math.floor(j), Math.floor(k)))

                              const isRevealed =
                                indices.filter(i => State.isRevealed(state[i]) || State.isFlagged(state[i])).length >
                                indices.length / 2

                              return (
                                <td key={userId + i(x, y)}>
                                  <div
                                    className={`game-active-competitor-tile-${isRevealed ? "revealed" : "hidden"}`}
                                  />
                                </td>
                              )
                            })}
                          </tr>
                        ))}
                      </tbody>
                    </table>
                  </div>
                </div>
              ))}
            </div>
          </div>

          <ButtonList>
            <ButtonList horizontal>
              <Box onClick={reset}>
                <Text type="big">Reset</Text>
              </Box>
              <Box onClick={skip} disabled={settings.seed !== 0}>
                <Text type="big">Skip</Text>
              </Box>
            </ButtonList>
            <Box onClick={leaveParty}>
              <Text type="big">Leave Party</Text>
            </Box>
          </ButtonList>
        </div>
      </Page>
    </RollingBackground>
  )
}
