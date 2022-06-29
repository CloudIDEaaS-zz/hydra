using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace CreateTest.NetCoreReflectionShim.Generators
{
    /// <summary>   An entity model generator. </summary>
    ///
    /// <remarks>   Ken, 10/11/2020. </remarks>

    public static class ReflectionGenerator
    {
        public static event ReflectMemberEventHandler ReflectMember;

        public static void GenerateClass(Type type)
        {
            var host = new TemplateEngineHost();
            var hydraSolutionPath = Path.GetFullPath(Environment.ExpandEnvironmentVariables("%HYDRASOLUTIONPATH%"));
            Dictionary<string, object> sessionVariables;
            FileInfo fileInfo;
            string fileLocation;
            string filePath;
            string output;

            try
            {
                sessionVariables = new Dictionary<string, object>();

                sessionVariables.Add("Type", type);
                sessionVariables.Add("ReflectMemberCallback", ReflectMember);

                fileLocation = Path.Combine(hydraSolutionPath, @"NetCoreReflectionShim.Service\JsonTypes");

                filePath = Path.Combine(fileLocation, type.Name + ".cs");
                fileInfo = new FileInfo(filePath);

                output = host.Generate<JsonReflectionClassTemplate>(sessionVariables, false);

                if (fileInfo.Exists)
                {
                    fileInfo.Delete();
                }

                File.WriteAllText(fileInfo.FullName, output);

                fileLocation = Path.Combine(hydraSolutionPath, @"NetCoreReflectionShim.Service\ShimTypes");

                filePath = Path.Combine(fileLocation, type.Name + ".cs");
                fileInfo = new FileInfo(filePath);

                output = host.Generate<ShimReflectionClassTemplate>(sessionVariables, false);

                if (fileInfo.Exists)
                {
                    fileInfo.Delete();
                }

                File.WriteAllText(fileInfo.FullName, output);
            }
            catch (Exception ex)
            {
                DebugUtils.Break();
            }
        }
    }
}
