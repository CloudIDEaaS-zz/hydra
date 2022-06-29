using DllExport.Parsing;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace DllExport.Parsing.Actions
{
	[ParserStateAction(ParserState.ClassDeclaration)]
	public sealed class ClassDeclarationParserAction : IlParser.ParserStateAction
	{
		public ClassDeclarationParserAction()
		{
		}

		public override void Execute(ParserStateValues state, string trimmedLine)
		{
			if (!trimmedLine.StartsWith("{"))
			{
				state.ClassDeclaration = string.Concat(state.ClassDeclaration, " ", trimmedLine);
				state.AddLine = true;
				return;
			}
			state.State = ParserState.Class;
			string className = ClassDeclarationParserAction.GetClassName(state);
			if (state.ClassNames.Count > 0)
			{
				className = string.Concat(state.ClassNames.Peek(), "/", className);
			}
			state.ClassNames.Push(className);
		}

		private static string GetClassName(ParserStateValues state)
		{
			bool flag = false;
			StringBuilder stringBuilder = new StringBuilder(state.ClassDeclaration.Length);
			IlParsingUtils.ParseIlSnippet(state.ClassDeclaration, ParsingDirection.Forward, (IlParsingUtils.IlSnippetLocation s) => {
				if (s.WithinString)
				{
					flag = true;
					if (s.CurrentChar != '\'')
					{
						stringBuilder.Append(s.CurrentChar);
					}
				}
				else if (flag)
				{
					if (s.CurrentChar == '.' || s.CurrentChar == '/')
					{
						stringBuilder.Append(s.CurrentChar);
					}
					else if (s.CurrentChar != '\'')
					{
						return false;
					}
				}
				return true;
			}, null);
			return stringBuilder.ToString();
		}
	}
}