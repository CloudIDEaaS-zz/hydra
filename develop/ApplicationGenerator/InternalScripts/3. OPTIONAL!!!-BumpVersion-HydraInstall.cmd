set TargetDir="%HYDRASOLUTIONPATH%\HydraCLI\HydraInstall"

%TargetDir:~1,2%
cd %TargetDir%
grunt bump
