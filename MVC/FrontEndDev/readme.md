# Front End Dev System
This system is a starting point for front end development.  It contains the 'core' set of tools that most all websites need:

1. Sass Rendering
1. Typescript (and optional React) Rendering
1. CSS/JS Bundling
1. CSS/JS Minification/Gzip and mapping
1. Image Optimization + WebP

This repo uses Gulp, which is an older technology.  There may be faster and better tools out there, which feel free to leverage and replace as your Front End Development grows.

# Installation
To install, follow these steps:

1. Clone the FrontEndDev folder into your solution
1. Within the FrontEndDev folder, run `npm install`
1. Edit the `taskconfigs.js` and add in your sass, typescript, react, javascript, images, and css files into their appropriate folders (see below)

## Concept
This tool contains various "Tasks" that perform each type of action.  The `taskconfigs.js` file contains your configuration(s) that are passed to each task.  You can modify and add configurations to the `taskconfigs.js` file.

Within the FrontEndDev system exists various folders:

* **css/individual**: Individual CSS files
* **css/bundles/BUNDLENAME**: Bundles of CSS Files
* **js/individual**: Individual JS Files
* **js/bundles/BUNDLENAME**: Bundles of JS Files
* **sass**: Sass files used for generating CSS (Either to css/individual/generated or css/bundles/BUNDLENAME/generated)
* **typescript**: Typescript files used either in React, or for generating Javascript (Either to js/individual/generated or js/bundles/BUNDLENAME/generated)
* **react**: React files used for generating Javascript (Either to js/individual/generated or js/bundles/BUNDLENAME/generated)
* **images/source**: Source images, any images here get optimized to the images/optimized or images/webp folders if they are jpeg/jpg/png
* **tasks**: The Gulp Tasks (shouldn't need to touch)

These files ultimately build out then to the final destination via the CssRaw, CssBundle, JsRaw, JsBundle, ImageOptimization, and Copy Tasks.

## Order of Operations
The tasks run by default in the following order:

Javascript:
1. Typescript Compilation
2. React Compilation
3. JS Bundling
4. JS Raw Copy

CSS:
1. Sass Compilation
2. CSS Bundling
3. CSS Raw Copy

Images:
1. Image Optimization

Copy:
1. General Copy

## Configuration Properties
Below are all the configurations and which are applicable for each task.
|                               | Required | Default     | Type                     | Description                                                                                                                                                                                                        | typescript | react | jsbundles | jsraw | scss | cssbundles | cssraw | images | copy |
|-------------------------------|----------|-------------|--------------------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|------------|-------|-----------|-------|------|------------|--------|--------|------|
| environments                  | x        |             | Array[string]            | Environments this task should run on ('dev' or 'production')                                                                                                                                                       | x          | x     | x         | x     | x    | x          | x      | x      | x    |
| paths                         | x        |             | Blob                     | Array of gulp blob paths of the files to include in the operation. For a bundle, these files will be bundled together.                                                                                             |            |       | x         | x     |      | x          | x      | x      | x    |
| entry                         |          |             | string                   | The Path to the entry Typescript/React file (if you are not using a manual webpack)                                                                                                                                | x          | x     |           |       |      |            |        |        |      |
| manualWebpackConfig           |          |             | string                   | The path to the webpack file (if you wish to do it manually)                                                                                                                                                       | x          | x     |           |       |      |            |        |        |      |
| base                          | x        |             | Object (with base value) | Json object with property "base" at minimum, which establishes the 'starting folder' for your paths.  When files are generated to the dest folder, it will replicate any folder structure beyond the 'base' value. |            |       | x         | x     | x    | x          | x      | x      | x    |
| dest                          | x        |             | string                   | The path to the folder where the generated file will be written                                                                                                                                                    | x          | x     | x         | x     | x    | x          | x      | x      | x    |
| fileName                      | x        |             | string                   | The fileName that will be generated (ex: foo.css or bar.js)                                                                                                                                                        | x          | x     |           |       | x    |            |        |        |      |
| bundleName                    | x        |             | string                   | The bundle File Name that will be generated (ex my-bundle.css or my-bundle.js)                                                                                                                                     |            |       | x         |       |      | x          |        |        |      |
| watchPaths                    |          | paths value | Gulp Blob                | Paths that should be watched for changes (for Gulp Watch). Changes in these files trigger the task to re-run. If not provided, uses the paths property                                                             |            |       |           |       |      |            |        |        |      |
| includeMapOnProduction        | x        | false       | boolean                  | If map files should be generated when run on production                                                                                                                                                            | x          | x     | x         |       | x    | x          |        |        |      |
| minify                        |          | false       | boolean                  | If a minified version should be generated along side the normal                                                                                                                                                    | x          | x     | x         | x     |      | x          | x      |        |      |
| gzip                          |          | false       | boolean                  | If a gzipped version should be generated along side the normal                                                                                                                                                     |            |       | x         | x     |      | x          | x      |        |      |
| bundleOnDev                   |          | false       | boolean                  | If you should bundle the files when on the dev environment                                                                                                                                                         |            |       | x         |       |      | x          |        |        |      |
| obfuscateOnProduction         |          | false       | boolean                  | If the javascript should be obfuscated for production                                                                                                                                                              |            |       | x         | x     |      |            |        |        |      |
| optimizationLevelOnProduction | x        | int (0-2)   |                          | The optimization level (used only on production, 0 on Dev)                                                                                                                                                         |            |       |           |       | x    |            |        |        |      |
| copyUnoptimized               |          | false       | boolean                  | If unoptimized images should also be copied over (recommended true)                                                                                                                                                |            |       |           |       |      |            |        | x      |      |
| originalFolder                |          | string      |                          | Folder name that gets appended to the destination location. Used if copyUnoptimized is true to indicate the 'source' folder                                                                                        |            |       |           |       |      |            |        | x      |      |
| optimizeImages                |          | true        | boolean                  | If images should be optimized                                                                                                                                                                                      |            |       |           |       |      |            |        | x      |      |
| optimizationFolder            |          | string      |                          | Folder name that gets appended to the destination location. Used if optimizedImages is true to indicate where the optimized images are.                                                                            |            |       |           |       |      |            |        | x      |      |
| webpImages                    |          | true        | boolean                  | If images should be converted to webp                                                                                                                                                                              |            |       |           |       |      |            |        | x      |      |
| webpFolder                    |          | string      |                          | Folder name that gets appended to the destination location. Used if webpImages is true to indicate where the webp images are.                                                                                      |            |       |           |       |      |            |        | x      |      |
| webpPng                       |          | false       | boolean                  | If PNGs should be webp, PNGs can often get larger in size if they are made up of few colors, however you should set to true if you have pngs of photographs with transparency as webp products smaller file size.  |            |       |           |       |      |            |        | x      |      |
| jpegCompression               |          | int         | 80                       | Jpeg compression value, 0-100                                                                                                                                                                                      |            |       |           |       |      |            |        | x      |      |
| jpegProgressive               |          | boolean     | true                     | If progressive Jpeg compression should be used.                                                                                                                                                                    |            |       |           |       |      |            |        | x      |      |