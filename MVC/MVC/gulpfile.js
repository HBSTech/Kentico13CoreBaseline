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
const typescript = require('gulp-typescript');
const sourcemaps = require('gulp-sourcemaps');
const concat = require('gulp-concat');

// run npm install --save-dev gulp-react
//const react = require('gulp-react');



//create functions
function headerLess() {
    return src("wwwroot/css/less/*.less")
        .pipe(less())
        .pipe(prefix())
        .pipe(rename({ extname: ".css" }))
        .pipe(dest("wwwroot/css"));
}

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

function footerTypescript() {
    return src("wwwroot/js/typescript/footer/*.{ts,tsx}")
        .pipe(typescript())
        .pipe(rename("footer.js"))
        .pipe(dest('wwwroot/js/scripts'));
}

function footerJs() {
    return src([
        "wwwroot/js/jquery.unobtrusive.ajax/jquery.unobtrusive-ajax.min.js",
        "wwwroot/js/scripts/footer.js",
        "wwwroot/js/bootstrap/popper.min.js",
        "wwwroot/js/bootstrap/bootstrap.min.js"
    ], { base: "wwwroot/" })
        .pipe(concat("FooterJS.js"))
        .pipe(dest('wwwroot/js/bundles'))
        .pipe(sourcemaps.init({ loadMaps: true, sourcemaps: true }))
        .pipe(terser())
        .pipe(sourcemaps.write('./maps'))
        .pipe(rename("FooterJS.min.js"))
        .pipe(dest('wwwroot/js/bundles'))
        .pipe(gzip())
        .pipe(rename("FooterJS.min.js.gz"))
        .pipe(dest('wwwroot/js/bundles'));
}

function headerTypescript() {
    return src("wwwroot/js/typescript/header/*.{ts,tsx}")
        .pipe(typescript())
        .pipe(rename("header.js"))
        .pipe(dest('wwwroot/js/scripts'));
}

function headerJs() {
    return src([
        "wwwroot/js/scripts/jquery-3.5.1.min.js",
        "wwwroot/js/scripts/header.js"
    ], { base: "wwwroot/" })
        .pipe(concat("HeaderJS.js"))
        .pipe(dest('wwwroot/js/bundles'))
        .pipe(sourcemaps.init({ loadMaps: true, sourcemaps: true }))
        .pipe(terser())
        .pipe(sourcemaps.write('./maps'))
        .pipe(rename("HeaderJS.min.js"))
        .pipe(dest('wwwroot/js/bundles'))
        .pipe(gzip())
        .pipe(rename("HeaderJS.min.js.gz"))
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
        series(headerTypescript, headerJs),
        series(footerTypescript, footerJs),
        series(headerLess, headerCss),
        series(imageOptimize, imageWebp)
    )
);

// packages.json's development task (npm run development) is set to execute this watch on visual studio open.
task("Watch", function () {
    watch(["wwwroot/js/typescript/header/*.{ts,tsx}"], series(headerTypescript, headerJs));
    watch(["wwwroot/js/typescript/footer/*.{ts,tsx}"], series(footerTypescript, footerJs));
    watch(["wwwroot/css/less/*.less"], series(headerLess, headerCss));
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