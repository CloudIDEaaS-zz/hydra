using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DllExport
{
	public sealed class AssemblyExports
	{
		internal readonly static Dictionary<CallingConvention, string> ConventionTypeNames;
		private readonly Dictionary<string, ExportedClass> _ClassesByName = new Dictionary<string, ExportedClass>();
		private readonly List<DuplicateExports> _DuplicateExportMethods = new List<DuplicateExports>();
		private readonly Dictionary<object, ExportedMethod> _DuplicateExportMethodsbyFullName = new Dictionary<object, ExportedMethod>();
		private readonly Dictionary<string, ExportedMethod> _MethodsByExportName = new Dictionary<string, ExportedMethod>();
		private readonly ReadOnlyCollection<DuplicateExports> _ReadOnlyDuplicateExportMethods;

		internal Dictionary<string, ExportedClass> ClassesByName
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
				return this.MethodsByExportName.Count;
			}
		}

		public string DllExportAttributeAssemblyName
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

		public string DllExportAttributeFullName
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

		public ReadOnlyCollection<DuplicateExports> DuplicateExportMethods
		{
			get
			{
				return this._ReadOnlyDuplicateExportMethods;
			}
		}

		public IInputValues InputValues
		{
			get;
			set;
		}

		internal Dictionary<string, ExportedMethod> MethodsByExportName
		{
			get
			{
				return this._MethodsByExportName;
			}
		}

		static AssemblyExports()
		{
			Dictionary<CallingConvention, string> callingConventions = new Dictionary<CallingConvention, string>()
			{
				{ CallingConvention.Cdecl, typeof(CallConvCdecl).FullName },
				{ CallingConvention.FastCall, typeof(CallConvFastcall).FullName },
				{ CallingConvention.StdCall, typeof(CallConvStdcall).FullName },
				{ CallingConvention.ThisCall, typeof(CallConvThiscall).FullName },
				{ CallingConvention.Winapi, typeof(CallConvStdcall).FullName }
			};
			AssemblyExports.ConventionTypeNames = callingConventions;
		}

		public AssemblyExports()
		{
			this._ReadOnlyDuplicateExportMethods = new ReadOnlyCollection<DuplicateExports>(this._DuplicateExportMethods);
		}

		public ExportedMethod GetDuplicateExport(string fullTypeName, string memberName)
		{
			ExportedMethod result;
			if (!this.TryGetDuplicateExport(fullTypeName, memberName, out result))
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
			DuplicateExports dupe;
			int methodIndex = 0;
			this.MethodsByExportName.Clear();
			this._DuplicateExportMethods.Clear();
			Dictionary<string, DuplicateExports> duplicateExports = new Dictionary<string, DuplicateExports>();

			foreach (ExportedClass exportClass in this.ClassesByName.Values)
			{
				List<ExportedMethod> dupeMethods = new List<ExportedMethod>(exportClass.Methods.Count);

				foreach (ExportedMethod i in exportClass.Methods)
				{
					if (duplicateExports.TryGetValue(i.ExportName, out dupe))
					{
						dupeMethods.Add(i);
						dupe.Duplicates.Add(i);
					}
					else
					{
						int num = methodIndex;
						methodIndex = num + 1;
						i.VTableOffset = num;
						this.MethodsByExportName.Add(i.MemberName, i);
						duplicateExports.Add(i.ExportName, new DuplicateExports(i));
					}
				}

				ExportedClass exportedClass = exportClass;

				dupeMethods.ForEach((ExportedMethod m) => exportedClass.Methods.Remove(m));
				exportClass.Refresh();
			}

			foreach (DuplicateExports duplicateExport in duplicateExports.Values)
			{
				if (duplicateExport.Duplicates.Count <= 0)
				{
					continue;
				}
				this._DuplicateExportMethods.Add(duplicateExport);
				foreach (ExportedMethod dupe2 in duplicateExport.Duplicates)
				{
					this._DuplicateExportMethodsbyFullName.Add(AssemblyExports.GetKey(dupe2.ExportedClass.FullTypeName, dupe2.MemberName), dupe2);
				}
			}
			
			this._DuplicateExportMethods.Sort((DuplicateExports l, DuplicateExports r) => string.CompareOrdinal(l.UsedExport.ExportName, r.UsedExport.ExportName));
		}

		public bool TryGetDuplicateExport(string fullTypeName, string memberName, out ExportedMethod exportedMethod)
		{
			return this._DuplicateExportMethodsbyFullName.TryGetValue(AssemblyExports.GetKey(fullTypeName, memberName), out exportedMethod);
		}
	}
}