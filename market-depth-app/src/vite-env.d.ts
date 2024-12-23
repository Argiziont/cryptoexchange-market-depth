/// <reference types="vite/client" />

interface ImportMetaEnv {
  readonly VITE_APP_SIGNALR_HUB_URL: string;
}

interface ImportMeta {
  readonly env: ImportMetaEnv;
}
