using System;
using System.Windows.Forms;
using WizardBase;

namespace WizardDemo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void demoWizard_FinishButtonClick(object sender, EventArgs e)
        {
            MessageBox.Show("Finishing the wizard.");
            Close();
        }

        private void demoWizard_CancelButtonClick(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to exit.",  Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            {
                Close();
            }
        }

        private void wizardControl1_EulaButtonClick(object sender, EventArgs e)
        {
            MessageBox.Show("Eula link clicked.");
        }

   }
}