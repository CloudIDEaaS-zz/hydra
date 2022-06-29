using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;
using Utils.Controls.ColorPicker;

namespace ColorWheel
{
	/// <summary>   A form color chooser. </summary>
	///
	/// <remarks>   CloudIDEaaS, 11/24/2020. </remarks>

	public partial class frmColorChooser : ColorPickerDialog
	{
		/// <summary>   Default constructor. </summary>
		///
		/// <remarks>   CloudIDEaaS, 11/24/2020. </remarks>

		public frmColorChooser()
		{
			this.Load += frmColorChooser_Load;
		}

		private void frmColorChooser_Load(object sender, EventArgs e)
		{
			if (this.Color == null)
			{
				DebugUtils.Break();
			}
		}
	}
}
