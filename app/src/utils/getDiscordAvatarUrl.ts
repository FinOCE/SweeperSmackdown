/**
 * Get a user's avatar URL from Discord.
 * @param id The user's ID
 * @param hash The user's avatar hash
 * @returns The fully formed URL to the user's avatar
 */
export function getDiscordAvatarUrl(id: string, hash: string | null | undefined) {
  const defaultAvatarIndex = Math.abs(Number(id) >> 22) % 6

  return hash
    ? `https://cdn.discordapp.com/avatars/${id}/${hash}.png?size=256`
    : `https://cdn.discordapp.com/embed/avatars/${defaultAvatarIndex}.png`
}
