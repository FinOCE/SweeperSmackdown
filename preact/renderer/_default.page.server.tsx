export const passToClient = ["pageProps", "urlPathname"]

import { renderToString } from "preact-render-to-string"
import { escapeInject, dangerouslySkipEscape } from "vite-plugin-ssr/server"

export async function render(pageContext) {
  const { Page, pageProps } = pageContext
  if (!Page)
    throw new Error("My render() hook expects pageContext.Page to be defined")
  const pageHtml = renderToString(<Page {...pageProps} />)

  const { documentProps } = pageContext.exports
  const title = (documentProps && documentProps.title) || "Vite SSR + Preact"
  const desc =
    (documentProps && documentProps.description) ||
    "Preact app with Vite and vite-plugin-ssr"

  const documentHtml = escapeInject`<!DOCTYPE html>
		<html lang="en">
		<head>
			<meta charset="UTF-8" />
			<!-- <link rel="icon" type="image/svg+xml" href="/vite.svg" /> -->
			<meta name="viewport" content="width=device-width, initial-scale=1.0" />
			<meta name="color-scheme" content="light dark" />
			<meta name="description" content="${desc}" />
			<title>${title}</title>
		</head>
		<body>
			<div id="app">${dangerouslySkipEscape(pageHtml)}</div>
		</body>
		</html>`

  return { documentHtml, pageContext: {} }
}
