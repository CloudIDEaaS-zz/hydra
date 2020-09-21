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
          createTag: true,
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
}