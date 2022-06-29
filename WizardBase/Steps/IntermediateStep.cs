using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using WizardBase.Properties;

namespace WizardBase
{
    [ToolboxItem(false), Designer(typeof (WizardStepDesigner)), DefaultEvent("Click")]
    [Serializable]
    public class IntermediateStep : WizardStep
    {
        #region Private Fields

        private ColorPair headerPair = new ColorPair();
        private Image bindingImage = Resources.Top;
        private string subtitle = "Description for the new step.";
        TextAppearence subtitleAppearence = new TextAppearence();
        private string title = "New WizardControl step.";
        private TextAppearence titleAppearence = new TextAppearence();

        #endregion

        #region Constructor

        public IntermediateStep()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
#pragma warning disable DoNotCallOverridableMethodsInConstructor
            Reset();
#pragma warning restore DoNotCallOverridableMethodsInConstructor
        }

        #endregion

        #region Virtual Methods

        protected virtual void GetTextBounds(out RectangleF titleRect, out RectangleF subtitleRect, Graphics graphics)
        {
            StringFormat format = new StringFormat(StringFormatFlags.FitBlackBox);
            format.Trimming = StringTrimming.EllipsisCharacter;
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;
            format.Trimming = StringTrimming.None;
            SizeF sz = graphics.MeasureString(Title, titleAppearence.Font, Width, format);
            titleRect = new RectangleF(subtitleAppearence.Font.SizeInPoints, subtitleAppearence.Font.SizeInPoints, sz.Width, sz.Height);
            SizeF sz1 = graphics.MeasureString(Subtitle, subtitleAppearence.Font, Width, format);
            subtitleRect = new RectangleF(2 * subtitleAppearence.Font.SizeInPoints, titleRect.Height + subtitleAppearence.Font.SizeInPoints, sz1.Width, sz1.Height);
        }

        #endregion

        #region Private Methods

        protected void GetTextBounds(out RectangleF titleRect, out RectangleF subtitleRect)
        {
            Graphics graphics = CreateGraphics();
            try
            {
                GetTextBounds(out titleRect, out subtitleRect, graphics);
            }
            finally
            {
                if (graphics != null)
                {
                    graphics.Dispose();
                }
            }
        }

        protected Region GetTextBounds()
        {
            RectangleF titleRect;
            RectangleF subtitleRect;
            GetTextBounds(out titleRect, out subtitleRect);
            return GetTextBounds(titleRect, subtitleRect);
        }

        protected Region GetTextBounds(RectangleF titleRect, RectangleF subtitleRectangle)
        {
            if (!titleRect.IsEmpty)
            {
                if (!subtitleRectangle.IsEmpty)
                {
                    return new Region(new RectangleF(6f, Width - 12, (Width - 66), (6f + titleRect.Height) + subtitleRectangle.Height));
                }
                else
                {
                    return new Region(titleRect);
                }
            }
            else
            {
                if (!subtitleRectangle.IsEmpty)
                {
                    return new Region(subtitleRectangle);
                }
                return new Region(RectangleF.Empty);
            }
        }

        #endregion

        #region Override

        ///<summary>
        ///Raises the <see cref="E:System.Windows.Forms.Control.Paint"></see> event.
        ///</summary>
        ///
        ///<param name="e">A <see cref="T:System.Windows.Forms.PaintEventArgs"></see> that contains the event data. </param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics graphics = e.Graphics;
            Rectangle rect = HeaderRectangle;
            Rectangle rectangle;
            RectangleF titleRect;
            RectangleF subtitleRect;
            GetTextBounds(out titleRect, out subtitleRect);
            if (bindingImage != null)
            {
                graphics.DrawImage(bindingImage, rect);
                rectangle = new Rectangle(rect.Left, rect.Bottom, rect.Width, 2);
                ControlPaint.DrawBorder3D(graphics, rectangle);
            }
            else
            {
                using (Brush brush = new LinearGradientBrush(rect, headerPair.BackColor1, headerPair.BackColor2, headerPair.Gradient))
                {
                    graphics.FillRectangle(brush, rect);
                    rectangle = new Rectangle(rect.Left, rect.Bottom, rect.Width, 2);
                    ControlPaint.DrawBorder3D(graphics, rectangle);
                }
            }
            DrawText(graphics, titleRect, title, titleAppearence);
            DrawText(graphics, subtitleRect, subtitle, subtitleAppearence);
        }

        internal override void Reset()
        {
            ResetHeaderPair();
            ResetBindingImage();
            ResetBackColor();
            ResetBindingImage();
            ResetTitleAppearence();
            ResetSubtitleAppearence();
            BackgroundImage = null;
            ForeColor = SystemColors.ControlText;
            Title = "New Wizard step.";
            Subtitle = "Description for the new step.";
        }

        #endregion

        #region Public Property

        [Description("The title text appearence of step."), Category("Appearance")]
        public TextAppearence TitleAppearence
        {
            get { return titleAppearence; }
            set
            {
                if (titleAppearence != value)
                {
                    titleAppearence = value;
                    Invalidate();
                }
            }
        }

        [Description("The sub title appearence of step."), Category("Appearance")]
        public TextAppearence SubtitleAppearence
        {
            get { return subtitleAppearence; }
            set
            {
                if (subtitleAppearence != value)
                {
                    subtitleAppearence = value;
                    Invalidate();
                }
            }
        }

        [Description("The background image of the panel."), Category("Appearance")]
        public Image BindingImage
        {
            get { return bindingImage; }
            set
            {
                if (value != bindingImage)
                {
                    bindingImage = value;
                    Invalidate();
                    OnBindingImageChanged();
                }
            }
        }

        protected virtual Rectangle HeaderRectangle
        {
            get { return new Rectangle(0, 0, Width, 60); }
        }

        [Category("Appearance"), DefaultValue("Description for the new step."), Description("The subtitle of the step."), Editor(typeof (MultilineStringEditor), typeof (UITypeEditor))]
        public string Subtitle
        {
            get { return subtitle; }
            set
            {
                if (!string.IsNullOrEmpty(subtitle) && value != subtitle)
                {
                    Region refreshRegion = GetTextBounds();
                    subtitle = value;
                    refreshRegion.Union(GetTextBounds());
                    Invalidate(refreshRegion);
                }
            }
        }

        [Description("The title of the step."), DefaultValue("New Wizard step."), Editor(typeof (MultilineStringEditor), typeof (UITypeEditor)), Category("Appearance")]
        public string Title
        {
            get { return title; }
            set
            {
                if (!string.IsNullOrEmpty(title) && value != title)
                {
                    Region refreshRegion = GetTextBounds();
                    title = value;
                    refreshRegion.Union(GetTextBounds());
                    Invalidate(refreshRegion);
                }
            }
        }

        [Description("Appearence of header."), Category("Appearance")]
        public ColorPair HeaderPair
        {
            get { return headerPair; }
            set
            {
                if (value != headerPair)
                {
                    headerPair = value;
                    Invalidate(HeaderRectangle);
                }
            }
        }

        #endregion

        #region Should Serialize implementation

        protected virtual bool ShouldSerializeSubtitleAppearence()
        {
            TextAppearence sa = new TextAppearence();
            sa.Font = new Font("Microsoft Sans", 8.25f, GraphicsUnit.Point);
            return SubtitleAppearence != sa;
        }

        protected virtual bool ShouldSerializeTitleAppearence()
        {
            return TitleAppearence.DefaultChanged();
        }

        protected virtual bool ShouldSerializeBindingImage()
        {
            return bindingImage != Resources.Top;
        }

        protected virtual bool ShouldSerializeHeaderPair()
        {
            return headerPair != new ColorPair();
        }

        #endregion

        protected virtual void ResetTitleAppearence()
        {
            titleAppearence = new TextAppearence();
        }

        protected virtual void ResetHeaderPair()
        {
            headerPair = new ColorPair();
        }

        protected virtual void ResetSubtitleAppearence()
        {
            subtitleAppearence = new TextAppearence();
            subtitleAppearence.Font = new Font("Microsoft Sans", 8.25f, GraphicsUnit.Point);
        }

        protected virtual void ResetBindingImage()
        {
            bindingImage = Resources.Top;
        }

    }
}