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
import { Buffer } from "node:buffer"

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
   * Whether or not the event manager is currently connected to Web PubSub.
   */
  connected: boolean

  /**
   * Set the web pubsub client to be used to send messages.
   * @param client The web pubsub client
   */
  setClient(client: WebPubSubClient): void

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

  private _client?: WebPubSubClient
  public connected: boolean = false

  public constructor() {
    super()

    this.on("connected", () => (this.connected = true))
    this.on("disconnected", () => (this.connected = false))

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

  public setClient(client: WebPubSubClient) {
    this._client = client
  }

  public sendToLobby<T extends Websocket.Message>(lobbyId: string, data: T) {
    if (!this._client) throw new Error("Client not set")

    this._client.sendToGroup(lobbyId, data, "json", { fireAndForget: true })
  }
}
