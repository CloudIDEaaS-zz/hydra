using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace NetCoreReflectionShim.CodeGen.Generators
{
    /// <summary>   An entity model generator. </summary>
    ///
    /// <remarks>   Ken, 10/11/2020. </remarks>

    public static class ReflectionGenerator
    {
        public static event ReflectMemberEventHandler ReflectMember;
        public static event GetApiBodyEventHandler GetApiBody;

        public static void GenerateApi(List<ApiMember> apiMembers)
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
                // client api

                sessionVariables = new Dictionary<string, object>();

                sessionVariables.Add("ApiMembers", apiMembers);
                sessionVariables.Add("GetApiBodyCallback", GetApiBody);

                fileLocation = Path.Combine(hydraSolutionPath, @"NetCoreReflectionShim.Agent");

                filePath = Path.Combine(fileLocation, "NetCoreReflectionAgent.ClientAPI.cs");
                fileInfo = new FileInfo(filePath);

                output = host.Generate<ShimReflectionClientApiClassTemplate>(sessionVariables, false);

                if (fileInfo.Exists)
                {
                    fileInfo.Delete();
                }

                File.WriteAllText(fileInfo.FullName, output);

                // client api interface

                filePath = Path.Combine(fileLocation, "INetCoreReflectionAgent.cs");
                fileInfo = new FileInfo(filePath);

                output = host.Generate<ShimReflectionClientApiInterfaceTemplate>(sessionVariables, false);

                if (fileInfo.Exists)
                {
                    fileInfo.Delete();
                }

                File.WriteAllText(fileInfo.FullName, output);

                // commands

                fileLocation = Path.Combine(hydraSolutionPath, @"NetCoreReflectionShim.Service");

                filePath = Path.Combine(fileLocation, "Commands.cs");
                fileInfo = new FileInfo(filePath);

                output = host.Generate<ShimReflectionCommandsClassTemplate>(sessionVariables, false);

                if (fileInfo.Exists)
                {
                    fileInfo.Delete();
                }

                File.WriteAllText(fileInfo.FullName, output);

                // server reflection extensions

                filePath = Path.Combine(fileLocation, "ReflectionExtensions.cs");
                fileInfo = new FileInfo(filePath);

                output = host.Generate<ServerReflectionExtensionsClassTemplate>(sessionVariables, false);

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

        public static string GenerateApiBody(ApiMember apiMember)
        {
            var host = new TemplateEngineHost();
            Dictionary<string, object> sessionVariables;
            string output;

            try
            {
                sessionVariables = new Dictionary<string, object>();

                sessionVariables.Add("ApiMember", apiMember);

                output = host.Generate<ShimReflectionClientAPIMethodBodyTemplate>(sessionVariables, false);

                return output;
            }
            catch (Exception ex)
            {
                DebugUtils.Break();
                return null;
            }
        }

        public static void GenerateClass(Type type, bool noShim = false)
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
                sessionVariables.Add("NoShim", noShim);

                fileLocation = Path.Combine(hydraSolutionPath, @"NetCoreReflectionShim.Service\JsonTypes");

                filePath = Path.Combine(fileLocation, type.Name + ".cs");
                fileInfo = new FileInfo(filePath);

                output = host.Generate<JsonReflectionClassTemplate>(sessionVariables, false);

                if (fileInfo.Exists)
                {
                    fileInfo.Delete();
                }

                File.WriteAllText(fileInfo.FullName, output);

                if (!noShim)
                {
                    fileLocation = Path.Combine(hydraSolutionPath, @"NetCoreReflectionShim.Agent\ShimTypes");

                    filePath = Path.Combine(fileLocation, type.Name + ".cs");
                    fileInfo = new FileInfo(filePath);

                    output = host.Generate<ShimReflectionClassTemplate>(sessionVariables, false);

                    if (fileInfo.Exists)
                    {
                        fileInfo.Delete();
                    }

                    File.WriteAllText(fileInfo.FullName, output);
                }
            }
            catch (Exception ex)
            {
                DebugUtils.Break();
            }
        }
    }
}
