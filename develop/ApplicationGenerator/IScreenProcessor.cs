using AForge.Imaging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AbstraX
{
    /// <summary>   Screen painter. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 11/29/2020. </remarks>
    ///
    /// <param name="sender">   Source of the event. </param>
    /// <param name="e">        Paint event information. </param>

    public delegate void ScreenPainter(object sender, PaintEventArgs e);

    /// <summary>   Interface for screen processor. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 11/27/2020. </remarks>

    public interface IScreenProcessor : IBlobsFilter
    {
        /// <summary>   Gets the named blobs. </summary>
        ///
        /// <value> The named blobs. </value>

        List<NamedBlob> NamedBlobs { get; }
        /// <summary>   Gets or sets information describing the resource. </summary>
        ///
        /// <value> Information describing the resource. </value>

        IResourceData ResourceData { get; set; }

        /// <summary>   Gets or sets a collection of fonts. </summary>
        ///
        /// <value> A collection of fonts. </value>

        PrivateFontCollection FontCollection { get; set; }

        /// <summary>   Gets or sets a collection of images. </summary>
        ///
        /// <value> A collection of images. </value>

        Dictionary<string, Bitmap> ImageCollection { get; set; }

        /// <summary>   Draw parts. </summary>
        ///
        /// <param name="graphics">     The graphics. </param>
        /// <param name="boundingRect"> The bounding rectangle. </param>

        void DrawParts(Graphics graphics, Rectangle boundingRect);
    }

    /// <summary>   Interface for previous screen processor. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 11/29/2020. </remarks>

    public interface IPreviousScreenProcessor : IScreenProcessor
    {
        /// <summary>   Draw previous screen. </summary>
        ///
        /// <param name="previousScreenProcessor">  The previous screen processor. </param>
        /// <param name="graphics">                 The graphics. </param>
        /// <param name="boundingRect">             The bounding rectangle. </param>
        /// <param name="previousScreenPainter">    The previous screen painter. </param>

        void DrawPreviousScreen(IScreenProcessor previousScreenProcessor, Graphics graphics, Rectangle boundingRect, ScreenPainter previousScreenPainter);
    }
}