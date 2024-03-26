import { Websocket } from "../types/Websocket"

export function isEvent<T extends Websocket.Message>(eventName: T["eventName"], data: Websocket.Message): data is T {
  return data.eventName === eventName
}
