import { hydrate } from "preact"

export async function render(pageContext) {
  const { Page, pageProps } = pageContext
  if (!Page)
    throw new Error(
      "Client-side render() hook expects pageContext.Page to be defined"
    )
  const root = document.getElementById("app")
  if (!root) throw new Error("DOM element #app not found")

  hydrate(<Page {...pageProps} />, root)
}
