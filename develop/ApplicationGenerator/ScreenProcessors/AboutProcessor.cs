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

    public class AboutProcessor : IPreviousScreenProcessor
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

        public AboutProcessor()
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

                    switch (y)
                    {
                        case 0:
                            namedBlob = new NamedBlob("AboutBanner", blob);
                            break;
                        case 324:
                            namedBlob = new NamedBlob("BottomDivider", blob);
                            break;
                        default:
                            DebugUtils.Break();
                            break;
                    }

                    break;
                case 8:
                    namedBlob = new NamedBlob("BottomTabButtons", blob);
                    break;
                case 9:
                    namedBlob = new NamedBlob("AboutArea", blob);
                    break;
                case 10:
                    namedBlob = new NamedBlob("AboutTitle", blob);
                    break;
                case 13:
                    namedBlob = new NamedBlob("MenuIcon", blob);
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
        ///

        public void DrawParts(Graphics graphics, Rectangle boundingRect)
        {
            var backgroundColor = ColorTranslator.FromHtml(this.ResourceData.BackgroundColor);

            foreach (var blob in this.NamedBlobs)
            {
                switch (blob.Name)
                {
                    case "AboutBanner":
                        {
                            var color = Color.FromArgb(210, 210, 210);
                            var menuColor = ColorTranslator.FromHtml("#424242");
                            var xOffset = blob.Blob.Rectangle.Width - 15;
                            var aboutBanner = (string)this.ResourceData.AboutBanner;
                            Bitmap image;

                            if (aboutBanner != null)
                            {
                                if (!this.ImageCollection.ContainsKey(aboutBanner))
                                {
                                    using (var imageOriginal = (Bitmap)Bitmap.FromFile(aboutBanner))
                                    {
                                        image = imageOriginal.Clone(new Rectangle(0, 0, imageOriginal.Width, imageOriginal.Height), imageOriginal.PixelFormat);
                                    }

                                    this.ImageCollection.Add(aboutBanner, image);
                                }

                                image = this.ImageCollection[aboutBanner];
                            }
                            else
                            {
                                image = this.ImageCollection["AboutBanner"];
                            }

                            blob.DrawBackground(graphics, boundingRect, backgroundColor);
                            blob.DrawImage(graphics, boundingRect, image);

                            color = backgroundColor;

                            blob.DrawHorzontalLine(graphics, boundingRect, color, new Point(11, 14), 12, 1);
                            blob.DrawHorzontalLine(graphics, boundingRect, color, new Point(11, 17), 12, 1);
                            blob.DrawHorzontalLine(graphics, boundingRect, color, new Point(11, 20), 12, 1);

                            blob.DrawFilledEllipse(graphics, boundingRect, new Rectangle(xOffset, 12, 2, 2), color);
                            blob.DrawFilledEllipse(graphics, boundingRect, new Rectangle(xOffset, 16, 2, 2), color);
                            blob.DrawFilledEllipse(graphics, boundingRect, new Rectangle(xOffset, 20, 2, 2), color);

                            blob.DrawHorzontalLine(graphics, boundingRect, color, blob.Blob.Rectangle.Height - 1);
                        }
                        break;
                    case "AboutArea":
                        {
                            var textColor = ColorTranslator.FromHtml("#424242");
                            var dividerColor = Color.FromArgb(240, 240, 240);
                            var height = blob.Blob.Rectangle.Height;
                            var width = blob.Blob.Rectangle.Width;
                            var fontFamily = this.FontCollection.Families.Single(f => f.Name == "Roboto Light");
                            var aboutText = "The Hydra Conference has been the global stage for innovation. Hydra 2021 will continue to be a platform to launch products, engage with global brands and define the future of the tech industry.";
                            int remainingHeight = 0;
                            int textAreaHeight = 0;
                            int lineHeight = 0;

                            using (var font = new Font(fontFamily, 6, FontStyle.Regular))
                            {
                                var size = new Size(width, 0);

                                if (graphics.GetTextExtentPoint(font, aboutText, ref size))
                                {
                                    double rows;

                                    lineHeight = size.Height + 2;
                                    rows = Math.Round((float)((size.Width) / (width - 15)), 0);
                                    
                                    textAreaHeight = (int)(lineHeight * rows);
                                    remainingHeight = (int)(height - textAreaHeight);
                                }
                            }


                            blob.DrawBackground(graphics, boundingRect, backgroundColor);
                            blob.DrawText(graphics, boundingRect, fontFamily, aboutText, textColor, backgroundColor, 6f, FontStyle.Regular, StringAlignment.Near);

                            if (remainingHeight > 0)
                            {
                                var x = 0;
                                var currentLocation = textAreaHeight + lineHeight;
                                var mediumfontFamily = this.FontCollection.Families.Single(f => f.Name == "Roboto");
                                var textColorBold = Color.Black;

                                while (currentLocation < height)
                                {
                                    blob.DrawHorzontalLine(graphics, boundingRect, dividerColor, currentLocation);

                                    switch (x)
                                    {
                                        case 0:
                                            {
                                                blob.DrawText(graphics, boundingRect, fontFamily, "Version 2.7", textColorBold, new Point(0, currentLocation + 5), 6f, FontStyle.Regular, StringAlignment.Near);
                                                break;
                                            }
                                        case 1:
                                            {
                                                blob.DrawText(graphics, boundingRect, fontFamily, "Advertise with us", textColorBold, new Point(0, currentLocation + 5), 6f, FontStyle.Regular, StringAlignment.Near);
                                                break;
                                            }
                                        case 2:
                                            {
                                                blob.DrawText(graphics, boundingRect, fontFamily, "Connect with us", textColorBold, new Point(0, currentLocation + 5), 6f, FontStyle.Regular, StringAlignment.Near);
                                                break;
                                            }
                                        case 3:
                                            {
                                                var bitmap = this.ImageCollection["ContactUsIcon"];

                                                blob.DrawImage(graphics, boundingRect, bitmap, 0, currentLocation + 6);
                                                blob.DrawText(graphics, boundingRect, fontFamily, "Contact us", textColor, new Point(14, currentLocation + 5), 6f, FontStyle.Regular, StringAlignment.Near);

                                                break;
                                            }
                                        case 4:
                                            {
                                                var bitmap = this.ImageCollection["YoutubeIcon"];

                                                blob.DrawImage(graphics, boundingRect, bitmap, 0, currentLocation + 6);
                                                blob.DrawText(graphics, boundingRect, fontFamily, "Watch us on Youtube", textColor, new Point(14, currentLocation + 5), 6f, FontStyle.Regular, StringAlignment.Near);
                                                break;
                                            }
                                        case 5:
                                            {
                                                var bitmap = this.ImageCollection["TwitterIcon"];

                                                blob.DrawImage(graphics, boundingRect, bitmap, 0, currentLocation + 6);
                                                blob.DrawText(graphics, boundingRect, fontFamily, "Follow us on Twitter", textColor, new Point(14, currentLocation + 5), 6f, FontStyle.Regular, StringAlignment.Near);
                                                break;
                                            }
                                        case 6:
                                            {
                                                var bitmap = this.ImageCollection["FacebookIcon"];

                                                blob.DrawImage(graphics, boundingRect, bitmap, 0, currentLocation + 6);
                                                blob.DrawText(graphics, boundingRect, fontFamily, "Like us on Facebook", textColor, new Point(14, currentLocation + 5), 6f, FontStyle.Regular, StringAlignment.Near);
                                                break;
                                            }
                                        case 7:
                                            {
                                                var bitmap = this.ImageCollection["PlayStoreIcon"];

                                                blob.DrawImage(graphics, boundingRect, bitmap, 0, currentLocation + 6);
                                                blob.DrawText(graphics, boundingRect, fontFamily, "Rate us on Play Store", textColor, new Point(14, currentLocation + 5), 6f, FontStyle.Regular, StringAlignment.Near);
                                                break;
                                            }
                                    }

                                    currentLocation += 18;

                                    x++;
                                }
                            }
                        }
                        break;
                    case "AboutTitle":
                        {
                            var color = ColorTranslator.FromHtml("#424242");
                            var fontFamily = this.FontCollection.Families.Single(f => f.Name == "Roboto Medium");

                            blob.DrawText(graphics, boundingRect, fontFamily, "About", color, backgroundColor, 10f, FontStyle.Regular, StringAlignment.Near);
                        }
                        break;
                    case "BottomTabButtons":
                        {
                            var colorActive = ColorTranslator.FromHtml(this.ResourceData.PrimaryColor);
                            var colorInactive = Color.FromArgb(100, 0, 0, 0);
                            var fontFamily = this.FontCollection.Families.Single(f => f.Name == "Roboto Medium");

                            blob.DrawBackground(graphics, boundingRect, backgroundColor);

                            blob.DrawText(graphics, boundingRect, fontFamily, "Schedule", colorInactive, 0, 7f);
                            blob.DrawText(graphics, boundingRect, fontFamily, "Speakers", colorInactive, 55, 7f);
                            blob.DrawText(graphics, boundingRect, fontFamily, "Map", colorInactive, 115, 7f);
                            blob.DrawText(graphics, boundingRect, fontFamily, "About", colorActive, 150, 7f);
                        }
                        break;
                    case "BottomDivider":
                        {
                            var color = Color.FromArgb(210, 210, 210);

                            blob.DrawBackground(graphics, boundingRect, backgroundColor);
                            blob.DrawHorzontalLine(graphics, boundingRect, color);
                        }
                        break;
                    default:
                        DebugUtils.Break();
                        break;
                }
            }
        }

        /// <summary>   Draw previous screen. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/30/2020. </remarks>
        ///
        /// <param name="previousScreenProcessor">  The previous screen processor. </param>
        /// <param name="graphics">                 The graphics. </param>
        /// <param name="boundingRect">             The bounding rectangle. </param>
        /// <param name="previousScreenPainter">    The previous screen painter. </param>

        public void DrawPreviousScreen(IScreenProcessor previousScreenProcessor, Graphics graphics, Rectangle boundingRect, ScreenPainter previousScreenPainter)
        {
        }
    }
}
