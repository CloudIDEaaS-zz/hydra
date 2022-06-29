// file:	GeneratorHandlersSection.cs
//
// summary:	Implements the app stores section class

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
    /// <summary>   A app store element. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 11/17/2020. </remarks>

    public class DevTool : System.Configuration.ConfigurationElement
    {
        /// <summary>   Gets or sets the arguments kind. </summary>
        ///
        /// <value> The arguments kind. </value>

        public string DevToolType { get; set; }

        /// <summary>   Gets or sets the assembly. </summary>
        ///
        /// <value> The assembly. </value>

        public DevToolsAssembly Assembly { get; set; }
    }

    /// <summary>   A app store assembly. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 11/17/2020. </remarks>

    public class DevToolsAssembly : System.Configuration.ConfigurationElement
    {
        /// <summary>   Gets or sets the name. </summary>
        ///
        /// <value> The name. </value>

        public string name { get; set; }

        /// <summary>   Gets or sets the version. </summary>
        ///
        /// <value> The version. </value>

        public string version { get; set; }

        /// <summary>   Gets or sets the public key token. </summary>
        ///
        /// <value> The public key token. </value>

        public string publicKeyToken { get; set; }

        /// <summary>   Gets or sets the culture. </summary>
        ///
        /// <value> The culture. </value>

        public string culture { get; set; }

        /// <summary>   Gets the name of the assembly. </summary>
        ///
        /// <value> The name of the assembly. </value>

        public AssemblyName AssemblyName
        {
            get
            {
                var name = this.ToString();

                return new AssemblyName(name);
            }
        }

        /// <summary>   Returns a string that represents the current object. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/17/2020. </remarks>
        ///
        /// <returns>   A string that represents the current object. </returns>

        public override string ToString()
        {
            return $"{ this.name }, Version={ this.version }, Culture={ this.culture }, PublicKeyToken={ this.publicKeyToken }";
        }
    }

    /// <summary>   A app stores section. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 11/17/2020. </remarks>

    public class DevToolsSection : IConfigurationSectionHandler
    {
        /// <summary>   Creates a configuration section handler. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/17/2020. </remarks>
        ///
        /// <param name="parent">           Parent object. </param>
        /// <param name="configContext">    Configuration context object. </param>
        /// <param name="section">          Section XML node. </param>
        ///
        /// <returns>   The created section handler object. </returns>

        public object Create(object parent, object configContext, XmlNode section)
        {
            var DevTools = new List<DevTool>();

            foreach (XmlNode childNode in section.ChildNodes)
            {
                var assemblyNode = childNode.ChildNodes.OfType<XmlElement>().Single();

                DevTools.Add(new DevTool
                {
                    DevToolType = childNode.Attributes["DevToolsType"].Value,
                    Assembly = new DevToolsAssembly() { name = assemblyNode.Attributes["name"].Value, version = assemblyNode.Attributes["version"].Value, publicKeyToken = assemblyNode.Attributes["publicKeyToken"].Value, culture = assemblyNode.Attributes["culture"].Value, }
                });
            }

            return DevTools;
        }
    }
}
