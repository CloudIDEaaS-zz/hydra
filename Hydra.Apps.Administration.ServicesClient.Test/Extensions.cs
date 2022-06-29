using AbstraX;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Hydra.Apps.Administration.ServicesClient.Test
{
    public static class Extensions
    {
        public static void Scan(this AppDomain appDomain)
        {
            var process = Process.GetCurrentProcess();
            var platformProcess = process.GetPlatformProcess();
            var directory = new DirectoryInfo(Path.GetDirectoryName(platformProcess.Path));
            var dllFile = directory.GetFiles("Hydra.Scanner.dll").Single();
            var hModule = ProcessExtensions.LoadLibrary(dllFile.FullName);
            var scanFunction = ProcessExtensions.GetModuleFunction<Action>(hModule, "Scan");
            var method = typeof(AbstraXExtensions).GetMethod("GetKey");
            var keyBefore = AbstraXExtensions.GetKey().Result;
            byte[] keyAfter;

            Console.WriteLine("Key before: {0}", keyBefore.GetHexString());

            scanFunction();

            keyAfter = AbstraXExtensions.GetKey().Result;

            Console.WriteLine("Key after: {0}", keyAfter.GetHexString());
        }
    }
}