const path = require("path");
const RazorPartialViewsWebpackPlugin = require("razor-partial-views-webpack-plugin");
const ModuleFederationPlugin = require("webpack/lib/container/ModuleFederationPlugin");
const HtmlWebPackPlugin = require("html-webpack-plugin");

const deps = require("./package.json").dependencies;
module.exports = {
    entry: {
        app: path.resolve(__dirname, "wwwroot/js/mf1.jsx")
    },
    output: {
        filename: "[name].js",
        path: path.resolve(__dirname, "wwwroot/js/output"),
		publicPath: "~/js/output",
    },
    resolve: {
        extensions: [".js", ".jsx"]
    },
    module: {
        rules: [
            {
              test: /\.m?js/,
              type: "javascript/auto",
              resolve: {
                fullySpecified: false,
              },
            },
            {
              test: /\.(css|s[ac]ss)$/i,
              use: ["style-loader", "css-loader", "postcss-loader"],
            },
            {
              test: /\.(ts|tsx|js|jsx)$/,
              exclude: /node_modules/,
              use: {
                loader: "babel-loader",
              },
            },
        ]
    },
	plugins: [
		new ModuleFederationPlugin({
		  name: "NetCoreApp",
		  filename: "remoteEntry.js",
		  remotes: {
			statistics: "statistics@http://localhost:3001/remoteEntry.js/"
		  }
		}),
        new HtmlWebPackPlugin({
			template: "templates/stats-mf.html",
		})
    ]
};