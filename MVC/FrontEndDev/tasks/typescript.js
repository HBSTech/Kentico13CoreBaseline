const configs = require('../taskconfigs').configs;
const env = require('../taskconfigs').environment;
const webpack = require('webpack');
const path = require('path');
const { getEnvironmentConfigs, removeExtension } = require('./gulpTaskHelpers');


function typescript() {
    var tsConfigs = configs.typescript;
    var envConfigs = getEnvironmentConfigs(tsConfigs);
    return webpackResolve(envConfigs);
}

function react() {
    var reactConfigs = configs.react;
    var envConfigs = getEnvironmentConfigs(reactConfigs);
    return webpackResolve(envConfigs);
}

// Could not get webpack to work as a readable stream for merge, so have to send an array of configs
function webpackResolve(envConfigs){
    return new Promise((resolve, reject) => {
        // Loop through configs and webpack.
        for(var i=0; i<envConfigs.length ; i++) {
            // Get or build webpackConfig
            var config = getWebpackConfig(envConfigs[i]);
            
            //log.info("Running webpack: "+config.entry+" => "+config.output.path+"\\"+config.output.filename);
            //log.info(config);
            webpack(config, (err, stats) => {
                if (err) {
                    return reject(err)
                }
                if (stats.hasErrors()) {
                    return reject(new Error(stats.compilation.errors.join('\n')))
                }
            });
        }
        // Resolve finished
        resolve()

    });
}

function getWebpackConfig(config) {
    // Link to webpack.config already given.
    if(config.manualWebpackConfig){
        return require(config.manualWebpackConfig);
    }
    // Generate webpack.config from configuration
    var webpackConfig = {};
    webpackConfig.entry = path.resolve(config.entry);
    webpackConfig.mode = env.isDev ? "development" : "production";
    webpackConfig.output = {
        filename: (config.minify ? removeExtension(config.fileName, ".js")+".min.js" : config.fileName),
        path: path.resolve(config.dest)
    };
    var sourceMap = (env.isDev || config.includeMapOnProduction);
    if(sourceMap) {
        webpackConfig.devtool = "source-map";
        webpackConfig.module = {
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
        }
    }
    webpackConfig.resolve = { extensions: [".ts", ".tsx", ".js"]};
    
    webpackConfig.optimization = {
        minimize: config.minify
    }
    
    return webpackConfig;
}


module.exports = {
    typescript, react, webpackResolve
}