using System;
using System.Drawing;
using System.Windows.Forms;

namespace AddinExpress.Installer.Prerequisites
{
	internal class MyToolStrip : ToolStrip
	{
		public MyToolStrip()
		{
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			Graphics graphics = e.Graphics;
			using (SolidBrush solidBrush = new SolidBrush(base.Parent.BackColor))
			{
				graphics.FillRectangle(solidBrush, 0, base.Height - 2, base.Width, base.Height);
			}
		}

		protected override void OnPaintBackground(PaintEventArgs e)
		{
			base.OnPaintBackground(e);
		}
	}
}