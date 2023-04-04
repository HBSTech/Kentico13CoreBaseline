//Build Paths; add or remove to this variable to configure additional paths for compilation.
const configs = {
    typescript: [
        { 
            environments: ['dev', 'production'],
            // manualWebpackConfig: "../typescript/Helper/webpack.config.js",
            entry: "./typescript/Helper/index.ts",
            dest: "./js/bundles/footer-bundle/generated",
            fileName: "helpers.js",
            includeMapOnProduction: true,
            minify: false,
			watchPaths: ['./typescript/Helpers/**/*.ts']
        }
    ],
    react: [
        //{ 
        //    environments: ['dev', 'production'],
        //    // manualWebpackConfig: "../react/Sample/webpack.config.js",
        //    entry: "./react/SampleReact/app.tsx",
        //    dest: "./js/individual/generated",
        //    fileName: "sampleReact.js",
        //    includeMapOnProduction: true,
        //    minify: false,
		//	  watchPaths: ['./typescript/Helpers/**/*.ts',
		//	  './react/SampleReact/**/*.ts']
        }
    ],
    jsbundles: [
        {
			environments: ['dev', 'production'],
			paths: [
				// can also do map files of any raw of these, which copy over only on dev
				'./js/bundles/footer-bundle/**/*.js',
			],
			base: { base: "./js/bundles/footer-bundle" },
			dest: "../MVC/wwwroot/js/bundles",
			bundleName: "footer-bundle.js",
			bundleOnDev: true,
			minify: true, 
			gzip: true,
			obfuscateOnProduction: false, 
			includeMapOnProduction: true
		}
	],
    jsraw: [
		{
			name: "IndividualJS",
			environments: ['dev', 'production'],
			paths: ['./js/individual/**/*.js',
			'./js/individual/generated/*.js.map'],
			base: { base: "./js" },
			dest: "../MVC/wwwroot/js",
			minify: true,
			gzip: true,
			obfuscateOnProduction: false,
			includeMapOnProduction: true,
			// watchPaths optional, default to paths
		},
		{
			name: "BundleJS",
			environments: ['dev', 'production'],
			paths: ['./js/bundles/**/*.js',
			'./js/bundles/**/generated/*.js.map'],
			base: { base: "./js" },
			dest: "../MVC/wwwroot/js",
			bundle: false,
			minify: false,
			gzip: false,
			obfuscateOnProduction: false,
			includeMapOnProduction: true,
			// watchPaths optional, default to paths
		}
	],
    scss: [
		{
			paths: ['./sass/01_theme/*.scss',
				'./sass/02_custom/*.scss' ],
			base: { base: "'./sass" },
			dest: "../css/bundles/main-bundle/generated",
			fileName: "styles.css",
			optimizationLevelOnProduction: 2,
			includeMapOnProduction: true
		}
    ],
    cssbundles: [
        {
			environments: ['dev', 'production'],
			paths: [
				// can also do map files of any raw of these, which copy over only on dev
				'./css/bundles/main-bundle/**/*.css'
			],
			base: { base: "./css/bundles/main-bundle" },
			dest: "../MVC/wwwroot/css/bundles/main-bundle",
			bundleName: "main-bundle.css",
			bundleOnDev: true,
			minify: true,
			gzip: true,
			includeMapOnProduction: true
		}
	],
    cssraw: [
		{
			name: "IndividualCss",
			environments: ['dev', 'production'],
			paths: ['./css/individual/**/*.css',
				'./css/individual/generated/*.css'],
			base: { base: "./css" },
			dest: "../MVC/wwwroot/css",
			minify: true,
			gzip: true,
			includeMapOnProduction: true
		},
		{
			name: "BundleCSS",
			environments: ['dev', 'production'],
			paths: ['./css/bundles/**/*.css',
				'./css/bundles/generated/*.css'],
			base: { base: "./css" },
			dest: "../MVC/wwwroot/css",
			bundle: false,
			minify: false,
			gzip: false,
			includeMapOnProduction: true
		}
	],
    images: [
        {
            environments: ['dev', 'production'],
            paths: [
                './images/source/**/*.{jpg,jpeg,png}'
            ],
            base: { base: "./images/source" },
            dest: "../MVC/wwwroot/images",

            copyUnoptimized: true,
            originalFolder: "source",

            optimizeImages: true,
            optimizationFolder: "optimized",
            jpegCompression: 80,
            jpegProgressive: true,
            pngOptimizationLevel: 2,

            webpImages: true,
            webpFolder: "webp",
            webpPng: false
        }
    ],
    copy: [
        //{
        //    environments: ['dev', 'production'],
        //    paths: [
        //        // can also do map files of any raw of these, which copy over only on dev
        //        './fonts/**/**'
        //    ],
        //    base: { base: "./fonts" },
        //    dest: "../MVC/wwwroot/fonts"
        //}
    ]
};

const environment = {
    isDev: (typeof (process.env.NODE_ENV) === 'undefined' || process.env.NODE_ENV.toLowerCase() === "development" ? true : false)
};

module.exports = { configs, environment };