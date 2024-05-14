import React, { createContext, useContext, useEffect, useState } from "react"
import { useApi } from "../useApi"
import { useVoteData } from "../data/useVoteData"
import { useEmbeddedAppSdk } from "../useEmbeddAppSdk"

type TVoteContext = {
  votes: Record<string, string[]> | null
  choices: string[] | null
  requiredVotes: number | null
  countdownExpiry: Date | null
  addVote: (choice: string, force?: boolean) => Promise<void>
  removeVote: (force?: boolean) => Promise<void>
}

const VoteContext = createContext<TVoteContext>({
  votes: null,
  choices: null,
  requiredVotes: null,
  countdownExpiry: null,
  async addVote(choice, force) {},
  async removeVote(force) {}
})
export const useVotes = () => useContext(VoteContext)

export function VoteProvider(props: { children?: React.ReactNode }) {
  const { api } = useApi()
  const { user } = useEmbeddedAppSdk()
  const { voteData, countdown } = useVoteData()

  const [votes, setVotes] = useState<Record<string, string[]> | null>(null)
  const [choices, setChoices] = useState<string[] | null>(null)
  const [requiredVotes, setRequiredVotes] = useState<number | null>(null)
  const [countdownExpiry, setCountdownExpiry] = useState<Date | null>(null)

  useEffect(() => {
    setVotes(voteData ? voteData.votes : null)
    setChoices(voteData ? voteData.choices : null)
    setRequiredVotes(voteData ? voteData.requiredVotes : null)
    setCountdownExpiry(countdown ? countdown : null)
  }, [voteData, countdown])

  async function addVote(choice: string, force?: boolean) {
    if (!user) throw new Error("No user")
    if (!voteData) throw new Error("No vote")

    const [err] = await api
      .votePut(voteData.lobbyId, user.id, choice, force)
      .then(([data]) => [null, data] as const)
      .catch((err: Error) => [err, null] as const)

    if (err) throw new Error("Failed to add vote")
  }

  async function removeVote(force?: boolean) {
    if (!user) throw new Error("No user")
    if (!voteData) throw new Error("No vote")

    const [err] = await api
      .voteDelete(voteData.lobbyId, user.id, force)
      .then(([data]) => [null, data] as const)
      .catch((err: Error) => [err, null] as const)

    if (err) throw new Error("Failed to remove vote")
  }

  return (
    <VoteContext.Provider value={{ votes, choices, requiredVotes, countdownExpiry, addVote, removeVote }}>
      {props.children}
    </VoteContext.Provider>
  )
}
