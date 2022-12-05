$runApp = $false;

taskkill -f -im Node.exe
taskkill -f -im ApplicationGenerator.exe
taskkill -f -im NetCoreReflectionShim.Service.exe
Clear

Set-ItemProperty HKCU:\Console VirtualTerminalLevel -Type DWORD 1
Set-Location -Path \

if (Test-Path -Path 'C:\Users\kenln\Documents\HydraProjects\MyApp')
{
    Remove-Item -Force -Recurse 'C:\Users\kenln\Documents\HydraProjects\MyApp'
}

Clear

New-Item -ItemType 'directory' -Path 'C:\Users\kenln\Documents\HydraProjects\MyApp'
Set-Location -Path C:\Users\kenln\Documents\HydraProjects\MyApp

hydra generate workspace --logToConsole --appName MyApp --appDescription "The app for my personal usage" --organizationName "My Org"
hydra generate businessmodel --logToConsole --template blank
hydra generate entities --logToConsole --template blank
hydra generate entities --json default

Set-Location -Path 'MyApp.Web'
hydra start MyApp --logToConsole
Set-Location -Path 'MyApp'
hydra generate app --logToConsole
npm run build

if ($runApp)
{
    npm run start
}

code .
