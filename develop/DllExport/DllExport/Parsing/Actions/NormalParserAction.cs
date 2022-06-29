using DllExport;
using DllExport.Parsing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace DllExport.Parsing.Actions
{
	[ParserStateAction(ParserState.Normal)]
	public sealed class NormalParserAction : IlParser.ParserStateAction
	{
		public NormalParserAction()
		{
		}

		public override void Execute(ParserStateValues state, string trimmedLine)
		{
			string assemblyName;
			string assemblyAlias;

			if (trimmedLine.StartsWith(".corflags", StringComparison.Ordinal))
			{
				List<string> result = state.Result;
				CultureInfo invariantCulture = CultureInfo.InvariantCulture;
				object[] str = new object[1];
				int coreFlagsForPlatform = Utilities.GetCoreFlagsForPlatform(state.Cpu);
				str[0] = coreFlagsForPlatform.ToString("X8", CultureInfo.InvariantCulture);
				result.Add(string.Format(invariantCulture, ".corflags 0x{0}", str));
				state.AddLine = false;
				return;
			}

			if (trimmedLine.StartsWith(".class", StringComparison.Ordinal))
			{
				state.State = ParserState.ClassDeclaration;
				state.AddLine = true;
				state.ClassDeclaration = trimmedLine;
				return;
			}

			if (trimmedLine.StartsWith(".module", StringComparison.Ordinal) && !trimmedLine.StartsWith(".module extern", StringComparison.Ordinal))
			{
				state.State = ParserState.Module;
				state.AddLine = true;
				state.Module = trimmedLine;

				return;
			}

			if (this.IsExternalAssemblyReference(trimmedLine, out assemblyName, out assemblyAlias))
			{
				state.RegisterMsCorelibAlias(assemblyName, assemblyAlias);
			}
		}

		private bool IsExportAttributeAssemblyReference(string trimmedLine)
		{
			return trimmedLine.StartsWith(string.Concat(".assembly extern '", base.DllExportAttributeAssemblyName, "'"), StringComparison.Ordinal);
		}

		private bool IsExternalAssemblyReference(string trimmedLine, out string assemblyName, out string aliasName)
		{
			assemblyName = null;
			aliasName = null;

			if (trimmedLine.Length < ".assembly extern ".Length || !trimmedLine.StartsWith(".assembly extern ", StringComparison.Ordinal))
			{
				return false;
			}

			List<string> strs = new List<string>();
			IlParsingUtils.ParseIlSnippet(trimmedLine.Substring(".assembly extern ".Length), ParsingDirection.Forward, (IlParsingUtils.IlSnippetLocation current) => {
				if (!current.WithinString && current.CurrentChar == '\'' && current.LastIdentifier != null)
				{
					strs.Add(current.LastIdentifier);
					if (strs.Count > 1)
					{
						return false;
					}
				}
				return true;
			}, null);

			if (strs.Count == 0)
			{
				return false;
			}

			if (strs.Count > 0)
			{
				assemblyName = strs[0];
			}

			aliasName = (strs.Count > 1 ? strs[1] : strs[0]);
			return true;
		}
	}
}