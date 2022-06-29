using DllExport;
using Microsoft.Build.Utilities;
using System;

namespace DllExport.MSBuild
{
	public interface IDllExportTask : IInputValues, IServiceProvider
	{
        TaskLoggingHelper Log
        {
            get;
        }

        string Platform
		{
			get;
			set;
		}

		string PlatformTarget
		{
			get;
			set;
		}

		bool? SkipOnAnyCpu
		{
			get;
			set;
		}

		string TargetFrameworkVersion
		{
			get;
			set;
		}
	}
}