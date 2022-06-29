using SassParser;
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

namespace AbstraX
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

		/// <summary>   Gets or sets the name of the resource. </summary>
		///
		/// <value> The name of the resource. </value>

		public string ResourceName { get; set; }

		/// <summary>   Gets or sets information describing the resource. </summary>
		///
		/// <value> Information describing the resource. </value>

		public IResourceData ResourceData { get; set; }

		/// <summary>   Event handler. Called by frmColorChooser for load events. </summary>
		///
		/// <remarks>   CloudIDEaaS, 11/24/2020. </remarks>
		///
		/// <param name="sender">   Source of the event. </param>
		/// <param name="e">        Event information. </param>

		private void frmColorChooser_Load(object sender, EventArgs e)
		{
			if (this.Color == null)
			{
				var expression = @"^(?<groupName>\w+)(-(?<offsetColorName>\w+))?$";
				var groupName = this.ResourceName.RegexGet(expression, "groupName");
				var offsetColorName = this.ResourceName.RegexGet(expression, "offsetColorName");

				this.Color = this.ResourceData.FindColor(this.ResourceName);
				this.Text += " - " + groupName + (offsetColorName.IsNullOrEmpty() ? string.Empty : " - " + offsetColorName);
			}
		}
	}
}
