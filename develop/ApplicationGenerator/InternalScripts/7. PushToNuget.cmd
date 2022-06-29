# TODO - get the versions from the 2 projects from project properties and put below, be sure and update \ApplicationGenerator.IonicAngular\ExportedProjectTemplates\*

set TargetDir="%HYDRASOLUTIONPATH%\Hydra.Interfaces\bin\Debug"

%TargetDir:~1,2%
cd %TargetDir%

for /f %%i in ('dir /b/a-d/od/t:c') do set LATEST_PKG=%%i
echo The most recently created package is %LATEST_PKG%

dotnet nuget push %LATEST_PKG% --api-key Oy2if7o53bupexeo4whqima5dpwci2gule4k3bw2j4yul4 --source https://api.nuget.org/v3/index.json

set TargetDir="%HYDRASOLUTIONPATH%\Utils.Core\bin\Debug"

%TargetDir:~1,2%
cd %TargetDir%

for /f %%i in ('dir /b/a-d/od/t:c') do set LATEST_PKG=%%i
echo The most recently created package is %LATEST_PKG%

dotnet nuget push %LATEST_PKG% --api-key Oy2if7o53bupexeo4whqima5dpwci2gule4k3bw2j4yul4 --source https://api.nuget.org/v3/index.json