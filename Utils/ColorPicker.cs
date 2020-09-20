using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Collections.Specialized;
using System.Collections;

namespace Utils
{
    public partial class ColorPicker : ComboBox
    {
        public OrderedDictionary Colors { get; private set; }

        public ColorPicker()
        {
            var colorType = typeof(System.Drawing.Color);
            var properties = colorType.GetProperties(BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.Public);

            this.Colors = new OrderedDictionary();

            foreach (PropertyInfo property in properties)
            {
                this.Colors.Add(property.Name, Color.FromName(property.Name));
            }

            this.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.DrawItem += new DrawItemEventHandler(ColorPicker_DrawItem);
        }

        private void ColorPicker_DrawItem(object sender, DrawItemEventArgs e)
        {
            var graphics = e.Graphics;
            var rect = e.Bounds;

            if (e.Index >= 0 && e.Index < this.Items.Count)
            {
                var colorText = this.Items[e.Index].ToString();
                var color = (Color) this.Colors[colorText];
                var brush = new SolidBrush(color);

                graphics.FillRectangle(new SolidBrush(Color.White), rect.X, rect.Y, rect.Width, rect.Height);
                graphics.DrawString(colorText, this.Font, Brushes.Black, rect.X, rect.Top);
                graphics.FillRectangle(brush, rect.X + 110, rect.Y + 5, rect.Width - 10, rect.Height - 10);
            }
        }

        protected override void OnCreateControl()
        {
            foreach (DictionaryEntry pair in this.Colors)
            {
                this.Items.Add(pair.Key);
            }

            base.OnCreateControl();
        }
    }
}
