$coreInstallScript = "https://dotnet.microsoft.com/download/dotnet-core/scripts/v1/dotnet-install.ps1"

Write-Host "2. Downloading and installing .NET Core 3.1..." -ForegroundColor Cyan

$wc = New-Object System.Net.WebClient

$wc.DownloadFile($coreInstallScript, "dotnet-install.ps1")
.\dotnet-install.ps1