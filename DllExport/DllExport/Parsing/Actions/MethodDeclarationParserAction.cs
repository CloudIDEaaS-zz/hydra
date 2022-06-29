using DllExport;
using DllExport.Parsing;
using DllExport.Properties;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace DllExport.Parsing.Actions
{
	[ParserStateAction(ParserState.MethodDeclaration)]
	public sealed class MethodDeclarationParserAction : IlParser.ParserStateAction
	{
		private readonly static Regex _CilManagedRegex;

		static MethodDeclarationParserAction()
		{
			MethodDeclarationParserAction._CilManagedRegex = new Regex("\\b(?:cil|managed)\\b", RegexOptions.Compiled);
		}

		public MethodDeclarationParserAction()
		{
		}

		public override void Execute(ParserStateValues state, string trimmedLine)
		{
			ExportedClass exportedClass;
			if (!trimmedLine.StartsWith("{", StringComparison.Ordinal))
			{
				state.Method.Declaration = string.Concat(state.Method.Declaration, " ", trimmedLine);
				state.AddLine = false;
				return;
			}
			if (!this.GetIsExport(state, out exportedClass))
			{
				state.Result.Add(state.Method.Declaration);
				state.State = ParserState.Method;
				state.MethodPos = 0;
				return;
			}
			base.Notify(-1, DllExportLogginCodes.MethodFound, string.Format(Resources.Found_method_0_1_, exportedClass.FullTypeName, state.Method.Declaration), new object[0]);
			state.MethodPos = state.Result.Count;
			state.State = ParserState.MethodProperties;
		}

		private static string ExtractMethodName(string result, out string attributesWithResult)
		{
			string methodName = null;
			string str = null;
			StringBuilder stringBuilder = new StringBuilder(result.Length);
			IlParsingUtils.ParseIlSnippet(result, ParsingDirection.Backward, (IlParsingUtils.IlSnippetLocation s) => {
				if (s.CurrentChar == '\'')
				{
					return true;
				}
				if (!s.WithinString && s.CurrentChar != '.' && s.CurrentChar != ',' && s.CurrentChar != '/' && s.CurrentChar != '<' && s.CurrentChar != '>' && s.CurrentChar != '!')
				{
					return false;
				}
				stringBuilder.Insert(0, s.CurrentChar);
				return true;
			}, (IlParsingUtils.IlSnippetFinalizaton f) => str = (f.LastPosition > -1 ? result.Substring(0, f.LastPosition) : null));
			methodName = stringBuilder.ToString();
			attributesWithResult = str;
			return methodName;
		}

		private bool ExtractMethodParts(ParserStateValues state)
		{
			string methodName;
			string foundResult;
			string foundMethodAttributes;
			string afterMethodName;
			string foundResultModifier;
			if (!this.GetPartBeforeParameters(state.Method.Declaration, out methodName, out afterMethodName, out foundResult, out foundResultModifier, out foundMethodAttributes))
			{
				return false;
			}
			state.Method.After = afterMethodName;
			state.Method.Name = methodName;
			state.Method.Attributes = foundMethodAttributes;
			state.Method.Result = foundResult;
			state.Method.ResultAttributes = foundResultModifier;
			return true;
		}

		private static void ExtractResultModifier(ref string foundResult, out string foundResultModifier)
		{
			int index = -1;
			string str = foundResult;
			string str1 = null;
			IlParsingUtils.ParseIlSnippet(foundResult, ParsingDirection.Backward, (IlParsingUtils.IlSnippetLocation s) => {
				if (s.WithinString || !s.AtOuterBracket)
				{
					return true;
				}
				if (s.CurrentChar == ')')
				{
					index = s.Index;
					return true;
				}
				string substring = s.InputText.Substring(0, s.Index);
				int idx = substring.LastIndexOf(' ');
				if (idx > -1 && substring.Substring(idx + 1) == "marshal")
				{
					str1 = s.InputText.Substring(idx + 1, index - idx);
					str = s.InputText.Remove(idx + 1, index - idx);
				}
				return true;
			}, null);
			foundResult = str;
			foundResultModifier = str1;
		}

		private bool GetIsExport(ParserStateValues state, out ExportedClass exportedClass)
		{
			if (!this.ExtractMethodParts(state))
			{
				exportedClass = null;
				return false;
			}
			bool hasClass = (!base.Exports.ClassesByName.TryGetValue(state.ClassNames.Peek(), out exportedClass) ? false : exportedClass != null);
			List<ExportedMethod> exportedMethods = null;
			if (hasClass && exportedClass.HasGenericContext)
			{
				if (exportedClass.MethodsByName.TryGetValue(state.Method.Name, out exportedMethods))
				{
					exportedMethods.ForEach((ExportedMethod method) => base.Notify(2, DllExportLogginCodes.ExportInGenericType, Resources.The_type_1_cannot_export_the_method_2_as_0_because_it_is_generic_or_is_nested_within_a_generic_type, new object[] { method.ExportName, method.ExportedClass.FullTypeName, method.MemberName }));
				}
				return false;
			}
			bool isExport = (!hasClass ? false : exportedClass.MethodsByName.TryGetValue(state.Method.Name, out exportedMethods));
			if (!hasClass || isExport)
			{
				List<ExportedMethod> exportedMethods1 = exportedMethods ?? exportedClass.NullSafeCall<ExportedClass, List<ExportedMethod>>((ExportedClass i) => i.Methods);
				exportedMethods = exportedMethods1;
				if (exportedMethods1 != null)
				{
					foreach (ExportedMethod exportedMethod in exportedMethods)
					{
						if (!exportedMethod.IsStatic)
						{
							isExport = false;
							string methodIsNotStatic = DllExportLogginCodes.MethodIsNotStatic;
							string theMethod12IsNotStaticExportName0_ = Resources.The_method_1_2_is_not_static_export_name_0_;
							object[] exportName = new object[] { exportedMethod.ExportName, exportedMethod.ExportedClass.FullTypeName, exportedMethod.MemberName };
							base.Notify(state, 2, methodIsNotStatic, theMethod12IsNotStaticExportName0_, exportName);
						}
						if (!exportedMethod.IsGeneric)
						{
							continue;
						}
						isExport = false;
						string exportOnGenericMethod = DllExportLogginCodes.ExportOnGenericMethod;
						string theMethod12IsGenericExportName0GenericMethodsCannotBeExported_ = Resources.The_method_1_2_is_generic_export_name_0_Generic_methods_cannot_be_exported_;
						object[] objArray = new object[] { exportedMethod.ExportName, exportedMethod.ExportedClass.FullTypeName, exportedMethod.MemberName };
						base.Notify(state, 2, exportOnGenericMethod, theMethod12IsGenericExportName0GenericMethodsCannotBeExported_, objArray);
					}
				}
			}
			else
			{
				ExportedMethod duplicate = base.Exports.GetDuplicateExport(exportedClass.FullTypeName, state.Method.Name);
				base.ValidateExportNameAndLogError(duplicate, state);
				if (duplicate != null)
				{
					string ambigiguousExportName = DllExportLogginCodes.AmbigiguousExportName;
					string ambiguousExportName0On12_ = Resources.Ambiguous_export_name_0_on_1_2_;
					object[] exportName1 = new object[] { duplicate.ExportName, duplicate.ExportedClass.FullTypeName, duplicate.MemberName };
					base.Notify(state, 1, ambigiguousExportName, ambiguousExportName0On12_, exportName1);
				}
			}
			return isExport;
		}

		private bool GetPartBeforeParameters(string line, out string methodName, out string afterMethodName, out string foundResult, out string foundResultModifier, out string foundMethodAttributes)
		{
			string attributesWithResult;
			string str = line;
			methodName = null;
			foundResult = null;
			foundResultModifier = null;
			afterMethodName = null;
			foundMethodAttributes = null;
			str = str.TrimStart(new char[0]);
			if (!str.StartsWith(".method"))
			{
				return false;
			}
			str = str.Substring(".method".Length).TrimStart(new char[0]);
			StringBuilder stringBuilder = new StringBuilder(str.Length);
			string str1 = null;
			IlParsingUtils.ParseIlSnippet(str, ParsingDirection.Backward, (IlParsingUtils.IlSnippetLocation s) => {
				if (!s.WithinString && s.AtOuterBracket)
				{
					if (s.CurrentChar != ')')
					{
						str1 = str.Substring(0, s.Index);
						stringBuilder.Insert(0, s.CurrentChar);
						return false;
					}
					MethodDeclarationParserAction.RemoveCilManagedFromMethodSuffix(ref stringBuilder);
				}
				stringBuilder.Insert(0, s.CurrentChar);
				return true;
			}, null);
			if (stringBuilder.Length > 0)
			{
				afterMethodName = stringBuilder.ToString();
			}
			if (str1 != null)
			{
				methodName = MethodDeclarationParserAction.ExtractMethodName(str1, out attributesWithResult);
				if (this.SplitAttributesAndResult(attributesWithResult, out foundResult, out foundMethodAttributes))
				{
					if (foundResult != null && foundResult.Contains("("))
					{
						MethodDeclarationParserAction.ExtractResultModifier(ref foundResult, out foundResultModifier);
					}
					return true;
				}
			}
			return false;
		}

		private static StringBuilder OldExtractMethodName(string result, out string attributesWithResult)
		{
			string str;
			StringBuilder methodNameBuilder = new StringBuilder(result.Length);
			bool withinString = false;
			int lastPosition = -1;
			for (int i = result.Length - 1; i > -1; i--)
			{
				char currentChar = result[i];
				if (currentChar != '\'')
				{
					if (!withinString && currentChar != '.' && currentChar != ',' && currentChar != '/' && currentChar != '<' && currentChar != '>' && currentChar != '!')
					{
						break;
					}
					methodNameBuilder.Insert(0, currentChar);
				}
				else
				{
					withinString = !withinString;
				}
				lastPosition = i;
			}
			if (lastPosition > -1)
			{
				str = result.Substring(0, lastPosition);
			}
			else
			{
				str = null;
			}
			attributesWithResult = str;
			return methodNameBuilder;
		}

		private static bool RemoveCilManagedFromMethodSuffix(ref StringBuilder afterMethodNameBuilder)
		{
			bool flag = false;
			string t = MethodDeclarationParserAction._CilManagedRegex.Replace(afterMethodNameBuilder.ToString(), (Match m) => {
				flag = true;
				return "";
			});
			if (flag)
			{
				afterMethodNameBuilder = new StringBuilder(t, afterMethodNameBuilder.Capacity);
			}
			return flag;
		}

		private bool SplitAttributesAndResult(string attributesWithResult, out string foundResult, out string foundMethodAttributes)
		{
			if (string.IsNullOrEmpty(attributesWithResult))
			{
				foundResult = null;
				foundMethodAttributes = null;
				return false;
			}
			int previousPosition = -1;
			int resultStart = -1;
			int i = 0;
			while (i < attributesWithResult.Length)
			{
				char currentChar = attributesWithResult[i];
				if (currentChar != '\'')
				{
					if (!char.IsWhiteSpace(currentChar) && i != attributesWithResult.Length - 1)
					{
						if (previousPosition < 0)
						{
							previousPosition = i;
						}
					}
					else if (previousPosition > -1)
					{
						string currentLine = attributesWithResult.Substring(previousPosition, i - previousPosition).Trim();
						if (base.Parser.MethodAttributes.Contains(currentLine))
						{
							previousPosition = i + 1;
						}
						else
						{
							resultStart = previousPosition;
							break;
						}
					}
					i++;
				}
				else
				{
					resultStart = i;
					break;
				}
			}
			if (resultStart <= -1)
			{
				foundResult = null;
				foundMethodAttributes = null;
				return false;
			}
			foundMethodAttributes = attributesWithResult.Substring(0, resultStart).Trim();
			foundResult = attributesWithResult.Substring(resultStart).Trim();
			return true;
		}
	}
}