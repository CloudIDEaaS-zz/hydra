using DllExport.Parsing;
using System;

namespace DllExport.Parsing.Actions
{
	[ParserStateAction(ParserState.Method)]
	public sealed class MethodParserAction : IlParser.ParserStateAction
	{
		public MethodParserAction()
		{
		}

		public override void Execute(ParserStateValues state, string trimmedLine)
		{
			if (trimmedLine.StartsWith("} // end of method", StringComparison.Ordinal))
			{
				state.State = ParserState.Class;
			}
		}
	}
}