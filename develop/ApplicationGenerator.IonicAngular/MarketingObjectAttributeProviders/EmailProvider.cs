using AbstraX.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.MarketingObjectAttributeProviders
{
    [MarketingObject("Email", new[] { "email-address", "email-title", "email-body" })]
    public class EmailProvider : IMarketingObjectAttributeProvider
    {
        public Dictionary<string, List<string>> SupportedVariables { get; set; }

        public Attribute[] GetAttributes(string property)
        {
            var attributes = new List<Attribute>();

            return attributes.ToArray();
        }
    }
}
