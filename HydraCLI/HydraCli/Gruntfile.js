module.exports = function(grunt) {

  grunt.loadNpmTasks('grunt-bump');

  grunt.initConfig({
      bump: {
        options: {
          files: ['package.json'],
          updateConfigs: [],
          commit: true,
          commitMessage: 'Release v%VERSION%',
          commitFiles: ['package.json'],
          createTag: false,
          tagName: 'v%VERSION%',
          tagMessage: 'Version %VERSION%',
          push: false,
          gitDescribeOptions: '--tags --always --abbrev=1 --dirty=-d',
          globalReplace: false,
          prereleaseName: false,
          metadata: '',
          regExp: false
        }
      },
    });

    grunt.registerTask("convertResx", "Converts resx files to resourceManager", () => {

      let resxConverter = require("resx-json-typescript-converter");

      grunt.log.writeln("Converting language resources");
     
      resxConverter.convertResx(["./HydraCli.en-US.resx" /*, "HydraCli.zh-CHS.resx" */], "./src/resources", { defaultResxCulture: "HydraCli.en-US.resx", mergeCulturesToSingleFile: true, generateTypeScriptResourceManager: true, searchRecursive: true });
    });
}