import { useEffect, useState } from "react"

export function useCountdown(callback: () => void) {
  const [countdown, setCountdown] = useState<number | null>(null)
  const [expiry, setExpiry] = useState<number | null>(null)

  useEffect(() => {
    if (!expiry) setCountdown(null)

    const interval = expiry ? setInterval(() => setCountdown(Math.ceil((expiry - Date.now()) / 1000)), 100) : undefined
    const timeout = expiry ? setTimeout(() => (callback(), setExpiry(null)), expiry - Date.now()) : undefined

    return () => {
      clearInterval(interval)
      clearTimeout(timeout)
    }
  }, [expiry])

  return {
    countdown,
    start: (duration: number) => setExpiry(Date.now() + duration),
    stop: () => setExpiry(null)
  }
}
