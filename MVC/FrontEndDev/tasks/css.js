const configs = require('../taskconfigs').configs;
const env = require('../taskconfigs').environment;

const { dest, src } = require('gulp');
const gzip = require('gulp-gzip');
const concat = require('gulp-concat');
const rename = require('gulp-rename');
const merge2 = require('merge2')
const gulpif = require('gulp-if');
const sourcemaps = require('gulp-sourcemaps');
const minify = require('gulp-clean-css');
const removeSourcemaps = require('gulp-remove-sourcemaps');
const gulpIgnore = require('gulp-ignore');
const { getEnvironmentConfigs, removeExtension } = require('./gulpTaskHelpers');


function cssRaw() {
    var cssConfigs = configs.cssraw;
    var envConfigs = getEnvironmentConfigs(cssConfigs);
    return merge2(envConfigs.map(cssConfig => cssRawStream(cssConfig)));
}

function cssRawStream(cssConfig) {
    let stream = src(cssConfig.paths, cssConfig.base);

    if (cssConfig.minify) {
        // Obfuscate is incompatible with minification

        // Push original files and any map files (possibly generated from Typescript/react compilation)
        // Then ignore map files as we are going to make a map for minification only if set
        stream = stream.pipe(dest(cssConfig.dest))
            .pipe(gulpIgnore.exclude("*.map"))
            .pipe(gulpIgnore.exclude("*.map.css"));

        const includeSourceMap = (env.isDev || (cssConfig.includeMapOnProduction));
        stream = MinifyStream(stream, cssConfig, includeSourceMap, true);
    } else {
        stream = stream.pipe(dest(cssConfig.dest));
    }
    if (cssConfig.gzip) {
        stream = GZipStream(stream, cssConfig);
    }
    return stream;
}


function cssBundle() {
    var cssConfigs = configs.cssbundles;
    var envConfigs = getEnvironmentConfigs(cssConfigs);
    return merge2(envConfigs.map(cssConfig => cssBundleStream(cssConfig)));
};

function cssBundleStream(cssConfig) {
    // Copy over for Dev
    let stream = src(cssConfig.paths, cssConfig.base);
    if (env.isDev) {
        // Push original files and any map files (possibly generated from Typescript/react compilation)
        // Then ignore map files as we are going to make a map for minification only if set
        stream = stream
            .pipe(dest(cssConfig.dest))
    }

    // Remove source maps before concat
    stream = stream
        .pipe(gulpIgnore.exclude("*.map"))
        .pipe(gulpIgnore.exclude("*.map.css"))
        .pipe(removeSourcemaps());

    if (!env.isDev || cssConfig.bundleOnDev) {
        let includeMap = (env.isDev || cssConfig.includeMapOnProduction);

        stream = stream
            .pipe(gulpif(includeMap, sourcemaps.init()))
            .pipe(concat(cssConfig.bundleName))

        if (cssConfig.minify) {
            stream = stream
                .pipe(dest(cssConfig.dest));
            stream = MinifyStream(stream, cssConfig, includeMap, false);
        } else {
            stream = stream.pipe(dest(cssConfig.dest));
        }
        if (cssConfig.gzip) {
            stream = GZipStream(stream, cssConfig);
        }
        return stream;
    }
    return stream;
}

function MinifyStream(stream, cssConfig, includeSourceMap, initSourceMap) {
    return stream
		.pipe(removeSourcemaps()) // Source map links also blow up when minifying
        .pipe(gulpif(includeSourceMap && initSourceMap, sourcemaps.init()))
        .pipe(minify())
        .pipe(rename(function (path) {
            path.basename = removeExtension(removeExtension(path.basename, ".css"), ".min");
            path.extname = ".min.css";
        }))
        .pipe(gulpif(includeSourceMap, sourcemaps.write(".", {
            mapFile: function (mapFilePath) {
                return mapFilePath.replace(".min.css.map", cssConfig.gzip ? ".min.map.gz" : ".min.map")
            }
        })))
        .pipe(dest(cssConfig.dest));
}

function GZipStream(stream, cssConfig) {
    return stream
        .pipe(gzip())
        .pipe(rename(function (path) {
            var isMap = path.basename.indexOf(".min.map.gz") > -1;
            var baseName = isMap ? path.basename.replace(".min.map.gz", "") : removeExtension(removeExtension(path.basename, ".css"), ".min");
            var extName = (cssConfig.minify ? ".min" : "") + (isMap ? ".map.gz" : ".css.gz");
            path.basename = baseName;
            path.extname = extName;
        }))
        .pipe(dest(cssConfig.dest)); // Write only .gz version
}



module.exports = { cssBundle, cssBundleStream,  cssRaw, cssRawStream };