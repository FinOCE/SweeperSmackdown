/// <reference types="vite/client" />

interface ImportMetaEnv {
  readonly PUBLIC_ENV__API_BASE_URL: string
}

interface ImportMeta {
  readonly env: ImportMetaEnv
}
