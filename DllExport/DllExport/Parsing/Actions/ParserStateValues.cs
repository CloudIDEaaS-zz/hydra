using DllExport;
using DllExport.Parsing;
using DllExport.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace DllExport.Parsing.Actions
{
	public sealed class ParserStateValues
	{
		public readonly Stack<string> ClassNames = new Stack<string>();
		public readonly ParserStateValues.MethodStateValues Method = new ParserStateValues.MethodStateValues();
		private readonly CpuPlatform _Cpu;
		private readonly ReadOnlyCollection<string> _InputLines;
		private readonly List<string> _Result = new List<string>();
		public bool AddLine;
		public string ClassDeclaration;
		public string Module;
		public int MethodPos;
		public ParserState State;
		private readonly IList<ExternalAssemlyDeclaration> _ReadonlyExternalAssemlyDeclarations;
		private readonly List<ExternalAssemlyDeclaration> _ExternalAssemlyDeclarations = new List<ExternalAssemlyDeclaration>();

		public CpuPlatform Cpu
		{
			get
			{
				return this._Cpu;
			}
		}

		public IList<ExternalAssemlyDeclaration> ExternalAssemlyDeclarations
		{
			get
			{
				return this._ReadonlyExternalAssemlyDeclarations;
			}
		}

		public IList<string> InputLines
		{
			get
			{
				return this._InputLines;
			}
		}

		public int InputPosition
		{
			get;
			internal set;
		}

		public List<string> Result
		{
			get
			{
				return this._Result;
			}
		}

		public ParserStateValues(CpuPlatform cpu, IList<string> inputLines)
		{
			this._Cpu = cpu;
			this._InputLines = new ReadOnlyCollection<string>(inputLines);
			this._ReadonlyExternalAssemlyDeclarations = this._ExternalAssemlyDeclarations.AsReadOnly();
		}

		public SourceCodeRange GetRange()
		{
			for (int i = this.InputPosition; i < this.InputLines.Count; i++)
			{
				string line = this.InputLines[i];
				if (line != null)
				{
					string str = line.Trim();
					line = str;
					if (str.StartsWith(".line", StringComparison.Ordinal))
					{
						return SourceCodeRange.FromMsIlLine(line);
					}
				}
			}
			return null;
		}

		public ExternalAssemlyDeclaration RegisterMsCorelibAlias(string assemblyName, string alias)
		{
			ExternalAssemlyDeclaration location = new ExternalAssemlyDeclaration(this.Result.Count, assemblyName, alias);
			this._ExternalAssemlyDeclarations.Add(location);
			return location;
		}

		public sealed class MethodStateValues
		{
			public string After { get; set; }
			public string Attributes { get; set; }
			public string Declaration { get; set; }
			public string Name { get; set; }
			public string Result { get; set; }
			public string ResultAttributes { get; set; }

			public MethodStateValues()
			{
				this.Reset();
			}

			public void Reset()
			{
				this.Declaration = "";
				this.Name = "";
				this.Attributes = "";
				this.Result = "";
				this.ResultAttributes = "";
				this.After = "";
			}

			public override string ToString()
			{
				return string.Concat(this.Name.IfEmpty(Resources.no_name___), "; ", this.Declaration.IfEmpty(Resources.no_declaration___));
			}
		}
	}
}