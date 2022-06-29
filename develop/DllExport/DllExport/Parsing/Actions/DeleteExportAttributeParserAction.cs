using DllExport;
using DllExport.Parsing;
using DllExport.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace DllExport.Parsing.Actions
{
	[ParserStateAction(ParserState.DeleteExportAttribute)]
	public sealed class DeleteExportAttributeParserAction : IlParser.ParserStateAction
	{
		public DeleteExportAttributeParserAction()
		{
		}

		public override void Execute(ParserStateValues state, string trimmedLine)
		{
			ExportedClass exportedClass;
			if (!trimmedLine.StartsWith(".custom", StringComparison.Ordinal) && !trimmedLine.StartsWith("// Code", StringComparison.Ordinal))
			{
				state.AddLine = false;
				return;
			}
			if (!base.Exports.ClassesByName.TryGetValue(state.ClassNames.Peek(), out exportedClass))
			{
				state.AddLine = false;
			}
			else
			{
				ExportedMethod i = exportedClass.MethodsByName[state.Method.Name][0];
				string oldMethodDeclaration = state.Method.Declaration;
				StringBuilder newDeclaration = new StringBuilder(250);
				newDeclaration.Append(".method ").Append(state.Method.Attributes.NullSafeTrim(new char[0])).Append(" ");
				newDeclaration.Append(state.Method.Result.NullSafeTrim(new char[0]));
				newDeclaration.Append(" modopt(['mscorlib']'").Append(AssemblyExports.ConventionTypeNames[i.CallingConvention]).Append("') ");
				if (!string.IsNullOrEmpty(state.Method.ResultAttributes))
				{
					newDeclaration.Append(" ").Append(state.Method.ResultAttributes);
				}
				newDeclaration.Append(" '").Append(state.Method.Name).Append("'").Append(state.Method.After.NullSafeTrim(new char[0]));
				bool isValidExport = base.ValidateExportNameAndLogError(i, state);
				if (isValidExport)
				{
					state.Method.Declaration = newDeclaration.ToString();
				}
				if (state.MethodPos != 0)
				{
					state.Result.Insert(state.MethodPos, state.Method.Declaration);
				}
				if (isValidExport)
				{
					IDllExportNotifier notifier = base.Notifier;
					string oldDeclaration = DllExportLogginCodes.OldDeclaration;
					string str = string.Concat("\t", Resources.OldDeclaration_0_);
					object[] objArray = new object[] { oldMethodDeclaration };
					notifier.Notify(-2, oldDeclaration, str, objArray);
					IDllExportNotifier dllExportNotifier = base.Notifier;
					string str1 = DllExportLogginCodes.NewDeclaration;
					string str2 = string.Concat("\t", Resources.NewDeclaration_0_);
					object[] declaration = new object[] { state.Method.Declaration };
					dllExportNotifier.Notify(-2, str1, str2, declaration);
					List<string> result = state.Result;
					CultureInfo invariantCulture = CultureInfo.InvariantCulture;
					object[] vTableOffset = new object[] { i.VTableOffset, i.ExportName };
					result.Add(string.Format(invariantCulture, "    .export [{0}] as '{1}'", vTableOffset));
					IDllExportNotifier notifier1 = base.Notifier;
					string addingVtEntry = DllExportLogginCodes.AddingVtEntry;
					string str3 = string.Concat("\t", Resources.AddingVtEntry_0_export_1_);
					object[] vTableOffset1 = new object[] { i.VTableOffset, i.ExportName };
					notifier1.Notify(-1, addingVtEntry, str3, vTableOffset1);
				}
			}
			state.State = ParserState.Method;
		}
	}
}