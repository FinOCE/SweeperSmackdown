import { useState } from "react"
import { Api } from "../types/Api"

type SettingsViewModel = Required<Omit<Api.Request.LobbyPatch, "hostId">>

export function useLobbySettingsState(initialSettings: SettingsViewModel) {
  const [settings, setSettings] = useState<SettingsViewModel>(initialSettings)
  const [changes, setChanges] = useState<Api.Request.LobbyPatch>({})

  function change(changes: { [K in keyof SettingsViewModel]?: SettingsViewModel[K] }) {
    setChanges(prev => ({ ...prev, ...changes }))
    setSettings(prev => ({ ...prev, ...changes }))
  }

  function clear() {
    setChanges({})
  }

  function update(apiSettings: Api.GameSettings) {
    if (
      settings.mode !== apiSettings.mode ||
      settings.height !== apiSettings.height ||
      settings.width !== apiSettings.width ||
      settings.mines !== apiSettings.mines ||
      settings.lives !== apiSettings.lives ||
      settings.timeLimit !== apiSettings.timeLimit ||
      settings.boardCount !== apiSettings.boardCount ||
      (settings.shareBoards && apiSettings.seed === 0) ||
      (!settings.shareBoards && apiSettings.seed !== 0)
    )
      setSettings({
        mode: apiSettings.mode,
        height: apiSettings.height,
        width: apiSettings.width,
        mines: apiSettings.mines,
        lives: apiSettings.lives,
        timeLimit: apiSettings.timeLimit,
        boardCount: apiSettings.boardCount,
        shareBoards: apiSettings.seed !== 0
      })
  }

  return { settings, changes, change, update, clear }
}
