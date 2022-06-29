using System;
using System.Runtime.CompilerServices;

namespace DllExport
{
	public class ValueDisposable<T> : GenericDisposable
	{
		public T Value
		{
			get;
			private set;
		}

		public ValueDisposable(T value, Action<T> action) : base(() => action(value))
		{
			this.Value = value;
		}
	}
}