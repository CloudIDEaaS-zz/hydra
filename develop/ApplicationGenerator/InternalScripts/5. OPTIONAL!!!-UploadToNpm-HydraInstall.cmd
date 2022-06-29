set TargetDir="%HYDRASOLUTIONPATH%\HydraCLI\HydraInstall"

%TargetDir:~1,2%
cd %TargetDir%
npm publish
echo publish complete
