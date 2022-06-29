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

namespace Utils
{
    public partial class frmFloatingProgressBar : Form
    {
        private Form frmParent;

        public frmFloatingProgressBar()
        {
            InitializeComponent();
        }

        public frmFloatingProgressBar(Form frmParent = null)
        {
            this.frmParent = frmParent;

            InitializeComponent();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                const int CS_DROPSHADOW = 0x20000;
                const int WS_EX_NOACTIVATE = 0x08000000;
                const int WS_EX_TOOLWINDOW = 0x00000080;

                CreateParams cp = base.CreateParams;

                cp.ClassStyle |= CS_DROPSHADOW;
                cp.ExStyle |= (int)(WS_EX_NOACTIVATE | WS_EX_TOOLWINDOW);

                return cp;
            }
        }

        public string Message
        {
            get
            {
                return lblMessage.Text;
            }

            set
            {
                lblMessage.Text = value;
            }
        }

        public Image Image
        {
            get
            {
                return pictureBox.Image;
            }

            set
            {
                pictureBox.Image = value;
                pictureBox.Visible = true;
            }
        }

        public int Progress
        {
            get
            {
                return progressBar.Value;
            }

            set
            {
                progressBar.Value = value;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var rect = this.ClientRectangle;
            var graphics = e.Graphics;

            graphics.DrawBorder(Pens.LightGray, this);

            base.OnPaint(e);
        }

        private void frmFloatingMessage_Load(object sender, EventArgs e)
        {
            if (frmParent != null)
            {
                this.CenterOver(frmParent, new Point(0, -100));
            }
            else
            {
                var secondaryScreen = Screen.AllScreens.SingleOrDefault(s => s != Screen.PrimaryScreen);

                this.CenterOver(secondaryScreen);
            }
        }

        public void ShowTemporarily(int milliseconds)
        {
            this.Show();

            this.DelayInvoke(milliseconds, () =>
            {
                this.Close();
            });
        }
    }
}
