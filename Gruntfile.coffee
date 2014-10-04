module.exports = (grunt) ->
  require('time-grunt')(grunt)

  grunt.initConfig
    pkg: grunt.file.readJSON 'package.json'
    bump:
      options:
        files: ['package.json']
        updateConfigs: []
        commit: true
        commitMessage: 'Release v%VERSION%'
        commitFiles: ['package.json']
        createTag: true
        tagName: 'v%VERSION%'
        tagMessage: 'Version %VERSION%'
        push: true
        pushTo: 'upstream'
        gitDescribeOptions: '--tags --always --abbrev=1 --dirty=-d'
    clean:
      build: ['src/**/bin', 'src/**/obj', '*.nupkg']
    assemblyinfo:
      options:
        files: ['src/MultiStyle.sln']
        info:
          title: '<%= pkg.name %>'
          description: 'Compose your WPF styles just like in CSS.'
          configuration: 'Debug'
          company: '<%= pkg.company %>'
          product: '<%= pkg.name %>'
          copyright: 'Copyright 2014 © <%= pkg.company %>'
          trademark: '<%= pkg.company %>'
          version: '<%= pkg.version %>'
          fileVersion: '<%= pkg.version %>'
    nugetrestore:
      restore:
        src: 'src/MultiStyle.sln'
        dest: 'src/packages/'
    msbuild:
      src: ['src/MultiStyle.sln']
      options:
        projectConfiguration: 'Debug'
        targets: ['Clean', 'Rebuild']
        stdout: true
        version: 4.0
        verbosity: 'quiet'
    mspec:
      options:
        toolsPath: 'src/packages/Machine.Specifications.0.8.1/tools'
        output: 'reports/mspec'
      specs:
        src: ['src/**/bin/Debug/*.Tests.dll']
    nugetpack:
      dist:
        src: 'src/MultiStyle/MultiStyle.csproj'
        dest: '.'
    nugetpush:
      dist:
        src: '*.nupkg'

  grunt.task.registerTask 'banner', () ->
    console.log(grunt.file.read('banner.txt'))

  # grunt.loadNpmTasks 'grunt-exec'
  grunt.loadNpmTasks 'grunt-bump'
  grunt.loadNpmTasks 'grunt-contrib-clean'
  grunt.loadNpmTasks 'grunt-dotnet-assembly-info'
  grunt.loadNpmTasks 'grunt-msbuild'
  grunt.loadNpmTasks 'grunt-nuget'
  grunt.loadNpmTasks 'grunt-dotnet-mspec'

  grunt.registerTask 'default', ['ci']
  grunt.registerTask 'publish', ['banner','bump','nugetpush']
  grunt.registerTask 'release', ['publish']
  grunt.registerTask 'deploy', ['publish']
  grunt.registerTask 'ci', ['banner','clean','assemblyinfo','nugetrestore','msbuild','mspec','nugetpack']

  null
