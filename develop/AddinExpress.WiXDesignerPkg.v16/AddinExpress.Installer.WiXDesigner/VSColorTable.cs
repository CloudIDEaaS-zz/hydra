using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace AddinExpress.Installer.WiXDesigner
{
	[ComVisible(false)]
	internal class VSColorTable : ProfessionalColorTable
	{
		private IVsUIShell2 uiShell;

		private ProfessionalColorTable colorTable;

		private Color menuBorder;

		public override Color ButtonCheckedGradientBegin
		{
			get
			{
				if (this.colorTable != null)
				{
					return this.colorTable.ButtonCheckedGradientBegin;
				}
				return this.GetShellColor(-20);
			}
		}

		public override Color ButtonCheckedGradientEnd
		{
			get
			{
				if (this.colorTable != null)
				{
					return this.colorTable.ButtonCheckedGradientEnd;
				}
				return this.GetShellColor(-20);
			}
		}

		public override Color ButtonCheckedGradientMiddle
		{
			get
			{
				if (this.colorTable != null)
				{
					return this.colorTable.ButtonCheckedGradientMiddle;
				}
				return this.GetShellColor(-20);
			}
		}

		public override Color ButtonPressedBorder
		{
			get
			{
				if (this.colorTable != null)
				{
					return this.colorTable.ButtonPressedBorder;
				}
				return this.GetShellColor(-294);
			}
		}

		public override Color ButtonPressedGradientBegin
		{
			get
			{
				if (this.colorTable != null)
				{
					return this.colorTable.ButtonPressedGradientBegin;
				}
				return this.GetShellColor(-291);
			}
		}

		public override Color ButtonPressedGradientEnd
		{
			get
			{
				if (this.colorTable != null)
				{
					return this.colorTable.ButtonPressedGradientEnd;
				}
				return this.GetShellColor(-293);
			}
		}

		public override Color ButtonPressedGradientMiddle
		{
			get
			{
				if (this.colorTable != null)
				{
					return this.colorTable.ButtonPressedGradientMiddle;
				}
				return this.GetShellColor(-292);
			}
		}

		public override Color ButtonSelectedBorder
		{
			get
			{
				if (this.colorTable != null)
				{
					return this.colorTable.ButtonSelectedBorder;
				}
				return this.GetShellColor(-294);
			}
		}

		public override Color ButtonSelectedGradientBegin
		{
			get
			{
				if (this.colorTable != null)
				{
					return this.colorTable.ButtonSelectedGradientBegin;
				}
				return this.GetShellColor(-295);
			}
		}

		public override Color ButtonSelectedGradientEnd
		{
			get
			{
				if (this.colorTable != null)
				{
					return this.colorTable.ButtonSelectedGradientEnd;
				}
				return this.GetShellColor(-298);
			}
		}

		public override Color ButtonSelectedGradientMiddle
		{
			get
			{
				if (this.colorTable != null)
				{
					return this.colorTable.ButtonSelectedGradientMiddle;
				}
				return this.GetShellColor(-296);
			}
		}

		public override Color CheckBackground
		{
			get
			{
				if (this.colorTable != null)
				{
					return this.colorTable.CheckBackground;
				}
				return this.GetShellColor(-287);
			}
		}

		public override Color CheckPressedBackground
		{
			get
			{
				if (this.colorTable != null)
				{
					return this.colorTable.CheckPressedBackground;
				}
				return this.GetShellColor(-292);
			}
		}

		public override Color CheckSelectedBackground
		{
			get
			{
				if (this.colorTable != null)
				{
					return this.colorTable.CheckSelectedBackground;
				}
				return this.GetShellColor(-18);
			}
		}

		public override Color GripDark
		{
			get
			{
				if (this.colorTable != null)
				{
					return this.colorTable.GripDark;
				}
				return this.GetShellColor(-53);
			}
		}

		public override Color GripLight
		{
			get
			{
				if (this.colorTable != null)
				{
					return this.colorTable.GripLight;
				}
				return this.GetShellColor(-53);
			}
		}

		public override Color ImageMarginGradientBegin
		{
			get
			{
				if (this.colorTable != null)
				{
					return this.colorTable.ImageMarginGradientBegin;
				}
				return this.GetShellColor(-287);
			}
		}

		public override Color ImageMarginGradientEnd
		{
			get
			{
				if (this.colorTable != null)
				{
					return this.colorTable.ImageMarginGradientEnd;
				}
				return this.GetShellColor(-287);
			}
		}

		public override Color ImageMarginGradientMiddle
		{
			get
			{
				if (this.colorTable != null)
				{
					return this.colorTable.ImageMarginGradientMiddle;
				}
				return this.GetShellColor(-287);
			}
		}

		public override Color ImageMarginRevealedGradientBegin
		{
			get
			{
				if (this.colorTable != null)
				{
					return this.colorTable.ImageMarginRevealedGradientBegin;
				}
				return this.GetShellColor(-287);
			}
		}

		public override Color ImageMarginRevealedGradientEnd
		{
			get
			{
				if (this.colorTable != null)
				{
					return this.colorTable.ImageMarginRevealedGradientEnd;
				}
				return this.GetShellColor(-287);
			}
		}

		public override Color ImageMarginRevealedGradientMiddle
		{
			get
			{
				if (this.colorTable != null)
				{
					return this.colorTable.ImageMarginRevealedGradientMiddle;
				}
				return this.GetShellColor(-287);
			}
		}

		public override Color MenuBorder
		{
			get
			{
				if (this.colorTable != null)
				{
					return this.menuBorder;
				}
				return this.GetShellColor(-286);
			}
		}

		public override Color MenuItemBorder
		{
			get
			{
				if (this.colorTable != null)
				{
					return this.colorTable.MenuItemBorder;
				}
				return this.GetShellColor(-10);
			}
		}

		public override Color MenuItemPressedGradientBegin
		{
			get
			{
				if (this.colorTable != null)
				{
					return this.colorTable.MenuItemPressedGradientBegin;
				}
				return this.GetShellColor(-20);
			}
		}

		public override Color MenuItemPressedGradientEnd
		{
			get
			{
				if (this.colorTable != null)
				{
					return this.colorTable.MenuItemPressedGradientEnd;
				}
				return this.GetShellColor(-20);
			}
		}

		public override Color MenuItemPressedGradientMiddle
		{
			get
			{
				if (this.colorTable != null)
				{
					return this.colorTable.MenuItemPressedGradientMiddle;
				}
				return this.GetShellColor(-20);
			}
		}

		public override Color MenuItemSelected
		{
			get
			{
				if (this.colorTable != null)
				{
					return this.colorTable.MenuItemSelected;
				}
				return this.GetShellColor(-20);
			}
		}

		public override Color MenuItemSelectedGradientBegin
		{
			get
			{
				if (this.colorTable != null)
				{
					return this.colorTable.MenuItemSelectedGradientBegin;
				}
				return this.GetShellColor(-295);
			}
		}

		public override Color MenuItemSelectedGradientEnd
		{
			get
			{
				if (this.colorTable != null)
				{
					return this.colorTable.MenuItemSelectedGradientEnd;
				}
				return this.GetShellColor(-298);
			}
		}

		public override Color MenuStripGradientBegin
		{
			get
			{
				if (this.colorTable != null)
				{
					return this.colorTable.MenuStripGradientBegin;
				}
				return this.GetShellColor(-284);
			}
		}

		public override Color MenuStripGradientEnd
		{
			get
			{
				if (this.colorTable != null)
				{
					return this.colorTable.MenuStripGradientEnd;
				}
				return this.GetShellColor(-285);
			}
		}

		public override Color OverflowButtonGradientBegin
		{
			get
			{
				if (this.colorTable != null)
				{
					return this.colorTable.OverflowButtonGradientBegin;
				}
				return this.GetShellColor(-285);
			}
		}

		public override Color OverflowButtonGradientEnd
		{
			get
			{
				if (this.colorTable != null)
				{
					return this.colorTable.OverflowButtonGradientEnd;
				}
				return this.GetShellColor(-285);
			}
		}

		public override Color OverflowButtonGradientMiddle
		{
			get
			{
				if (this.colorTable != null)
				{
					return this.colorTable.OverflowButtonGradientMiddle;
				}
				return this.GetShellColor(-285);
			}
		}

		public override Color RaftingContainerGradientBegin
		{
			get
			{
				if (this.colorTable != null)
				{
					return this.colorTable.RaftingContainerGradientBegin;
				}
				return this.GetShellColor(-328);
			}
		}

		public override Color RaftingContainerGradientEnd
		{
			get
			{
				if (this.colorTable != null)
				{
					return this.colorTable.RaftingContainerGradientEnd;
				}
				return this.GetShellColor(-329);
			}
		}

		public override Color SeparatorDark
		{
			get
			{
				if (this.colorTable != null)
				{
					return this.colorTable.SeparatorDark;
				}
				return this.GetShellColor(-289);
			}
		}

		public override Color SeparatorLight
		{
			get
			{
				if (this.colorTable != null)
				{
					return this.colorTable.SeparatorLight;
				}
				return this.GetShellColor(-289);
			}
		}

		public override Color ToolStripBorder
		{
			get
			{
				if (this.colorTable != null)
				{
					return this.colorTable.ToolStripBorder;
				}
				return this.GetShellColor(-13);
			}
		}

		public override Color ToolStripDropDownBackground
		{
			get
			{
				if (this.colorTable != null)
				{
					return this.colorTable.ToolStripDropDownBackground;
				}
				return this.GetShellColor(-285);
			}
		}

		public override Color ToolStripGradientBegin
		{
			get
			{
				if (this.colorTable != null)
				{
					return this.colorTable.ToolStripGradientBegin;
				}
				return this.GetShellColor(-13);
			}
		}

		public override Color ToolStripGradientEnd
		{
			get
			{
				if (this.colorTable != null)
				{
					return this.colorTable.ToolStripGradientEnd;
				}
				return this.GetShellColor(-14);
			}
		}

		public override Color ToolStripGradientMiddle
		{
			get
			{
				if (this.colorTable != null)
				{
					return this.colorTable.ToolStripGradientMiddle;
				}
				return this.GetShellColor(-15);
			}
		}

		public VSColorTable(ProfessionalColorTable colorTable, Color menuBorder)
		{
			this.colorTable = colorTable;
			this.menuBorder = menuBorder;
		}

		public VSColorTable(IVsUIShell2 uiShell)
		{
			this.uiShell = uiShell;
		}

		private Color GetShellColor(int vsColor)
		{
			uint num;
			if (this.uiShell == null || this.uiShell.GetVSSysColorEx(vsColor, out num) != 0)
			{
				return Color.Empty;
			}
			return ColorTranslator.FromWin32((int)num);
		}
	}
}