using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils;
using Microsoft.VisualStudio.TextTemplating;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.IO;

namespace Utils
{
    public static class TemplateEngineExtensions
    {
        public static void InitializePartial(this ITemplateEngineBasePartialClass partialClass)
        {
            var properties = partialClass.GetPublicProperties();
            var session = (IDictionary<string, object>)properties.Single(p => p.Name == "Session").GetValue(partialClass);

            foreach (var pair in session)
            {
                if (properties.Any(p => p.Name.AsCaseless() == pair.Key))
                {
                    var property = properties.Single(p => p.Name.AsCaseless() == pair.Key);

                    property.SetValue(partialClass, pair.Value);
                }
                else
                {
                    throw new NotImplementedException($"Property { pair.Key } not found");
                }
            }
        }
    }
}
