// file:	frmVariations.cs
//
// summary:	Implements the form variations class

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

namespace AbstraX
{
    /// <summary>   A form variations. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 3/15/2021. </remarks>

    public partial class frmVariations : Form
    {
        private string paragraph;

        /// <summary>   Gets the variations. </summary>
        ///
        /// <value> The variations. </value>

        public List<string> Variations { get; }

        /// <summary>   Default constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/15/2021. </remarks>

        public frmVariations(string paragraph)
        {
            this.paragraph = paragraph;
            this.Variations = new List<string>();

            InitializeComponent();
        }

        private void frmVariations_Load(object sender, EventArgs e)
        {
            var words = paragraph.SplitToWords().Where(w => !w.ToLower().IsInsignificantWord());
        }
    }
}
