using AbstraX.Resources;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.Generators
{
    public class LinkInfo : ILinkInfo
    {
        public HtmlNode HtmlLink { get; }
        public string UrlPath { get; }
        public string ResourceName { get; }

        public LinkInfo(HtmlNode link, string resourceName)
        {
            this.HtmlLink = link;
            this.UrlPath = link.Attributes["href"].Value;
            this.ResourceName = resourceName;
        }
    }
}
