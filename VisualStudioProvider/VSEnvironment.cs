using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using Utils;

namespace VisualStudioProvider
{
    public static class VSEnvironment
    {
        public static string ExpandEnvironmentVariables(string str)
        {
            var builder = new StringBuilder(str);
            var regex = new Regex(@"(?<varmacro>\$\((?<variable>[^)]*)\))");

            if (regex.IsMatch(str))
            {
                foreach (Match match in regex.Matches(str))
                {
                    var var = match.Groups["variable"].Value;
                    var macro = match.Groups["varmacro"].Value;

                    // all hard coded for now

                    switch (var)
                    {
                        case "VCInstallDir":

                            builder.Replace(macro, @"%PROGRAMFILES(X86)%\Microsoft Visual Studio 10.0\VC\".Expand(), match.Index, macro.Length);
                            break;

                        case "SdkDir":

                            builder.Replace(macro, @"%PROGRAMFILES(X86)%\Microsoft SDKs\Windows\v7.0A\".Expand(), match.Index, macro.Length);
                            break;

                        case "FrameworkSDKDir":
                        case "WindowsSdkDir":

                            builder.Replace(macro, @"%PROGRAMFILES(X86)%\Microsoft SDKs\Windows\v7.0A\".Expand(), match.Index, macro.Length);
                            break;

                        default:
                            Debugger.Break();
                            break;
                    }
                }
            }

            return builder.ToString();
        }
    }
}
