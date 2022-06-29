using DllExport;
using DllExport.Parsing;
using DllExport.Properties;
using System;
using System.Collections.Generic;

namespace DllExport.Parsing.Actions
{
	[ParserStateAction(ParserState.MethodProperties)]
	public sealed class MethodPropertiesParserAction : IlParser.ParserStateAction
	{
		public MethodPropertiesParserAction()
		{
		}

		public override void Execute(ParserStateValues state, string trimmedLine)
		{
			if (!trimmedLine.StartsWith(".custom instance void ", StringComparison.Ordinal) || !trimmedLine.Contains(base.Parser.DllExportAttributeIlAsmFullName))
			{
				if (trimmedLine.StartsWith("// Code", StringComparison.Ordinal))
				{
					state.State = ParserState.Method;
					if (state.MethodPos != 0)
					{
						state.Result.Insert(state.MethodPos, state.Method.Declaration);
					}
				}
				return;
			}
			state.AddLine = false;
			state.State = ParserState.DeleteExportAttribute;
			IDllExportNotifier notifier = base.Notifier;
			string removingDllExportAttribute = DllExportLogginCodes.RemovingDllExportAttribute;
			string removing0From1_ = Resources.Removing_0_from_1_;
			object[] dllExportAttributeFullName = new object[] { Utilities.DllExportAttributeFullName, string.Concat(state.ClassNames.Peek(), ".", state.Method.Name) };
			notifier.Notify(-2, removingDllExportAttribute, removing0From1_, dllExportAttributeFullName);
		}
	}
}