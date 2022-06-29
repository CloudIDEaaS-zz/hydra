using Mono.Cecil;
using Mono.Collections.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace DllExport
{
	internal class AssemblyInspector : MarshalByRefObject, IAssemblyInspector
	{
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
		public string ModuleInitializerAttributeFullName
		{
			get
			{
				if (this.InputValues == null)
				{
					return Utilities.ModuleInitializerAttributeFullName;
				}

				return this.InputValues.ModuleInitializerAttributeFullName;
			}
		}


		public IInputValues InputValues
		{
			get
			{
				return JustDecompileGenerated_get_InputValues();
			}
			set
			{
				JustDecompileGenerated_set_InputValues(value);
			}
		}

		private IInputValues JustDecompileGenerated_InputValues_k__BackingField;

		public IInputValues JustDecompileGenerated_get_InputValues()
		{
			return this.JustDecompileGenerated_InputValues_k__BackingField;
		}

		private void JustDecompileGenerated_set_InputValues(IInputValues value)
		{
			this.JustDecompileGenerated_InputValues_k__BackingField = value;
		}

		public AssemblyInspector(IInputValues inputValues)
		{
			if (inputValues == null)
			{
				throw new ArgumentNullException("inputValues");
			}
			this.InputValues = inputValues;
		}

		private void CheckForExportedMethods(Func<ExportedMethod> createExportMethod, ExtractExportHandler exportFilter, List<ExportedMethod> exportMethods, MethodDefinition method)
		{
			IExportInfo exportInfo = null;

			if (exportFilter(method, out exportInfo))
			{
				var exportedMethod = createExportMethod();
				StringBuilder builder;

				exportedMethod.IsStatic = method.IsStatic;
				exportedMethod.IsGeneric = method.HasGenericParameters;

				builder = new StringBuilder(method.Name, method.Name.Length + 5);

				if (method.HasGenericParameters)
				{
					var paramCount = 0;

					builder.Append("<");

					foreach (GenericParameter genericParameter in method.GenericParameters)
					{
						paramCount++;

						if (paramCount > 1)
						{
							builder.Append(",");
						}

						builder.Append(genericParameter.Name);
					}

					builder.Append(">");
				}

				exportedMethod.MemberName = builder.ToString();
				exportedMethod.AssignFrom(exportInfo);

				if (string.IsNullOrEmpty(exportedMethod.ExportName))
				{
					exportedMethod.ExportName = method.Name;
				}

				if ((int)exportedMethod.CallingConvention == 0)
				{
					exportedMethod.CallingConvention = CallingConvention.Winapi;
				}
				
				exportMethods.Add(exportedMethod);
			}
		}

		public AssemblyExports ExtractExports(AssemblyDefinition assemblyDefinition)
		{
			return this.ExtractExports(assemblyDefinition, new ExtractExportHandler(this.TryExtractExport));
		}

		public AssemblyExports ExtractExports()
		{
			AssemblyDefinition asm = this.LoadAssembly(this.InputValues.InputFileName);
			return this.ExtractExports(asm);
		}

		public AssemblyExports ExtractExports(string fileName)
		{
			return this.ExtractExports(this.LoadAssembly(fileName));
		}

		public AssemblyExports ExtractExports(AssemblyDefinition assemblyDefinition, ExtractExportHandler exportFilter)
		{
			var types = this.TraverseNestedTypes(assemblyDefinition.Modules.SelectMany<ModuleDefinition, TypeDefinition>((ModuleDefinition m) => m.Types).ToList<TypeDefinition>());
			var assemblyExport = new AssemblyExports() { InputValues = this.InputValues };

			foreach (var typeRef in types)
			{
				var exportMethods = new List<ExportedMethod>();

				foreach (var method in typeRef.Methods)
				{
					var typeDefinition = typeRef;

					this.CheckForExportedMethods(() => new ExportedMethod(this.GetExportedClass(typeDefinition, assemblyExport)), exportFilter, exportMethods, method);
				}

				foreach (var method in exportMethods)
				{
					this.GetExportedClass(typeRef, assemblyExport).Methods.Add(method);
				}
			}

			assemblyExport.Refresh();

			return assemblyExport;
		}

		public AssemblyExports ExtractExports(string fileName, ExtractExportHandler exportFilter)
		{
			AssemblyExports result;
			string oldCurrentDir = Directory.GetCurrentDirectory();
			try
			{
				Directory.SetCurrentDirectory(Path.GetDirectoryName(fileName));
				result = this.ExtractExports(this.LoadAssembly(fileName), exportFilter);
			}
			finally
			{
				Directory.SetCurrentDirectory(oldCurrentDir);
			}
			return result;
		}

		public bool SafeExtractExports(string fileName, Stream stream)
		{
			bool result;
			AssemblyExports exports = this.ExtractExports(fileName);
			if (exports.Count != 0)
			{
				(new BinaryFormatter()).Serialize(stream, exports);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		private ExportedClass GetExportedClass(TypeDefinition td, AssemblyExports result)
		{
			ExportedClass exportedClass;
			if (!result.ClassesByName.TryGetValue(td.FullName, out exportedClass))
			{
				TypeDefinition usedType = td;
				while (!usedType.HasGenericParameters && usedType.IsNested)
				{
					usedType = usedType.DeclaringType;
				}
				exportedClass = new ExportedClass(td.FullName, usedType.HasGenericParameters);
				result.ClassesByName.Add(exportedClass.FullTypeName, exportedClass);
			}
			return exportedClass;
		}

		private void CheckForInitializeMethods(Func<InitializerMethod> createInitializerMethod, ExtractInitializerHandler initializerFilter, List<InitializerMethod> initializeMethods, MethodDefinition method)
		{
			IInitializerInfo initializerInfo = null;

			if (initializerFilter(method, out initializerInfo))
			{
				var initializerMethod = createInitializerMethod();
				StringBuilder builder;

				initializerMethod.IsStatic = method.IsStatic;
				initializerMethod.IsGeneric = method.HasGenericParameters;

				builder = new StringBuilder(method.Name, method.Name.Length + 5);

				if (method.HasGenericParameters)
				{
					var paramCount = 0;

					builder.Append("<");

					foreach (GenericParameter genericParameter in method.GenericParameters)
					{
						paramCount++;

						if (paramCount > 1)
						{
							builder.Append(",");
						}

						builder.Append(genericParameter.Name);
					}

					builder.Append(">");
				}

				initializerMethod.MemberName = builder.ToString();
				initializerMethod.AssignFrom(initializerInfo);

				if (string.IsNullOrEmpty(initializerMethod.InitializerName))
				{
					initializerMethod.InitializerName = method.Name;
				}

				if ((int)initializerMethod.CallingConvention == 0)
				{
					initializerMethod.CallingConvention = CallingConvention.Winapi;
				}

				initializeMethods.Add(initializerMethod);
			}
		}

		public AssemblyInitializers ExtractInitializers(AssemblyDefinition assemblyDefinition)
		{
			return this.ExtractInitializers(assemblyDefinition, new ExtractInitializerHandler(this.TryExtractInitializer));
		}

		public AssemblyInitializers ExtractInitializers()
		{
			AssemblyDefinition asm = this.LoadAssembly(this.InputValues.InputFileName);
			return this.ExtractInitializers(asm);
		}

		public AssemblyInitializers ExtractInitializers(string fileName)
		{
			return this.ExtractInitializers(this.LoadAssembly(fileName));
		}

		public AssemblyInitializers ExtractInitializers(AssemblyDefinition assemblyDefinition, ExtractInitializerHandler intializerFilter)
		{
			var types = this.TraverseNestedTypes(assemblyDefinition.Modules.SelectMany<ModuleDefinition, TypeDefinition>((ModuleDefinition m) => m.Types).ToList<TypeDefinition>());
			var assemblyInitializer = new AssemblyInitializers() { InputValues = this.InputValues };

			foreach (var typeRef in types)
			{
				var initializerMethods = new List<InitializerMethod>();

				foreach (var method in typeRef.Methods)
				{
					var typeDefinition = typeRef;

					this.CheckForInitializeMethods(() => new InitializerMethod(this.GetInitializeClass(typeDefinition, assemblyInitializer)), intializerFilter, initializerMethods, method);
				}

				foreach (var method in initializerMethods)
				{
					this.GetInitializeClass(typeRef, assemblyInitializer).Methods.Add(method);
				}
			}

			assemblyInitializer.Refresh();

			return assemblyInitializer;
		}

		public AssemblyInitializers ExtractInitializers(string fileName, ExtractInitializerHandler initializerFilter)
		{
			AssemblyInitializers result;
			string oldCurrentDir = Directory.GetCurrentDirectory();
			try
			{
				Directory.SetCurrentDirectory(Path.GetDirectoryName(fileName));
				result = this.ExtractInitializers(this.LoadAssembly(fileName), initializerFilter);
			}
			finally
			{
				Directory.SetCurrentDirectory(oldCurrentDir);
			}
			return result;
		}

		public bool SafeExtractInitializers(string fileName, Stream stream)
		{
			bool result;
			AssemblyInitializers Initializers = this.ExtractInitializers(fileName);
			if (Initializers.Count != 0)
			{
				(new BinaryFormatter()).Serialize(stream, Initializers);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		private InitializerClass GetInitializeClass(TypeDefinition td, AssemblyInitializers result)
		{
			InitializerClass initializerClass;
			if (!result.ClassesByName.TryGetValue(td.FullName, out initializerClass))
			{
				TypeDefinition usedType = td;
				while (!usedType.HasGenericParameters && usedType.IsNested)
				{
					usedType = usedType.DeclaringType;
				}
				initializerClass = new InitializerClass(td.FullName, usedType.HasGenericParameters);
				result.ClassesByName.Add(initializerClass.FullTypeName, initializerClass);
			}

			return initializerClass;
		}

		public AssemblyBinaryProperties GetAssemblyBinaryProperties(string assemblyFileName)
		{
			AssemblyBinaryProperties result;
			if (File.Exists(assemblyFileName))
			{
				AssemblyDefinition asm = this.LoadAssembly(assemblyFileName);
				ModuleDefinition peHeader = asm.MainModule;
				string keyFileName = null;
				string keyContainer = null;
				Collection<CustomAttribute>.Enumerator enumerator = asm.CustomAttributes.GetEnumerator();
				try
				{
					do
					{
						if (!enumerator.MoveNext())
						{
							break;
						}
						CustomAttribute attr = enumerator.Current;
						string fullName = attr.Constructor.DeclaringType.FullName;
						string str = fullName;
						if (fullName == null)
						{
							continue;
						}
						if (str == "System.Reflection.AssemblyKeyFileAttribute")
						{
							keyFileName = Convert.ToString(attr.ConstructorArguments[0], CultureInfo.InvariantCulture);
						}
						else if (str == "System.Reflection.AssemblyKeyNameAttribute")
						{
							keyContainer = Convert.ToString(attr.ConstructorArguments[0], CultureInfo.InvariantCulture);
						}
					}
					while (string.IsNullOrEmpty(keyFileName) || string.IsNullOrEmpty(keyContainer));
				}
				finally
				{
					((IDisposable)enumerator).Dispose();
				}
				result = new AssemblyBinaryProperties(peHeader.Attributes, peHeader.Architecture, asm.Name.HasPublicKey, keyFileName, keyContainer);
			}
			else
			{
				result = AssemblyBinaryProperties.GetEmpty();
			}
			return result;
		}

		public AssemblyDefinition LoadAssembly(string fileName)
		{
			return AssemblyDefinition.ReadAssembly(fileName);
		}

		private static void SetFieldValue(IExportInfo ei, string name, object value)
		{
			string text = name.NullSafeToUpperInvariant();
			if (text != null)
			{
				if (!(text != "NAME") || !(text != "EXPORTNAME"))
				{
					ei.ExportName = value.NullSafeToString();
				}
				else if (text == "CALLINGCONVENTION" || text == "CONVENTION")
				{
					ei.CallingConvention = (CallingConvention)value;
					return;
				}
			}
		}

		private static void SetFieldValue(IInitializerInfo ii, string name, object value)
		{
			string text = name.NullSafeToUpperInvariant();
			if (text != null)
			{
				if (!(text != "NAME") || !(text != "EXPORTNAME"))
				{
					ii.InitializerName = value.NullSafeToString();
				}
				else if (text == "CALLINGCONVENTION" || text == "CONVENTION")
				{
					ii.CallingConvention = (CallingConvention)value;
					return;
				}
			}
		}

		private static void SetParamValue(IExportInfo ei, string name, object value)
		{
			if (name != null)
			{
				if (name == "System.String")
				{
					ei.ExportName = value.NullSafeCall<object, string>((object v) => v.ToString());
				}
				else if (name == "System.Runtime.InteropServices.CallingConvention")
				{
					ei.CallingConvention = (CallingConvention)value;
					return;
				}
			}
		}

		private static void SetParamValue(IInitializerInfo ii, string name, object value)
		{
			if (name != null)
			{
				if (name == "System.String")
				{
					ii.InitializerName = value.NullSafeCall<object, string>((object v) => v.ToString());
				}
				else if (name == "System.Runtime.InteropServices.CallingConvention")
				{
					ii.CallingConvention = (CallingConvention)value;
					return;
				}
			}
		}

		private IList<TypeDefinition> TraverseNestedTypes(ICollection<TypeDefinition> types)
		{
			List<TypeDefinition> result = new List<TypeDefinition>(types.Count);
			foreach (TypeDefinition typeRef in types)
			{
				result.Add(typeRef);
				if (!typeRef.HasNestedTypes)
				{
					continue;
				}
				result.AddRange(this.TraverseNestedTypes(typeRef.NestedTypes));
			}
			return result;
		}

		public bool TryExtractInitializer(ICustomAttributeProvider memberInfo, out IInitializerInfo initializerInfo)
		{
			IInitializerInfo ii;

			initializerInfo = null;
			int i;

			foreach (var attribute in memberInfo.CustomAttributes)
			{
				if (attribute.Constructor.DeclaringType.FullName != this.ModuleInitializerAttributeFullName)
				{
					continue;
				}

				initializerInfo = new InitializerInfo();
				ii = initializerInfo;

				i = -1;

				foreach (var customAttributeArgument in attribute.ConstructorArguments)
				{
					ParameterDefinition parameterDefinition;

					i++;

					parameterDefinition = attribute.Constructor.Parameters[i];
					AssemblyInspector.SetParamValue(ii, parameterDefinition.ParameterType.FullName, customAttributeArgument.Value);
				}

				using (var enumerator = (from arg in attribute.Fields.Concat<CustomAttributeNamedArgument>(attribute.Properties) select new { Name = arg.Name, Value = arg.Argument.Value }).Distinct().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						var current = enumerator.Current;

						AssemblyInspector.SetFieldValue(ii, current.Name, current.Value);
					}

					break;
				}
			}

			return initializerInfo != null;
		}

		public bool TryExtractExport(ICustomAttributeProvider memberInfo, out IExportInfo exportInfo)
		{
			exportInfo = null;
			int i;

			foreach (var attribute in memberInfo.CustomAttributes)
			{
				if (attribute.Constructor.DeclaringType.FullName != this.DllExportAttributeFullName)
				{
					continue;
				}

				exportInfo = new ExportInfo();
				IExportInfo ei = exportInfo;
				
				i = -1;

				foreach (var customAttributeArgument in attribute.ConstructorArguments)
				{
					ParameterDefinition parameterDefinition;

					i++;

					parameterDefinition = attribute.Constructor.Parameters[i];
					AssemblyInspector.SetParamValue(ei, parameterDefinition.ParameterType.FullName, customAttributeArgument.Value);
				}

				using (var enumerator = (from arg in attribute.Fields.Concat<CustomAttributeNamedArgument>(attribute.Properties) select new { Name = arg.Name, Value = arg.Argument.Value }).Distinct().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						var current = enumerator.Current;

						AssemblyInspector.SetFieldValue(ei, current.Name, current.Value);
					}

					break;
				}
			}

			return exportInfo != null;
		}
	}
}