using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Utils
{
    public partial class TipWindow : Form
    {
        public int AnimationDelay { get; set; } = 1000;

        public TipWindow()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            this.FadeIn(this.AnimationDelay);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            this.FadeOut(this.AnimationDelay);
        }
    }
}
