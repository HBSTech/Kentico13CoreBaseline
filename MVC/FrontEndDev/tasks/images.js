const configs = require('../taskconfigs').configs;

const { dest, src } = require('gulp');
const imagewebp = require('gulp-webp');
const imagemin = require('gulp-imagemin');
const merge2 = require('merge2')
const { getEnvironmentConfigs } = require('./gulpTaskHelpers');

function images() {
    var imgConfigs = configs.images;
    var envConfigs = getEnvironmentConfigs(imgConfigs);
    return merge2(envConfigs.map(imgConfig => imagesStream(imgConfig)));
}

function imagesStream(imgConfig) {
    let stream = src(imgConfig.paths, imgConfig.base);
    if((imgConfig.copyUnoptimized ?? false)){
        var sourceDest = imgConfig.dest;
        if(imgConfig.originalFolder) {
            sourceDest += "/"+imgConfig.originalFolder;
        }
        stream.pipe(dest(sourceDest));
    }
    // optimize
    if(imgConfig.optimizeImages ?? true) {
        var optimizationDest = imgConfig.dest;
        if(imgConfig.optimizationFolder) {
            optimizationDest += "/"+imgConfig.optimizationFolder;
        }
            stream.pipe(imagemin([
                imagemin.mozjpeg({ quality: (imgConfig.jpegCompression ?? 80), progressive: (imgConfig.jpegProgressive ?? true) }),
                imagemin.optipng({ optiminzationLevel: (imgConfig.pngOptimizationLevel ?? 2) }),
            ]))
            .pipe(dest(optimizationDest));
    }

    // webp
    if(imgConfig.webpImages ?? true) {
        var webpDest = imgConfig.dest;
        if(imgConfig.webpFolder) {
            webpDest += "/"+imgConfig.webpFolder;
        }
        if(imgConfig.webpPng ?? false) {
            stream.exclude("*.png");
        }
        stream.pipe(imagewebp())
            .pipe(dest(webpDest));
    }

    return stream;
}



module.exports = { images, imagesStream };