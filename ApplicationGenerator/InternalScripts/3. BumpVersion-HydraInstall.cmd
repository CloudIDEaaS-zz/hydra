set TargetDir="%HYDRASOLUTIONPATH%\HydraCLI\HydraInstall"

%TargetDir:~1,2%
cd %TargetDir%
grunt bump

set TargetDir="%HYDRASOLUTIONPATH%\HydraCLI\HydraCLI"

%TargetDir:~1,2%
cd %TargetDir%
grunt bump

echo bump complete