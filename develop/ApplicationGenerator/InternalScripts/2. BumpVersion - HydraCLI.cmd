set TargetDir="%HYDRASOLUTIONPATH%\HydraCLI\HydraCLI"

%TargetDir:~1,2%
cd %TargetDir%
grunt bump

echo bump complete