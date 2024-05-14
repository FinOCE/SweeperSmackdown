import React, { createContext, useContext, useEffect, useState } from "react"
import { useApi } from "../useApi"
import { Api } from "../../types/Api"
import { useLobbyData } from "../data/useLobbyData"

type TSettingsContext = {
  settings: Api.GameSettings | null
  updateSettings: (settings: Api.Request.LobbyPatch) => Promise<void>
}

const SettingsContext = createContext<TSettingsContext>({ settings: null, async updateSettings(settings) {} })
export const useSettings = () => useContext(SettingsContext)

export function SettingsProvider(props: { children?: React.ReactNode }) {
  const { api } = useApi()
  const { lobbyData, setLobbyData } = useLobbyData()

  const [settings, setSettings] = useState<Api.GameSettings | null>(null)

  useEffect(() => {
    setSettings(lobbyData ? lobbyData.settings : null)
  }, [lobbyData])

  async function updateSettings(settings: Api.Request.LobbyPatch) {
    if (!lobbyData) throw new Error("No lobby")

    const [err, lobby] = await api
      .lobbyPatch(lobbyData.lobbyId, settings)
      .then(([data]) => [null, data] as const)
      .catch((err: Error) => [err, null] as const)

    if (err) throw new Error("Failed to update settings")

    setLobbyData(lobby)
  }

  return <SettingsContext.Provider value={{ settings, updateSettings }}>{props.children}</SettingsContext.Provider>
}
