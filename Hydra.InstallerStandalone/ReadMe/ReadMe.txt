Manual steps:

If WIX editor screens do not show:
https://marketplace.visualstudio.com/items?itemName=Add-inExpress.DesignerforVisualStudioWiXSetupProjects&ssr=false#qna

In Hydra.InstallerStandalone, update dependencies:
	Go into Product.wxs.. 
	Assure deletion of all elements under Feature Element
	Assure deletion of all elements under Directory Element which is under Fragment
		
	Right click project.. View WiX Editors.. File System Editor
	Right Application Folder project.. Add.. Project Output
	Select ApplicationGenerator.. Primary Output  (Hit Okay, double clicking does not work)
	Delete duplicate files
		log4net
		Newtonsoft
		System.Threading.Tasks.Extensions
		System.ValueTuple
	Add these files manually
		System.Net.Http.dll
		System.Runtime.CompilerServices.Unsafe.dll
		System.Runtime.dll
		
	Assure in Product.wxs that Wix/Product/@Version !(bind.FileVersion.[Guid]) matches the following:
		_ApplicationGenerator.xml -> ProjectOutput/SharedFileId

Build Hydra.InstallerStandalone
Build Hydra.Installer

Test Hydra.Installer
	Hydra.Installer\bin\Debug\Hydra.Installer.exe
	Click "Install"
	Installer splash screen should match version in ApplicationGenerator\Properties\AssemblyInfo.cs

Run the scripts under ApplicationGenerator\InternalScripts\
