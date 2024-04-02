import {
  OnConnectedArgs,
  OnDisconnectedArgs,
  OnGroupDataMessageArgs,
  OnRejoinGroupFailedArgs,
  OnServerDataMessageArgs,
  OnStoppedArgs,
  WebPubSubClient
} from "@azure/web-pubsub-client"
import EventEmitter from "events"
import { Websocket } from "../types/Websocket"

type EventHandler<T> = (e: T) => void

type Event = {
  connected: EventHandler<OnConnectedArgs>
  disconnected: EventHandler<OnDisconnectedArgs>
  stopped: EventHandler<OnStoppedArgs>
  "rejoin-group-failed": EventHandler<OnRejoinGroupFailedArgs>
  "group-message": EventHandler<OnGroupDataMessageArgs>
  "server-message": EventHandler<OnServerDataMessageArgs>
}

export interface IEventManager {
  /**
   * Register a new event handler.
   * @param event The event to handle
   * @param callback The handler for the event
   */
  register<T extends keyof Event>(event: T, callback: Event[T]): void

  /**
   * Clear any currently registered handlers.
   */
  clear(): void

  /**
   * Send a message to all members in a lobby.
   * @param lobbyId The lobby to send the message to
   * @param data The message to send
   */
  sendToLobby<T extends Websocket.Message>(lobbyId: string, data: T): void
}

export class EventManager extends EventEmitter implements IEventManager {
  public readonly handlers: { [K in keyof Event]: Event[K][] } = {
    connected: [],
    disconnected: [],
    stopped: [],
    "rejoin-group-failed": [],
    "group-message": [],
    "server-message": []
  }

  private _client: WebPubSubClient

  public constructor(client: WebPubSubClient) {
    super()

    this._client = client

    this.on("connected", (e: OnConnectedArgs) => this.handlers.connected.forEach(handler => handler(e)))
    this.on("disconnected", (e: OnDisconnectedArgs) => this.handlers.disconnected.forEach(handler => handler(e)))
    this.on("stopped", (e: OnStoppedArgs) => this.handlers.stopped.forEach(handler => handler(e)))
    this.on("rejoin-group-failed", (e: OnRejoinGroupFailedArgs) =>
      this.handlers["rejoin-group-failed"].forEach(handler => handler(e))
    )
    this.on("group-message", (e: OnGroupDataMessageArgs) =>
      this.handlers["group-message"].forEach(handler => handler(e))
    )
    this.on("server-message", (e: OnServerDataMessageArgs) =>
      this.handlers["server-message"].forEach(handler => handler(e))
    )

    this.clear()
  }

  public register<T extends keyof Event>(event: T, callback: Event[T]) {
    if (!this.handlers[event].includes(callback)) this.handlers[event].push(callback)
  }

  public clear() {
    this.handlers.connected = []
    this.handlers.disconnected = []
    this.handlers.stopped = []
    this.handlers["rejoin-group-failed"] = []
    this.handlers["group-message"] = []
    this.handlers["server-message"] = []
  }

  public sendToLobby<T extends Websocket.Message>(lobbyId: string, data: T) {
    this._client.sendToGroup(lobbyId, data, "json", { fireAndForget: true })
  }
}
