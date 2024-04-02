import { Api } from "./Api"

export type LobbyWithoutSettings = Omit<Api.Lobby, "settings">
