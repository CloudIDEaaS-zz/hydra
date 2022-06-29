using System;
using System.Runtime.CompilerServices;

namespace DllExport.Parsing.Actions
{
	public sealed class ExternalAssemlyDeclaration
	{
		public string AliasName
		{
			get;
			private set;
		}

		public string AssemblyName
		{
			get;
			private set;
		}

		public int InputLineIndex
		{
			get;
			private set;
		}

		public ExternalAssemlyDeclaration(int inputLineIndex, string assemblyName, string aliasName)
		{
			this.InputLineIndex = inputLineIndex;
			this.AssemblyName = assemblyName;
			this.AliasName = aliasName;
		}
	}
}