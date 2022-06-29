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
    /// <summary>   A map processor. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 11/27/2020. </remarks>

    public class MapProcessor : IScreenProcessor
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

        public MapProcessor()
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
                    namedBlob = new NamedBlob("MapArea", blob);
                    break;
                case 46:
                    namedBlob = new NamedBlob("HeaderText", blob);
                    break;
                case 8:
                    namedBlob = new NamedBlob("BottomTabButtons", blob);
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

        public void DrawParts(Graphics graphics, Rectangle boundingRect)
        {
            var backgroundColor = ColorTranslator.FromHtml(this.ResourceData.BackgroundColor);

            foreach (var blob in this.NamedBlobs)
            {
                switch (blob.Name)
                {
                    case "MapArea":
                        {
                            var image = this.ImageCollection["Map"];
                            var color = Color.FromArgb(210, 210, 210);

                            blob.DrawImage(graphics, boundingRect, image);

                            blob.DrawHorzontalLine(graphics, boundingRect, color);
                            blob.DrawHorzontalLine(graphics, boundingRect, color, blob.Blob.Rectangle.Height - 1);
                        }
                        break;
                    case "HeaderText":
                        {
                            var color = ColorTranslator.FromHtml("#424242");
                            var fontFamily = this.FontCollection.Families.Single(f => f.Name == "Roboto Medium");

                            blob.DrawText(graphics, boundingRect, fontFamily, "Map", color, backgroundColor, 10f);
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
                            blob.DrawText(graphics, boundingRect, fontFamily, "Map", colorActive, 115, 7f);
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
                    default:
                        DebugUtils.Break();
                        break;
                }
            }
        }
    }
}
