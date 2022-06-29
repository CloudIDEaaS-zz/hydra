using AForge.Imaging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX.ScreenProcessors
{
    /// <summary>   A welcome processor. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 11/27/2020. </remarks>

    public class ScheduleProcessor : IScreenProcessor
    {
        /// <summary>   Gets or sets information describing the resource. </summary>
        ///
        /// <value> Information describing the resource. </value>

        public IResourceData ResourceData { get; set; }

        /// <summary>   Gets the named blobs. </summary>
        ///
        /// <value> The named blobs. </value>

        public List<NamedBlob> NamedBlobs { get; }

        /// <summary>   Gets or sets a collection of images. </summary>
        ///
        /// <value> A collection of images. </value>

        public Dictionary<string, Bitmap> ImageCollection { get; set; }

        /// <summary>   Gets or sets a collection of fonts. </summary>
        ///
        /// <value> A collection of fonts. </value>

        public PrivateFontCollection FontCollection { get; set; }

        /// <summary>   Default constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/28/2020. </remarks>

        public ScheduleProcessor()
        {
            this.NamedBlobs = new List<NamedBlob>();
        }

        /// <summary>   Check specified blob and decide if should be kept or not. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/28/2020. </remarks>
        ///
        /// <param name="blob"> Blob to check. </param>
        ///
        /// <returns>
        /// Return <see langword="true" /> if the blob should be kept or
        /// <see langword="false" /> if it should be removed.
        /// </returns>

        public bool Check(Blob blob)
        {
            var rect = blob.Rectangle;
            var x = rect.X;
            var y = rect.Y;
            NamedBlob namedBlob = null;

            switch (x)
            {
                case 0:
                    namedBlob = new NamedBlob("CalendarArea", blob);
                    break;
                case 46:
                    namedBlob = new NamedBlob("HeaderText", blob);
                    break;
                case 149:
                    namedBlob = new NamedBlob("SearchButton", blob);
                    break;
                case 179:
                    namedBlob = new NamedBlob("SlidersIcon", blob);
                    break;
                case 8:
                    namedBlob = new NamedBlob("BottomTabButtons", blob);
                    break;
                case 13:
                    namedBlob = new NamedBlob("MenuIcon", blob);
                    break;
                case 4:
                    namedBlob = new NamedBlob("TopMenu", blob);
                    break;
                default:
                    DebugUtils.Break();
                    break;
            }

            if (namedBlob != null)
            {
                this.NamedBlobs.Add(namedBlob);
            }

            return true;
        }

        /// <summary>   Draw part. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/27/2020. </remarks>
        ///
        /// <param name="graphics">     The graphics. </param>
        /// <param name="boundingRect"> The bounding rectangle. </param>

        public void DrawParts(Graphics graphics, Rectangle boundingRect)
        {
            var backgroundColor = ColorTranslator.FromHtml(this.ResourceData.BackgroundColor);

            foreach (var blob in this.NamedBlobs)
            {
                switch (blob.Name)
                {
                    case "CalendarArea":
                        {
                            DrawCalendarArea(graphics, boundingRect, blob);
                        }
                        break;
                    case "HeaderText":
                        {
                            var color = ColorTranslator.FromHtml("#424242");
                            var fontFamily = this.FontCollection.Families.Single(f => f.Name == "Roboto Medium");

                            blob.DrawText(graphics, boundingRect, fontFamily, "Schedule", color, backgroundColor, 10f);
                        }
                        break;
                    case "SearchButton":
                        {
                            var color = ColorTranslator.FromHtml("#424242");

                            blob.DrawBackground(graphics, boundingRect, backgroundColor);
                            blob.DrawEllipse(graphics, boundingRect, new Rectangle(0, 0, 9, 9), color, 1);
                            blob.DrawLineNWSE(graphics, boundingRect, new Rectangle(8, 8, 4, 4), color, 1);
                        }
                        break;
                    case "SlidersIcon":
                        {
                            var color = ColorTranslator.FromHtml("#424242");

                            blob.DrawBackground(graphics, boundingRect, backgroundColor);

                            blob.DrawHorzontalLine(graphics, boundingRect, color, 1);
                            blob.DrawHorzontalLine(graphics, boundingRect, color, 5);
                            blob.DrawHorzontalLine(graphics, boundingRect, color, 9);

                            blob.DrawFilledEllipse(graphics, boundingRect, new Rectangle(9, 0, 2, 2), color);
                            blob.DrawFilledEllipse(graphics, boundingRect, new Rectangle(3, 4, 2, 2), color);
                            blob.DrawFilledEllipse(graphics, boundingRect, new Rectangle(9, 8, 2, 2), color);
                        }
                        break;
                    case "BottomTabButtons":
                        {
                            var colorActive = ColorTranslator.FromHtml(this.ResourceData.PrimaryColor);
                            var colorInactive = Color.FromArgb(100, 0, 0, 0);
                            var fontFamily = this.FontCollection.Families.Single(f => f.Name == "Roboto Medium");

                            blob.DrawBackground(graphics, boundingRect, backgroundColor);

                            blob.DrawText(graphics, boundingRect, fontFamily, "Schedule", colorActive, 0, 7f);
                            blob.DrawText(graphics, boundingRect, fontFamily, "Speakers", colorInactive, 55, 7f);
                            blob.DrawText(graphics, boundingRect, fontFamily, "Map", colorInactive, 115, 7f);
                            blob.DrawText(graphics, boundingRect, fontFamily, "About", colorInactive, 150, 7f);
                        }
                        break;
                    case "MenuIcon":
                        {
                            var color = ColorTranslator.FromHtml("#424242");

                            blob.DrawBackground(graphics, boundingRect, backgroundColor);

                            blob.DrawHorzontalLine(graphics, boundingRect, color, 0);
                            blob.DrawHorzontalLine(graphics, boundingRect, color, 3);
                            blob.DrawHorzontalLine(graphics, boundingRect, color, 6);
                        }
                        break;
                    case "TopMenu":
                        {
                            var colorActive = ColorTranslator.FromHtml(this.ResourceData.PrimaryColor);
                            var colorInactive = Color.FromArgb(100, 0, 0, 0);
                            var fontFamily = this.FontCollection.Families.Single(f => f.Name == "Roboto Medium");

                            blob.DrawBackground(graphics, boundingRect, backgroundColor);

                            blob.DrawText(graphics, boundingRect, fontFamily, "ALL", colorActive, 40, 7f);
                            blob.DrawText(graphics, boundingRect, fontFamily, "FAVORITES", colorInactive, 110, 7f);
                        }
                        break;
                    default:
                        DebugUtils.Break();
                        break;
                }
            }
        }

        private void DrawCalendarArea(Graphics graphics, Rectangle boundingRect, NamedBlob blob)
        {
            var colorSelected = ColorTranslator.FromHtml(this.ResourceData.PrimaryColor);
            var fontFamily = this.FontCollection.Families.Single(f => f.Name == "Roboto Light");
            var colorInactive = Color.FromArgb(100, 0, 0, 0);
            var colorActiveText = Color.Black;
            var backgroundColor = ColorTranslator.FromHtml(this.ResourceData.BackgroundColor);
            var color = Color.FromArgb(210, 210, 210);

            blob.DrawBackground(graphics, boundingRect, backgroundColor);
            blob.DrawHorzontalLine(graphics, boundingRect, colorSelected, 0, 90, 2);
            
            blob.DrawHorzontalShadowLine(graphics, boundingRect, color, 3, 2);

            blob.DrawHorzontalLine(graphics, boundingRect, color, 31, 1);
            blob.DrawText(graphics, boundingRect, fontFamily, "8:00 am", colorInactive, new Point(12, 15), 7f);
            blob.DrawVerticalLine(graphics, boundingRect, ColorTranslator.FromHtml("#58cfcc"), new Point(13, 36), 27, 2);
            blob.DrawText(graphics, boundingRect, fontFamily, "Breakfast", colorActiveText, new Point(17, 38), 7f);
            blob.DrawText(graphics, boundingRect, fontFamily, "8:00 am - 9:00 am: Dining Hall", colorInactive, new Point(17, 53), 7f);

            blob.DrawHorzontalLine(graphics, boundingRect, color, new Point(12, 72), 1);

            blob.DrawText(graphics, boundingRect, fontFamily, "9:15 am", colorInactive, new Point(12, 81), 7f);
            blob.DrawHorzontalLine(graphics, boundingRect, color, 97, 1);

            blob.DrawVerticalLine(graphics, boundingRect, ColorTranslator.FromHtml("#5593ff"), new Point(13, 105), 27, 2);
            blob.DrawText(graphics, boundingRect, fontFamily, "Getting Started with Hydra", colorActiveText, new Point(17, 106), 7f);
            blob.DrawText(graphics, boundingRect, fontFamily, "9:30 am - 9:45 am: Hall 2", colorInactive, new Point(17, 121), 7f);

            blob.DrawHorzontalLine(graphics, boundingRect, color, new Point(12, 138), 1);

            blob.DrawVerticalLine(graphics, boundingRect, ColorTranslator.FromHtml("#fe666b"), new Point(13, 146), 27, 2);
            blob.DrawText(graphics, boundingRect, fontFamily, "Code Generation - Automatic and Custom", colorActiveText, new Point(17, 147), 7f);
            blob.DrawText(graphics, boundingRect, fontFamily, "9:45 am - 10:00 am: Executive Ballroom", colorInactive, new Point(17, 162), 7f);

            blob.DrawHorzontalLine(graphics, boundingRect, color, new Point(12, 179), 1);

            blob.DrawVerticalLine(graphics, boundingRect, ColorTranslator.FromHtml("#5593ff"), new Point(13, 187), 27, 2);
            blob.DrawText(graphics, boundingRect, fontFamily, "Hydra Extensibility", colorActiveText, new Point(17, 188), 7f);
            blob.DrawText(graphics, boundingRect, fontFamily, "9:15 am - 9:30 am: Hall 3", colorInactive, new Point(17, 203), 7f);

            blob.DrawHorzontalLine(graphics, boundingRect, color, new Point(12, 219), 1);

            blob.DrawText(graphics, boundingRect, fontFamily, "10:00 am", colorInactive, new Point(12, 228), 7f);
            blob.DrawHorzontalLine(graphics, boundingRect, color, 245, 1);

            blob.DrawVerticalLine(graphics, boundingRect, ColorTranslator.FromHtml("#5593ff"), new Point(13, 253), 2, 2);

            color = Color.FromArgb(100, 210, 210, 210);

            blob.DrawHorzontalLine(graphics, boundingRect, color, 255, 1);

            blob.DrawRaisedEllipse(graphics, boundingRect, new Rectangle(155, 280, 38, 38), backgroundColor);

            color = ColorTranslator.FromHtml("#3880ff");
            blob.DrawLine(graphics, boundingRect, 170, 230, 178, 225, color, 1);
            blob.DrawLine(graphics, boundingRect, 170, 230, 178, 235, color, 1);

            blob.DrawFilledEllipse(graphics, boundingRect, new Rectangle(169, 229, 3, 3), color);
            blob.DrawFilledEllipse(graphics, boundingRect, new Rectangle(176, 224, 3, 3), color);
            blob.DrawFilledEllipse(graphics, boundingRect, new Rectangle(176, 233, 3, 3), color);
        }
    }
}
