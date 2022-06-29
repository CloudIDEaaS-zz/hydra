set TargetDir="%HYDRASOLUTIONPATH%\HydraCLI\HydraCLI"

%TargetDir:~1,2%
cd %TargetDir%
npm publish
echo publish complete