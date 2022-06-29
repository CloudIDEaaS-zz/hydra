using DllExport;
using DllExport.Parsing.Actions;
using DllExport.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace DllExport.Parsing
{
	public sealed class IlParser : HasServiceProvider
	{
		private HashSet<string> _MethodAttributes;

		[Localizable(false)]
		private readonly static string[] _DefaultMethodAttributes;

		public string DllExportAttributeAssemblyName
		{
			get
			{
				return this.Exports.DllExportAttributeAssemblyName;
			}
		}

		public string DllExportAttributeFullName
		{
			get
			{
				return this.Exports.DllExportAttributeFullName;
			}
		}

		public string DllExportAttributeIlAsmFullName
		{
			get
			{
				string result = this.Exports.DllExportAttributeFullName;
				if (!string.IsNullOrEmpty(result))
				{
					int idx = result.LastIndexOf('.');
					if (idx <= -1)
					{
						result = string.Concat("'", result, "'");
					}
					else
					{
						string[] strArrays = new string[] { "'", result.Substring(0, idx), "'.'", result.Substring(idx + 1), "'" };
						result = string.Concat(strArrays);
					}
				}
				return result;
			}
		}

		public AssemblyExports Exports { get; set; }
		public AssemblyInitializers Initializers { get; set; }
		public IInputValues InputValues { get; set; }
		public string Files { get; set; }

		public HashSet<string> MethodAttributes
		{
			get
			{
				HashSet<string> strs;
				lock (this)
				{
					HashSet<string> strs1 = this._MethodAttributes;
					if (strs1 == null)
					{
						HashSet<string> methodAttributes = this.GetMethodAttributes();
						HashSet<string> strs2 = methodAttributes;
						this._MethodAttributes = methodAttributes;
						strs1 = strs2;
					}
					strs = strs1;
				}
				return strs;
			}
		}

		public bool ProfileActions
		{
			get;
			set;
		}

		public string TempDirectory
		{
			get;
			set;
		}

		static IlParser()
		{
			string[] strArrays = new string[] { "static", "public", "private", "family", "final", "specialname", "virtual", "abstract", "assembly", "famandassem", "famorassem", "privatescope", "hidebysig", "newslot", "strict", "rtspecialname", "flags", "unmanagedexp", "reqsecobj", "pinvokeimpl" };
			IlParser._DefaultMethodAttributes = strArrays;
		}

		public IlParser(IServiceProvider serviceProvider) : base(serviceProvider)
		{
		}

		private static IEnumerable<string> EnumerateStreamLines(StreamReader streamReader)
		{
			while (!streamReader.EndOfStream)
			{
				yield return streamReader.ReadLine();
			}
		}

		private static string GetExePath(string toolFileName, string installPath, string settingsName)
		{
			string exePath = "";

			if (!string.IsNullOrEmpty(installPath))
			{
				exePath = Path.Combine(Path.GetFullPath(installPath), toolFileName);

				if (!File.Exists(exePath))
				{
					exePath = Path.Combine(Path.Combine(Path.GetFullPath(installPath), "Bin"), toolFileName);
				}
			}
			else if (!string.IsNullOrEmpty((Settings.Default[settingsName] as string).NullSafeTrim(new char[0])))
			{
				exePath = Settings.Default.ILDasmPath;
			}

			if (string.IsNullOrEmpty(exePath) || !File.Exists(exePath))
			{
				exePath = toolFileName;
			}

			return exePath;
		}

		public IEnumerable<string> GetLines(CpuPlatform cpu)
		{
			IParserStateAction action3;
			IEnumerable<string> strs;
			Action<IParserStateAction, string> executeAction;

			using (this.GetNotifier().CreateContextName(this, Resources.ParseILContextName))
			{
				var actionsByState = IlParser.ParserStateAction.GetActionsByState(this);
				var lines = new List<string>(1000000);
				var watch = Stopwatch.StartNew();
				var parserStateValue = new ParserStateValues(cpu, lines)
				{
					State = ParserState.Normal
				};

				using (var stream = new FileStream(Path.Combine(this.TempDirectory, string.Concat(this.InputValues.FileName, ".il")), FileMode.Open))
				{
					using (StreamReader reader = new StreamReader(stream, Encoding.Unicode))
					{
						while (!reader.EndOfStream)
						{
							lines.Add(reader.ReadLine());
						}
					}
				}
				
				executeAction = (IParserStateAction action, string trimmedLine) => 
				{
					string actionTypeName = action.GetType().Name;

					using (IDisposable disposable = this.GetNotifier().CreateContextName(action, actionTypeName))
					{
						action.Execute(parserStateValue, trimmedLine);
					}
				};

				if (this.ProfileActions)
				{
					var action1 = executeAction;

					executeAction = (IParserStateAction action, string trimmedLine) => 
					{
						Stopwatch asw = Stopwatch.StartNew();

						action1(action, trimmedLine);
						asw.Stop();

						IParserStateAction milliseconds = action;
						milliseconds.Milliseconds = milliseconds.Milliseconds + asw.ElapsedMilliseconds;
					};
				}

                Dictionary<string, int> strs1 = new Dictionary<string, int>();

				for (int i = 0; i < lines.Count; i++)
				{
					parserStateValue.InputPosition = i;
					string line = lines[i];

					IlParsingUtils.ParseIlSnippet(line, ParsingDirection.Forward, (IlParsingUtils.IlSnippetLocation current) => 
					{
						if (!current.WithinString && current.CurrentChar == ']' && current.LastIdentifier != null && !strs1.ContainsKey(current.LastIdentifier))
						{
							strs1.Add(current.LastIdentifier, strs1.Count);
						}

						return true;

					}, null);

					string trimmedLine2 = line.NullSafeTrim(new char[0]);
					parserStateValue.AddLine = true;
					
					if (actionsByState.TryGetValue(parserStateValue.State, out action3))
					{
						executeAction(action3, trimmedLine2);
					}
					else
					{
						IDllExportNotifier notifier = this.GetNotifier();
						string noParserActionError = DllExportLogginCodes.NoParserActionError;
						string noActionForParserState0_ = Resources.No_action_for_parser_state_0_;
						object[] state = new object[] { parserStateValue.State };

						notifier.Notify(2, noParserActionError, noActionForParserState0_, state);
					}

					if (parserStateValue.AddLine)
					{
						parserStateValue.Result.Add(line);
					}
				}

				List<string> newResult = parserStateValue.Result;

				if (parserStateValue.ExternalAssemlyDeclarations.Count > 0)
				{
					newResult = new List<string>(parserStateValue.Result.Count);
					newResult.AddRange(parserStateValue.Result);

					List<ExternalAssemlyDeclaration> unusedMsCoreLibAliases = new List<ExternalAssemlyDeclaration>(parserStateValue.ExternalAssemlyDeclarations.Count);
					Dictionary<string, int> strs2 = new Dictionary<string, int>();

					foreach (string line in newResult)
					{
						if (line.Length < 3 || !line.Contains("["))
						{
							continue;
						}

						IlParsingUtils.ParseIlSnippet(line, ParsingDirection.Forward, (IlParsingUtils.IlSnippetLocation current) => {
						
							if (current.WithinScope && !current.WithinString && current.LastIdentifier != null && !strs2.ContainsKey(current.LastIdentifier))
							{
								strs2.Add(current.LastIdentifier, strs2.Count);
							}

							return true;

						}, null);
					}

					foreach (ExternalAssemlyDeclaration aliasLocation in parserStateValue.ExternalAssemlyDeclarations)
					{
						if (strs2.ContainsKey(aliasLocation.AliasName))
						{
							continue;
						}
					
						unusedMsCoreLibAliases.Add(aliasLocation);
					}

					if (unusedMsCoreLibAliases.Count > 0)
					{
						unusedMsCoreLibAliases.Reverse();

						foreach (ExternalAssemlyDeclaration aliasLocation in unusedMsCoreLibAliases)
						{
							int blockNesting = 0;
							int lastLine = -1;
							
							for (int i = aliasLocation.InputLineIndex; i < newResult.Count; i++)
							{
								string line = newResult[i].TrimStart(new char[0]);
								if (line == "{")
								{
									blockNesting++;
								}
								else if (line == "}")
								{
									if (blockNesting != 1)
									{
										blockNesting--;
									}
									else
									{
										lastLine = i;
										break;
									}
								}
							}
							
							if (lastLine <= -1)
							{
								continue;
							}

							this.GetNotifier().Notify(-2, DllExportLogginCodes.RemovingReferenceToDllExportAttributeAssembly, string.Format(Resources.Deleting_reference_to_0_, aliasLocation.AssemblyName, (aliasLocation.AliasName != aliasLocation.AssemblyName ? string.Format(Resources.AssemblyAlias, aliasLocation.AliasName) : "")), new object[0]);
							newResult.RemoveRange(aliasLocation.InputLineIndex, lastLine - aliasLocation.InputLineIndex + 1);
						}
					}
				}

				watch.Stop();
				IDllExportNotifier dllExportNotifier = this.GetNotifier();
				string parsing0LinesOfILTook1Ms_ = Resources.Parsing_0_lines_of_IL_took_1_ms_;
				object[] count = new object[] { lines.Count, watch.ElapsedMilliseconds };

				dllExportNotifier.Notify(-2, "EXPPERF02", parsing0LinesOfILTook1Ms_, count);

				if (this.ProfileActions)
				{
					foreach (KeyValuePair<ParserState, IParserStateAction> action2 in actionsByState)
					{
						IDllExportNotifier notifier1 = this.GetNotifier();
						string parsingAction0Took1Ms = Resources.Parsing_action_0_took_1_ms;
						object[] key = new object[] { action2.Key, action2.Value.Milliseconds };

						notifier1.Notify(-1, "EXPPERF03", parsingAction0Took1Ms, key);
					}
				}

				strs = newResult;
			}

			return strs;
		}

		private HashSet<string> GetMethodAttributes()
		{
			string str = (this.InputValues.MethodAttributes ?? "").Trim();
			object obj;
			if (!string.IsNullOrEmpty(str))
				obj = (object)((IEnumerable<string>)str.Split(new char[6]
				{
				  ' ',
				  ',',
				  ';',
				  '\t',
				  '\n',
				  '\r'
				}, StringSplitOptions.RemoveEmptyEntries)).Distinct<string>();
			else
				obj = (object)IlParser._DefaultMethodAttributes;
			return new HashSet<string>((IEnumerable<string>)obj);
		}

		internal IDllExportNotifier GetNotifier()
		{
			return base.ServiceProvider.GetService<IDllExportNotifier>();
		}	

		internal static int RunIlTool(string installPath, string toolFileName, string requiredPaths, string workingDirectory, string settingsName, string arguments, string toolLoggingCode, string verboseLoggingCode, IDllExportNotifier notifier, int timeout, Func<string, bool> suppressErrorOutputLine = null)
		{
			int result;
			int num;
			string item;
			Func<string, bool> func = suppressErrorOutputLine;

			notifier.Notify(0, string.Empty, string.Format("installPath: {0}", installPath));
			notifier.Notify(0, string.Empty, string.Format("toolFileName: {0}", toolFileName));
			notifier.Notify(0, string.Empty, string.Format("requiredPaths: {0}", requiredPaths));
			notifier.Notify(0, string.Empty, string.Format("workingDirectory: {0}", workingDirectory));
			notifier.Notify(0, string.Empty, string.Format("settingsName: {0}", settingsName));
			notifier.Notify(0, string.Empty, string.Format("arguments: {0}", arguments));
			notifier.Notify(0, string.Empty, string.Format("toolLoggingCode: {0}", toolLoggingCode));
			notifier.Notify(0, string.Empty, string.Format("verboseLoggingCode: {0}", verboseLoggingCode));
			notifier.Notify(0, string.Empty, string.Format("timeout: {0}", timeout));

			using (notifier.CreateContextName(null, toolFileName))
			{
				if (func == null)
				{
					func = (string l) => false;
				}
				else
				{
					Func<string, bool> func1 = func;
					func = (string line) => {
						if (line == null)
						{
							return false;
						}
						return func1(line);
					};
				}

				string toolName = Path.GetFileName(toolFileName);
				string exePath = IlParser.GetExePath(toolName, installPath, settingsName);
				toolName = Path.GetFileNameWithoutExtension(toolName);

				using (Process process = new Process())
				{
					var calling0With1_ = Resources.calling_0_with_1_;
					var objArray = new object[] { exePath, arguments };
					ProcessStartInfo processStartInfo;
					Stopwatch watch;
					StringBuilder output;
					StringBuilder errorOutput;
					Action<IEnumerable<string>, StringBuilder> action;
					Action<StreamReader, StringBuilder> readOutput;
					Action<StreamReader, StringBuilder> readErrorOutput;
					bool graceFulExit;

					notifier.Notify(-2, toolLoggingCode, calling0With1_, objArray);

					processStartInfo = new ProcessStartInfo(exePath, arguments)
					{
						UseShellExecute = false,
						CreateNoWindow = true,
						RedirectStandardOutput = true,
						RedirectStandardError = true
					};

					if (!string.IsNullOrEmpty(workingDirectory))
					{
						processStartInfo.WorkingDirectory = Path.GetFullPath(workingDirectory);
					}

					if (!string.IsNullOrEmpty(requiredPaths))
					{
						var environmentVariables = processStartInfo.EnvironmentVariables;
						var str = requiredPaths.Trim(new char[] { ';' });

						if (processStartInfo.EnvironmentVariables.ContainsKey("PATH"))
						{
							item = processStartInfo.EnvironmentVariables["PATH"];
						}
						else
						{
							item = null;
						}

						environmentVariables["PATH"] = string.Concat(str, ";", item);
					}

					process.StartInfo = processStartInfo;
					process.Start();

					watch = Stopwatch.StartNew();
					output = new StringBuilder();
					errorOutput = new StringBuilder();
					action = (IEnumerable<string> lines, StringBuilder sb) => lines.Aggregate<string, StringBuilder>(sb, (StringBuilder r, string line) => r.AppendLine(line));
					readOutput = (StreamReader sr, StringBuilder sb) => action(IlParser.EnumerateStreamLines(sr), sb);
					readErrorOutput = (StreamReader sr, StringBuilder sb) => action(
						from line in IlParser.EnumerateStreamLines(sr)
						where !func(line)
						select line, sb);

					while (watch.ElapsedMilliseconds < (long)timeout && !process.HasExited)
					{
						readOutput(process.StandardOutput, output);
						readErrorOutput(process.StandardError, errorOutput);
					}

					graceFulExit = process.HasExited;

					readOutput(process.StandardOutput, output);
					readErrorOutput(process.StandardError, errorOutput);

					if (!graceFulExit)
					{
						var couldKill = false;
						Exception killingException = null;

						try
						{
							process.Kill();
							couldKill = true;
						}
						catch (Exception exception)
						{
							killingException = exception;
						}
						if (!couldKill)
						{
							CultureInfo invariantCulture = CultureInfo.InvariantCulture;
							string r0DidNotReturnAfter1MsAndItCouldNotBeStopped = Resources.R_0_did_not_return_after_1_ms_and_it_could_not_be_stopped;
							object[] objArray1 = new object[] { toolName, timeout, killingException.Message };
							throw new InvalidOperationException(string.Format(invariantCulture, r0DidNotReturnAfter1MsAndItCouldNotBeStopped, objArray1));
						}

						CultureInfo cultureInfo = CultureInfo.InvariantCulture;
						string r0DidNotReturnAfter1Ms = Resources.R_0_did_not_return_after_1_ms;
						object[] objArray2 = new object[] { toolName, timeout };

						throw new InvalidOperationException(string.Format(cultureInfo, r0DidNotReturnAfter1Ms, objArray2));
					}

					string r01ReturnedGracefully = Resources.R_0_1_returned_gracefully;
					object[] objArray3 = new object[] { toolName, exePath };
					notifier.Notify(-2, toolLoggingCode, r01ReturnedGracefully, objArray3);
					int exitCode = process.ExitCode;

					if (exitCode != 0 || errorOutput.Length > 0)
					{
						throw new InvalidOperationException(((errorOutput.Length > 0 ? errorOutput : output)).ToString());
					}

					if (errorOutput.Length > 0)
					{
						notifier.Notify(-3, verboseLoggingCode, errorOutput.ToString(), new object[0]);
					}

					if (output.Length > 0)
					{
						notifier.Notify(-3, verboseLoggingCode, output.ToString(), new object[0]);
					}

					result = exitCode;
				}

				num = result;
			}

			return num;
		}

		public abstract class ParserStateAction : IParserStateAction
		{
			protected string DllExportAttributeAssemblyName
			{
				get
				{
					return this.Parser.DllExportAttributeAssemblyName;
				}
			}

			protected string DllExportAttributeFullName
			{
				get
				{
					return this.Parser.DllExportAttributeFullName;
				}
			}

			protected AssemblyExports Exports
			{
				get
				{
					return this.Parser.Exports;
				}
			}

			public long Milliseconds
			{
				get;
				set;
			}

			protected IDllExportNotifier Notifier
			{
				get
				{
					return this.Parser.GetNotifier();
				}
			}

			public IlParser Parser
			{
				get;
				set;
			}

			protected ParserStateAction()
			{
			}

			public abstract void Execute(ParserStateValues state, string trimmedLine);

			public static Dictionary<ParserState, IParserStateAction> GetActionsByState(IlParser parser)
			{
				Dictionary<ParserState, IParserStateAction> parserStates = new Dictionary<ParserState, IParserStateAction>()
				{
					{ ParserState.Module, new ModuleParserAction() },
					{ ParserState.ClassDeclaration, new ClassDeclarationParserAction() },
					{ ParserState.Class, new ClassParserAction() },
					{ ParserState.DeleteExportAttribute, new DeleteExportAttributeParserAction() },
					{ ParserState.MethodDeclaration, new MethodDeclarationParserAction() },
					{ ParserState.Method, new MethodParserAction() },
					{ ParserState.MethodProperties, new MethodPropertiesParserAction() },
					{ ParserState.Normal, new NormalParserAction() }
				};

				Dictionary<ParserState, IParserStateAction> result = parserStates;

				foreach (IParserStateAction action in result.Values)
				{
					action.Parser = parser;
				}

				return result;
			}

			protected void Notify(int severity, string code, string message, params object[] values)
			{
				this.Notify(null, severity, code, message, values);
			}

			protected void Notify(ParserStateValues stateValues, int severity, string code, string message, params object[] values)
			{
				if (stateValues != null)
				{
					SourceCodeRange range = stateValues.GetRange();
					SourceCodeRange codeRange = range;
					if (range != null)
					{
						this.Notifier.Notify(severity, code, codeRange.FileName, new SourceCodePosition?(codeRange.StartPosition), new SourceCodePosition?(codeRange.EndPosition), message, values);
						return;
					}
				}
				this.Notifier.Notify(severity, code, message, values);
			}

			protected bool ValidateExportNameAndLogError(ExportedMethod exportMethod, ParserStateValues stateValues)
			{
				bool result;
				if (exportMethod == null)
				{
					result = false;
				}
				else if (exportMethod.ExportName == null || !exportMethod.ExportName.Contains("'") && !Regex.IsMatch(exportMethod.ExportName, "\\P{IsBasicLatin}"))
				{
					result = true;
				}
				else
				{
					string exportNamesHaveToBeBasicLatin = DllExportLogginCodes.ExportNamesHaveToBeBasicLatin;
					string exportName0On1_2IsUnicodeWindowsExportNamesHaveToBeBasicLatin = Resources.Export_name_0_on_1__2_is_Unicode_windows_export_names_have_to_be_basic_latin;
					object[] exportName = new object[] { exportMethod.ExportName, exportMethod.ExportedClass.FullTypeName, exportMethod.MemberName };
					this.Notify(stateValues, 3, exportNamesHaveToBeBasicLatin, exportName0On1_2IsUnicodeWindowsExportNamesHaveToBeBasicLatin, exportName);
					result = false;
				}
				return result;
			}
		}
	}
}