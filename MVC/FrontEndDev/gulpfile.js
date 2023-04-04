'use strict';
//Tasks
const configs = require('./taskconfigs').configs;

const { series, task, parallel, watch } = require("gulp");
const { getEnvironmentConfigs } = require('./tasks/gulpTaskHelpers');

// Modules, these are the various tasks you wish to use.
// Should have one that will grab the variables and processes them themselves
// and another that takes an individual config (for watches)
const { scss, scssStream } = require('./tasks/sass');
const { cssRaw, cssRawStream, cssBundle, cssBundleStream } = require('./tasks/css');
const { typescript, react, webpackResolve} = require('./tasks/typescript');
const { jsBundle, jsBundleStream, jsRaw, jsRawStream } = require('./tasks/javascript');
const { images, imagesStream } = require('./tasks/images');
const { copy, copyStream } = require('./tasks/copy');

function clean(callback) {
    // You can modify this to clean up any files not needed, this is called before both CSS and JS runs
    callback();
}

task("clean", series(clean));

task("build", series(
    parallel(
        series(clean, scss, cssRaw, cssBundle),
        series(clean, typescript, react, jsRaw, jsBundle),
        series(clean, images)
    ),
    series(clean, copy)
));

task("build:css", series(clean, scss, cssRaw, cssBundle))
task("build:scss", series(clean, scss));
task("build:cssbundle", series(clean, cssBundle));
task("build:cssraw", series(clean, cssRaw));

task("build:js", series(clean, typescript, react, jsRaw, jsBundle));
task("build:typescript", series(clean, typescript));
task("build:react", series(clean, react));
task("build:jsbundle", series(clean, jsBundle));
task("build:jsraw", series(clean, jsRaw));

task("build:images", series(clean, images));

task("watch", () => {
    getEnvironmentConfigs(configs.scss, true).forEach((config) => {
        var paths = config.watchPaths ?? config.paths;
        if(paths) {
            var watchFunction = () => scssStream(config);
            watchFunction.displayName = "scss:"+(config.name ?? config.outputName);
            watch(paths, watchFunction);
        }
    });
    getEnvironmentConfigs(configs.cssraw, true).forEach((config) => {
        var paths = config.watchPaths ?? config.paths;
        if(paths) {
            var watchFunction = () => cssRawStream(config);
            watchFunction.displayName = "cssRaw:"+(config.name ?? config.dest);
            watch(paths, watchFunction);
        }
    });
    getEnvironmentConfigs(configs.cssbundles, true).forEach((config) => {
        var paths = config.watchPaths ?? config.paths;
        if(paths) {
            var watchFunction = () => cssBundleStream(config);
            watchFunction.displayName = "cssBundles:"+(config.name ?? config.bundleName);
            watch(paths, watchFunction);
        }
    });
    getEnvironmentConfigs(configs.typescript, true).forEach((config) => {
        var paths = config.watchPaths ?? config.paths;
        if(paths) {
            var configArray = new Array();
            configArray.push(config);
            var watchFunction = () => webpackResolve(configArray);
            watchFunction.displayName = "typescript:"+(config.name ?? config.filename);
            watch(paths, watchFunction);
        }
    });
    getEnvironmentConfigs(configs.react, true).forEach((config) => {
        var paths = config.watchPaths ?? config.paths;
        if(paths) {
            var configArray = new Array();
            configArray.push(config);
            var watchFunction = () => webpackResolve(configArray);
            watchFunction.displayName = "react:"+(config.name ?? config.filename);
            watch(paths, watchFunction);
        }
    });
    getEnvironmentConfigs(configs.jsraw, true).forEach((config) => {
        var paths = config.watchPaths ?? config.paths;
        if(paths) {
            var watchFunction = () => jsRawStream(config);
            watchFunction.displayName = "jsRaw:"+(config.name ?? config.dest);
            watch(paths, watchFunction);
        }
    });
    getEnvironmentConfigs(configs.jsbundles, true).forEach((config) => {
        var paths = config.watchPaths ?? config.paths;
        if(paths) {
            var watchFunction = () => jsBundleStream(config);
            watchFunction.displayName = "jsBundles:"+(config.name ?? config.bundleName);
            watch(paths, watchFunction);
        }
    });
    getEnvironmentConfigs(configs.images, true).forEach((config) => {
        var paths = config.watchPaths ?? config.paths;
        if(paths) {
            var watchFunction = () => imagesStream(config);
            watchFunction.displayName = "images:"+(config.name ?? config.dest);
            watch(paths, watchFunction);
        }
    });
    getEnvironmentConfigs(configs.copy, true).forEach((config) => {
        var paths = config.watchPaths ?? config.paths;
        if(paths) {
            var watchFunction = () => copyStream(config);
            watchFunction.displayName = "copy:"+(config.name ?? config.dest);
            watch(paths, watchFunction);
        }
    });
});
