const env = require('../taskconfigs').environment;

// Filters the config by the environments if present, if no environment defined or it's an empty array, then includes it as a default.
// For watch will also filter out any config that has watch: false.
function getEnvironmentConfigs(configs, forWatch = false) {
    var configArray = [];
    for (var i = 0; i < configs.length; i++) {
        var config = configs[i];
        if (!config.environments || config.environments.length == 0 || config.environments.includes(env.isDev ? 'dev' : 'production')) {
            if(!forWatch || (config.watch ?? true)) {
                configArray.push(config);
            }
        }
    }
    return configArray;
}

// Removes the given extension from the given path, used for file renaming purposes
function removeExtension(path, ext) {
    if (ext.indexOf(".") != 0) {
        ext = "." + ext;
    }
    var newPath = path;
    if (path.endsWith(ext)) {
        newPath = path.substring(0, path.length - ext.length);
    }
    return newPath;
}

module.exports = { removeExtension, getEnvironmentConfigs }