// file:	TypeMappings\TypeMappings.cs
//
// summary:	Implements the type mappings class

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
    /// <summary>   A type mapper. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 11/13/2020. </remarks>

    public static class TypeMapper
    {
        /// <summary>   The mappings. </summary>
        private static TypeMappings mappings = new TypeMappings();

        /// <summary>   Gets the mappings. </summary>
        ///
        /// <value> The mappings. </value>

        public static TypeMappings Mappings
        {
            get
            {
                return mappings;
            }
        }

        /// <summary>   A type mappings. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/13/2020. </remarks>

        public class TypeMappings
        {
            /// <summary>   The mappings. </summary>
            private Dictionary<string, Mapping> mappings = new Dictionary<string, Mapping>();
            /// <summary>   True to server version. </summary>
            private bool serverVersion = false;
            /// <summary>   The assembly. </summary>
            private Assembly assembly;

            /// <summary>   Indexer to get items within this collection using array index syntax. </summary>
            ///
            /// <param name="mapping">  The mapping. </param>
            ///
            /// <returns>   The indexed item. </returns>

            public Mapping this[string mapping]
            {
                get
                {
                    return mappings[mapping];
                }
            }

            /// <summary>   Default constructor. </summary>
            ///
            /// <remarks>   CloudIDEaaS, 11/13/2020. </remarks>

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

            /// <summary>   Loads a mapping. </summary>
            ///
            /// <remarks>   CloudIDEaaS, 11/13/2020. </remarks>
            ///
            /// <param name="path"> Full pathname of the file. </param>

            private void LoadMapping(string path)
            {
                var stream = assembly.GetManifestResourceStream(path);

                var mapping = Mapping.GetMapping(stream, path);
                mappings.Add(mapping.Name, mapping);
            }
        }

        /// <summary>   Gets the object mappings. </summary>
        ///
        /// <value> The object mappings. </value>

        public static Dictionary<string, MapEntry> ObjectMappings
        {
            get
            {
                return TypeMapper.Mappings["ObjectTypes"].Map;
            }
        }

        /// <summary>   Gets the dot net mappings. </summary>
        ///
        /// <value> The dot net mappings. </value>

        public static Dictionary<string, MapEntry> DotNetMappings
        {
            get
            {
                return TypeMapper.Mappings["DotNetTypes"].Map;
            }
        }

        /// <summary>   A Type extension method that gets XML type. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/13/2020. </remarks>
        ///
        /// <param name="type"> The type to act on. </param>
        ///
        /// <returns>   The XML type. </returns>

        public static XmlTypeCode GetXMLType(string type)
        {
            var xmlTypeCode = (XmlTypeCode)Enum.Parse(typeof(XmlTypeCode), type, true);

            return xmlTypeCode;
        }

        /// <summary>   An XmlTypeCode extension method that gets dot net type. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/13/2020. </remarks>
        ///
        /// <param name="typeCode"> The type code. </param>
        ///
        /// <returns>   The dot net type. </returns>

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

        /// <summary>   A Type extension method that gets XML type. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/13/2020. </remarks>
        ///
        /// <param name="type"> The type to act on. </param>
        ///
        /// <returns>   The XML type. </returns>

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

        /// <summary>   A Type extension method that maps to. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/13/2020. </remarks>
        ///
        /// <param name="type">     The type to act on. </param>
        /// <param name="typeCode"> The type code. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

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


        /// <summary>   A Type extension method that gets enumerable generic type. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/13/2020. </remarks>
        ///
        /// <param name="type"> The type to act on. </param>
        ///
        /// <returns>   The enumerable generic type. </returns>

        public static Type GetEnumerableGenericType(this Type type)
        {
            try
            {
                foreach (Type intType in type.GetInterfaces())
                {
                    if (intType.IsEnumerableGeneric())
                    {
                        return intType.GetGenericArguments()[0];
                    }
                }

                if (type.IsEnumerableGeneric())
                {
                    return type.GetGenericArguments()[0];
                }
            }
            catch (Exception ex)
            {
                if (type.IsEnumerableGeneric())
                {
                    return type.GetGenericArguments()[0];
                }
            }

            return null;
        }

        /// <summary>   A Type extension method that query if 'type' is dictionary generic. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/13/2020. </remarks>
        ///
        /// <param name="type"> The type to act on. </param>
        ///
        /// <returns>   True if dictionary generic, false if not. </returns>

        //public static bool IsDictionaryGeneric(this Type type)
        //{
        //    try
        //    {
        //        foreach (Type intType in type.GetInterfaces())
        //        {
        //            if (intType.IsGenericType && intType.GetGenericTypeDefinition() == typeof(IDictionary<,>))
        //            {
        //                return true;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        var underlyingType = type.BaseType;

        //        return underlyingType.IsDictionaryGeneric();
        //    }

        //    return false;
        //}

        /// <summary>   A Type extension method that gets dictionary generic type. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/13/2020. </remarks>
        ///
        /// <param name="type"> The type to act on. </param>
        ///
        /// <returns>   The dictionary generic type. </returns>

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
        