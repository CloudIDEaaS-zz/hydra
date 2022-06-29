using DllExport;
using DllExport.Parsing;
using System;
using System.Collections.Generic;

namespace DllExport.Parsing.Actions
{
	[ParserStateAction(ParserState.Class)]
	public sealed class ClassParserAction : IlParser.ParserStateAction
	{
		public ClassParserAction()
		{
		}

		public override void Execute(ParserStateValues state, string trimmedLine)
		{
			ExportedClass exportedClass;

			if (trimmedLine.StartsWith(".class", StringComparison.Ordinal))
			{
				state.State = ParserState.ClassDeclaration;
				state.AddLine = true;
				state.ClassDeclaration = trimmedLine;

				return;
			}

			if (trimmedLine.StartsWith(".method", StringComparison.Ordinal))
			{
				if (state.ClassNames.Count != 0 && base.Parser.Exports.ClassesByName.TryGetValue(state.ClassNames.Peek(), out exportedClass))
				{
					state.Method.Reset();
					state.Method.Declaration = trimmedLine;
					state.AddLine = false;
					state.State = ParserState.MethodDeclaration;

					return;
				}
			}
			else if (trimmedLine.StartsWith("} // end of class", StringComparison.Ordinal))
			{
				state.ClassNames.Pop();
				state.State = (state.ClassNames.Count > 0 ? ParserState.Class : ParserState.Normal);
			}
		}
	}
}