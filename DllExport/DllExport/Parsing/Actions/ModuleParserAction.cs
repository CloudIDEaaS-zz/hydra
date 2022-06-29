using DllExport.Parsing;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace DllExport.Parsing.Actions
{
	[ParserStateAction(ParserState.Module)]
	public sealed class ModuleParserAction : IlParser.ParserStateAction
	{
		public ModuleParserAction()
		{
		}

		public override void Execute(ParserStateValues state, string trimmedLine)
		{
			if (base.Parser.Initializers.Count > 0 || base.Parser.Exports.Count > 0 || base.Parser.Files != null)
			{
				var position = state.InputPosition;
				String test;

				if (trimmedLine.Length == 0)
				{
					if (base.Parser.Initializers.Count > 0)
					{
						var initializer = base.Parser.Initializers.MethodsByInitializerName.Values.Single();
						var className = initializer.InitializerClass.FullTypeName;
						var methodName = initializer.Name;

						state.Result.Insert(position++, string.Empty);
						state.Result.Insert(position++, "// ================== GLOBAL METHODS =========================");
						state.Result.Insert(position++, string.Empty);
						state.Result.Insert(position++, ".method private hidebysig specialname rtspecialname static");
						state.Result.Insert(position++, "        void  .cctor() cil managed");
						state.Result.Insert(position++, "{");
						state.Result.Insert(position++, "// Code size       6 (0x6)");
						state.Result.Insert(position++, ".maxstack  8");
						state.Result.Insert(position++, string.Format("IL_0000:  call       void {0}::{1}()", className, methodName));
						state.Result.Insert(position++, "IL_0005:  ret");
						state.Result.Insert(position++, "} // end of global method .cctor");
						state.Result.Insert(position++, string.Empty);
					}

					if (base.Parser.Files != null)
					{
						var files = base.Parser.Files.Split(';');

						foreach (var file in files)
						{
							var shortName = Path.GetFileName(file);
							var contents = File.ReadAllText(file);
							var bytes = Extensions.EncryptString(base.Parser.InputValues.PrivateKey, contents);
							var hashBuilder = new StringBuilder();

							foreach (var b in bytes)
                            {
								hashBuilder.AppendFormat("{0:x2} ", b);
							}

							state.Result.Insert(position++, string.Empty);
							state.Result.Insert(position++, "// ================== FILES =========================");
							state.Result.Insert(position++, string.Empty);
							state.Result.Insert(position++, string.Format(".file nometadata '{0}' .hash = ({1})", shortName, hashBuilder.ToString()));
							state.Result.Insert(position++, string.Empty);
						}
					}

					state.State = ParserState.Normal;
				}
				else
				{
					state.State = ParserState.Module;
				}

				state.AddLine = true;
			}
		}
	}
}