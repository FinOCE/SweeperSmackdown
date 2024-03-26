import { createContext } from "preact"
import { Dispatch, StateUpdater, useContext, useState } from "preact/hooks"
import { MainMenu } from "../screens/MainMenu"
import { GameConfigure } from "../screens/GameConfigure"

type Navigation = {
  navigate: Dispatch<StateUpdater<Page>>
}

const NavigationContext = createContext<Navigation>(null)
export const useNavigation = () => useContext(NavigationContext)

export function NavigationProvider() {
  const [page, navigate] = useState<Page>("MainMenu")

  return (
    <NavigationContext.Provider value={{ navigate }}>
      {page === "MainMenu" && <MainMenu />}
      {page === "GameConfigure" && <GameConfigure />}
    </NavigationContext.Provider>
  )
}

type Page = "MainMenu" | "GameConfigure"
