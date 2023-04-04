const configs = require('../taskconfigs').configs;
const env = require('../taskconfigs').environment;

const { dest, src } = require('gulp');
const obfuscate = require('gulp-obfuscate');
const terser = require('gulp-terser');
const gzip = require('gulp-gzip');
const concat = require('gulp-concat');
const rename = require('gulp-rename');
const merge2 = require('merge2');
const gulpif = require('gulp-if');
const sourcemaps = require('gulp-sourcemaps');
const removeSourcemaps = require('gulp-remove-sourcemaps');
const gulpIgnore = require('gulp-ignore');
const { getEnvironmentConfigs, removeExtension } = require('./gulpTaskHelpers');


function jsRaw() {
    var jsConfigs = configs.jsraw;
	var envConfigs = getEnvironmentConfigs(jsConfigs);
	return merge2(envConfigs.map(jsConfig => jsRawStream(jsConfig)));
}

function jsRawStream(jsConfig) {
    let stream = src(jsConfig.paths, jsConfig.base);

    // Only obfuscate on production
    if (jsConfig.obfuscateOnProduction && !env.isDev) {
        stream = ObfuscatePipeline(stream);
    } else if (jsConfig.minify) {
        // Obfuscate is incompatible with minification

        // Push original files and any map files (possibly generated from Typescript/react compilation)
        // Then ignore map files as we are going to make a map for minification only if set
        stream = stream.pipe(dest(jsConfig.dest))
            .pipe(gulpIgnore.exclude("*.map"))
            .pipe(gulpIgnore.exclude("*.map.js"));

        const includeSourceMap = (env.isDev || (jsConfig.includeMapOnProduction && !jsConfig.obfuscateOnProduction));
        stream = MinifyStream(stream, jsConfig, includeSourceMap, true);
    } else {
        stream = stream.pipe(dest(jsConfig.dest));
    }
    if (jsConfig.gzip) {
        stream = GZipStream(stream, jsConfig);
    }
    return stream;
}


function jsBundle() {
    var jsConfigs = configs.jsbundles;
	var envConfigs = getEnvironmentConfigs(jsConfigs);
	return merge2(envConfigs.map(jsConfig => jsBundleStream(jsConfig)));
};

function jsBundleStream(jsConfig) {
    // Copy over for Dev
    let stream = src(jsConfig.paths, jsConfig.base);
    if (env.isDev) {
         // Push original files and any map files (possibly generated from Typescript/react compilation)
        // Then ignore map files as we are going to make a map for minification only if set
        stream = stream
            .pipe(dest(jsConfig.dest));
    }

    // Remove source maps before concat and obfuscate operations
    stream = stream
        .pipe(gulpIgnore.exclude("*.map"))
        .pipe(gulpIgnore.exclude("*.map.js"))
        .pipe(removeSourcemaps())

    if (!env.isDev || jsConfig.bundleOnDev) {
        let obfuscate = !env.isDev && jsConfig.obfuscateOnProduction;
        let includeMap = !obfuscate && (env.isDev || jsConfig.includeMapOnProduction);

        stream = stream
            .pipe(gulpif(includeMap, sourcemaps.init()))
            .pipe(concat(jsConfig.bundleName))

        // Only obfuscate on production
        if (obfuscate) {
            stream = ObfuscatePipeline(stream);
        } else if (jsConfig.minify) {
            // Minify isn't compatible with obfuscate right now.
            stream = stream
                .pipe(dest(jsConfig.dest));
            stream = MinifyStream(stream, jsConfig, includeMap, false);
        } else {
            stream = stream.pipe(dest(jsConfig.dest));
        }
        if (jsConfig.gzip) {
            stream = GZipStream(stream, jsConfig);
        }
        return stream;
    }
    return stream;
}

function ObfuscatePipeline(stream) {
    return stream.pipe(obfuscate())
        .pipe(dest(jsConfig.dest));
}

function MinifyStream(stream, jsConfig, includeSourceMap, initSourceMap) {
    return stream
        .pipe(gulpif(includeSourceMap && initSourceMap, sourcemaps.init()))
        .pipe(terser())
        .pipe(rename(function (path) {
            path.basename = removeExtension(removeExtension(path.basename, ".js"), ".min");
            path.extname = ".min.js";
        }))
        .pipe(gulpif(includeSourceMap, sourcemaps.write(".", {
            mapFile: function (mapFilePath) {
                return mapFilePath.replace(".min.js.map", jsConfig.gzip ? ".min.map.gz" : ".min.map")
            }
        })))
        .pipe(dest(jsConfig.dest));
}

function GZipStream(stream, jsConfig) {
    return stream
        .pipe(gzip())
        .pipe(rename(function (path) {
            var isMap = path.basename.indexOf(".min.map.gz") > -1;
            var baseName = isMap ? path.basename.replace(".min.map.gz", "") : removeExtension(removeExtension(path.basename, ".js"), ".min");
            var extName = (jsConfig.minify ? ".min" : "") + (isMap ? ".map.gz" : ".js.gz");
            path.basename = baseName;
            path.extname = extName;
        }))
        .pipe(dest(jsConfig.dest)); // Write only .gz version
}

module.exports = { jsBundle, jsBundleStream, jsRaw, jsRawStream };