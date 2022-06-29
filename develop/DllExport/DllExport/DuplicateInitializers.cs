using System;
using System.Collections.Generic;

namespace DllExport
{
	public sealed class DuplicateInitializers
	{
		private readonly List<InitializerMethod> _Duplicates = new List<InitializerMethod>();
		private readonly InitializerMethod _UsedInitializer;

		public ICollection<InitializerMethod> Duplicates
		{
			get
			{
				return this._Duplicates;
			}
		}

		public InitializerMethod UsedInitializer
		{
			get
			{
				return this._UsedInitializer;
			}
		}

		internal DuplicateInitializers(InitializerMethod usedInitializer)
		{
			this._UsedInitializer = usedInitializer;
		}
	}
}