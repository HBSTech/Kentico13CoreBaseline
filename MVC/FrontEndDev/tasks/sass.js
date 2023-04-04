const { src, dest } = require("gulp");
const sass = require('gulp-sass')(require('sass'));
const maps = require('gulp-sourcemaps');
const concat = require('gulp-concat');
const env = require('../taskconfigs').environment;
const configs = require('../taskconfigs').configs;
const merge2 = require('merge2')

const gulpif = require('gulp-if');
const { getEnvironmentConfigs } = require('./gulpTaskHelpers');

function scss() {
   var scssConfigs = configs.scss;
   var envConfigs = getEnvironmentConfigs(scssConfigs);
   return merge2(envConfigs.map(scssConfig => scssStream(scssConfig)));
}

function scssStream(scssConfig) {
    const includeSourceMap = (env.isDev || (scssConfig.includeMapOnProduction));

    //assign sass to stream
    let stream = src(scssConfig.paths, scssConfig.base)
        .pipe(gulpif(includeSourceMap, maps.init()))
        .pipe(sass(
            {
                outputStyle: 'expanded',
                compatibility: '*', // (default) - Internet Explorer 10+ compatibility mode
                inline: ['all'], // enables all inlining, same as ['local', 'remote']
                level: env.isDev ? 0 : scssConfig.optimizationLevelOnProduction // Optimization levels. The level option can be either 0, 1 (default), or 2, e.g.
                // Please note that level 1 optimization options are generally safe while level 2 optimizations should be safe for most users.
            }
        ).on('error', sass.logError))
        .pipe(concat(scssConfig.fileName))
        .pipe(gulpif(includeSourceMap, maps.write(".")))//write sourcemaps
        .pipe(dest(scssConfig.dest)); //write file
    return stream;
}

module.exports = { scss, scssStream }