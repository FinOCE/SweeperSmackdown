import React, { createContext, useContext, useEffect, useState } from "react"
import { useApi } from "../useApi"
import { useVoteData } from "../data/useVoteData"
import { useEmbeddedAppSdk } from "../useEmbeddAppSdk"
import { useLobbyData } from "../data/useLobbyData"

type TVoteContext = {
  votes: Record<string, string[]> | null
  choices: string[] | null
  requiredVotes: number | null
  countdownExpiry: number | null
  addVote: (choice: string, force?: boolean) => Promise<void>
  removeVote: (force?: boolean) => Promise<void>
  fetchVotes: () => Promise<void>
}

const VoteContext = createContext<TVoteContext>({
  votes: null,
  choices: null,
  requiredVotes: null,
  countdownExpiry: null,
  async addVote(choice, force) {},
  async removeVote(force) {},
  async fetchVotes() {}
})
export const useVotes = () => useContext(VoteContext)

export function VoteProvider(props: { children?: React.ReactNode }) {
  const { api } = useApi()
  const { user } = useEmbeddedAppSdk()
  const { lobbyData } = useLobbyData()
  const { voteData, countdown, setVoteData } = useVoteData()

  const [votes, setVotes] = useState<Record<string, string[]> | null>(null)
  const [choices, setChoices] = useState<string[] | null>(null)
  const [requiredVotes, setRequiredVotes] = useState<number | null>(null)
  const [countdownExpiry, setCountdownExpiry] = useState<number | null>(null)

  useEffect(() => {
    setVotes(voteData ? voteData.votes : null)
    setChoices(voteData ? voteData.choices : null)
    setRequiredVotes(voteData ? voteData.requiredVotes : null)
  }, [voteData])

  useEffect(() => {
    setCountdownExpiry(countdown ? countdown : null)
  }, [countdown])

  async function fetchVotes() {
    if (!lobbyData) throw new Error("No lobby")

    const [err, votes] = await api
      .voteGetAll(lobbyData.lobbyId)
      .then(([data]) => [null, data] as const)
      .catch((err: Error) => [err, null] as const)

    if (err) throw new Error("Failed to fetch votes")

    setVoteData(votes)
  }

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
    <VoteContext.Provider value={{ votes, choices, requiredVotes, countdownExpiry, addVote, removeVote, fetchVotes }}>
      {props.children}
    </VoteContext.Provider>
  )
}
