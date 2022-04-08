/// <binding ProjectOpened='Watch' />
//list dependences
const { src, dest, watch, series, task, parallel } = require('gulp');
const gulpif = require('gulp-if');
const rename = require('gulp-rename');
const less = require('gulp-less');
const prefix = require('gulp-autoprefixer');
const minify = require('gulp-clean-css');
const terser = require('gulp-terser');
const imagewebp = require('gulp-webp');
const imagemin = require('gulp-imagemin');
const gzip = require('gulp-gzip');
const concat = require('gulp-concat');
const webpack = require('webpack');

const isDevelopment = (typeof(process.env.NODE_ENV) === 'undefined' || process.env.NODE_ENV.toLowerCase() === "development" ? true : false);

// This will make a .css for each less file found
function headerLess() {
    return src("FrontEndDev/less/*.less")
        .pipe(less())
        .pipe(prefix())
        .pipe(rename({ extname: ".css" }))
        .pipe(dest("wwwroot/css"));
}


const webpackConfigHelper = require('./FrontEndDev/typescript/Helper/webpack.config');
function helperTypescript(cb) {
    return new Promise((resolve, reject) => {
        webpack(webpackConfigHelper, (err, stats) => {
            if (err) {
                return reject(err)
            }
            if (stats.hasErrors()) {
                return reject(new Error(stats.compilation.errors.join('\n')))
            }
            resolve()
        })
    });
}

const webpackConfigCustom = require('./FrontEndDev/typescript/Custom/webpack.config');
function customTypescript(cb) {
    return new Promise((resolve, reject) => {
        webpack(webpackConfigCustom, (err, stats) => {
            if (err) {
                return reject(err)
            }
            if (stats.hasErrors()) {
                return reject(new Error(stats.compilation.errors.join('\n')))
            }
            resolve()
        })
    });
}

const webpackConfigHeader = require('./FrontEndDev/typescript/Header/webpack.config');
function headerTypescript() {
    return new Promise((resolve, reject) => {
        webpack(webpackConfigHeader, (err, stats) => {
            if (err) {
                return reject(err)
            }
            if (stats.hasErrors()) {
                return reject(new Error(stats.compilation.errors.join('\n')))
            }
            resolve()
        })
    });
}

const webpackConfigReactSample = require('./FrontEndDev/react/sampleapp/webpack.config');
function sampleReactApp() {
    return new Promise((resolve, reject) => {
        webpack(webpackConfigReactSample, (err, stats) => {
            if (err) {
                return reject(err)
            }
            if (stats.hasErrors()) {
                return reject(new Error(stats.compilation.errors.join('\n')))
            }
            resolve()
        })
    });
}


/* Bunding / minifying CSS / Javascript */
function headerCss() {
    return src([
        "wwwroot/css/bootstrap/bootstrap.min.css",
        "wwwroot/css/custom.css"
    ], { base: "wwwroot/" })
        .pipe(concat("HeaderStyles.css"))
        .pipe(dest('wwwroot/css/bundles'))
        .pipe(minify())
        .pipe(rename("HeaderStyles.min.css"))
        .pipe(dest('wwwroot/css/bundles'))
        .pipe(gzip())
        .pipe(rename("HeaderStyles.min.css.gz"))
        .pipe(dest("wwwroot/css/bundles"));
}

function headerJs() {
    return src([
        "wwwroot/js/scripts/jquery-3.5.1.min.js",
        "wwwroot/js/scripts/HeaderCustom.js"
    ], { base: "wwwroot/" })
        .pipe(concat("HeaderJS.js"))
        .pipe(dest('wwwroot/js/bundles'))
        .pipe(terser())
        .pipe(rename("HeaderJS.min.js"))
        .pipe(dest('wwwroot/js/bundles'))
        .pipe(gzip())
        .pipe(rename("HeaderJS.min.js.gz"))
        .pipe(dest('wwwroot/js/bundles'));
}

function footerJs() {
    // Can optionally include your react apps here as well
    return src([
        "wwwroot/js/jquery.unobtrusive.ajax/jquery.unobtrusive-ajax.min.js",
        "wwwroot/js/scripts/helpers.js",
        "wwwroot/js/scripts/Custom.js",
        "wwwroot/js/bootstrap/popper.min.js",
        "wwwroot/js/bootstrap/bootstrap.min.js"
    ], { base: "wwwroot/" })
        .pipe(concat("FooterJS.js"))
        .pipe(dest('wwwroot/js/bundles'))
        .pipe(terser())
        .pipe(rename("FooterJS.min.js"))
        .pipe(dest('wwwroot/js/bundles'))
        .pipe(gzip())
        .pipe(rename("FooterJS.min.js.gz"))
        .pipe(dest('wwwroot/js/bundles'));
}


// optimized images, if adjusted make sure to adjust OptimizedPictureTagHelper.cs as well.
function imageOptimize() {
    return src('wwwroot/images/src/*.{jpg,jpeg,png}')
        .pipe(imagemin([
            imagemin.mozjpeg({ quality: 80, progressive: true }),
            imagemin.optipng({ optiminzationLevel: 2 }),
        ]))
        .pipe(dest('wwwroot/images/optimized'));
}

// webp images, if adjusted make sure to adjust OptimizedPictureTagHelper.cs as well.
function imageWebp() {
    // PNG is often 'larger' than optimized
    return src('wwwroot/images/src/*.{jpg,jpeg}')
        .pipe(imagewebp())
        .pipe(dest('wwwroot/images/webp'));
}

// Individual Tasks that can be ran from Task Runner Explorer
task("Build",
    parallel(
        series(headerLess, headerCss),
        series(headerTypescript, headerJs),
        series(customTypescript, helperTypescript, sampleReactApp, footerJs),
        series(imageOptimize, imageWebp)
    )
);

// packages.json's development task (npm run development) is set to execute this watch on visual studio open.
task("Watch", function () {
    watch(["FrontEndDev/less/*.less"], series(headerLess, headerCss));
    watch(["FrontEndDev/typescript/Header/*.{js,ts}"], series(headerTypescript, headerJs));
    watch(["FrontEndDev/typescript/Custom/*.{js,ts}"], series(customTypescript, footerJs));
    watch(["FrontEndDev/typescript/Helper/*.{js,ts}"], series(helperTypescript, footerJs));
    watch(["FrontEndDev/react/sampleapp/*.{ts,tsx,js,jsx}"], series(sampleReactApp, footerJs));
    watch(["wwwroot/css/images/src/*.{jpg,jpeg,png}"], series(imageOptimize, imageWebp));
})

// Helper Function
function gulpTaskIf(condition, task) {
    task = series(task) // make sure we have a function that takes callback as first argument
    return function (cb) {
        if (condition()) {
            task(cb)
        } else {
            cb()
        }
    }
}