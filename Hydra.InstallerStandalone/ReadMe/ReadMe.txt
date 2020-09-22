To update dependencies:
	Go into Product.wxs.. 
	Delete all elements under Feature Element
	Delete all elements under Directory Element which is under Fragment
	Assure it compiles
	Right click project.. View WiX Editors.. File System Editor
	Right Application Folder project.. Add.. Project Output
	Select ApplicationGenerator.. Primary Output
	
		log4net
		Newtonsoft
		System.ValueTuple
		System.Threading.Tasks.Extensions