using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace DllExport
{
	public class ExportedClass
	{
		private readonly List<ExportedMethod> _Methods = new List<ExportedMethod>();

		private readonly Dictionary<string, List<ExportedMethod>> _MethodsByName = new Dictionary<string, List<ExportedMethod>>();

		public string FullTypeName
		{
			get;
			private set;
		}

		public bool HasGenericContext
		{
			get;
			private set;
		}

		internal List<ExportedMethod> Methods
		{
			get
			{
				return this._Methods;
			}
		}

		internal Dictionary<string, List<ExportedMethod>> MethodsByName
		{
			get
			{
				return this._MethodsByName;
			}
		}

		public ExportedClass(string fullTypeName, bool hasGenericContext)
		{
			this.FullTypeName = fullTypeName;
			this.HasGenericContext = hasGenericContext;
		}

		internal void Refresh()
		{
			List<ExportedMethod> methodNames;
			lock (this)
			{
				this.MethodsByName.Clear();
				foreach (ExportedMethod i in this.Methods)
				{
					if (!this.MethodsByName.TryGetValue(i.Name, out methodNames))
					{
						Dictionary<string, List<ExportedMethod>> methodsByName = this.MethodsByName;
						string name = i.Name;
						List<ExportedMethod> exportedMethods = new List<ExportedMethod>();
						methodNames = exportedMethods;
						methodsByName.Add(name, exportedMethods);
					}
					methodNames.Add(i);
				}
			}
		}
	}
}