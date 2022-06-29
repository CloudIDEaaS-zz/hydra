set TargetDir="%HYDRASOLUTIONPATH%\Hydra.Interfaces\bin\Debug"

%TargetDir:~1,2%
cd %TargetDir%

for /f %%i in ('dir /b/a-d/od/t:c') do set LATEST_PKG=%%i
echo The most recently created file is %LATEST_PKG%