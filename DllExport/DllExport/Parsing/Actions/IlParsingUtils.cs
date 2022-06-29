using System;
using System.Runtime.CompilerServices;

namespace DllExport.Parsing.Actions
{
	internal static class IlParsingUtils
	{
		public static void ParseIlSnippet(string inputText, ParsingDirection direction, Func<IlParsingUtils.IlSnippetLocation, bool> predicate, Action<IlParsingUtils.IlSnippetFinalizaton> finalization = null)
		{
			bool withinString = false;
			bool withinScope = false;
			bool atOuterBracket = false;
			int bracketNesting = 0;
			int length = inputText.Length - 1;
			bool breakIssued = false;
			int stringStartIndex = -1;
			int step = (direction == ParsingDirection.Forward ? 1 : -1);
			int startIndex = (direction == ParsingDirection.Forward ? 0 : length);
			Func<int, bool> loopCondition = (direction == ParsingDirection.Forward ? new Func<int, bool>((int i) => i <= length) : new Func<int, bool>((int i) => i > -1));
			int lastPosition = -1;
			char previousChar = '\0';	
			string lastIdentifier = null;
			int num = startIndex;

			while (loopCondition(num))
			{
				char currentChar = inputText[num];
				atOuterBracket = false;

				if (currentChar != '\'')
				{
					if (withinString && previousChar == '\'')
					{
						stringStartIndex = num;
					}
					if (!withinString)
					{
						stringStartIndex = -1;
						if (withinScope && currentChar == ']')
						{
							withinScope = false;
						}
						if (currentChar == ')')
						{
							atOuterBracket = bracketNesting == 0;
							bracketNesting++;
						}
						else if (currentChar == '(')
						{
							bracketNesting--;
							atOuterBracket = bracketNesting == 0;
						}
					}
				}
				else
				{
					withinString = !withinString;

					if (withinString || stringStartIndex <= -1)
					{
						lastIdentifier = null;
					}
					else
					{
						int previousIndex = num - step;
						int firstIndex = Math.Min(previousIndex, stringStartIndex);
						int lastIndex = Math.Max(previousIndex, stringStartIndex);

						if (stringStartIndex != previousIndex)
						{
							string substring = inputText.Substring(0, lastIndex + 1);
							lastIdentifier = substring.Substring(firstIndex);
						}
						else
						{
							lastIdentifier = "";
						}
					}

					if (withinString && previousChar == '[')
					{
						withinScope = true;
					}
				}

				if (predicate(new IlParsingUtils.IlSnippetLocation(inputText, num, currentChar, lastIdentifier, withinString, withinScope, bracketNesting, atOuterBracket)))
				{
					lastPosition = num;
					previousChar = currentChar;
					num += step;
				}
				else
				{
					breakIssued = true;
					break;
				}
			}

			if (finalization != null)
			{
				finalization(new IlParsingUtils.IlSnippetFinalizaton(inputText, lastPosition, breakIssued, lastIdentifier, withinString, withinScope, bracketNesting, atOuterBracket));
			}
		}

		public sealed class IlSnippetFinalizaton : IlParsingUtils.IlSnippetLocationBase
		{
			public int LastPosition
			{
				get;
				private set;
			}

			public bool WasInterupted
			{
				get;
				private set;
			}

			public IlSnippetFinalizaton(string inputText, int lastPosition, bool wasInterupted, string lastIdentifier, bool withinString, bool withinScope, int nestedBrackets, bool atOuterBracket) : base(inputText, lastIdentifier, withinString, withinScope, nestedBrackets, atOuterBracket)
			{
				this.LastPosition = lastPosition;
				this.WasInterupted = wasInterupted;
			}
		}

		public class IlSnippetLocation : IlParsingUtils.IlSnippetLocationBase
		{
			public char CurrentChar
			{
				get;
				private set;
			}

			public int Index
			{
				get;
				private set;
			}

			public IlSnippetLocation(string inputText, int index, char currentChar, string lastIdentifier, bool withinString, bool withinScope, int nestedBrackets, bool atOuterBracket) : base(inputText, lastIdentifier, withinString, withinScope, nestedBrackets, atOuterBracket)
			{
				this.Index = index;
				this.CurrentChar = currentChar;
			}
		}

		public class IlSnippetLocationBase
		{
			public bool AtOuterBracket
			{
				get;
				private set;
			}

			public string InputText
			{
				get;
				private set;
			}

			public string LastIdentifier
			{
				get;
				set;
			}

			public int NestedBrackets
			{
				get;
				private set;
			}

			public bool WithinScope
			{
				get;
				set;
			}

			public bool WithinString
			{
				get;
				private set;
			}

			protected IlSnippetLocationBase(string inputText, string lastIdentifier, bool withinString, bool withinScope, int nestedBrackets, bool atOuterBracket)
			{
				if (inputText == null)
				{
					throw new ArgumentNullException("inputText");
				}
				this.InputText = inputText;
				this.LastIdentifier = lastIdentifier;
				this.WithinString = withinString;
				this.WithinScope = withinScope;
				this.NestedBrackets = nestedBrackets;
				this.AtOuterBracket = atOuterBracket;
			}
		}
	}
}