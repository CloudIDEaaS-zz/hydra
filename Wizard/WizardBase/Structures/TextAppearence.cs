using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Text;

namespace WizardBase
{
    [Editor(typeof(TextAppearenceEditor), typeof(UITypeEditor))]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [Serializable]
    public class TextAppearence : ICloneable
    {
        private Font font = new Font("Microsoft Sans", 8.25f, FontStyle.Bold, GraphicsUnit.Point);
        private Color textColor = Color.Black;
        private Color textShadowColor = Color.LightGray;
        private float xshift = 1.5f;
        private float yshift = 1.5f;

        public static bool operator ==(TextAppearence p1, TextAppearence p2)
        {
            if (ReferenceEquals(p1, null))
            {
                return ReferenceEquals(p2, null);
            }
            return p1.Equals(p2);

        }

        ///<summary>
        ///Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        ///</summary>
        ///
        ///<returns>
        ///A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        ///</returns>
        ///<filterpriority>2</filterpriority>
        public override string ToString()
        {
            return "TextAppearence";
        }

        ///<summary>
        ///Determines whether the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>.
        ///</summary>
        ///
        ///<returns>
        ///true if the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>; otherwise, false.
        ///</returns>
        ///
        ///<param name="obj">The <see cref="T:System.Object"></see> to compare with the current <see cref="T:System.Object"></see>. </param><filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            TextAppearence app = obj as TextAppearence;
            if (app != null)
            {
                return app.textColor.Equals(TextColor) && app.TextShadowColor.Equals(TextShadowColor) && app.xshift == xshift && app.yshift == yshift;
            }
            return false;
        }

        ///<summary>
        ///Serves as a hash function for a particular type. <see cref="M:System.Object.GetHashCode"></see> is suitable for use in hashing algorithms and data structures like a hash table.
        ///</summary>
        ///
        ///<returns>
        ///A hash code for the current <see cref="T:System.Object"></see>.
        ///</returns>
        ///<filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public static bool operator !=(TextAppearence p1, TextAppearence p2)
        {
            return !(p1 == p2);
        }

        public Color TextColor
        {
            get { return textColor; }
            set
            {
                if (textColor != value)
                {
                    textColor = value;
                    OnAppearenceChanged(new GenericEventArgs<bool>(true));
                }
            }
        }
        public Color TextShadowColor
        {
            get { return textShadowColor; }
            set
            {
                if (textShadowColor != value)
                {
                    textShadowColor = value;
                    OnAppearenceChanged(new GenericEventArgs<bool>(true));
                }
            }
        }

        public float Xshift
        {
            get { return xshift; }
            set
            {
                if (xshift != value)
                {
                    xshift = value;
                    OnAppearenceChanged(new GenericEventArgs<bool>(true));
                }
            }
        }
        public float Yshift
        {
            get { return yshift; }
            set
            {
                if (yshift != value)
                {
                    yshift = value;
                    OnAppearenceChanged(new GenericEventArgs<bool>(true));
                }
            }
        }
        [Editor(typeof(FontEditor), typeof(UITypeEditor))]
        public Font Font
        {
            get { return font; }
            set
            {
                if (!font.Equals(value))
                {
                    font = value;
                    OnAppearenceChanged(new GenericEventArgs<bool>(true));
                }
            }
        }

        #region ICloneable Members

        ///<summary>
        ///Creates a new object that is a copy of the current instance.
        ///</summary>
        ///
        ///<returns>
        ///A new object that is a copy of this instance.
        ///</returns>
        ///<filterpriority>2</filterpriority>
        public object Clone()
        {
            TextAppearence appearence = new TextAppearence();
            appearence.TextColor = TextColor;
            appearence.TextShadowColor = TextShadowColor;
            appearence.Xshift = Xshift;
            appearence.font = (Font) font.Clone();
            appearence.Yshift = Yshift;
            return appearence;
        }

        #endregion

        public event GenericEventHandler<bool > AppearenceChanged;

        protected virtual void OnAppearenceChanged(GenericEventArgs<bool> e)
        {
            if (AppearenceChanged != null)
            {
                AppearenceChanged(this, e);
            }
        }

        internal void Reset()
        {
            ResetFont();
            ResetTextColor();
            ResetTextShadowColor();
            ResetXshift();
            ResetYshift();
        }

        public virtual bool DefaultChanged()
        {
            return ShouldSerializeTextColor() || ShouldSerializeTextShadowColor() && ShouldSerializeXshift() && ShouldSerializeYshift() && ShouldSerializeFont();
        }

        private bool ShouldSerializeFont()
        {
            return font != new Font("Microsoft Sans", 8.25f, FontStyle.Bold, GraphicsUnit.Point);
        }

        private bool ShouldSerializeTextColor()
        {
            return textColor != Color.Black;
        }

        private bool ShouldSerializeXshift()
        {
            return Xshift != 1.5f;
        }

        private bool ShouldSerializeYshift()
        {
            return Yshift != 1.5f;
        }

        private bool ShouldSerializeTextShadowColor()
        {
            return textShadowColor != Color.LightGray;
        }

        private void ResetTextColor()
        {
            textColor = Color.Black;
        }

        private void ResetTextShadowColor()
        {
            textShadowColor = Color.LightGray;
        }

        private void ResetXshift()
        {
            Xshift = 1.5f;
        }

        private void ResetYshift()
        {
            Yshift = 1.5f;
        }

        private void ResetFont()
        {
            Font = new Font("Microsoft Sans", 8.25f, FontStyle.Bold, GraphicsUnit.Point);
        }

        #region Nested type: TextAppearenceEditor

        internal class TextAppearenceEditor : UITypeEditor
        {
            ///<summary>
            ///Indicates whether the specified context supports painting a representation of an object's value within the specified context.
            ///</summary>
            ///
            ///<returns>
            ///true if <see cref="M:System.Drawing.Design.UITypeEditor.PaintValue(System.Object,System.Drawing.Graphics,System.Drawing.Rectangle)"></see> is implemented; otherwise, false.
            ///</returns>
            ///
            ///<param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that can be used to gain additional context information. </param>
            public override bool GetPaintValueSupported(ITypeDescriptorContext context)
            {
                return true;
            }

            public override void PaintValue(PaintValueEventArgs e)
            {
                base.PaintValue(e);
                TextAppearence app = e.Value as TextAppearence;
                if (app != null)
                {
                    e.Graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
                    StringFormat format = new StringFormat();
                    format.Trimming = StringTrimming.EllipsisCharacter;
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Center;
                    SolidBrush brush = new SolidBrush(app.textShadowColor);
                    Font font = new Font("Microsoft Sans Serif", 8f);
                    RectangleF rect = e.Bounds;
                    e.Graphics.DrawString("ab", font, brush, rect, format);
                    brush = new SolidBrush(app.textColor);
                    rect.X -= 1.5f;
                    rect.Y -= 1.5f;
                    e.Graphics.DrawString("ab", font, brush, rect, format);
                    brush.Dispose();
                }
            }
        }

        #endregion
    }
}
