using System;
using System.Collections.Generic;

namespace DllExport
{
	public sealed class DuplicateExports
	{
		private readonly List<ExportedMethod> _Duplicates = new List<ExportedMethod>();
		private readonly ExportedMethod _UsedExport;

		public ICollection<ExportedMethod> Duplicates
		{
			get
			{
				return this._Duplicates;
			}
		}

		public ExportedMethod UsedExport
		{
			get
			{
				return this._UsedExport;
			}
		}

		internal DuplicateExports(ExportedMethod usedExport)
		{
			this._UsedExport = usedExport;
		}
	}
}