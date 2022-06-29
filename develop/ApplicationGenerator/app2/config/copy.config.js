module.exports = {
    jasmineContent:
    {
        src: ['{{SRC}}/JasmineTests.html'],
        dest: '{{WWW}}'
    },
    jasmineBuild:
    {
        src: [
                '{{ROOT}}/node_modules/jasmine-core/lib/jasmine-core/jasmine.js',
                '{{ROOT}}/node_modules/jasmine-core/lib/jasmine-core/jasmine-html.js',
                '{{ROOT}}/node_modules/jasmine-core/lib/jasmine-core/boot.js',
                '{{ROOT}}/node_modules/zone.js/dist/zone.js',
                '{{ROOT}}/node_modules/zone.js/dist/long-stack-trace-zone.js',
                '{{ROOT}}/node_modules/zone.js/dist/proxy.js',
                '{{ROOT}}/node_modules/zone.js/dist/sync-test.js',
                '{{ROOT}}/node_modules/zone.js/dist/jasmine-patch.js',
                '{{ROOT}}/node_modules/zone.js/dist/async-test.js',
                '{{ROOT}}/node_modules/zone.js/dist/fake-async-test.js'
            ],
        dest: '{{BUILD}}'
    },
    copyAssets: {
        src: ['{{SRC}}/assets/**/*'],
        dest: '{{WWW}}/assets'
    },
    copyIndexContent: {
        src: ['{{SRC}}/index.html', '{{SRC}}/manifest.json', '{{SRC}}/service-worker.js'],
        dest: '{{WWW}}'
    },
    copyFonts: {
        src: ['{{ROOT}}/node_modules/ionicons/dist/fonts/**/*', '{{ROOT}}/node_modules/ionic-angular/fonts/**/*'],
        dest: '{{WWW}}/assets/fonts'
    },
    copyPolyfills: {
        src: ['{{ROOT}}/node_modules/ionic-angular/polyfills/polyfills.js'],
        dest: '{{BUILD}}'
    },
    copySwToolbox: {
        src: ['{{ROOT}}/node_modules/sw-toolbox/sw-toolbox.js'],
        dest: '{{BUILD}}'
    },
    copyAnimateCss: {
        src: './node_modules/animate.css/animate.css',
        dest: '{{BUILD}}'
    }
}
