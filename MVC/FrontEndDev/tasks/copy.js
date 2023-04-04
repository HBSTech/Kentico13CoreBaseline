const configs = require('../taskconfigs').configs;

const { dest, src } = require('gulp');
const merge2 = require('merge2')
const { getEnvironmentConfigs } = require('./gulpTaskHelpers');

function copy() {
    var copyConfigs = configs.copy;
    var envConfigs = getEnvironmentConfigs(copyConfigs);
    return merge2(envConfigs.map(copyConfig => copyStream(copyConfig)));
}

function copyStream(copyConfig) {
    let stream = src(copyConfig.paths, copyConfig.base);
    stream.pipe(dest(copyConfig.dest));
    return stream;
}

module.exports = { copy, copyStream };