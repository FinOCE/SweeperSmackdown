import { Participant, User } from "../hooks/useEmbeddAppSdk"

export type DisplayDetails = {
  id: string
  displayName: string
  avatarUrl: string | null
  wins: number
  score: number
}

export function getDisplayDetails(
  id: string,
  user: User | null,
  participants: Participant[] | null,
  wins: Record<string, number>,
  scores: Record<string, number>
): DisplayDetails {
  return {
    id,
    displayName: getUsername(id, user, participants ?? []) ?? id,
    avatarUrl: getAvatarUrl(id, user, participants ?? []),
    wins: wins[id] ?? 0,
    score: scores[id] ?? 0
  }
}

export function getDefaultAvatarIndex(user: User | Participant) {
  return user.discriminator === "0" ? (parseInt(user.id) >> 22) % 6 : parseInt(user.discriminator) % 5
}

export function getAvatarUrl(id: string, user: User | null, participants: Participant[]) {
  if (!user) return null

  if (id === user.id) {
    if (user.discriminator === "-1") return null

    return user.avatar
      ? `https://cdn.discordapp.com/avatars/${id}/${user.avatar}.${user.avatar.startsWith("a_") ? "gif" : "png"}`
      : `https://cdn.discordapp.com/embed/avatars/${getDefaultAvatarIndex(user)}.png`
  } else if (participants.some(p => p.id === id)) {
    const participant = participants.find(p => p.id === id)!

    return participant.avatar
      ? `https://cdn.discordapp.com/avatars/${id}/${participant.avatar}.${
          participant.avatar.startsWith("a_") ? "gif" : "png"
        }`
      : `https://cdn.discordapp.com/embed/avatars/${getDefaultAvatarIndex(participant)}.png`
  } else {
    return null
  }
}

export function getUsername(id: string, user: User | null, participants: Participant[]) {
  if (!user) return null

  if (id === user.id) {
    return user.username
  } else if (participants.some(p => p.id === id)) {
    return participants.find(p => p.id === id)!.username
  } else {
    return null
  }
}
