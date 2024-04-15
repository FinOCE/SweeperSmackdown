import { Api } from "./Api"

export type LobbyWithoutNested = Omit<Api.Lobby, "settings" | "scores" | "wins">
