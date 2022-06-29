using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class DropDownPaintEventArgs : PaintEventArgs
	{
		public Rectangle Bounds
		{
			get;
			private set;
		}

		public DropDownPaintEventArgs(System.Drawing.Graphics graphics, Rectangle clipRect, Rectangle bounds) : base(graphics, clipRect)
		{
			this.Bounds = bounds;
		}

		public void DrawFocusRectangle()
		{
			Rectangle bounds = this.Bounds;
			bounds.Inflate(-3, -3);
			ControlPaint.DrawFocusRectangle(base.Graphics, bounds);
		}
	}
}