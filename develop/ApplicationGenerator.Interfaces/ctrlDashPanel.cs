// file:	ctrlDashPanel.cs
//
// summary:	Implements the control dash panel class

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
    /// <summary>   Panel for editing the control dash. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 4/4/2021. </remarks>

    public partial class ctrlDashPanel : Panel
    {
        /// <summary>   True if lighted. </summary>
        private bool lighted;
        /// <summary>   The lit background color. </summary>
        private Color litBackgroundColor;
        private int radius;

        /// <summary>   Default constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 4/4/2021. </remarks>

        public ctrlDashPanel()
        {

            InitializeComponent();
        }

        /// <summary>   Gets or sets the color of the lit background. </summary>
        ///
        /// <value> The color of the lit background. </value>

        public Color LitBackgroundColor
        {
            get
            {
                return litBackgroundColor;
            }

            set
            {
                litBackgroundColor = value;
                this.Refresh();
            }
        }

        /// <summary>   Gets or sets the radius. </summary>
        ///
        /// <value> The radius. </value>

        public int Radius 
        {
            get
            {
                return radius;
            }

            set
            {
                radius = value;
                this.Refresh();
            }
        }

        private void UpdateCorners()
        {
            this.AddRoundedCorners(10);
        }

        /// <summary>   Gets or sets a value indicating whether the lighted. </summary>
        ///
        /// <value> True if lighted, false if not. </value>

        public bool Lighted
        {
            get
            {
                return lighted;
            }

            set
            {
                lighted = value;
                this.Refresh();
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.SizeChanged" /> event.
        /// </summary>
        ///
        /// <remarks>   CloudIDEaaS, 4/4/2021. </remarks>
        ///
        /// <param name="e">    An <see cref="T:System.EventArgs" /> that contains the event data. </param>

        protected override void OnSizeChanged(EventArgs e)
        {
            UpdateCorners();

            base.OnSizeChanged(e);
        }

        /// <summary>   Paints the background of the control. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 4/4/2021. </remarks>
        ///
        /// <param name="e">    A <see cref="T:System.Windows.Forms.PaintEventArgs" /> that contains the
        ///                     event data. </param>

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            var clipRect = this.ClientRectangle;
            var specularityRect = this.ClientRectangle;

            base.OnPaintBackground(e);

            using (var brush = new SolidBrush(Color.Transparent))
            {
                e.Graphics.FillRectangle(brush, this.ClientRectangle);
            }

            if (this.lighted)
            {
                var rect = this.ClientRectangle;

                rect.Inflate(-4, -3);

                using (var brush = new SolidBrush(litBackgroundColor))
                {
                    e.Graphics.FillRectangle(brush, rect);
                }
            }

            clipRect.Y = this.ClientRectangle.Height - 10;
            specularityRect.Inflate(1, -3);

            e.Graphics.SetClip(clipRect);

            using (var pen = new Pen(Color.LightGray, 2))
            {
                e.Graphics.DrawRoundedRectangle(pen, specularityRect, radius);
            }

            e.Graphics.ResetClip();
        }
    }
}
