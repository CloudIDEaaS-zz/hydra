using AbstraX.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.MarketingObjectAttributeProviders
{
    [MarketingObject("*")]
    public class GenericProvider : IMarketingObjectAttributeProvider
    {
        public Dictionary<string, List<string>> SupportedVariables { get; set; }

        public Attribute[] GetAttributes(string property)
        {
            var attributes = new List<Attribute>();

            return attributes.ToArray();
        }
    }
}
