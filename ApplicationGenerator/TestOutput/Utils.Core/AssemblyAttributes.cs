// -----------------------------------------------------------------------
// <copyright file="AssemblyAttributes.cs" company="CloudIDEaaS Inc.">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Reflection;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class AssemblyAttributes
    {
        private Assembly assembly;

        public AssemblyAttributes(Assembly assembly)
        {
            this.assembly = assembly;
        }

        public string Product
        {
            get
            {
                var attributes = assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false);

                return attributes.Length == 0 ? "" : ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public string Company
        { 
            get 
            {
                var attributes = assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false); 
                
                return attributes.Length == 0 ? "" : ((AssemblyCompanyAttribute)attributes[0]).Company; 
            } 
        }

        public string Copyright
        {
            get
            {
                var attributes = assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);

                return attributes.Length == 0 ? "" : ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public Version VersionRaw 
        {
            get
            {
                var assemblyName = new AssemblyName(assembly.FullName);

                if (assemblyName == null)
                {
                    return null;
                }

                var version = assemblyName.Version;

                if (version == null)
                {
                    return null;
                }

                return version;
            }
        }

        public string Version
        {
            get
            {
                var assemblyName = new AssemblyName(assembly.FullName);

                if (assemblyName == null)
                {
                    return string.Empty;
                }

                var version = assemblyName.Version;

                if (version == null)
                {
                    return string.Empty;
                }

                return version.ToString();
            }
        }
    }
}
