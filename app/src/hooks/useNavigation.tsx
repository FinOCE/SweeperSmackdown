import React, { createContext, useContext, useRef, useState } from "react"
import { Entrypoint } from "../pages/Entrypoint"
import { FadeWrapper } from "../components/ui/FadeWrapper"

type TNavigationContext = {
  navigate: <E extends (...args: any[]) => React.JSX.Element>(element: P<E>["element"], ...args: P<E>["args"]) => void
  navigateWithoutAnimation: <E extends (...args: any[]) => React.JSX.Element>(
    element: P<E>["element"],
    ...args: P<E>["args"]
  ) => void
}

type P<E extends (...args: any[]) => React.JSX.Element> = {
  element: E
  args: Parameters<E>
}

const NavigationContext = createContext<TNavigationContext>({
  navigate(element, ...args) {},
  navigateWithoutAnimation(element, ...args) {}
})
export const useNavigation = () => useContext(NavigationContext)

export function NavigationProviderr() {
  const [animate, setAnimate] = useState<"in" | "out" | "off">("off")
  const page = useRef<React.JSX.Element>(<Entrypoint />)

  function navigate<E extends (...args: any[]) => React.JSX.Element>(element: P<E>["element"], ...args: P<E>["args"]) {
    setAnimate("out")

    setTimeout(() => {
      page.current = element(args)
      setAnimate("in")
    }, 500)

    setTimeout(() => setAnimate("off"), 1000)
  }

  function navigateWithoutAnimation<E extends (...args: any[]) => React.JSX.Element>(
    element: P<E>["element"],
    ...args: P<E>["args"]
  ) {
    page.current = element(args)
  }

  return (
    <NavigationContext.Provider value={{ navigate, navigateWithoutAnimation }}>
      <FadeWrapper animate={animate}>{page.current}</FadeWrapper>
    </NavigationContext.Provider>
  )
}
