$SourceDir="$Env:HYDRASOLUTIONPATH"
$TargetDir="$Env:HYDRASOLUTIONPATH\..\ApplicationGeneratorPublic\"

Write-Output "syncing $SourceDir to $TargetDir"

xcopy "$SourceDir\\ApplicationGenerator.sln" "$TargetDir" /i /d /y 
xcopy "$SourceDir\\.gitignore" "$TargetDir" /i /d /y 
xcopy "$SourceDir\\ApplicationGenerator\LICENSE" "$TargetDir" /i /d /y 
xcopy "$SourceDir\\ApplicationGenerator\README.md" "$TargetDir" /i /d /y 

$SubDir = "ApplicationGenerator"
xcopy "$SourceDir\\$SubDir" "$TargetDir\\$SubDir" /i /d /y /E /EXCLUDE:"$SourceDir\Excludes.txt"

$SubDir = "ApplicationGenerator.Interfaces"
xcopy "$SourceDir\\$SubDir" "$TargetDir\\$SubDir" /i /d /y /E /EXCLUDE:"$SourceDir\Excludes.txt"

$SubDir = "ApplicationGenerator.Overrides"
xcopy "$SourceDir\\$SubDir" "$TargetDir\\$SubDir" /i /d /y /E /EXCLUDE:"$SourceDir\Excludes.txt"

$SubDir = "CodeInterfaces"
xcopy "$SourceDir\\$SubDir" "$TargetDir\\$SubDir" /i /d /y /E /EXCLUDE:"$SourceDir\Excludes.txt"

$SubDir = "Hydra.Extension"
xcopy "$SourceDir\\$SubDir" "$TargetDir\\$SubDir" /i /d /y /E /EXCLUDE:"$SourceDir\Excludes.txt"

$SubDir = "Hydra.Installer"
xcopy "$SourceDir\\$SubDir" "$TargetDir\\$SubDir" /i /d /y /E /EXCLUDE:"$SourceDir\Excludes.txt"

$SubDir = "Hydra.InstallerStandalone"
xcopy "$SourceDir\\$SubDir" "$TargetDir\\$SubDir" /i /d /y /E /EXCLUDE:"$SourceDir\Excludes.txt"

$SubDir = "Utils"
xcopy "$SourceDir\\$SubDir" "$TargetDir\\$SubDir" /i /d /y /E /EXCLUDE:"$SourceDir\Excludes.txt"

$SubDir = "VisualStudioProvider"
xcopy "$SourceDir\\$SubDir" "$TargetDir\\$SubDir" /i /d /y /E /EXCLUDE:"$SourceDir\Excludes.txt"

$SubDir = "Rtf2Html"
xcopy "$SourceDir\\$SubDir" "$TargetDir\\$SubDir" /i /d /y /E /EXCLUDE:"$SourceDir\Excludes.txt"

$SubDir = "ColorMine"
xcopy "$SourceDir\\$SubDir" "$TargetDir\\$SubDir" /i /d /y /E /EXCLUDE:"$SourceDir\Excludes.txt"

$SubDir = "CppParser"
xcopy "$SourceDir\\$SubDir" "$TargetDir\\$SubDir" /i /d /y /E /EXCLUDE:"$SourceDir\Excludes.txt"

$SubDir = "ProcessDiagnosticsLibrary"
xcopy "$SourceDir\\$SubDir" "$TargetDir\\$SubDir" /i /d /y /E /EXCLUDE:"$SourceDir\Excludes.txt"

$SubDir = "SDKInterfaceLibrary.Entities"
xcopy "$SourceDir\\$SubDir" "$TargetDir\\$SubDir" /i /d /y /E /EXCLUDE:"$SourceDir\Excludes.txt"

$SubDir = "SharpSerializer"
xcopy "$SourceDir\\$SubDir" "$TargetDir\\$SubDir" /i /d /y /E /EXCLUDE:"$SourceDir\Excludes.txt"

$SubDir = "ApplicationGenerator.Data"
xcopy "$SourceDir\\$SubDir" "$TargetDir\\$SubDir" /i /d /y /E /EXCLUDE:"$SourceDir\Excludes.txt"

$SubDir = "Utils.Core"
xcopy "$SourceDir\\$SubDir" "$TargetDir\\$SubDir" /i /d /y /E /EXCLUDE:"$SourceDir\Excludes.txt"

$SubDir = "Binaries"
xcopy "$SourceDir\\$SubDir" "$TargetDir\\$SubDir" /i /d /y /E /EXCLUDE:"$SourceDir\Excludes.txt"

$SubDir = "XPathParser"
xcopy "$SourceDir\\$SubDir" "$TargetDir\\$SubDir" /i /d /y /E /EXCLUDE:"$SourceDir\Excludes.txt"

$SubDir = "TypeScriptAST"
xcopy "$SourceDir\\$SubDir" "$TargetDir\\$SubDir" /i /d /y /E /EXCLUDE:"$SourceDir\Excludes.txt"

$SubDir = "ModuleImportsHelper"
xcopy "$SourceDir\\$SubDir" "$TargetDir\\$SubDir" /i /d /y /E /EXCLUDE:"$SourceDir\Excludes.txt"

$SubDir = "HydraCLI"
xcopy "$SourceDir\\$SubDir" "$TargetDir\\$SubDir" /i /d /y /E /EXCLUDE:"$SourceDir\Excludes.txt"
