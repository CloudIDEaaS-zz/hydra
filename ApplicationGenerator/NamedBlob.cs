// file:	NamedBlob.cs
//
// summary:	Implements the named BLOB class

using AForge.Imaging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX
{
    /// <summary>   A named blob. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 11/28/2020. </remarks>

    public class NamedBlob
    {
        /// <summary>   Gets the name. </summary>
        ///
        /// <value> The name. </value>

        public string Name { get; }

        /// <summary>   Gets the BLOB. </summary>
        ///
        /// <value> The BLOB. </value>

        public Blob Blob { get; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/28/2020. </remarks>
        ///
        /// <param name="name"> The name. </param>
        /// <param name="blob"> The BLOB. </param>

        public NamedBlob(string name, Blob blob)
        {
            Name = name;
            Blob = blob;
        }

        /// <summary>   Draw background. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/28/2020. </remarks>
        ///
        /// <param name="graphics">     The graphics. </param>
        /// <param name="boundingRect"> The bounding rectangle. </param>
        /// <param name="color">        The pen. </param>

        public void DrawBackground(Graphics graphics, Rectangle boundingRect, Color color)
        {
            var rect = this.Blob.Rectangle;
            using (var brush = new SolidBrush(color))
            {
                using (var pen = new Pen(color))
                {
                    rect.Width -= 1;
                    rect.Offset(boundingRect.X, boundingRect.Y);

                    graphics.DrawRectangle(pen, rect);
                    graphics.FillRectangle(brush, rect);
                }
            }
        }

        /// <summary>   Draw rounded rect. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/29/2020. </remarks>
        ///
        /// <param name="graphics">     The graphics. </param>
        /// <param name="boundingRect"> The bounding rectangle. </param>
        /// <param name="rect">         The rectangle. </param>
        /// <param name="color">        The pen. </param>
        /// <param name="cornerRadius"> The corner radius. </param>

        public void DrawRoundedRect(Graphics graphics, Rectangle boundingRect, Rectangle rect, Color color, int cornerRadius)
        {
            var blobRect = this.Blob.Rectangle;

            using (var brush = new SolidBrush(color))
            {
                using (var pen = new Pen(color))
                {
                    rect.Width -= 1;
                    rect.Offset(blobRect.X + boundingRect.X, blobRect.Y + boundingRect.Y);

                    graphics.SetClip(boundingRect);

                    graphics.DrawRoundedRectangle(pen, rect, cornerRadius);
                    graphics.FillRoundedRectangle(brush, rect, cornerRadius);

                    graphics.ResetClip();
                }
            }
        }

        /// <summary>   Draw vertical line. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/29/2020. </remarks>
        ///
        /// <param name="graphics">     The graphics. </param>
        /// <param name="boundingRect"> The bounding rectangle. </param>
        /// <param name="color">        The pen. </param>
        /// <param name="offset">       The offset. </param>
        /// <param name="width">        (Optional) The width. </param>

        public void DrawVerticalLine(Graphics graphics, Rectangle boundingRect, Color color, Point offset, int width = 1)
        {
            var rect = this.Blob.Rectangle;

            using (var pen = new Pen(color, width))
            {
                var xOffset = offset.X;
                var yOffset = offset.Y;

                rect.Offset(boundingRect.X, boundingRect.Y);

                graphics.DrawLine(pen, rect.X + xOffset, rect.Y + yOffset, rect.X + xOffset, rect.Y + rect.Height);
            }
        }

        /// <summary>   Draw vertical line. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/29/2020. </remarks>
        ///
        /// <param name="graphics">     The graphics. </param>
        /// <param name="boundingRect"> The bounding rectangle. </param>
        /// <param name="color">        The pen. </param>
        /// <param name="offset">       The offset. </param>
        /// <param name="lineLength">   Length of the line. </param>
        /// <param name="width">        (Optional) The width. </param>

        public void DrawVerticalLine(Graphics graphics, Rectangle boundingRect, Color color, Point offset, int lineLength, int width)
        {
            var rect = this.Blob.Rectangle;

            using (var pen = new Pen(color, width))
            {
                var xOffset = offset.X;
                var yOffset = offset.Y;

                rect.Offset(boundingRect.X, boundingRect.Y);

                graphics.DrawLine(pen, rect.X + xOffset, rect.Y + yOffset, rect.X + xOffset, rect.Y + yOffset + lineLength);
            }
        }

        /// <summary>   Draw horzontal line. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/28/2020. </remarks>
        ///
        /// <param name="graphics">     The graphics. </param>
        /// <param name="boundingRect"> The bounding rectangle. </param>
        /// <param name="color">        The pen. </param>
        /// <param name="yOffset">      The offset. </param>
        /// <param name="lineLength">   Length of the line. </param>
        /// <param name="width">        (Optional) The width. </param>

        public void DrawHorzontalLine(Graphics graphics, Rectangle boundingRect, Color color, int yOffset, int lineLength, int width = 1)
        {
            var rect = this.Blob.Rectangle;

            using (var pen = new Pen(color, width))
            {
                rect.Offset(boundingRect.X, boundingRect.Y);

                graphics.DrawLine(pen, rect.X, rect.Y, rect.X + lineLength, rect.Y);
            }
        }

        /// <summary>   Draw horzontal line. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/28/2020. </remarks>
        ///
        /// <param name="graphics">     The graphics. </param>
        /// <param name="boundingRect"> The bounding rectangle. </param>
        /// <param name="color">        The pen. </param>

        public void DrawHorzontalLine(Graphics graphics, Rectangle boundingRect, Color color)
        {
            var rect = this.Blob.Rectangle;

            using (var pen = new Pen(color))
            {
                rect.Offset(boundingRect.X, boundingRect.Y);

                graphics.DrawLine(pen, rect.X, rect.Y, rect.X + rect.Width - 1, rect.Y);
            }
        }

        /// <summary>   Draw horzontal line. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/29/2020. </remarks>
        ///
        /// <param name="graphics">     The graphics. </param>
        /// <param name="boundingRect"> The bounding rectangle. </param>
        /// <param name="color">        The pen. </param>
        /// <param name="offset">       The offset. </param>
        /// <param name="width">        (Optional) The width. </param>

        public void DrawHorzontalLine(Graphics graphics, Rectangle boundingRect, Color color, Point offset, int width = 1)
        {
            var rect = this.Blob.Rectangle;

            using (var pen = new Pen(color, width))
            {
                var xOffset = offset.X;
                var yOffset = offset.Y;

                rect.Offset(boundingRect.X, boundingRect.Y);

                graphics.DrawLine(pen, rect.X + xOffset, rect.Y + yOffset, rect.X + rect.Width, rect.Y + yOffset);
            }
        }

        /// <summary>   Draw horzontal line. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/30/2020. </remarks>
        ///
        /// <param name="graphics">     The graphics. </param>
        /// <param name="boundingRect"> The bounding rectangle. </param>
        /// <param name="color">        The pen. </param>
        /// <param name="offset">       The offset. </param>
        /// <param name="lineLength">   Length of the line. </param>
        /// <param name="width">        The width. </param>

        public void DrawHorzontalLine(Graphics graphics, Rectangle boundingRect, Color color, Point offset, int lineLength, int width)
        {
            var rect = this.Blob.Rectangle;

            using (var pen = new Pen(color, width))
            {
                var xOffset = offset.X;
                var yOffset = offset.Y;

                rect.Offset(boundingRect.X, boundingRect.Y);

                graphics.DrawLine(pen, rect.X + xOffset, rect.Y + yOffset, rect.X + xOffset + lineLength, rect.Y + yOffset);
            }
        }

        /// <summary>   Draw horzontal line. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/28/2020. </remarks>
        ///
        /// <param name="graphics">     The graphics. </param>
        /// <param name="boundingRect"> The bounding rectangle. </param>
        /// <param name="color">        The pen. </param>
        /// <param name="yOffset">      The offset. </param>
        /// <param name="width">        (Optional) The width. </param>

        public void DrawHorzontalLine(Graphics graphics, Rectangle boundingRect, Color color, int yOffset, int width = 1)
        {
            var rect = this.Blob.Rectangle;

            using (var pen = new Pen(color, width))
            {
                rect.Offset(boundingRect.X, boundingRect.Y);

                graphics.DrawLine(pen, rect.X, rect.Y + yOffset, rect.X + rect.Width - 1, rect.Y + yOffset);
            }
        }

        /// <summary>   Draw vertical line. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/30/2020. </remarks>
        ///
        /// <param name="graphics">     The graphics. </param>
        /// <param name="boundingRect"> The bounding rectangle. </param>
        /// <param name="color">        The pen. </param>
        /// <param name="xOffset">      (Optional) The offset. </param>
        /// <param name="width">        (Optional) The width. </param>

        public void DrawVerticalLine(Graphics graphics, Rectangle boundingRect, Color color, int xOffset = 0, int width = 1)
        {
            var rect = this.Blob.Rectangle;

            using (var pen = new Pen(color, width))
            {
                rect.Offset(boundingRect.X, boundingRect.Y);

                graphics.DrawLine(pen, rect.X + xOffset, rect.Y, rect.X + xOffset, rect.Y + rect.Height - 1);
            }
        }

        /// <summary>   Draw horzontal shadow line. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/28/2020. </remarks>
        ///
        /// <param name="graphics">     The graphics. </param>
        /// <param name="boundingRect"> The bounding rectangle. </param>
        /// <param name="startColor">   The start color. </param>
        /// <param name="height">       The height. </param>
        /// <param name="yOffset">      (Optional) The offset. </param>

        public void DrawHorzontalShadowLine(Graphics graphics, Rectangle boundingRect, Color startColor, int height, int yOffset = 0)
        {
            var rect = this.Blob.Rectangle;
            var color = startColor;

            rect.Offset(boundingRect.X, boundingRect.Y);

            for (var x = 0; x < height; x++)
            {
                using (var pen = new Pen(color))
                {
                    graphics.DrawLine(pen, rect.X, rect.Y + x + yOffset, rect.X + rect.Width - 1, rect.Y + x + yOffset);

                    color = color.Lighten(.3);
                }
            }
        }

        /// <summary>   Draw vertical shadow line. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/30/2020. </remarks>
        ///
        /// <param name="graphics">     The graphics. </param>
        /// <param name="boundingRect"> The bounding rectangle. </param>
        /// <param name="startColor">   The start color. </param>
        /// <param name="width">       The height. </param>
        /// <param name="xOffset">      (Optional) The offset. </param>

        public void DrawVerticalShadowLine(Graphics graphics, Rectangle boundingRect, Color startColor, int width, int xOffset = 0)
        {
            var rect = this.Blob.Rectangle;
            var color = startColor;

            rect.Offset(boundingRect.X, boundingRect.Y);

            for (var x = 0; x < width; x++)
            {
                using (var pen = new Pen(color))
                {
                    graphics.DrawLine(pen, rect.X + x + xOffset, rect.Y, rect.X + x + xOffset, rect.Y + rect.Height - 1);

                    color = color.Lighten(.2);
                }
            }
        }

        /// <summary>   Draw line nwse. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/28/2020. </remarks>
        ///
        /// <param name="graphics">     The graphics. </param>
        /// <param name="boundingRect"> The bounding rectangle. </param>
        /// <param name="rect">         The rectangle. </param>
        /// <param name="color">        The pen. </param>
        /// <param name="width">        (Optional) The width. </param>

        public void DrawLineNWSE(Graphics graphics, Rectangle boundingRect, Rectangle rect, Color color, int width = 1)
        {
            using (var pen = new Pen(color, width))
            {
                var blobRect = this.Blob.Rectangle;

                rect.Offset(blobRect.X + boundingRect.X, blobRect.Y + boundingRect.Y);

                graphics.DrawLine(pen, rect.X, rect.Y, rect.X + rect.Width, rect.Y + rect.Height);
            }
        }

        /// <summary>   Draw line. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/29/2020. </remarks>
        ///
        /// <param name="graphics">     The graphics. </param>
        /// <param name="boundingRect"> The bounding rectangle. </param>
        /// <param name="x1">           The first x value. </param>
        /// <param name="y1">           The first y value. </param>
        /// <param name="x2">           The second x value. </param>
        /// <param name="y2">           The second y value. </param>
        /// <param name="color">        The pen. </param>
        /// <param name="width">        (Optional) The width. </param>

        public void DrawLine(Graphics graphics, Rectangle boundingRect, int x1, int y1, int x2, int y2, Color color, int width = 1)
        {
            using (var pen = new Pen(color, width))
            {
                var blobRect = this.Blob.Rectangle;
                var point1 = new Point(x1, y1);
                var point2 = new Point(x2, y2);

                point1.Offset(blobRect.X + boundingRect.X, blobRect.Y + boundingRect.Y);
                point2.Offset(blobRect.X + boundingRect.X, blobRect.Y + boundingRect.Y);

                graphics.DrawLine(pen, point1, point2);
            }
        }

        /// <summary>   Draw ellipse. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/28/2020. </remarks>
        ///
        /// <param name="graphics">     The graphics. </param>
        /// <param name="boundingRect"> The bounding rectangle. </param>
        /// <param name="rect">         The rectangle. </param>
        /// <param name="color">        The pen. </param>
        /// <param name="width">        (Optional) The width. </param>

        public void DrawEllipse(Graphics graphics, Rectangle boundingRect, Rectangle rect, Color color, int width = 1)
        {
            using (var pen = new Pen(color, width))
            {
                var blobRect = this.Blob.Rectangle;

                rect.Offset(blobRect.X + boundingRect.X, blobRect.Y + boundingRect.Y);

                graphics.DrawEllipse(pen, rect.X, rect.Y, rect.Width, rect.Height);
            }
        }

        /// <summary>   Fill ellipse. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/28/2020. </remarks>
        ///
        /// <param name="graphics">     The graphics. </param>
        /// <param name="boundingRect"> The bounding rectangle. </param>
        /// <param name="rect">         The rectangle. </param>
        /// <param name="color">        The pen. </param>

        public void DrawFilledEllipse(Graphics graphics, Rectangle boundingRect, Rectangle rect, Color color)
        {
            using (var pen = new Pen(color, 1))
            {
                using (var brush = new SolidBrush(color))
                {
                    var blobRect = this.Blob.Rectangle;

                    rect.Offset(blobRect.X + boundingRect.X, blobRect.Y + boundingRect.Y);

                    graphics.DrawEllipse(pen, rect.X, rect.Y, rect.Width, rect.Height);
                    graphics.FillEllipse(brush, rect.X, rect.Y, rect.Width, rect.Height);
                }
            }
        }

        /// <summary>   Draw text. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/28/2020. </remarks>
        ///
        /// <param name="graphics">         The graphics. </param>
        /// <param name="boundingRect">     The bounding rectangle. </param>
        /// <param name="fontFamily">       The font family. </param>
        /// <param name="text">             The text. </param>
        /// <param name="textColor"></param>
        /// <param name="backColor"></param>
        /// <param name="emSize">           (Optional) Size of the em. </param>
        /// <param name="fontStyle">        (Optional) The font style. </param>
        /// <param name="stringAlignment">  (Optional) The string alignment. </param>

        public void DrawText(Graphics graphics, Rectangle boundingRect, FontFamily fontFamily, string text, Color textColor, Color backColor, float emSize = 1, FontStyle fontStyle = FontStyle.Regular, StringAlignment stringAlignment = StringAlignment.Center)
        {
            var rect = this.Blob.Rectangle;

            using (var textBrush = new SolidBrush(textColor))
            {
                using (var backBrush = new SolidBrush(backColor))
                {
                    using (var font = new Font(fontFamily, emSize, fontStyle))
                    { 
                        Rectangle backgroundRect;

                        rect.Offset(boundingRect.X, boundingRect.Y);
                        backgroundRect = rect;
                        backgroundRect.Inflate(1, 1);

                        graphics.FillRectangle(backBrush, backgroundRect);

                        graphics.DrawString(text, font, textBrush, rect, stringAlignment);
                    }
                }
            }
        }

        /// <summary>   Draw text. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/28/2020. </remarks>
        ///
        /// <param name="graphics">         The graphics. </param>
        /// <param name="boundingRect">     The bounding rectangle. </param>
        /// <param name="fontFamily">       The font family. </param>
        /// <param name="text">             The text. </param>
        /// <param name="textColor">        . </param>
        /// <param name="xOffset">          The offset. </param>
        /// <param name="emSize">           (Optional) Size of the em. </param>
        /// <param name="fontStyle">        (Optional) The font style. </param>
        /// <param name="stringAlignment">  (Optional) The string alignment. </param>

        public void DrawText(Graphics graphics, Rectangle boundingRect, FontFamily fontFamily, string text, Color textColor, int xOffset, float emSize = 1, FontStyle fontStyle = FontStyle.Regular, StringAlignment stringAlignment = StringAlignment.Near)
        {
            var rect = this.Blob.Rectangle;

            using (var textBrush = new SolidBrush(textColor))
            {
                using (var font = new Font(fontFamily, emSize, fontStyle))
                {
                    rect.Offset(boundingRect.X, boundingRect.Y);

                    rect.Offset(xOffset, 0);
                    graphics.DrawString(text, font, textBrush, rect, stringAlignment);
                }
            }
        }

        /// <summary>   Draw text. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/29/2020. </remarks>
        ///
        /// <param name="graphics">         The graphics. </param>
        /// <param name="boundingRect">     The bounding rectangle. </param>
        /// <param name="fontFamily">       The font family. </param>
        /// <param name="text">             The text. </param>
        /// <param name="textColor">        . </param>
        /// <param name="offset">           The offset. </param>
        /// <param name="emSize">           (Optional) Size of the em. </param>
        /// <param name="fontStyle">        (Optional) The font style. </param>
        /// <param name="stringAlignment">  (Optional) The string alignment. </param>

        public void DrawText(Graphics graphics, Rectangle boundingRect, FontFamily fontFamily, string text, Color textColor, Point offset, float emSize = 1, FontStyle fontStyle = FontStyle.Regular, StringAlignment stringAlignment = StringAlignment.Near)
        {
            var rect = this.Blob.Rectangle;

            using (var textBrush = new SolidBrush(textColor))
            {
                var font = new Font(fontFamily, emSize, fontStyle);
                var xOffset = offset.X;
                var yOffset = offset.Y;

                rect.Offset(boundingRect.X, boundingRect.Y);

                rect.Offset(xOffset, yOffset);
                graphics.DrawString(text, font, textBrush, rect, stringAlignment);
            }
        }

        /// <summary>   Draw image. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/28/2020. </remarks>
        ///
        /// <param name="graphics">     The graphics. </param>
        /// <param name="boundingRect"> The bounding rectangle. </param>
        /// <param name="size">         The size. </param>
        /// <param name="image">        The image. </param>
        /// <param name="xOffset">      The offset. </param>
        /// <param name="yOffset">      The offset. </param>

        public void DrawImage(Graphics graphics, Rectangle boundingRect, Size size, Bitmap image, int xOffset, int yOffset)
        {
            var rect = this.Blob.Rectangle;

            rect.Offset(boundingRect.X + xOffset, boundingRect.Y + yOffset);
            rect.Size = size;

            graphics.DrawImage(image, rect);
        }

        /// <summary>   Draw image. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/28/2020. </remarks>
        ///
        /// <param name="graphics">     The graphics. </param>
        /// <param name="boundingRect"> The bounding rectangle. </param>
        /// <param name="image">        The image. </param>

        public void DrawImage(Graphics graphics, Rectangle boundingRect, Bitmap image)
        {
            var rect = this.Blob.Rectangle;

            rect.Offset(boundingRect.X, boundingRect.Y);

            graphics.DrawImage(image, rect);
        }

        /// <summary>   Draw image. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/29/2020. </remarks>
        ///
        /// <param name="graphics">     The graphics. </param>
        /// <param name="boundingRect"> The bounding rectangle. </param>
        /// <param name="image">        The image. </param>
        /// <param name="xOffset">      The offset. </param>
        /// <param name="yOffset">      The offset. </param>

        public void DrawImage(Graphics graphics, Rectangle boundingRect, Bitmap image, int xOffset, int yOffset)
        {
            var rect = new Rectangle(0, 0, image.Width, image.Height);
            var blobRect = this.Blob.Rectangle;

            rect.Offset(blobRect.X + boundingRect.X + xOffset, blobRect.Y + boundingRect.Y + yOffset);

            graphics.DrawImage(image, rect);
        }

        /// <summary>   Draw raised circle. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/29/2020. </remarks>
        ///
        /// <param name="graphics">     The graphics. </param>
        /// <param name="boundingRect"> The bounding rectangle. </param>
        /// <param name="rect">         The rectangle. </param>
        /// <param name="color">        The pen. </param>

        public void DrawRaisedEllipse(Graphics graphics, Rectangle boundingRect, Rectangle rect, Color color)
        {
            Rectangle haloRect;
            var shadow = Color.FromArgb(50, 16, 16, 16);

            rect.Offset(boundingRect.X, boundingRect.Y);

            haloRect = rect;

            for (int i = 0; i < 5; i++)
            {
                using (var brush = new SolidBrush(Color.FromArgb(20 - i * 5, shadow)))
                {
                    graphics.FillEllipse(brush, haloRect.X, haloRect.Y, haloRect.Width, haloRect.Height);
                    haloRect.Inflate(i * 2, i * 2);
                }
            }

            for (int i = 0; i < 8; i++)
            {
                using (var brush = new SolidBrush(Color.FromArgb(20 - i * 2, shadow)))
                {
                    graphics.FillEllipse(brush, rect.X, rect.Y + i, rect.Width, rect.Height);
                }
            }

            using (var brush = new SolidBrush(color))
            {
                graphics.FillEllipse(brush, rect.X, rect.Y, rect.Width, rect.Height);
            }

            //graphics.TranslateTransform(80, 0);

            //for (int i = 0; i < 8; i++)
            //{
            //    using (var pen = new Pen(Color.FromArgb(80 - i * 10, shadow), 2.5f))
            //    {
            //        graphics.DrawEllipse(pen, rect.X + i * 1.25f, rect.Y + i, 60, 60);
            //    }
            //}

            //using (Pen pen = new Pen(color))
            //{
            //    graphics.DrawEllipse(pen, rect.X, rect.Y, 60, 60);
            //}
        }
    }
}
