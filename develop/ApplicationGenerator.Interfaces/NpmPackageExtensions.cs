// file:	NpmPackage.cs
//
// summary:	Implements the npm package class

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX.PackageExtensions
{
    /// <summary>   A npm package extensions. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 2/1/2021. </remarks>

    public static class NpmPackageExtensions
    {
        /// <summary>   A DirectoryInfo extension method that matches package name. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 2/1/2021. </remarks>
        ///
        /// <param name="directoryInfo">    The directoryInfo to act on. </param>
        /// <param name="packageName">      Name of the package. </param>
        ///
        /// <returns>   True if matches package name, false if not. </returns>

        public static bool MatchesPackageName(this DirectoryInfo directoryInfo, string packageName)
        {
            return directoryInfo.Name.RegexIsMatch("^" + packageName + @"(?![\w-_\.])");
        }
    }
}
