using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows;
using System.Drawing;

namespace Utils
{
    public partial class TextEdit : Control
    {
        private Caret caret;
        private TEXTMETRIC tm;
        private int nCharX;
        private int nCharY;
        private ushort nWindowX;
        private ushort nWindowY;
        private int nWindowCharsX;
        private int nWindowCharsY;
        private int nCaretPosX;
        private int nCaretPosY;
        private StringBuilder builder;

        public TextEdit()
        {
            builder = new StringBuilder();
            this.Text = string.Empty;

            SetStyle(ControlStyles.Selectable, true);

            InitializeComponent();
        }

        public override string Text
        {
            get
            {
                return builder.ToString();
            }
        }

        protected override void WndProc(ref Message m)
        {
            var message = (Utils.ControlExtensions.WindowsMessage)m.Msg;
            Graphics graphics;
            Keys key;
            System.Drawing.SizeF size;
            System.Drawing.Point point;
            RectangleF rect;
            SolidBrush brush;
            Brush fontBrush;
            StringFormat stringFormat;

            switch (message)
            {
                case ControlExtensions.WindowsMessage.CREATE:

                    using (graphics = this.CreateGraphics())
                    {
                        tm = graphics.GetTextMetrics(this.Font);
                    }

                    nCharX = tm.tmAveCharWidth; 
                    nCharY = tm.tmHeight;
                    this.BackColor = SystemColors.Window;

                    builder = new StringBuilder();

                    break;

                case ControlExtensions.WindowsMessage.SIZE:

                    var loHigh = m.LParam.ToLowHiWord();

                    nWindowX = loHigh.Low;
                    nWindowCharsX = Math.Max(1, nWindowX / nCharX); 

                    nWindowY = loHigh.High;
                    nWindowCharsY = Math.Max(1, nWindowY/nCharY);

                    if (caret != null)
                    {
                        caret.Position = new System.Drawing.Point(0, 0);
                    }

                    break;

                case ControlExtensions.WindowsMessage.KEYDOWN:

                    key = (Keys)m.WParam;

                    switch (key)
                    {
                        case Keys.Home:
                            nCaretPosX = 0; 
                            break;
                        case Keys.End:
                            nCaretPosX = nWindowCharsX - 1; 
                            break;
                        case Keys.PageUp:
                            nCaretPosY = 0; 
                            break;
                        case Keys.PageDown:
                            nCaretPosY = nWindowCharsY - 1; 
                            break;
                        case Keys.Left:
                            nCaretPosX = Math.Max(nCaretPosX - 1, 0); 
                            break;
                        case Keys.Right:
                            nCaretPosX = Math.Min(nCaretPosX + 1, nWindowCharsX - 1); 
                            break;
                        case Keys.Up:
                            nCaretPosY = Math.Max(nCaretPosY - 1, 0); 
                            break;
                        case Keys.Down:
                            nCaretPosY = Math.Min(nCaretPosY + 1, nWindowCharsY - 1); 
                            break;
                        case Keys.Delete:

                            if (builder.Length > 0)
                            {
                                builder.RemoveEnd(1);

                                caret.Hide();

                                using (graphics = this.CreateGraphics())
                                {
                                    brush = new SolidBrush(this.BackColor);
                                    fontBrush = SystemBrushes.WindowText;
                                    stringFormat = new StringFormat(StringFormatFlags.MeasureTrailingSpaces);

                                    size = graphics.MeasureString(this.Text, this.Font, int.MaxValue, stringFormat);
                                    rect = new RectangleF(0, 0, size.Width, size.Height);
                                    point = new System.Drawing.Point((int)rect.Right, (int)rect.Top);

                                    graphics.FillRectangle(brush, this.ClientRectangle);

                                    graphics.DrawString(this.Text, this.Font, fontBrush, rect.Location);

                                    brush.Dispose();
                                }

                                caret.Position = point;
                                caret.Show();
                            }

                            return;
                    }

                    break;

                case ControlExtensions.WindowsMessage.CHAR:

                    var ch = (char)m.WParam;

                    key = ch.ToKey();

                    switch (key)
                    {
                        case Keys.Back:

                            this.SendMessage(ControlExtensions.WindowsMessage.KEYDOWN, (int) Keys.Delete, 1L);
                            break;

                        case Keys.Tab:

                                do 
                                {
                                    this.SendMessage(ControlExtensions.WindowsMessage.CHAR, ' ', 1L); 
                                } 
                                while (nCaretPosX % 4 != 0); 

                            break;

                        case Keys.Return:
                   
                            nCaretPosX = 0;

                            if (++nCaretPosY == nWindowCharsY)
                            {
                                nCaretPosY = 0;
                            }

                            break;

                        case Keys.Escape:
                        case Keys.LineFeed:

                            this.Beep(ControlExtensions.BeepType.OK);
                            break;

                        default:

                            builder.Append(key.ToAscii());

                            caret.Hide();

                            using (graphics = this.CreateGraphics())
                            {
                                brush = new SolidBrush(this.BackColor);
                                fontBrush = SystemBrushes.WindowText;
                                stringFormat = new StringFormat(StringFormatFlags.MeasureTrailingSpaces);

                                size = graphics.MeasureString(this.Text, this.Font, int.MaxValue, stringFormat);
                                rect = new RectangleF(0, 0, size.Width, size.Height);
                                point = new System.Drawing.Point((int)rect.Right, (int)rect.Top);

                                graphics.FillRectangle(brush, this.ClientRectangle);

                                graphics.DrawString(this.Text, this.Font, fontBrush, rect.Location);

                                brush.Dispose();
                            }

                            caret.Position = point;
                            caret.Show();

                            break;
                    }

                    break;

                case ControlExtensions.WindowsMessage.PAINT:

                    break;

                case ControlExtensions.WindowsMessage.SETFOCUS:

                    caret = new Caret(this, 0, this.Height);
                    caret.Position = new System.Drawing.Point(0, 0);
                    caret.Show();

                    break;

                case ControlExtensions.WindowsMessage.KILLFOCUS:

                    if (caret != null)
                    {
                        caret.Dispose();
                    }

                    break;
            }

            base.WndProc(ref m);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            using (var graphics = this.CreateGraphics())
            {
                var brush = new SolidBrush(this.BackColor);
                var fontBrush = SystemBrushes.WindowText;
                var stringFormat = new StringFormat(StringFormatFlags.MeasureTrailingSpaces);
                var size = graphics.MeasureString(this.Text, this.Font, int.MaxValue, stringFormat);
                var rect = new RectangleF(0, 0, size.Width, size.Height);
                var point = new System.Drawing.Point((int)rect.Right, (int)rect.Top);
                var clientRect = this.ClientRectangle;

                graphics.FillRectangle(brush, clientRect);
                graphics.DrawString(this.Text, this.Font, fontBrush, rect.Location);

                brush.Dispose();
            }

            base.OnPaint(e);
        }
    }
}
