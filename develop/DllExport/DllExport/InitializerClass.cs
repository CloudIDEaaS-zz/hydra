using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace DllExport
{
	public class InitializerClass
	{
		private readonly List<InitializerMethod> _Methods = new List<InitializerMethod>();

		private readonly Dictionary<string, List<InitializerMethod>> _MethodsByName = new Dictionary<string, List<InitializerMethod>>();

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

		internal List<InitializerMethod> Methods
		{
			get
			{
				return this._Methods;
			}
		}

		internal Dictionary<string, List<InitializerMethod>> MethodsByName
		{
			get
			{
				return this._MethodsByName;
			}
		}

		public InitializerClass(string fullTypeName, bool hasGenericContext)
		{
			this.FullTypeName = fullTypeName;
			this.HasGenericContext = hasGenericContext;
		}

		internal void Refresh()
		{
			List<InitializerMethod> methodNames;
			lock (this)
			{
				this.MethodsByName.Clear();

				foreach (InitializerMethod i in this.Methods)
				{
					if (!this.MethodsByName.TryGetValue(i.Name, out methodNames))
					{
						Dictionary<string, List<InitializerMethod>> methodsByName = this.MethodsByName;
						string name = i.Name;
						List<InitializerMethod> initializerMethods = new List<InitializerMethod>();
						methodNames = initializerMethods;
						methodsByName.Add(name, initializerMethods);
					}
					methodNames.Add(i);
				}
			}
		}
	}
}