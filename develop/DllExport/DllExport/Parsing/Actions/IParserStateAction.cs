using DllExport.Parsing;
using System;

namespace DllExport.Parsing.Actions
{
	public interface IParserStateAction
	{
		long Milliseconds
		{
			get;
			set;
		}

		IlParser Parser
		{
			get;
			set;
		}

		void Execute(ParserStateValues state, string trimmedLine);
	}
}