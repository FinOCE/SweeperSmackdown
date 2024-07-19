import React, { createContext, useContext, useEffect, useState } from "react"
import { Entrypoint } from "../pages/Entrypoint"
import { FadeWrapper } from "../components/ui/FadeWrapper"
import { MainMenu } from "../pages/MainMenu"
import { GameConfigure } from "../pages/GameConfigure"
import { GameActive } from "../pages/GameActive"
import { GameCelebration } from "../pages/GameCelebration"
import { useLobby } from "./resources/useLobby"
import { Api } from "../types/Api"
import { useEmbeddedAppSdk } from "./useEmbeddAppSdk"

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
  const { user } = useEmbeddedAppSdk()
  const { lobby } = useLobby()

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

  useEffect(() => {
    if (!lobby.resolved || !user) return

    const expectedPage = ((): Page => {
      switch (lobby.status.status) {
        case Api.Enums.ELobbyStatus.Configuring:
        case Api.Enums.ELobbyStatus.Starting:
          return { name: "GameConfigure", args: { lobbyId: lobby.id } }
        case Api.Enums.ELobbyStatus.Playing:
        case Api.Enums.ELobbyStatus.Concluding:
          return { name: "GameActive", args: { lobbyId: lobby.id, userId: user.id } }
        case Api.Enums.ELobbyStatus.Celebrating:
          return { name: "GameCelebration", args: { lobbyId: lobby.id } }
      }
    })()

    if (page.name !== expectedPage.name) navigate(expectedPage.name, expectedPage.args)
  }, [lobby.status?.status, user])

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
