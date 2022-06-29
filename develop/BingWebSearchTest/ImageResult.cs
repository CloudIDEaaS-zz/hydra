using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BingWebSearchTest
{
    public class ImageResult
    {
        public string ThumbnailUrl { get; internal set; }
        public string EncodingFormat { get; internal set; }
        public string FileName { get; internal set; }
        public string ContentUrl { get; internal set; }
        public string HostPageDomainFriendlyName { get; internal set; }
        public string HostPageDisplayUrl { get; internal set; }
        public Exception DownloadException { get; internal set; }
        public Color AccentColor { get; internal set; }
    }
}
