using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using AbstraX.TypeMappings;
using System.Collections;
using System.IO;

namespace AbstraX
{
    public static class GeneratorExtensions
    {
        public static string ProperCase(this string text)
        {
            return text.Substring(0, 1).ToUpper() + text.Substring(1, text.Length - 1);
        }

        public static string CamelCase(this string text)
        {
            return text.Substring(0, 1).ToLower() + text.Substring(1, text.Length - 1);
        }

        public static bool IsSingular(this string text)
        {
            var service = PluralizationService.CreateService(new CultureInfo("en-US"));

            return service.IsSingular(text);
        }

        public static string Singularize(this string text)
        {
            var service = PluralizationService.CreateService(new CultureInfo("en-US"));

            return service.Singularize(text);
        }

        public static bool IsPlural(this string text)
        {
            var service = PluralizationService.CreateService(new CultureInfo("en-US"));

            return service.IsPlural(text);
        }

        public static string Pluralize(this string text)
        {
            var service = PluralizationService.CreateService(new CultureInfo("en-US"));

            return service.Pluralize(text);
        }

        public static string CamelCasePluralize(this string text)
        {
            return text.Pluralize().CamelCase();
        }

        public static string XmlTypeToDotNet(string type)
        {
            return TypeMapper.ObjectMappings[type].MappedFromType;
        }
    }
}
