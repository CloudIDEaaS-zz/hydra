using AbstraX.Resources;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.Generators
{
    public class ImageInfo : IImageInfo
    {
        public HtmlNode HtmlImage { get; }
        public string UrlPath { get; }
        public string OriginalPath { get; }
        public string CopyToFilePath { get; }
        public string ResourceName { get; }
        public string CleanPattern { get; }
        public string DescriptionHtml { get; set; }
        public string DescriptionPageName { get; set; }
        public string Description { get; set; }
        public string Alt { get; set; }
        public ObjectPropertiesDictionary ObjectProperties { get; set; }

        public ImageInfo(HtmlNode image, string resourceName, string cleanPattern)
        {
            this.HtmlImage = image;
            this.UrlPath = image.Attributes["src"].Value;
            this.ResourceName = resourceName;
            this.CleanPattern = cleanPattern;

            if (image.Attributes.Contains("alt"))
            {
                this.Alt = image.Attributes["alt"].Value;
            }
            else
            {
                this.Alt = resourceName;
            }
        }

        public ImageInfo(HtmlNode image, string resourceName, string cleanPattern, string originalPath, string copyToFilePath)
        {
            this.HtmlImage = image;
            this.UrlPath = image.Attributes["src"].Value;
            this.ResourceName = resourceName;
            this.OriginalPath = originalPath;
            this.CopyToFilePath = copyToFilePath;
            this.CleanPattern = cleanPattern;

            if (image.Attributes.Contains("alt"))
            {
                this.Alt = image.Attributes["alt"].Value;
            }
            else
            {
                this.Alt = resourceName;
            }
        }
    }
}
