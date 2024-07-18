declare namespace NodeJS {
  interface ProcessEnv {
    PUBLIC_ENV__API_BASE_URL: string
    PUBLIC_ENV__DISCORD_CLIENT_ID: string
    PUBLIC_ENV__DEV_SOLVE_BUTTON: string | undefined
    PUBLIC_ENV__DEV_PREVIEW: string | undefined
  }
}
