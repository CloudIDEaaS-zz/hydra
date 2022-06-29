// file:	Handlers\CommandHandlers\NugetCommandHandler.cs
//
// summary:	Implements the nuget command handler class

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX.CommandHandlers
{
    /// <summary>   A nuget command handler. </summary>
    ///
    /// <remarks>   Ken, 10/12/2020. </remarks>

    public class NpmCommandHandler : BaseWindowsCommandHandler
    {
        /// <summary>   Default constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>

        public NpmCommandHandler() : base("npm")
        {
        }

        /// <summary>   Shows the package version. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>
        ///
        /// <param name="package">  The package. </param>

        public void ShowPackageVersion(string package)
        {
            base.RunCommand("show", Environment.CurrentDirectory, package, "version");
        }

        /// <summary>   Configuration set. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 4/18/2021. </remarks>
        ///
        /// <param name="setting">  The setting. </param>
        /// <param name="value">    The value. </param>
        /// <param name="global">   (Optional) True to global. </param>

        public void ConfigSet(string setting, string value, bool global = true)
        {
            base.RunCommand("config set", Environment.CurrentDirectory, setting, value, global ? "--global" : string.Empty);
        }

        /// <summary>   Configuration get. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 4/18/2021. </remarks>
        ///
        /// <param name="setting">  The setting. </param>
        /// <param name="global">   (Optional) True to global. </param>
        ///
        /// <returns>   A string. </returns>

        public string ConfigGet(string setting, bool global = true)
        {
            var builder = new StringBuilder();

            this.OutputWriteLine = (format, args) =>
            {
                builder.AppendLineFormat(format, args);
            };

            this.ErrorWriteLine = (format, args) =>
            {
                builder.AppendLineFormat(format, args);
            };

            base.RunCommand("config get", Environment.CurrentDirectory, setting, global ? "--global" : string.Empty);

            this.Wait();

            return builder.ToString();
        }

        /// <summary>   Publishes this.  </summary>
        ///
        /// <remarks>   CloudIDEaaS, 6/28/2022. </remarks>

        public void Publish()
        {
            base.RunCommand("publish", Environment.CurrentDirectory);
        }
    }
}
