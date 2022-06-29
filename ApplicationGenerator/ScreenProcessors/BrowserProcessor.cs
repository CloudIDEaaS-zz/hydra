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
    /// <summary>   A browser processor. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 12/10/2020. </remarks>

    public class BrowserProcessor : IScreenProcessor
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

        public BrowserProcessor()
        {
            this.NamedBlobs = new List<NamedBlob>();
        }

        /// <summary>   Check specified blob and decide if should be kept or not. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/10/2020. </remarks>
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
                case 1:
                    namedBlob = new NamedBlob("Body", blob);
                    break;
                case 16:
                    namedBlob = new NamedBlob("Icon", blob);
                    break;
                case 34:
                    namedBlob = new NamedBlob("Title", blob);
                    break;
                case 135:
                    namedBlob = new NamedBlob("Url", blob);
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

        /// <summary>   Draw parts. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/10/2020. </remarks>
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
                    case "Title":
                        {
                            var color = ColorTranslator.FromHtml("#666666");
                            var backColor = ColorTranslator.FromHtml("#eeeeee");
                            var fontFamily = this.FontCollection.Families.Single(f => f.Name == "Roboto Medium");
                            var appName = (string)this.ResourceData.AppName;

                            if (appName.IsNullOrEmpty())
                            {
                                appName = "Hydra Conference";
                            }

                            blob.DrawText(graphics, boundingRect, fontFamily, appName, color, backColor, 8f, FontStyle.Regular, StringAlignment.Near);
                        }
                        break;

                    case "Url":
                        {
                            var color = ColorTranslator.FromHtml("#666666");
                            var backColor = ColorTranslator.FromHtml("#c8c9ca");
                            var fontFamily = this.FontCollection.Families.Single(f => f.Name == "Roboto Medium");
                            var url = (string)this.ResourceData.AppName;

                            if (url.IsNullOrEmpty())
                            {
                                url = "HydraConference";
                            }

                            url = "https://www." + url.RemoveSpaces() + ".com";

                            blob.DrawText(graphics, boundingRect, fontFamily, url, color, backColor, 6.5f, FontStyle.Regular, StringAlignment.Near);
                        }
                        break;

                    case "Body":
                        {
                            var logo = (string)this.ResourceData.Logo;
                            var lineColor = Color.FromArgb(210, 210, 210);
                            var textColor = ColorTranslator.FromHtml("#424242");
                            var fontFamily = this.FontCollection.Families.Single(f => f.Name == "Roboto Medium");
                            var mapImage = this.ImageCollection["MapBrowser"];
                            Bitmap logoImage;

                            if (logo != null)
                            {
                                if (!this.ImageCollection.ContainsKey(logo))
                                {
                                    using (var imageOriginal = (Bitmap)Bitmap.FromFile(logo))
                                    {
                                        logoImage = imageOriginal.Clone(new Rectangle(0, 0, imageOriginal.Width, imageOriginal.Height), imageOriginal.PixelFormat);
                                    }

                                    this.ImageCollection.Add(logo, logoImage);
                                }

                                logoImage = this.ImageCollection[logo];
                            }
                            else
                            {
                                logoImage = this.ImageCollection["HydraLogo"];
                            }

                            blob.DrawBackground(graphics, boundingRect, backgroundColor);
                            blob.DrawImage(graphics, boundingRect, new Size(30, 30), logoImage, 390, 5);
                            blob.DrawText(graphics, boundingRect, fontFamily, "Map", textColor, new Point(10, 10), 10f, FontStyle.Regular, StringAlignment.Near);

                            blob.DrawHorzontalShadowLine(graphics, boundingRect, lineColor, 2, 40);

                            blob.DrawImage(graphics, boundingRect, mapImage, 0, 43);
                        }
                        break;

                    case "Icon":
                        {
                            var icon = (string)this.ResourceData.Icon;
                            Bitmap image;

                            if (icon != null)
                            {
                                if (!this.ImageCollection.ContainsKey(icon))
                                {
                                    using (var imageOriginal = (Bitmap)Bitmap.FromFile(icon))
                                    {
                                        image = imageOriginal.Clone(new Rectangle(0, 0, imageOriginal.Width, imageOriginal.Height), imageOriginal.PixelFormat);
                                    }

                                    this.ImageCollection.Add(icon, image);
                                }

                                image = this.ImageCollection[icon];
                            }
                            else
                            {
                                image = this.ImageCollection["HydraIcon"];
                            }

                            blob.DrawBackground(graphics, boundingRect, backgroundColor);
                            blob.DrawImage(graphics, boundingRect, image);
                        }
                        break;
                }
            }
        }
    }
}
