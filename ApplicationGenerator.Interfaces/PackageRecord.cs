// file:	PackageRecord.cs
//
// summary:	Implements the package record class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX
{
    /// <summary>   Information about the package. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>

    public class PackageRecord
    {
        /// <summary>   Gets the type of the dependency. </summary>
        ///
        /// <value> The type of the dependency. </value>

        public string DependencyType { get; }

        /// <summary>   Gets the name of the package. </summary>
        ///
        /// <value> The name of the package. </value>

        public string PackageName { get; }

        /// <summary>   Gets the name of the package. </summary>
        ///
        /// <value> The name of the package. </value>

        public string PackageTypeName { get; }


        /// <summary>   Gets the name of the import handler. </summary>
        ///
        /// <value> The name of the import handler. </value>

        public string ImportHandlerName { get; }

        /// <summary>   Gets to install version. </summary>
        ///
        /// <value> to install version. </value>

        public string ToInstallVersion { get; }

        /// <summary>   Gets or sets the existing version. </summary>
        ///
        /// <value> The existing version. </value>

        public string ExistingVersion { get; set; }

        /// <summary>   Gets or sets the latest version. </summary>
        ///
        /// <value> The latest version. </value>

        public string LatestVersion { get; internal set; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>
        ///
        /// <param name="dependencyType">       Type of the dependency. </param>
        /// <param name="packageName">          Name of the package. </param>
        /// <param name="importHandlerName">    Name of the import handler. </param>
        /// <param name="toInstallName">        Name of to install. </param>
        /// <param name="toInstallVersion">     to install version. </param>

        public PackageRecord(string dependencyType, string packageName, string importHandlerName, string toInstallName, NpmVersion toInstallVersion)
        {
            DependencyType = dependencyType;
            PackageTypeName = packageName;
            ImportHandlerName = importHandlerName;
            PackageName = toInstallName;
            ToInstallVersion = toInstallVersion.ToString();
        }

        /// <summary>   Gets the columns. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>
        ///
        /// <returns>   The columns. </returns>

        public static string GetColumns()
        {
            var type = typeof(PackageRecord);
            var builder = new StringBuilder();

            foreach (var property in type.GetProperties())
            {
                builder.AppendWithLeadingIfLength("\t", property.Name);
            }

            builder.AppendLine();

            return builder.ToString();
        }

        /// <summary>   Returns a string that represents the current object. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>
        ///
        /// <returns>   A string that represents the current object. </returns>

        public override string ToString()
        {
            var type = typeof(PackageRecord);
            var builder = new StringBuilder();

            foreach (var property in type.GetProperties())
            {
                builder.AppendWithLeadingIfLength("\t", this.GetPropertyValue<string>(property.Name).AsDisplayText());
            }

            builder.AppendLine();

            return builder.ToString();
        }
    }
}
