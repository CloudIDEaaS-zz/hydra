set SourceDir=%HYDRASOLUTIONPATH%
set TargetFile=%HYDRASOLUTIONPATH%\..\root\Binaries\SolutionLibraries\ApplicationGeneratorBuildTasks.dll

taskkill /f /im MSBuild.exe
del "%TargetFile%""