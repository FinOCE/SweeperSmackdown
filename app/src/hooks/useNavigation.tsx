import React, { createContext, useContext, useState } from "react"
import { MainMenu } from "../pages/MainMenu"
import { GameConfigure } from "../pages/GameConfigure"
import { GameActive } from "../pages/GameActive"
import { GameCelebration } from "../pages/GameCelebration"
import { Entrypoint } from "../pages/Entrypoint"
import { FadeWrapper } from "../components/ui/FadeWrapper"

type Navigation = {
  navigate: (page: Page, animate?: boolean) => void
}

const NavigationContext = createContext<Navigation>({ navigate: () => {} })
export const useNavigation = () => useContext(NavigationContext)

export function NavigationProvider() {
  const [page, setPage] = useState<Page>("Entrypoint")
  const [animate, setAnimate] = useState<"in" | "out" | "off">("off")

  function navigate(page: Page, animate: boolean = true) {
    if (animate) {
      setAnimate("out")

      setTimeout(() => {
        setPage(page)
        setAnimate("in")
      }, 500)

      setTimeout(() => setAnimate("off"), 1000)
    } else {
      setPage(page)
    }
  }

  return (
    <NavigationContext.Provider value={{ navigate }}>
      <FadeWrapper animate={animate}>
        {page === "Entrypoint" && (() => <Entrypoint />)()}
        {page === "MainMenu" && (() => <MainMenu />)()}
        {page === "GameConfigure" && (() => <GameConfigure />)()}
        {page === "GameActive" && (() => <GameActive />)()}
        {page === "GameCelebration" && (() => <GameCelebration />)()}
      </FadeWrapper>
    </NavigationContext.Provider>
  )
}

type Page = "Entrypoint" | "MainMenu" | "GameConfigure" | "GameActive" | "GameCelebration"
