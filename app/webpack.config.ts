import path from "path"
import HtmlWebpackPlugin from "html-webpack-plugin"
import Dotenv from "dotenv-webpack"
import "webpack-dev-server"

export default {
  entry: "./src/index.tsx",
  output: {
    filename: "bundle.js",
    path: path.resolve(__dirname, "dist")
  },
  resolve: {
    extensions: [".tsx", ".ts", ".js"]
  },
  module: {
    rules: [
      {
        test: /\.(ts|tsx)$/,
        exclude: /node_modules/,
        use: "babel-loader"
      },
      {
        test: /\.s[ac]ss$/i,
        use: ["style-loader", "css-loader", "sass-loader"]
      }
    ]
  },
  plugins: [
    new HtmlWebpackPlugin({
      templateContent: `
        <!DOCTYPE html>
        <html lang="en">
          <head>
            <meta charset="UTF-8" />
            <meta name="viewport" content="width=device-width, initial-scale=1.0" />
            <title>Sweeper Smackdown</title>
          </head>
          <body>
            <div id="root"></div>
          </body>
        </html>
      `
    }),
    new Dotenv()
  ]
}
