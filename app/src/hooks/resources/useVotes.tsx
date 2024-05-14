import React, { createContext, useContext, useEffect, useState } from "react"
import { useApi } from "../useApi"
import { useWebsocket } from "../useWebsocket"
import { useLobby } from "./useLobby"

type TVoteContext = {
  votes: Record<string, string[]> | null
  choices: string[] | null
  requiredVotes: number | null
  countdownExpiry: Date | null
  addVote: (choice: string, force?: boolean) => Promise<void>
  removeVote: (choice: string, force?: boolean) => Promise<void>
}

const VoteContext = createContext<TVoteContext>({
  votes: null,
  choices: null,
  requiredVotes: null,
  countdownExpiry: null,
  async addVote(choice, force) {},
  async removeVote(choice, force) {}
})
export const useVotes = () => useContext(VoteContext)

export function VoteProvider(props: { children?: React.ReactNode }) {
  const { api } = useApi()
  const { ws } = useWebsocket()
  const { lobby } = useLobby()

  const [votes, setVotes] = useState<Record<string, string[]> | null>(null)
  const [choices, setChoices] = useState<string[] | null>(null)
  const [requiredVotes, setRequiredVotes] = useState<number | null>(null)
  const [countdownExpiry, setCountdownExpiry] = useState<Date | null>(null)

  async function addVote(choice: string, force?: boolean) {
    // TODO
  }

  async function removeVote(choice: string, force?: boolean) {
    // TODO
  }

  useEffect(() => {
    if (!lobby) setVotes(null)
  }, [lobby])

  useEffect(() => {
    if (!ws) return

    // TODO: Handle websocket events

    return () => {}
  }, [ws])

  return (
    <VoteContext.Provider value={{ votes, choices, requiredVotes, countdownExpiry, addVote, removeVote }}>
      {props.children}
    </VoteContext.Provider>
  )
}
