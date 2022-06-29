using AbstraX.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.MarketingObjectAttributeProviders
{
    [MarketingObject("Facebook", new[] { "facebook-account", "facebook-post-url" })]
    public class FacebookProvider : IMarketingObjectAttributeProvider
    {
        public Dictionary<string, List<string>> SupportedVariables { get; set; }

        public Attribute[] GetAttributes(string property)
        {
            var attributes = new List<Attribute>();

            return attributes.ToArray();
        }
    }
}
