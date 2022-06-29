// file:	frmAppDesigner.cs
//
// summary:	Implements the form application designer class

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AbstraX
{
    /// <summary>   Designer for Form Application. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 2/23/2021. </remarks>

    public partial class frmAppDesigner : Form
    {
        public string WorkingDirectory { get; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 2/23/2021. </remarks>
        ///

        public frmAppDesigner()
        {
            ctrlAppDesigner = new ctrlAppDesigner();

            InitializeComponent();
        }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 2/23/2021. </remarks>
        ///
        /// <param name="resourceData">     Information describing the resource. </param>
        /// <param name="localThemeFolder"> Pathname of the local theme folder. </param>
        /// <param name="workingDirectory"> Pathname of the working directory. </param>

        public frmAppDesigner(IResourceData resourceData, string localThemeFolder, string workingDirectory)
        {
            ctrlAppDesigner = new ctrlAppDesigner();

            ctrlAppDesigner.ResourceData = resourceData;
            ctrlAppDesigner.LocalThemeFolder = localThemeFolder;

            this.WorkingDirectory = workingDirectory;

            InitializeComponent();
        }

        private void frmAppDesigner_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            ctrlAppDesigner.InitializeControl(this.WorkingDirectory);
        }

        private void frmAppDesigner_FormClosing(object sender, FormClosingEventArgs e)
        {
            ctrlAppDesigner.FormClosing();
        }
    }
}
