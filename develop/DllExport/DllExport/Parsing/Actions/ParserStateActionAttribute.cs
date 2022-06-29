using DllExport.Parsing;
using System;

namespace DllExport.Parsing.Actions
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
	internal sealed class ParserStateActionAttribute : Attribute
	{
		public readonly DllExport.Parsing.ParserState ParserState;

		public ParserStateActionAttribute(DllExport.Parsing.ParserState parserState)
		{
			this.ParserState = parserState;
		}
	}
}