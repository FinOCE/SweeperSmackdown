import React, { createContext, useContext, useState } from "react"
import { Entrypoint } from "../pages/Entrypoint"
import { FadeWrapper } from "../components/ui/FadeWrapper"
import { MainMenu } from "../pages/MainMenu"
import { GameConfigure } from "../pages/GameConfigure"
import { GameActive } from "../pages/GameActive"
import { GameCelebration } from "../pages/GameCelebration"

const Pages = {
  Entrypoint: Entrypoint,
  MainMenu: MainMenu,
  GameConfigure: GameConfigure,
  GameActive: GameActive,
  GameCelebration: GameCelebration
} as const

type Page<P extends keyof typeof Pages = keyof typeof Pages> = {
  name: P
  args: Parameters<(typeof Pages)[P]>[0]
}

type TNavigationContext = {
  navigate: <E extends keyof typeof Pages>(element: E, args: Parameters<(typeof Pages)[E]>[0]) => void
  navigateWithoutAnimation: <E extends keyof typeof Pages>(element: E, args: Parameters<(typeof Pages)[E]>[0]) => void
}

const NavigationContext = createContext<TNavigationContext>({
  navigate(element, ...args) {},
  navigateWithoutAnimation(element, ...args) {}
})
export const useNavigation = () => useContext(NavigationContext)

export function NavigationProvider() {
  const [page, setPage] = useState<Page>({ name: "Entrypoint", args: {} })
  const [animate, setAnimate] = useState<"in" | "out" | "off">("off")

  function navigate<E extends keyof typeof Pages>(element: E, args: Parameters<(typeof Pages)[E]>[0]) {
    setAnimate("out")

    setTimeout(() => {
      setPage({ name: element, args })
      setAnimate("in")
    }, 500)

    setTimeout(() => setAnimate("off"), 1000)
  }

  function navigateWithoutAnimation<E extends keyof typeof Pages>(element: E, args: Parameters<(typeof Pages)[E]>[0]) {
    setPage({ name: element, args })
  }

  return (
    <NavigationContext.Provider value={{ navigate, navigateWithoutAnimation }}>
      <FadeWrapper animate={animate}>
        {page.name === "Entrypoint" && <Entrypoint {...(page as Page<"Entrypoint">).args} />}
        {page.name === "MainMenu" && <MainMenu {...(page as Page<"MainMenu">).args} />}
        {page.name === "GameConfigure" && <GameConfigure {...(page as Page<"GameConfigure">).args} />}
        {page.name === "GameActive" && <GameActive {...(page as Page<"GameActive">).args} />}
        {page.name === "GameCelebration" && <GameCelebration {...(page as Page<"GameCelebration">).args} />}
      </FadeWrapper>
    </NavigationContext.Provider>
  )
}
