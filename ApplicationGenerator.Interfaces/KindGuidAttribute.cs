using AbstraX.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Text;

namespace AbstraX
{
    [AttributeUsage(AttributeTargets.Field)]
    public class KindGuidAttribute : Attribute
    {
        public Guid Guid { get; }
        public UIFeatureKind FeatureKind { get; }

        public KindGuidAttribute(string guidValue, UIFeatureKind featureKind = UIFeatureKind.Custom)
        {
            this.Guid = Guid.Parse(guidValue);
            this.FeatureKind = featureKind;
        }
    }
}
    