const path = require('path');
const isDevelopment = (process.env.NODE_ENV !== undefined && process.env.NODE_ENV.toLowerCase() === "development" ? true : false);

module.exports = {
    entry: "/FrontEndDev/typescript/Header/header.ts",
    mode: isDevelopment ? "development" : "production",
    output: {
        filename: "HeaderCustom.js",
        path: path.resolve(__dirname, "../../../wwwroot/js/scripts")
    },
    // Enable sourcemaps for debugging webpack's output. 
    devtool: "source-map",
    resolve: {
        // Add '.ts' and '.tsx' as resolvable extensions. 
        extensions: [".ts", ".tsx", ".js"]
    },
    module: {
        rules: [
            {
                test: /\.ts(x?)$/,
                exclude: /node_modules/,
                use: [
                    {
                        loader: "ts-loader"
                    }
                ]
            },

            // All output '.js' files will have any sourcemaps re-processed by 'source-map-loader'. 
            {
                enforce: "pre",
                test: /\.js$/,
                loader: "source-map-loader"
            }
        ]
    },
    optimization: {
        minimize: false
    }
} 