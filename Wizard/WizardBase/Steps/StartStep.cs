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
    [Designer(typeof (WizardStepDesigner)), ToolboxItem(false), DefaultEvent("Click")]
    [Serializable]
    public class StartStep : WizardStep
    {
        private Image iconImage;
        private ColorPair leftPair = new ColorPair(ColorTranslator.FromHtml("#f1b3ad"), Color.White, 270);
        private Image bindingImage;
        private string subtitle = "Enter a brief description of the wizard here.";
        private TextAppearence subtitleAppearence = new TextAppearence();
        private string title = "Welcome to the DemoWizard.";
        private TextAppearence titleAppearence = new TextAppearence();

        public StartStep()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
#pragma warning disable DoNotCallOverridableMethodsInConstructor
            BackColor = SystemColors.ControlLightLight;
            Icon = Resources.icon;
            BindingImage = Resources.left;
            ResetSubtitleAppearence();
            ResetTitleAppearence();
            ResetLeftPair();
            leftPair.AppearenceChanged += leftPair_AppearenceChanged;
#pragma warning restore DoNotCallOverridableMethodsInConstructor
        }

        private void leftPair_AppearenceChanged(object sender, GenericEventArgs<bool> tArgs)
        {
            Invalidate();
        }

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

        protected virtual void GetTextBounds(out RectangleF titleRect, out RectangleF subtitleRect, Graphics graphics)
        {
            StringFormat format = new StringFormat(StringFormatFlags.FitBlackBox);
            format.Trimming = StringTrimming.EllipsisCharacter;
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;
            format.Trimming = StringTrimming.None;
            if (bindingImage != null)
            {
                SizeF sz = graphics.MeasureString(Title, titleAppearence.Font, Width - bindingImage.Width, format);
                titleRect = new RectangleF(bindingImage.Width + subtitleAppearence.Font.SizeInPoints, subtitleAppearence.Font.SizeInPoints, sz.Width, sz.Height);
                SizeF sz1 = graphics.MeasureString(Subtitle, subtitleAppearence.Font, Width - bindingImage.Width, format);
                subtitleRect = new RectangleF(bindingImage.Width + subtitleAppearence.Font.SizeInPoints, titleRect.Height + subtitleAppearence.Font.SizeInPoints, sz1.Width, sz1.Height);
            }
            else
            {
                SizeF sz = graphics.MeasureString(Title, titleAppearence.Font, Width - LeftRectangle.Width, format);
                titleRect = new RectangleF(LeftRectangle.Width + subtitleAppearence.Font.SizeInPoints, subtitleAppearence.Font.SizeInPoints, sz.Width, sz.Height);
                SizeF sz1 = graphics.MeasureString(Subtitle, subtitleAppearence.Font, Width - LeftRectangle.Width, format);
                subtitleRect = new RectangleF(LeftRectangle.Width + subtitleAppearence.Font.SizeInPoints, titleRect.Height + subtitleAppearence.Font.SizeInPoints, sz1.Width, sz1.Height);
            }
        }

        protected Region GetTextBounds()
        {
            RectangleF titleRect;
            RectangleF subtitleRect;
            GetTextBounds(out titleRect, out subtitleRect);
            return GetTextBounds(titleRect, subtitleRect);
        }

        protected Region GetTextBounds(RectangleF titleRect, RectangleF subtitleRect)
        {
            if (titleRect.IsEmpty)
            {
                if (!subtitleRect.IsEmpty)
                {
                    return new Region(subtitleRect);
                }
                return new Region(RectangleF.Empty);
            }
            else
            {
                if (!subtitleRect.IsEmpty)
                {
                    return new Region(new RectangleF(172f, 8f, (Width - 180), (8f + titleRect.Height) + subtitleRect.Height));
                }
                return new Region(titleRect);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Rectangle rect;
            Rectangle iconRect;
            RectangleF titleRect;
            RectangleF subtitleRect;
            base.OnPaint(e);
            Graphics graphics = e.Graphics;
            rect = LeftRectangle;
            GetTextBounds(out titleRect, out subtitleRect);
            if (bindingImage != null)
            {
                graphics.DrawImage(bindingImage, rect);
                iconRect = IconRectangle;
                iconRect.Inflate(-1, -1);
                if (iconImage != null)
                {
                    graphics.DrawImage(iconImage, iconRect);
                }
            }
            else
            {
                using (Brush brush = new LinearGradientBrush(rect, leftPair.BackColor1, leftPair.BackColor2, leftPair.Gradient))
                {
                    graphics.FillRectangle(brush, rect);
                    iconRect = IconRectangle;
                    iconRect.Inflate(-1, -1);
                    if (iconImage != null)
                    {
                        graphics.DrawImage(iconImage, iconRect);
                    }
                }
            }
            DrawText(graphics, titleRect, title, titleAppearence);
            DrawText(graphics, subtitleRect, subtitle, subtitleAppearence);
        }

        internal override void Reset()
        {
            ResetLeftPair();
            ResetBindingImage();
            ResetIcon();
            BackColor = SystemColors.ControlLightLight;
            BackgroundImage = null;
            BackgroundImageLayout = ImageLayout.Tile;
            ForeColor = SystemColors.ControlText;
            Title = "Welcome to the DemoWizard.";
            Subtitle = "Enter a brief description of the wizard here.";
        }

        [DefaultValue(typeof (Color), "ControlLightLight")]
        public override Color BackColor
        {
            get { return base.BackColor; }
            set { base.BackColor = value; }
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
        [Description("The title font of step."), Category("Appearance")]
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

        [Description("The icon image of the step."), Category("Appearance")]
        public virtual Image Icon
        {
            get { return iconImage; }
            set
            {
                if (value != iconImage)
                {
                    iconImage = value;
                    Invalidate();
                }
            }
        }

        protected virtual Rectangle IconRectangle
        {
            get { return new Rectangle(104, 12, 48, 48); }
        }

        [Description("The back color appearence of the left panel."), Category("Appearance")]
        public virtual ColorPair LeftPair
        {
            get { return leftPair; }
            set
            {
                if (leftPair == value)
                {
                    return;
                }
                leftPair = value;
                Invalidate(LeftRectangle);
            }
        }

        [Category("Appearance"), Description("The background image of the panel.")]
        public virtual Image BindingImage
        {
            get { return bindingImage; }
            set
            {
                if (value != bindingImage)
                {
                    bindingImage = value;
                    OnBindingImageChanged();
                    Invalidate();
                }
            }
        }

        protected virtual Rectangle LeftRectangle
        {
            get { return new Rectangle(0, 0, 160, Height); }
        }

        [Category("Appearance"), DefaultValue("Enter a brief description of the wizard here."), Editor(typeof(MultilineStringEditor), typeof(UITypeEditor)), Description("The subtitle of the step.")]
        public virtual string Subtitle
        {
            get { return subtitle; }
            set
            {
                if (subtitle == value)
                {
                    return;
                }
                Region refreshRegion = GetTextBounds();
                subtitle = value;
                refreshRegion.Union(GetTextBounds());
                Invalidate(refreshRegion);
                return;
            }
        }

        [DefaultValue("Welcome to the DemoWizard."), Description("The title of the step."), Category("Appearance"), Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public virtual string Title
        {
            get { return title; }
            set
            {
                if (title == value)
                {
                    return;
                }
                Region refreshRegion = GetTextBounds();
                title = value;
                refreshRegion.Union(GetTextBounds());
                Invalidate(refreshRegion);
            }
        }

        protected virtual bool ShouldSerializeSubtitleAppearence()
        {
            TextAppearence sa = new TextAppearence();
            sa.Font = new Font("Microsoft Sans", 8.25f, GraphicsUnit.Point);
            return SubtitleAppearence != sa;
        }

        protected virtual bool ShouldSerializeTitleAppearence()
        {
            TextAppearence ta = new TextAppearence();
            ta.Font = new Font("Verdana", 12f, FontStyle.Bold, GraphicsUnit.Point);
            return TitleAppearence != ta;
        }

        protected virtual void ResetTitleAppearence()
        {
            titleAppearence = new TextAppearence();
            titleAppearence.Font = new Font("Verdana", 12f, FontStyle.Bold, GraphicsUnit.Point);
        }

        protected virtual void ResetSubtitleAppearence()
        {
            subtitleAppearence = new TextAppearence();
            subtitleAppearence.Font = new Font("Microsoft Sans", 8.25f, GraphicsUnit.Point);
        }
        protected virtual bool ShouldSerializeLeftPair()
        {
            ColorPair pa = new ColorPair(Color.Orange, Color.White, 270);
            return leftPair != pa;
        }

        private void ResetLeftPair()
        {
            leftPair = new ColorPair(Color.Orange, Color.White, 270);
        }

        protected virtual bool ShouldSerializeBindingImage()
        {
            return BindingImage != Resources.left;
        }

        private void ResetBindingImage()
        {
            BindingImage = Resources.left;
        }

        protected virtual bool ShouldSerializeIcon()
        {
            return Icon != Resources.icon;
        }

        private void ResetIcon()
        {
            Icon = Resources.icon;
        }
    }
}