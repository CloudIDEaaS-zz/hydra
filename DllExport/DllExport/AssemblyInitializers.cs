using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DllExport
{
	public sealed class AssemblyInitializers
	{
		internal readonly static Dictionary<CallingConvention, string> ConventionTypeNames;
		private readonly Dictionary<string, InitializerClass> _ClassesByName = new Dictionary<string, InitializerClass>();
		private readonly List<DuplicateInitializers> _DuplicateInitializerMethods = new List<DuplicateInitializers>();
		private readonly Dictionary<object, InitializerMethod> _DuplicateInitializerMethodsbyFullName = new Dictionary<object, InitializerMethod>();
		private readonly Dictionary<string, InitializerMethod> _MethodsByInitializerName = new Dictionary<string, InitializerMethod>();
		private readonly ReadOnlyCollection<DuplicateInitializers> _ReadOnlyDuplicateInitializerMethods;

		internal Dictionary<string, InitializerClass> ClassesByName
		{
			get
			{
				return this._ClassesByName;
			}
		}

		public int Count
		{
			get
			{
				return this.MethodsByInitializerName.Count;
			}
		}

		public string DllInitializerAttributeAssemblyName
		{
			get
			{
				if (this.InputValues == null)
				{
					return Utilities.DllExportAttributeAssemblyName;
				}
				return this.InputValues.DllExportAttributeAssemblyName;
			}
		}

		public string DllInitializerAttributeFullName
		{
			get
			{
				if (this.InputValues == null)
				{
					return Utilities.DllExportAttributeFullName;
				}
				return this.InputValues.DllExportAttributeFullName;
			}
		}

		public ReadOnlyCollection<DuplicateInitializers> DuplicateInitializerMethods
		{
			get
			{
				return this._ReadOnlyDuplicateInitializerMethods;
			}
		}

		public IInputValues InputValues
		{
			get;
			set;
		}

		internal Dictionary<string, InitializerMethod> MethodsByInitializerName
		{
			get
			{
				return this._MethodsByInitializerName;
			}
		}

		static AssemblyInitializers()
		{
			Dictionary<CallingConvention, string> callingConventions = new Dictionary<CallingConvention, string>()
			{
				{ CallingConvention.Cdecl, typeof(CallConvCdecl).FullName },
				{ CallingConvention.FastCall, typeof(CallConvFastcall).FullName },
				{ CallingConvention.StdCall, typeof(CallConvStdcall).FullName },
				{ CallingConvention.ThisCall, typeof(CallConvThiscall).FullName },
				{ CallingConvention.Winapi, typeof(CallConvStdcall).FullName }
			};
			AssemblyInitializers.ConventionTypeNames = callingConventions;
		}

		public AssemblyInitializers()
		{
			this._ReadOnlyDuplicateInitializerMethods = new ReadOnlyCollection<DuplicateInitializers>(this._DuplicateInitializerMethods);
		}

		public InitializerMethod GetDuplicateInitializer(string fullTypeName, string memberName)
		{
			InitializerMethod result;
			if (!this.TryGetDuplicateInitializer(fullTypeName, memberName, out result))
			{
				return null;
			}
			return result;
		}

		private static object GetKey(string fullTypeName, string memberName)
		{
			return new { fullTypeName = fullTypeName, memberName = memberName };
		}

		internal void Refresh()
		{
			DuplicateInitializers dupe;
			int methodIndex = 0;
			this.MethodsByInitializerName.Clear();
			this._DuplicateInitializerMethods.Clear();
			Dictionary<string, DuplicateInitializers> duplicateInitializers = new Dictionary<string, DuplicateInitializers>();

			foreach (InitializerClass initializerClass in this.ClassesByName.Values)
			{
				List<InitializerMethod> dupeMethods = new List<InitializerMethod>(initializerClass.Methods.Count);

				foreach (InitializerMethod i in initializerClass.Methods)
				{
					if (duplicateInitializers.TryGetValue(i.InitializerName, out dupe))
					{
						dupeMethods.Add(i);
						dupe.Duplicates.Add(i);
					}
					else
					{
						int num = methodIndex;
						methodIndex = num + 1;
						i.VTableOffset = num;
						this.MethodsByInitializerName.Add(i.MemberName, i);
						duplicateInitializers.Add(i.InitializerName, new DuplicateInitializers(i));
					}
				}

				InitializerClass InitializerClass = initializerClass;

				dupeMethods.ForEach((InitializerMethod m) => InitializerClass.Methods.Remove(m));
				InitializerClass.Refresh();
			}

			foreach (DuplicateInitializers duplicateInitializer in duplicateInitializers.Values)
			{
				if (duplicateInitializer.Duplicates.Count <= 0)
				{
					continue;
				}
				this._DuplicateInitializerMethods.Add(duplicateInitializer);
				foreach (InitializerMethod dupe2 in duplicateInitializer.Duplicates)
				{
					this._DuplicateInitializerMethodsbyFullName.Add(AssemblyInitializers.GetKey(dupe2.InitializerClass.FullTypeName, dupe2.MemberName), dupe2);
				}
			}
			
			this._DuplicateInitializerMethods.Sort((DuplicateInitializers l, DuplicateInitializers r) => string.CompareOrdinal(l.UsedInitializer.InitializerName, r.UsedInitializer.InitializerName));
		}

		public bool TryGetDuplicateInitializer(string fullTypeName, string memberName, out InitializerMethod InitializerMethod)
		{
			return this._DuplicateInitializerMethodsbyFullName.TryGetValue(AssemblyInitializers.GetKey(fullTypeName, memberName), out InitializerMethod);
		}
	}
}