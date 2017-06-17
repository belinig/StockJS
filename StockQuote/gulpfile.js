/// <binding BeforeBuild='clean, css, min, min:css, min:js' Clean='clean:css, clean, clean:js' ProjectOpened='watchless' />
"use strict";

var gulp = require("gulp"),
    rimraf = require("rimraf"),
    concat = require("gulp-concat"),
    cssmin = require("gulp-cssmin"),
    uglify = require("gulp-uglify"),
    rename = require("gulp-rename");
var sourceMap = require('gulp-sourcemaps');
var watchLess = require('gulp-watch-less');
var less = require('gulp-less');

var paths = {
    webroot: "./wwwroot/"
};

paths.js = paths.webroot + "js/**/*.js";
paths.minJs = paths.webroot + "js/**/*.min.js";
paths.css = paths.webroot + "css/**/*.css";
paths.minCss = paths.webroot + "css/**/*.min.css";
paths.siteCss = paths.webroot + "css/site.css";
paths.concatJsDest = paths.webroot + "js/site.min.js";
paths.concatMinCssDest = paths.webroot + "css/site.min.css";
paths.concatCssDest = paths.webroot + "css/site.css";
paths.mapCss = paths.webroot + "css/*.map";
paths.siteLess = paths.webroot + 'less/stocks.less';
paths.watchLess = paths.webroot + 'less/*.less';
paths.destLess = paths.webroot + 'css';


gulp.task("clean:js", function (cb) {
    rimraf(paths.concatJsDest, cb);
});

gulp.task("clean:css", function (cb) {
    rimraf(paths.concatMinCssDest, function (error) {
        if (error) {
            cb(error);
        };
    });
    rimraf(paths.concatCssDest, function (error) {
        if (error) {
            cb(error);
        };
    });
    rimraf(paths.mapCss, cb);
});

gulp.task("clean", ["clean:js", "clean:css"]);

gulp.task("min:js", function () {
    return gulp.src([paths.js, "!" + paths.minJs], { base: "." })
        .pipe(concat(paths.concatJsDest))
        .pipe(uglify())
        .pipe(gulp.dest("."));
});

gulp.task("min:css", function () {
    return gulp.src([paths.css, "!" + paths.minCss])
        .pipe(sourceMap.init())
        .pipe(concat(paths.concatMinCssDest))
        .pipe(cssmin())
        .pipe(sourceMap.write('.'))
        .pipe(gulp.dest("."));
});

gulp.task("min", ["min:js", "min:css"]);


//gulp.task("concat:css", function () {
//    return gulp.src([paths.css, "!" + paths.minCss,"!"+paths.siteCss])
//        .pipe(sourceMap.init())
//        .pipe(concat(paths.concatCssDest))
//        .pipe(sourceMap.write('.'))
//        .pipe(gulp.dest("."));
//});


gulp.task('watchless', function () {
    return watchLess(paths.siteLess, function () {
        gulp.src(paths.siteLess)
            .pipe(rename({ basename: "site" }))
            .pipe(sourceMap.init())
            .pipe(less())
            .pipe(sourceMap.write('.'))
            .pipe(gulp.dest(paths.destLess));
    })
});

gulp.task('css', function () {
    return   gulp.src(paths.siteLess)
            .pipe(rename({ basename: "site" }))
            .pipe(sourceMap.init())
            .pipe(less())
            .pipe(sourceMap.write('.'))
            .pipe(gulp.dest(paths.destLess));
});