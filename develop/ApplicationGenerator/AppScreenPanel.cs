// file:	AppScreenPanel.cs
//
// summary:	Implements the application screen panel class

using AForge;
using AForge.Imaging;
using AForge.Math.Geometry;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;
using Point = System.Drawing.Point;

namespace AbstraX 
{
    /// <summary>   Panel for editing the application screen. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 11/27/2020. </remarks>

    public partial class AppScreenPanel : DoubleBufferedPanel
    {
        /// <summary>   The image. </summary>
        private Bitmap image;
        private Dictionary<string, Bitmap> imageCollection;
        private IResourceData resourceData;

        /// <summary>   Gets or sets a collection of images. </summary>
        ///
        /// <value> A collection of images. </value>

        public Dictionary<string, Bitmap> ImageCollection
        {
            get
            {
                return imageCollection;
            }

            set
            {
                imageCollection = value;
                this.ScreenProcessor.ImageCollection = imageCollection;
            }
        }

        /// <summary>   Gets or sets information describing the resource. </summary>
        ///
        /// <value> Information describing the resource. </value>

        public IResourceData ResourceData 
        {
            get
            {
                return resourceData;
            }

            set
            {
                resourceData = value;

                this.ScreenProcessor.ResourceData = resourceData;
            }
        }

        /// <summary>   Gets the screen processor. </summary>
        ///
        /// <value> The screen processor. </value>

        public IScreenProcessor ScreenProcessor { get; }

        /// <summary>   Gets or sets the previous panel. </summary>
        ///
        /// <value> The previous panel. </value>

        public AppScreenPanel PreviousPanel { get; set; }

        /// <summary>   Gets or sets a value indicating whether this  is browser. </summary>
        ///
        /// <value> True if this  is browser, false if not. </value>

        public bool IsBrowser { get; internal set; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/27/2020. </remarks>
        ///
        /// <param name="imageName">        Name of the image. </param>
        /// <param name="image">            The image. </param>
        /// <param name="fontCollection">   Collection of fonts. </param>
        /// <param name="imageCollection">  Collection of images. </param>
        /// <param name="resourceData">     Information describing the resource. </param>
        /// <param name="isBrowser">        True if this  is browser, false if not. </param>

        public AppScreenPanel(string imageName, Bitmap image, PrivateFontCollection fontCollection, Dictionary<string, Bitmap> imageCollection, IResourceData resourceData, bool isBrowser)
        {
            var pattern = @"^AbstraX\.Screens\.\d+?\. (?<name>.*)\.png$";
            Type processorType;

            this.Name = imageName.RegexGet(pattern, "name");
            this.IsBrowser = isBrowser;
            this.Paint += AppScreenPanel_Paint;

            if (this.IsBrowser)
            {
                this.Margin = new Padding(0, 0, 0, 0);
            }
            else
            {
                this.Margin = new Padding(10, 3, 10, 3);
            }

            this.BackColor = Color.White;

            this.image = image;
            this.imageCollection = imageCollection;
            this.resourceData = resourceData;
            this.Cursor = Cursors.Hand;

            processorType = Type.GetType("AbstraX.ScreenProcessors." + this.Name + "Processor");
            this.ScreenProcessor = (IScreenProcessor) Activator.CreateInstance(processorType);

            this.ScreenProcessor.FontCollection = fontCollection;
            this.ScreenProcessor.ImageCollection = imageCollection;
            this.ScreenProcessor.ResourceData = resourceData;
        }


        /// <summary>
        /// Raises the <see cref="M:System.Windows.Forms.Control.CreateControl" /> method.
        /// </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/27/2020. </remarks>

        protected override void OnCreateControl()
        {
            this.DelayInvoke(1, () =>
            {
                var blobCounter = new BlobCounter();
                Blob[] blobs;

                blobCounter.FilterBlobs = true;
                blobCounter.BlobsFilter = this.ScreenProcessor;

                blobCounter.ProcessImage(image);

                blobs = blobCounter.GetObjectsInformation();

                if (this.IsBrowser)
                {
                    this.Height = 160;
                    this.Width = 430;
                }
                else
                {
                    this.Height = 379;
                    this.Width = 211;
                }
            });

            base.OnCreateControl();
        }

        /// <summary>   Event handler. Called by AppScreenPanel for paint events. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/29/2020. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Paint event information. </param>

        public void AppScreenPanel_Paint(object sender, PaintEventArgs e)
        {
            var graphics = e.Graphics;
            var rect = new Rectangle(Point.Empty, new Size(image.Width, image.Height));
            var backgroundColor = ColorTranslator.FromHtml(resourceData.BackgroundColor);

            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = TextRenderingHint.AntiAlias;

            if (this.IsBrowser)
            {
                var drawImage = imageCollection["BrowserBackdrop"];

                graphics.DrawImage(drawImage, rect);
            }
            else
            {
                var roundedRect = rect;
                Rectangle indicatorRect;
                Bitmap drawImage;

                roundedRect.Inflate(4, 16);
                roundedRect.Offset(4, 16);
                roundedRect.Y = 0;
                indicatorRect = new Rectangle(roundedRect.Right - 12, 4, 3, 3);

                rect.Offset(4, 12);

                graphics.TextRenderingHint = TextRenderingHint.AntiAlias;

                graphics.FillRoundedRectangle(Brushes.Black, roundedRect, 8);
                graphics.FillEllipse(Brushes.OrangeRed, indicatorRect);

                drawImage = image.ChangeColor(Color.White, backgroundColor, 1);

                graphics.DrawImage(drawImage, rect);

                if (this.ScreenProcessor is IPreviousScreenProcessor previousScreenProcessor)
                {
                    var screenPainter = new ScreenPainter(this.PreviousPanel.AppScreenPanel_Paint);

                    previousScreenProcessor.DrawPreviousScreen(this.PreviousPanel.ScreenProcessor, e.Graphics, rect, screenPainter);
                }
            }

            ScreenProcessor.DrawParts(graphics, rect);
        }
    }
}
