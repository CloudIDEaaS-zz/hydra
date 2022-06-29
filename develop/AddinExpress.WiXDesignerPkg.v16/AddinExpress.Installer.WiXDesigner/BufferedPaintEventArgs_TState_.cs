using System;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class BufferedPaintEventArgs<TState> : EventArgs
	{
		public System.Drawing.Graphics Graphics
		{
			get;
			private set;
		}

		public TState State
		{
			get;
			private set;
		}

		public BufferedPaintEventArgs(TState state, System.Drawing.Graphics graphics)
		{
			this.State = state;
			this.Graphics = graphics;
		}
	}
}