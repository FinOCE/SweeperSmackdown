import React, { createContext, Dispatch, SetStateAction, useContext, useState } from "react"
import { MainMenu } from "../pages/MainMenu"
import { GameConfigure } from "../pages/GameConfigure"
import { GameActive } from "../pages/GameActive"
import { GameCelebration } from "../pages/GameCelebration"

type Navigation = {
  navigate: Dispatch<SetStateAction<Page>>
}

const NavigationContext = createContext<Navigation>({
  navigate() {}
})
export const useNavigation = () => useContext(NavigationContext)

export function NavigationProvider() {
  const [page, navigate] = useState<Page>("MainMenu")

  return (
    <NavigationContext.Provider value={{ navigate }}>
      {page === "MainMenu" && <MainMenu />}
      {page === "GameConfigure" && <GameConfigure />}
      {page === "GameActive" && <GameActive />}
      {page === "GameCelebration" && <GameCelebration />}
    </NavigationContext.Provider>
  )
}

type Page = "MainMenu" | "GameConfigure" | "GameActive" | "GameCelebration"
