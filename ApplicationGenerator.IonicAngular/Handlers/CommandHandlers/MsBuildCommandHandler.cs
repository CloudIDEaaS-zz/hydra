using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX.Handlers.CommandHandlers
{
    public class MsBuildCommandHandler : BaseWindowsCommandHandler
    {
        public MsBuildCommandHandler() : base(MsBuildCommandHandler.GetMsBuildLocation())
        {
        }

        public void Build(string solutionFile, params KeyValuePair<string, string>[] properties)
        {
            var currentDirectory = Environment.CurrentDirectory;

            base.RunCommand(solutionFile, currentDirectory, properties.Select(p => string.Format("-property:{0}={1}", p.Key, p.Value)).ToArray());
        }

        private static string GetMsBuildLocation()
        {
            //var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\MSBuild\ToolsVersions").ToIndexable();
            //var subKey = key.SubKeys.Where(k => k.SubName.RegexIsMatch(@"\d*?\.\d*")).OrderBy(k => Convert.ToSingle(k.SubName)).Last();

            //if (subKey.Values.Any(v => v.Key == "MSBuildToolsPath"))
            //{
            //    var value = subKey.Values.Single(k => k.Key == "MSBuildToolsPath").Value.ToString();

            //    return Path.Combine(value, "MSBuild.exe");
            //}

            return "C:\\Program Files (x86)\\Microsoft Visual Studio\\2019\\Community\\MSBuild\\Current\\Bin\\MSBuild.exe";
        }

        public void Restore(string solutionFile, params KeyValuePair<string, string>[] properties)
        {
            var currentDirectory = Environment.CurrentDirectory;

            base.RunCommand("-t:restore", currentDirectory, new string[] { solutionFile }.Concat(properties.Select(p => string.Format("-property:{0}={1}", p.Key, p.Value))).ToArray());
        }
    }
}
