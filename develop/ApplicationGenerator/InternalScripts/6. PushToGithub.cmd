set SourceDir="%HYDRASOLUTIONPATH%"
set TargetDir="%HYDRASOLUTIONPATH%\..\ApplicationGeneratorPublic\"
set Today=%date:/=-%

%TargetDir:~1,2%
cd %TargetDir%
git pull
git add .
git commit -m "Auto sync commit from %USERNAME% on %Today%"
git push
echo push complete