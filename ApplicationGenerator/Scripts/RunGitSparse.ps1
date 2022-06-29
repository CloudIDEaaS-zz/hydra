cls
cd d:\Users\Ken\Documents\HydraProjects\GitSparse
rmdir -LiteralPath d:\Users\Ken\Documents\HydraProjects\GitSparse\root -Force -Recurse
md root
cd root
D:\MC\CloudIDEaaS\root\ApplicationGenerator\Scripts\GitSparse.cmd
dir d:\Users\Ken\Documents\HydraProjects\GitSparse\root

msbuild "ApplicationGenerator.sln" -property:Configuration=Release -property:HYDRASOLUTIONPATH=%cd%

cls
msbuild "ApplicationGenerator.IonicAngular\ApplicationGenerator.IonicAngular.csproj" -property:Configuration=Release -property:HYDRASOLUTIONPATH=%cd% -verbosity:d
