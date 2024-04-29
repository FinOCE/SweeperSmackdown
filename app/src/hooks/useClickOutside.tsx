import { MutableRefObject, useEffect, useRef } from "react"

export function useClickOutside(refs: MutableRefObject<HTMLElement | null>[], callback: (event: Event) => void) {
  const callbackRef = useRef(callback)
  callbackRef.current = callback

  useEffect(() => {
    const handleClickOutside = (event: Event): void => {
      if (refs.every(ref => !ref.current?.contains(event.target as Node))) callbackRef.current?.(event)
    }

    document.addEventListener("click", handleClickOutside, true)
    return () => document.removeEventListener("click", handleClickOutside, true)
  }, [refs])
}
