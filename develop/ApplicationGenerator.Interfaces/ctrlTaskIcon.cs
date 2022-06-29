// file:	ctrlTaskIcon.cs
//
// summary:	Implements the control task icon class

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;

namespace AbstraX
{
    /// <summary>   A control task icon. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 3/25/2021. </remarks>

    public partial class ctrlTaskIcon : UserControl
    {
        /// <summary>   Gets the name of the process. </summary>
        ///
        /// <value> The name of the process. </value>

        public string ProcessName { get; }

        /// <summary>   Gets the process file. </summary>
        ///
        /// <value> The process file. </value>

        public FileInfo ProcessFile { get; }

        /// <summary>   Gets the processes. </summary>
        ///
        /// <value> The processes. </value>

        public List<PlatformProcess> Processes { get; private set; }

        /// <summary>   Gets or sets the number of process. </summary>
        ///
        /// <value> The number of process. </value>

        public int ProcessCount { get; private set; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/25/2021. </remarks>
        ///
        /// <param name="process">  The process. </param>

        public ctrlTaskIcon(PlatformProcess process)
        {
            Bitmap icon;

            this.ProcessName = process.ProcessName;

            InitializeComponent();

            UpdateList();

            process = this.Processes.FirstOrDefault();

            if (process == null)
            {
                return;
            }

            if (process.Path != null)
            {
                this.ProcessFile = new FileInfo(process.Path);
            }

            try
            {
                icon = this.ProcessFile.GetLargeIcon<Bitmap>();
            }
            catch
            {
                icon = typeof(ctrlTaskIcon).ReadResource<Bitmap>(@"images\ProcessIcon32.png");
            }

            this.pictureBoxIcon.Image = icon;
        }

        /// <summary>   Updates the processes. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 4/2/2021. </remarks>

        public void UpdateList()
        {

            this.Processes = ProcessExtensions.GetAllProcesses().Where(p => p.ProcessName == this.ProcessName).ToList();
            this.ProcessCount = this.Processes.Count;

            this.progressBarCount.Value = (int)(100f - (((float)this.ProcessCount / 20f) - .05f));

            this.toolTip.SetToolTip(this, this.ToolTipText);
            this.toolTip.SetToolTip(this.pictureBoxIcon, this.ToolTipText);
            this.toolTip.SetToolTip(this.progressBarCount, this.ToolTipText);
        }

        private string ToolTipText
        {
            get
            {
                return string.Format("{0} ({1})", this.ProcessName, this.Processes.Count);
            }
        }

        /// <summary>   Event handler. Called by terminateToolStripMenuItem for click events. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/25/2021. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>

        private void terminateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, $"Are you sure you want to end { ProcessName }?", "Terminate Process", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                var process = this.Processes.First();

                process.Process.Kill();
            }
        }
    }
}
