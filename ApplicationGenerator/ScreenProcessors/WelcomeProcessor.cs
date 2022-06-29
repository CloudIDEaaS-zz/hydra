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

    public class WelcomeProcessor : IScreenProcessor
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

        public WelcomeProcessor()
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
                    namedBlob = new NamedBlob("Line", blob);
                    break;
                case 34:
                    namedBlob = new NamedBlob("Description", blob);
                    break;
                case 30:
                    namedBlob = new NamedBlob("Copyright", blob);
                    break;
                case 16:
                    namedBlob = new NamedBlob("Name", blob);
                    break;
                case 8:
                    namedBlob = new NamedBlob("SplashScreen", blob);
                    break;
                case 172:
                    namedBlob = new NamedBlob("Skip", blob);
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
                    case "Line":
                        {
                            var color = Color.FromArgb(210, 210, 210);

                            blob.DrawHorzontalShadowLine(graphics, boundingRect, color, 3);
                        }
                        break;
                    case "Name":
                        {
                            var color = Color.Black;
                            var fontFamily = this.FontCollection.Families.Single(f => f.Name == "Roboto Light");
                            var appName = (string)this.ResourceData.AppName;

                            if (appName.IsNullOrEmpty())
                            {
                                appName = "Hydra Conference";
                            }

                            blob.DrawText(graphics, boundingRect, fontFamily, appName, color, backgroundColor, 11f);
                        }
                        break;
                    case "Description":
                        {
                            var color = Color.Black;
                            var fontFamily = this.FontCollection.Families.Single(f => f.Name == "Roboto Light");
                            var appDescription = (string)this.ResourceData.AppDescription;

                            if (appDescription.IsNullOrEmpty())
                            {
                                appDescription = "Hydra is a app generation tool with resulting source code. Generated front-end support for Ionic / Angular.Restful service layer support for .NET Core.Back - end support for SQL Server. Other supported technologies coming soon as Hydra is highly extensible.";
                            }

                            blob.DrawText(graphics, boundingRect, fontFamily, appDescription, color, backgroundColor, 6f);
                        }
                        break;
                    case "Copyright":
                        {
                            var color = Color.Black;
                            var fontFamily = this.FontCollection.Families.Single(f => f.Name == "Roboto Light");
                            var organizationName = (string)this.ResourceData.OrganizationName;
                            string copyright;

                            if (organizationName.IsNullOrEmpty())
                            {
                                organizationName = "CloudIDEaaS";
                            }

                            copyright = string.Format("{0}, All rights reserved © {1}", organizationName, DateTime.Today.Year);

                            blob.DrawText(graphics, boundingRect, fontFamily, copyright, color, backgroundColor, 5f);
                        }
                        break;
                    case "SplashScreen":
                        {
                            var splashScreen = (string)this.ResourceData.SplashScreen;
                            Bitmap image;

                            if (splashScreen != null)
                            {
                                if (!this.ImageCollection.ContainsKey(splashScreen))
                                {
                                    using (var imageOriginal = (Bitmap)Bitmap.FromFile(splashScreen))
                                    {
                                        image = imageOriginal.Clone(new Rectangle(0, 0, imageOriginal.Width, imageOriginal.Height), imageOriginal.PixelFormat);
                                    }

                                    this.ImageCollection.Add(splashScreen, image);
                                }

                                image = this.ImageCollection[splashScreen];
                            }
                            else
                            {
                                image = this.ImageCollection["SplashScreen"];
                            }

                            blob.DrawImage(graphics, boundingRect, image);
                        }
                        break;
                    case "Skip":
                        {
                            var color = ColorTranslator.FromHtml(this.ResourceData.PrimaryColor);
                            var fontFamily = this.FontCollection.Families.Single(f => f.Name == "Roboto");

                            blob.DrawText(graphics, boundingRect, fontFamily, "SKIP", color, backgroundColor, 6.5f);
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
