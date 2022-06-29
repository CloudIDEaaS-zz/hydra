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

namespace HydraCrashAnalyzer
{
    public partial class frmAnalyzer : Form, IAnalyzer
    {
        public frmAnalyzer()
        {
            InitializeComponent();
        }

        public void SetDebugInfo(DebugState debugState, string dumpFile)
        {
            var file = new FileInfo(dumpFile);
            var logFile = file.Directory.GetFiles("*.log").First();

            txtApplication.Text = debugState.ProcessName;
            txtError.Text = debugState.ExceptionMessage;

            txtDetails.AppendLine(debugState.ExceptionText);
            txtDetails.AppendLine(string.Empty);
            txtDetails.AppendLine(debugState.StackTrace.ToString());

            cmdOpenLog.Text = logFile.FullName;
            cmdOpenLog.Enabled = true;

            lblMessage.Text = "Debug information obtained. Creating dump file...";

            cmdOpenDumpFile.Text = dumpFile;
        }

        public void SetDumpFileCreated()
        {
            lblMessage.Text = "Dump file created.";
            cmdOpenDumpFile.Enabled = true;

            SubmitDetails();
        }

        private void SubmitDetails()
        {
            this.DelayInvoke(3000, () =>
            {
                lblMessage.Text = "Submitting details to server...";
                cmdViewPolicy.Visible = true;

                this.DelayInvoke(5000, () =>
                {
                    lblMessage.Text = "Details submitted";

                    cmdClose.Enabled = true;
                });
            });
        }

        public void SetInternalException(Exception ex)
        {
            txtApplication.Text = Process.GetCurrentProcess().ProcessName;
            txtError.Text = ex.Message;

            txtDetails.AppendLine(ex.ToString());

            lblMessage.Text = "Debug information obtained.";

            SubmitDetails();
        }

        private void frmAnalyzer_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!cmdClose.Enabled)
            {
                e.Cancel = true;
            }
        }

        private void cmdClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmAnalyzer_Load(object sender, EventArgs e)
        {
            txtCaseId.Text = Guid.NewGuid().ToString().Left(8);

            this.Flash(FlashWindowFlags.FLASHW_ALL);
        }

        private void cmdOpenLog_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = cmdOpenLog.Text,
                UseShellExecute = true
            };

            Process.Start(cmdOpenLog.Text);
        }

        private void cmdOpenDumpFile_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = cmdOpenDumpFile.Text,
                UseShellExecute = true
            };

            Process.Start(cmdOpenDumpFile.Text);
        }

        private void cmdSubmit_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            txtSteps.Clear();
        }

        private void cmdViewPolicy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }
    }
}
