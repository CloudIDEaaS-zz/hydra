using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.IO;
using AbstraX.TypeMappings.Schemas;
using System.Xml.Linq;
using System.Linq;
using System.Diagnostics;

namespace AbstraX.TypeMappings
{
    [DebuggerDisplay("{ DebugInfo }")]
    public class MapEntry
    {
        private string mappedToType;
        private string mappedFromType;
        private bool? needSize;
        private int? defaultSize;
        private bool? isPreferred;
        private string extra;

        public MapEntry(string xmlType, string mappedFromType)
        {
            this.mappedToType = xmlType;
            this.mappedFromType = mappedFromType;
        }

        public MapEntry(string xmlType, string mappedFromType, string extra)
        {
            this.mappedToType = xmlType;
            this.mappedFromType = mappedFromType;
            this.extra = extra;
        }

        public MapEntry(string xmlType, string mappedFromType, string extra, bool isPreferred)
        {
            this.mappedToType = xmlType;
            this.mappedFromType = mappedFromType;
            this.extra = extra;
            this.isPreferred = isPreferred;
        }

        public MapEntry(string xmlType, string mappedToType, bool needSize, int defaultSize)
        {
            this.mappedToType = xmlType;
            this.mappedFromType = mappedToType;
            this.needSize = needSize;
            this.defaultSize = defaultSize;
        }

        public string DebugInfo
        {
            get
            {
                StringBuilder debug = new StringBuilder();

                debug.Append(string.Format("Maps {0} -> {1}", mappedFromType, mappedToType));

                if (needSize != null)
                {
                    debug.Append(string.Format(", NeedSize={0}", needSize.ToString()));
                }

                if (defaultSize != null)
                {
                    debug.Append(string.Format(", DefaultSize={0}", defaultSize.ToString()));
                }
                
                if (isPreferred != null)
                {
                    debug.Append(string.Format(", IsPreferred={0}", isPreferred.ToString()));
                }

                if (extra != null)
                {
                    debug.Append(string.Format(", Extra='{0}'", extra));
                }

                return debug.ToString();
            }
        }

        public int? DefaultSize
        {
            get 
            { 
                return defaultSize; 
            }
            
            set 
            { 
                defaultSize = value; 
            }
        }

        public string Extra
        {
            get 
            { 
                return extra; 
            }
            
            set 
            { 
                extra = value; 
            
            }
        }

        public bool? IsPreferred
        {
            get
            {
                return isPreferred;
            }

            set
            {
                isPreferred = value;
            }
        }

        public bool? NeedSize
        {
            get 
            { 
                return needSize; 
            }
            
            set 
            { 
                needSize = value; 
            }
        }

        public string MappedToType
        {
            get 
            { 
                return mappedToType; 
            
            }
            set 
            { 
                mappedToType = value; 
            }
        }

        public string MappedFromType
        {
            get 
            {
                return mappedFromType; 
            }
            
            set 
            {
                mappedFromType = value; 
            }
        }

    }

    public class Mapping : Transform 
    {
        protected Dictionary<string, MapEntry> map = new Dictionary<string, MapEntry>();
        protected static Schema xmlSchema;

        public static Schema XmlSchema
        {
            get { return xmlSchema; }
            set { xmlSchema = value; }
        }

        public Dictionary<string, MapEntry> Map
        {
            get 
            {
                return map; 
            }
            
            set 
            { 
                map = value; 
            }
        }

        public virtual Dictionary<string, MapEntry> PreferredMap
        {
            get
            {
                return map;
            }
        }

        public Mapping(Stream stream, string name) : base(stream, name)
        {
        }

        public Mapping(XDocument transformCopy, string name) : base(transformCopy, name)
        {
        }

        public new string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        public static Mapping GetMapping(Stream stream, string path)
        {
            var name = Path.GetFileNameWithoutExtension(path);

            name = name.Remove(0, name.LastIndexOf(".") + 1);

            switch (name)
            {
                case "DataTypes":
                    return new DataTypeMapping(stream, name);
                case "ObjectTypes":
                    return new ObjectTypeMapping(stream, name);
                case "DotNetTypes":
                    return new DotNetTypeMapping(stream, name);
                case "ScriptTypes":
                    return new ScriptTypeMapping(stream, name);
            }

            return null;
        }
    }

    public class DotNetTypeMapping : Mapping
    {
        public DotNetTypeMapping(Stream stream, string name) : base(stream, name)
        {
            var document = XDocument.Load(stream);

            foreach (XElement element in document.XPathSelectElements("/xsl:stylesheet/xsl:template/uml:ObjectTypes/uml:Type", namespaceManager))
            {
                var mapFrom = element.Attribute(XName.Get("TypeName", string.Empty));
                var macroType = element.Attribute(XName.Get("MacroType", string.Empty));
                var mapToSelect = element.XPathSelectElement("./uml:MapTo/xsl:value-of", namespaceManager);
                XAttribute mapTo;

                mapTo = mapToSelect.Attribute(XName.Get("select"));

                map.Add(mapFrom.Value, new MapEntry(mapTo.Value, mapFrom.Value, macroType.Value));
            }
        }
    }

    public class DataTypeMapping : Mapping
    {
        public DataTypeMapping(Stream stream, string name) : base(stream, name)
        {
            var document = XDocument.Load(stream);

            foreach (XElement element in document.XPathSelectElements("/xsl:stylesheet/xsl:template/data:SQLDataTypes/data:Type", namespaceManager))
            {
                var mapFrom = element.Attribute(XName.Get("TypeName", string.Empty));
                var mapToSelect = element.XPathSelectElement("./data:MapTo/xsl:value-of", namespaceManager);
                string path;
                XElement mapToElement;
                string mapTo;
                bool needSize = false;
                int defaultSize = 0;
                var xml = Mapping.XmlSchema.GetXPath();

                path = mapToSelect.Attribute(XName.Get("select")).Value;

                if (element.Attribute(XName.Get("NeedSize", string.Empty)) != null)
                {
                    needSize = bool.Parse(element.Attribute(XName.Get("NeedSize", string.Empty)).Value.ToString());
                }

                if (needSize && element.Attribute(XName.Get("DefaultSize", string.Empty)) != null)
                {
                    defaultSize = int.Parse(element.Attribute(XName.Get("DefaultSize", string.Empty)).Value.ToString());
                }

                if (path == "/xs:schema/xs:simpleType")
                {
                    mapTo = "*";
                }
                else
                {
                    mapToElement = ((IEnumerable<object>)xml.XPathEvaluate(path, namespaceManager)).Cast<XElement>().First();
                    mapTo = mapToElement.Attribute(XName.Get("name", string.Empty)).Value;
                }

                if (needSize)
                {
                    map.Add(mapTo, new MapEntry(mapTo, mapFrom.Value, needSize, defaultSize));
                }
                else
                {
                    map.Add(mapTo, new MapEntry(mapTo, mapFrom.Value));
                }
            }
        }
    }

    public class ObjectTypeMapping : Mapping
    {
        public ObjectTypeMapping(Stream stream, string name) : base(stream, name)
        {
            var document = XDocument.Load(stream);

            foreach (XElement element in document.XPathSelectElements("/xsl:stylesheet/xsl:template/uml:ObjectTypes/uml:Type", namespaceManager))
            {
                var mapFrom = element.Attribute(XName.Get("TypeName", string.Empty));
                bool? isPreferred = null;
                var mapToSelect = element.XPathSelectElement("./uml:MapTo/xsl:value-of", namespaceManager);
                string path;
                string mapTo;
                XElement mapToElement;
                var xml = Mapping.XmlSchema.GetXPath();

                path = mapToSelect.Attribute(XName.Get("select")).Value;

                if (element.Attribute(XName.Get("Preferred", string.Empty)) != null)
                {
                    isPreferred = bool.Parse(element.Attribute(XName.Get("Preferred", string.Empty)).Value.ToString());
                }

                if (path == "/xs:schema/xs:simpleType")
                {
                    mapTo = "*";
                }
                else
                {
                    mapToElement = ((IEnumerable<object>)xml.XPathEvaluate(path, namespaceManager)).Cast<XElement>().First();
                    mapTo = mapToElement.Attribute(XName.Get("name", string.Empty)).Value;
                }

                if (isPreferred != null)
                {
                    map.Add(mapTo, new MapEntry(mapTo, mapFrom.Value, null, (bool) isPreferred));
                }
                else
                {
                    map.Add(mapTo, new MapEntry(mapTo, mapFrom.Value, null));
                }
            }
        }

        public override Dictionary<string, MapEntry> PreferredMap
        {
            get
            {
                Dictionary<string, MapEntry> preferred = new Dictionary<string, MapEntry>();

                foreach (KeyValuePair<string, MapEntry> pair in map)
                {
                    MapEntry entry = pair.Value;

                    if (entry.IsPreferred == true)
                    {
                        preferred.Add(entry.MappedFromType, entry);
                    }
                }

                return preferred;
            }
        }
    }

    public class ScriptTypeMapping : Mapping
    {
        public ScriptTypeMapping(Stream stream, string strName) : base(stream, strName)
        {
            var document = new XPathDocument(stream);
            var navigator = document.CreateNavigator();
            var iter1 = navigator.Select("/xsl:stylesheet/xsl:template/script:ScriptTypes/script:Type", namespaceManager);

            while (iter1.MoveNext())
            {
                var mapFrom = iter1.Current.GetAttribute("TypeName", string.Empty);
                var iter2 = iter1.Current.Select("./script:MapTo/xsl:value-of/@select", namespaceManager);
                XPathNodeIterator objIter3;
                string mapTo;
                string validator;
                var xml = Mapping.XmlSchema.GetXPath();

                iter2.MoveNext();
                mapTo = iter2.Current.ToString();

                objIter3 = iter1.Current.Select("./script:TypeValidator", namespaceManager);
                objIter3.MoveNext();

                validator = objIter3.Current.Value;

                map.Add(mapFrom, new MapEntry(mapTo, mapFrom, validator));
            }
        }
    }
}
