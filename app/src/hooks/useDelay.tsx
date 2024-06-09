import { useEffect } from "react"

/**
 * Delay the execution of a callback until the dependencies haven't changed for a given amount of time.
 *
 * @param callback The callback to run after the delay
 * @param delay The number of milliseconds before the callback should run
 * @param dependencies The dependencies that when modified reset the delay
 */
export function useDelay(callback: () => void | Promise<void>, delay: number, dependencies: any[]) {
  useEffect(() => {
    const timer = setTimeout(callback, delay)
    return () => clearTimeout(timer)
  }, dependencies)
}
