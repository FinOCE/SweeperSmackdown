import { useEffect, useState } from "react"

// This is currently not used but something similar may be useful in the future.
// If a while down the line this still isn't in use, safe to delete

export function useOptimisticReducer<T extends Record<string, any>>(authority: T | null) {
  type Change<K extends keyof T = keyof T> = [K, T[K]]

  const [realState, setRealState] = useState<T | null>(authority)
  const [pendingChanges, setPendingChanges] = useState<Change[]>([])

  useEffect(() => {
    if (!authority) return
    setRealState(authority)
  }, [authority])

  function reduce(
    expected: Partial<T>,
    resolve: () => Promise<Partial<T>>,
    handleError?: (err: Error, expected: Partial<T>) => void
  ) {
    if (!realState) return

    const changes: Change[] = Object.entries(expected)
    setPendingChanges(c => c.concat(changes))

    resolve()
      .then(real => setRealState(Object.assign(realState!, real)))
      .catch((err: Error) => handleError && handleError(err, expected))
      .finally(() => setPendingChanges(c => c.filter(c => changes.includes(c))))
  }

  const optimisticState =
    realState === null ? null : pendingChanges.reduce((pre, [key, value]) => ({ ...pre, [key]: value }), realState)

  return {
    optimisticState,
    pendingChanges,
    realState,
    reduce
  }
}
