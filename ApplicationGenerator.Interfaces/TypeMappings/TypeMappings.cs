using System;
using System.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Xml.XPath;
using System.Reflection;
using AbstraX.TypeMappings.Schemas;
using AbstraX.TypeMappings;
using System.Xml.Schema;
using System.Linq;
using Utils;

namespace AbstraX.TypeMappings
{
    public static class TypeMapper
    {
        private static TypeMappings mappings = new TypeMappings();

        public static TypeMappings Mappings
        {
            get
            {
                return mappings;
            }
        }

        public class TypeMappings
        {
            private Dictionary<string, Mapping> mappings = new Dictionary<string, Mapping>();
            private bool serverVersion = false;
            private Assembly assembly;

            public Mapping this[string mapping]
            {
                get
                {
                    return mappings[mapping];
                }
            }

            public TypeMappings()
            {
                Stream stream;

                assembly = Assembly.GetExecutingAssembly();
                stream = assembly.GetManifestResourceStream("AbstraX.ClientInterfaces.TypeMappings.Schemas.XMLSchema.xsd");

                if (stream == null)
                {
                    stream = assembly.GetManifestResourceStream("AbstraX.TypeMappings.Schemas.XMLSchema.xsd");
                    serverVersion = true;
                }

                Mapping.XmlSchema = new Schema(stream);

                if (serverVersion)
                {
                    LoadMapping("AbstraX.TypeMappings.Mappings.DataTypes.xslt");
                    LoadMapping("AbstraX.TypeMappings.Mappings.DotNetTypes.xslt");
                    LoadMapping("AbstraX.TypeMappings.Mappings.ObjectTypes.xslt");
                    LoadMapping("AbstraX.TypeMappings.Mappings.ScriptTypes.xslt");
                }
                else
                {
                    LoadMapping("AbstraX.ClientInterfaces.TypeMappings.Mappings.DataTypes.xslt");
                    LoadMapping("AbstraX.ClientInterfaces.TypeMappings.Mappings.DotNetTypes.xslt");
                    LoadMapping("AbstraX.ClientInterfaces.TypeMappings.Mappings.ObjectTypes.xslt");
                    LoadMapping("AbstraX.ClientInterfaces.TypeMappings.Mappings.ScriptTypes.xslt");
                }
            }

            private void LoadMapping(string path)
            {
                var stream = assembly.GetManifestResourceStream(path);

                var mapping = Mapping.GetMapping(stream, path);
                mappings.Add(mapping.Name, mapping);
            }
        }

        public static Dictionary<string, MapEntry> ObjectMappings
        {
            get
            {
                return TypeMapper.Mappings["ObjectTypes"].Map;
            }
        }

        public static Dictionary<string, MapEntry> DotNetMappings
        {
            get
            {
                return TypeMapper.Mappings["DotNetTypes"].Map;
            }
        }

        public static XmlTypeCode GetXMLType(string type)
        {
            var xmlTypeCode = (XmlTypeCode)Enum.Parse(typeof(XmlTypeCode), type, true);

            return xmlTypeCode;
        }

        public static Type GetDotNetType(this XmlTypeCode typeCode)
        {
            var objectMapping = TypeMapper.ObjectMappings.SingleOrDefault(p => p.Value.MappedToType.AsCaseless() == typeCode.ToString());

            if (DotNetMappings.Any(p => p.Value.MappedFromType == objectMapping.Value.MappedFromType))
            {
                var dotNetMapping = DotNetMappings.Single(p => p.Value.MappedFromType == objectMapping.Value.MappedFromType);
                var type = Type.GetType("System." + dotNetMapping.Value.MappedToType, true);

                return type;
            }

            return null;
        }

        public static XmlTypeCode GetXMLType(this Type type)
        {
            MapEntry doNetEntry;
            var xmlTypeCode = XmlTypeCode.None;

            if (TypeMapper.ObjectMappings.Any(p => p.Value.MappedFromType == type.Name))
            {
                var list = TypeMapper.ObjectMappings.Where(p => p.Value.MappedFromType == type.Name);

                if (list.Count() > 1)
                {
                    var xmlEntry = TypeMapper.ObjectMappings.Where(p => p.Value.MappedFromType == type.Name && p.Value.IsPreferred == true).Single().Value.MappedToType;

                    xmlTypeCode = (XmlTypeCode)Enum.Parse(typeof(XmlTypeCode), xmlEntry, true);
                }
                else
                {
                    var xmlEntry = TypeMapper.ObjectMappings.Where(p => p.Value.MappedFromType == type.Name).Single().Value.MappedToType;

                    if (xmlEntry == "*")
                    {
                        xmlTypeCode = XmlTypeCode.Element;
                    }
                    else
                    {
                        xmlTypeCode = (XmlTypeCode)Enum.Parse(typeof(XmlTypeCode), xmlEntry, true);
                    }
                }
            }
            else if (TypeMapper.DotNetMappings.Any(p => p.Value.MappedToType == type.Name))
            {
                doNetEntry = TypeMapper.DotNetMappings.Where(p => p.Value.MappedToType == type.Name).Single().Value;

                if (TypeMapper.ObjectMappings.Any(p => p.Value.MappedFromType == doNetEntry.MappedFromType))
                {
                    var list = TypeMapper.ObjectMappings.Where(p => p.Value.MappedFromType == doNetEntry.MappedFromType);

                    if (list.Count() > 1)
                    {
                        var xmlEntry = TypeMapper.ObjectMappings.Where(p => p.Value.MappedFromType == doNetEntry.MappedFromType && p.Value.IsPreferred == true).Single().Value.MappedToType;

                        xmlTypeCode = (XmlTypeCode)Enum.Parse(typeof(XmlTypeCode), xmlEntry, true);
                    }
                    else
                    {
                        var xmlEntry = TypeMapper.ObjectMappings.Where(p => p.Value.MappedFromType == doNetEntry.MappedFromType).Single().Value.MappedToType;

                        if (xmlEntry == "*")
                        {
                            xmlTypeCode = XmlTypeCode.Element;
                        }
                        else
                        {
                            xmlTypeCode = (XmlTypeCode)Enum.Parse(typeof(XmlTypeCode), xmlEntry, true);
                        }
                    }
                }
            }

            return xmlTypeCode;
        }

        public static bool MapsTo(this Type type, XmlTypeCode typeCode)
        {
            MapEntry doNetEntry;
            XmlTypeCode notUsed;

            if (TypeMapper.ObjectMappings.Any(p => p.Value.MappedFromType == type.Name))
            {
                var mapsTo = TypeMapper.ObjectMappings.Any(p => p.Value.MappedFromType == type.Name && (p.Value.MappedToType == "*" || (Enum.TryParse<XmlTypeCode>(p.Value.MappedToType, true, out notUsed) && ((XmlTypeCode)Enum.Parse(typeof(XmlTypeCode), p.Value.MappedToType, true)) == typeCode)));

                return mapsTo;
            }
            else if (TypeMapper.DotNetMappings.Any(p => p.Value.MappedToType == type.Name))
            {
                doNetEntry = TypeMapper.DotNetMappings.Where(p => p.Value.MappedToType == type.Name).Single().Value;

                if (TypeMapper.ObjectMappings.Any(p => p.Value.MappedFromType == doNetEntry.MappedFromType))
                {
                    var mapsTo = TypeMapper.ObjectMappings.Any(p => p.Value.MappedFromType == doNetEntry.MappedFromType && (p.Value.MappedToType == "*" || (Enum.TryParse<XmlTypeCode>(p.Value.MappedToType, true, out notUsed) && ((XmlTypeCode)Enum.Parse(typeof(XmlTypeCode), p.Value.MappedToType, true)) == typeCode)));

                    return mapsTo;
                }
            }

            return false;
        }

        public static bool IsEnumerableGeneric(this Type type)
        {
            foreach (Type intType in type.GetInterfaces())
            {
                if (intType.IsGenericType && intType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    return true;
                }
            }

            return false;
        }

        public static Type GetEnumerableGenericType(this Type type)
        {
            foreach (Type intType in type.GetInterfaces())
            {
                if (intType.IsGenericType && intType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    return intType.GetGenericArguments()[0];
                }
            }

            return null;
        }

        public static bool IsDictionaryGeneric(this Type type)
        {
            foreach (Type intType in type.GetInterfaces())
            {
                if (intType.IsGenericType && intType.GetGenericTypeDefinition() == typeof(IDictionary<,>))
                {
                    return true;
                }
            }

            return false;
        }

        public static Type GetDictionaryGenericType(this Type type)
        {
            foreach (Type intType in type.GetInterfaces())
            {
                if (intType.IsGenericType && intType.GetGenericTypeDefinition() == typeof(IDictionary<,>))
                {
                    return intType.GetGenericArguments()[1];
                }
            }

            return null;
        }
    }
}
        