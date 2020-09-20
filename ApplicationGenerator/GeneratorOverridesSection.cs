using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace AbstraX
{
    public class Override : System.Configuration.ConfigurationElement
    {
        public string ArgumentsKind { get; set; }
        public OverrideAssembly Assembly { get; set; }
    }

    public class OverrideAssembly : System.Configuration.ConfigurationElement
    {
        public string name { get; set; }
        public string version { get; set; }
        public string publicKeyToken { get; set; }
        public string culture { get; set; }

        public AssemblyName AssemblyName
        {
            get
            {
                var name = this.ToString();

                return new AssemblyName(name);
            }
        }

        public override string ToString()
        {
            return $"{ this.name }, Version={ this.version }, Culture={ this.culture }, PublicKeyToken={ this.publicKeyToken }";
        }
    }

    public class GeneratorOverridesSection : IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, XmlNode section)
        {
            var overrides = new List<Override>();

            foreach (XmlNode childNode in section.ChildNodes)
            {
                var assemblyNode = childNode.ChildNodes.OfType<XmlElement>().Single();

                overrides.Add(new Override
                {
                    ArgumentsKind = childNode.Attributes["argumentsKind"].Value,
                    Assembly = new OverrideAssembly() { name = assemblyNode.Attributes["name"].Value, version = assemblyNode.Attributes["version"].Value, publicKeyToken = assemblyNode.Attributes["publicKeyToken"].Value, culture = assemblyNode.Attributes["culture"].Value, }
                });
            }

            return overrides;
        }
    }
}
