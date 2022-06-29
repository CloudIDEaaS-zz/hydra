using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace AddinExpress.Installer.WiXDesigner
{
	[ToolboxItem(false)]
	internal class CustomTreeViewComboBox : ComboBox
	{
		private System.Windows.Forms.ToolStripControlHost _treeViewHost;

		private ToolStripDropDown _dropDown;

		private const int WM_USER = 1024;

		private const int WM_REFLECT = 8192;

		private const int WM_COMMAND = 273;

		private const int CBN_DROPDOWN = 7;

		internal System.Windows.Forms.TreeView TreeView
		{
			get
			{
				return this._treeViewHost.Control as System.Windows.Forms.TreeView;
			}
		}

		public CustomTreeViewComboBox()
		{
			System.Windows.Forms.TreeView treeView = new System.Windows.Forms.TreeView()
			{
				FullRowSelect = true,
				HideSelection = false,
				HotTracking = true,
				LabelEdit = false,
				ShowLines = false,
				ShowRootLines = false,
				BorderStyle = BorderStyle.None
			};
			this._treeViewHost = new System.Windows.Forms.ToolStripControlHost(treeView);
			this._dropDown = new ToolStripDropDown();
			this._dropDown.Items.Add(this._treeViewHost);
		}

		internal void CloseDropDown()
		{
			if (this._dropDown != null)
			{
				this._dropDown.Close();
				base.Focus();
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this._dropDown != null)
				{
					this._dropDown.Dispose();
					this._dropDown = null;
				}
				if (this._treeViewHost != null)
				{
					this._treeViewHost.Dispose();
					this._treeViewHost = null;
				}
			}
			base.Dispose(disposing);
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			if ((int)e.KeyCode - (int)Keys.End <= (int)Keys.XButton1)
			{
				e.Handled = true;
				this.ShowDropDown();
			}
			base.OnKeyDown(e);
		}

		internal void ShowDropDown()
		{
			if (this._dropDown != null)
			{
				this._treeViewHost.Control.Width = base.DropDownWidth;
				this._treeViewHost.Control.Height = base.DropDownHeight;
				this._treeViewHost.Width = base.DropDownWidth;
				this._treeViewHost.Height = base.DropDownHeight;
				this._dropDown.Width = base.DropDownWidth;
				this._dropDown.Height = base.DropDownHeight;
				this._dropDown.Show(this, 0, base.Height);
				this._treeViewHost.Focus();
			}
		}

		protected override void WndProc(ref Message m)
		{
			if (m.Msg == 8465 && ((int)m.WParam >> 16 & 65535) == 7)
			{
				this.ShowDropDown();
				return;
			}
			base.WndProc(ref m);
		}
	}
}