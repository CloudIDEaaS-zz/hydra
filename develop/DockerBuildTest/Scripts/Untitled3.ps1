Write-Host "Downloading .NET framework 4.0" 
((New-Object Net.WebClient).DownloadFile("https://download.microsoft.com/download/9/5/A/95A9616B-7A37-4AF6-BC36-D6EA96C8DAAE/dotNetFx40_Full_x86_x64.exe","dotNetFx40_Full_x86_x64.exe")) 
Write-Host "Installing" 
.\dotNetFx40_Full_x86_x64.exe /q
Write-Host "Install complete" 