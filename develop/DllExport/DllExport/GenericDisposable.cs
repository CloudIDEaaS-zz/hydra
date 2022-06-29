using System;

namespace DllExport
{
	public class GenericDisposable : IDisposable
	{
		private readonly Action _Action;

		public GenericDisposable(Action action)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			this._Action = action;
		}

		public void Dispose()
		{
			this._Action();
		}
	}
}