import React, { createContext, useContext, useState } from "react"
import { useApi } from "../useApi"
import { useWebsocket } from "../useWebsocket"
import { Api } from "../../types/Api"

type TSettingsContext = {
  settings: Api.GameSettings | null
  updateSettings: (settings: Partial<Api.GameSettings>) => Promise<void>
}

const SettingsContext = createContext<TSettingsContext>({ settings: null, async updateSettings(settings) {} })
export const useSettings = () => useContext(SettingsContext)

export function SettingsProvider(props: { children?: React.ReactNode }) {
  const { api } = useApi()
  const { ws } = useWebsocket()

  const [settings, setSettings] = useState<Api.GameSettings | null>(null)

  async function updateSettings(settings: Partial<Api.GameSettings>) {
    // TODO
  }

  return <SettingsContext.Provider value={{ settings, updateSettings }}>{props.children}</SettingsContext.Provider>
}
