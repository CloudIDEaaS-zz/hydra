set SourceDir=%HYDRASOLUTIONPATH%
set TargetDir=%HYDRASOLUTIONPATH%\..\ApplicationGeneratorPublic

echo syncing %SourceDir% to %TargetDir%

xcopy %SourceDir%\ApplicationGenerator.sln %TargetDir% /i /d /y 
xcopy %SourceDir%\.gitignore %TargetDir% /i /d /y 
xcopy %SourceDir%\ApplicationGenerator\LICENSE %TargetDir% /i /d /y 
xcopy %SourceDir%\ApplicationGenerator\README.md %TargetDir% /i /d /y 
xcopy %SourceDir%\Hydra.Installer\bin\Debug\Hydra.Installer.exe %SourceDir%\HydraCLI\HydraInstall\install /i /d /y 
xcopy %SourceDir%\HydraCLI\HydraCli\package.json %TargetDir% /i /d /y 

set SubDir=ApplicationGenerator
xcopy %SourceDir%\%SubDir% %TargetDir%\%SubDir% /i /d /y /E /EXCLUDE:%SourceDir%\Excludes.txt

set SubDir=ApplicationGenerator.Interfaces
xcopy %SourceDir%\%SubDir% %TargetDir%\%SubDir% /i /d /y /E /EXCLUDE:%SourceDir%\Excludes.txt

set SubDir=ApplicationGenerator.Overrides
xcopy %SourceDir%\%SubDir% %TargetDir%\%SubDir% /i /d /y /E /EXCLUDE:%SourceDir%\Excludes.txt

set SubDir=CodeInterfaces
xcopy %SourceDir%\%SubDir% %TargetDir%\%SubDir% /i /d /y /E /EXCLUDE:%SourceDir%\Excludes.txt

set SubDir=Hydra.Extension
xcopy %SourceDir%\%SubDir% %TargetDir%\%SubDir% /i /d /y /E /EXCLUDE:%SourceDir%\Excludes.txt

set SubDir=Hydra.Installer
xcopy %SourceDir%\%SubDir% %TargetDir%\%SubDir% /i /d /y /E /EXCLUDE:%SourceDir%\Excludes.txt

set SubDir=Hydra.InstallerStandalone
xcopy %SourceDir%\%SubDir% %TargetDir%\%SubDir% /i /d /y /E /EXCLUDE:%SourceDir%\Excludes.txt

set SubDir=Utils
xcopy %SourceDir%\%SubDir% %TargetDir%\%SubDir% /i /d /y /E /EXCLUDE:%SourceDir%\Excludes.txt

set SubDir=VisualStudioProvider
xcopy %SourceDir%\%SubDir% %TargetDir%\%SubDir% /i /d /y /E /EXCLUDE:%SourceDir%\Excludes.txt

set SubDir=Rtf2Html
xcopy %SourceDir%\%SubDir% %TargetDir%\%SubDir% /i /d /y /E /EXCLUDE:%SourceDir%\Excludes.txt

set SubDir=ColorMine
xcopy %SourceDir%\%SubDir% %TargetDir%\%SubDir% /i /d /y /E /EXCLUDE:%SourceDir%\Excludes.txt

set SubDir=CppParser
xcopy %SourceDir%\%SubDir% %TargetDir%\%SubDir% /i /d /y /E /EXCLUDE:%SourceDir%\Excludes.txt

set SubDir=ProcessDiagnosticsLibrary
xcopy %SourceDir%\%SubDir% %TargetDir%\%SubDir% /i /d /y /E /EXCLUDE:%SourceDir%\Excludes.txt

set SubDir=SDKInterfaceLibrary.Entities
xcopy %SourceDir%\%SubDir% %TargetDir%\%SubDir% /i /d /y /E /EXCLUDE:%SourceDir%\Excludes.txt

set SubDir=SharpSerializer
xcopy %SourceDir%\%SubDir% %TargetDir%\%SubDir% /i /d /y /E /EXCLUDE:%SourceDir%\Excludes.txt

set SubDir=ApplicationGenerator.Data
xcopy %SourceDir%\%SubDir% %TargetDir%\%SubDir% /i /d /y /E /EXCLUDE:%SourceDir%\Excludes.txt

set SubDir=Utils.Core
xcopy %SourceDir%\%SubDir% %TargetDir%\%SubDir% /i /d /y /E /EXCLUDE:%SourceDir%\Excludes.txt

set SubDir=Binaries
xcopy %SourceDir%\%SubDir% %TargetDir%\%SubDir% /i /d /y /E /EXCLUDE:%SourceDir%\Excludes.txt

set SubDir=XPathParser
xcopy %SourceDir%\%SubDir% %TargetDir%\%SubDir% /i /d /y /E /EXCLUDE:%SourceDir%\Excludes.txt

set SubDir=TypeScriptAST
xcopy %SourceDir%\%SubDir% %TargetDir%\%SubDir% /i /d /y /E /EXCLUDE:%SourceDir%\Excludes.txt

set SubDir=ModuleImportsHelper
xcopy %SourceDir%\%SubDir% %TargetDir%\%SubDir% /i /d /y /E /EXCLUDE:%SourceDir%\Excludes.txt

set SubDir=PackageCacheStatus
xcopy %SourceDir%\%SubDir% %TargetDir%\%SubDir% /i /d /y /E /EXCLUDE:%SourceDir%\Excludes.txt

set SubDir=HydraCLI
xcopy %SourceDir%\%SubDir% %TargetDir%\%SubDir% /i /d /y /E /EXCLUDE:%SourceDir%\Excludes.txt

