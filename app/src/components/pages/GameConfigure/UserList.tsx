import React from "react"
import "./UserList.scss"
import { Participant, useEmbeddedAppSdk, User } from "../../../hooks/useEmbeddAppSdk"
import { useLobby } from "../../../hooks/useLobby"
import { Flag } from "../../ui/icons/Flag"

export function UserList() {
  const { participants, user } = useEmbeddedAppSdk()
  const { lobby } = useLobby()

  function getDefaultAvatarIndex(user: User | Participant) {
    return user.discriminator === "0" ? (parseInt(user.id) >> 22) % 6 : parseInt(user.discriminator) % 5
  }

  function getAvatarUrl(id: string) {
    if (!user || !participants) return null

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

  function getUsername(id: string) {
    if (!user || !participants) return null

    if (id === user.id) {
      return user.username
    } else if (participants.some(p => p.id === id)) {
      return participants.find(p => p.id === id)!.username
    } else {
      return null
    }
  }

  return (
    <div className="user-list">
      {(lobby?.userIds ?? [])
        .map(id => [id, getAvatarUrl(id)] as const)
        .map(([id, avatarUrl]) => (
          <div key={id} className="user-list-user" title={getUsername(id) ?? id}>
            {avatarUrl ? (
              <img src={avatarUrl} alt={id} />
            ) : (
              <div className="user-list-default-icon">
                <Flag color="off-bg" />
              </div>
            )}
          </div>
        ))}
    </div>
  )
}
