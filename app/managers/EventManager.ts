import {
  OnConnectedArgs,
  OnDisconnectedArgs,
  OnGroupDataMessageArgs,
  OnRejoinGroupFailedArgs,
  OnServerDataMessageArgs,
  OnStoppedArgs
} from "@azure/web-pubsub-client"
import EventEmitter from "events"

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

  public constructor() {
    super()

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
    this.handlers[event].push(callback)
  }

  public clear() {
    this.handlers.connected = []
    this.handlers.disconnected = []
    this.handlers.stopped = []
    this.handlers["rejoin-group-failed"] = []
    this.handlers["group-message"] = []
    this.handlers["server-message"] = []
  }
}
