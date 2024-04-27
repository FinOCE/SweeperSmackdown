import React, { createContext, Dispatch, SetStateAction, useContext, useState } from "react"
import { MainMenu } from "../pages/MainMenu"
import { GameConfigure } from "../pages/GameConfigure"
import { GameActive } from "../pages/GameActive"
import { GameCelebration } from "../pages/GameCelebration"
import { Entrypoint } from "../pages/Entrypoint"

type Navigation = {
  navigate: Dispatch<SetStateAction<Page>>
}

const NavigationContext = createContext<Navigation>({ navigate: () => {} })
export const useNavigation = () => useContext(NavigationContext)

export function NavigationProvider() {
  const [page, navigate] = useState<Page>("Entrypoint")

  return (
    <NavigationContext.Provider value={{ navigate }}>
      {page === "Entrypoint" && <Entrypoint />}
      {page === "MainMenu" && <MainMenu />}
      {page === "GameConfigure" && <GameConfigure />}
      {page === "GameActive" && <GameActive />}
      {page === "GameCelebration" && <GameCelebration />}
    </NavigationContext.Provider>
  )
}

type Page = "Entrypoint" | "MainMenu" | "GameConfigure" | "GameActive" | "GameCelebration"
