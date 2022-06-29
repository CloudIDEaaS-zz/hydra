using AbstraX;
using CoreShim.Reflection;
using NetCoreReflectionShim.Agent;
using NetCoreReflectionShim.Service;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Reflection;
using Utils;

namespace NetCoreReflectionShim.Test
{
    public class Program
    {
        public static void Main(string[] args)
        {
            TestRuntimeProxy();

            using (var inputServerStream = new AnonymousPipeServerStream(PipeDirection.In, HandleInheritability.Inheritable))
            {
                using (var outputServerStream = new AnonymousPipeServerStream(PipeDirection.Out, HandleInheritability.Inheritable))
                {
                    var hydraSolutionPath = Path.GetFullPath(Environment.ExpandEnvironmentVariables("%HYDRASOLUTIONPATH%"));
                    var entitiesDllPath = new DirectoryInfo(Path.Combine(hydraSolutionPath, @"ApplicationGenerator\TestOutput\contoso.Entities\bin\Debug\netcoreapp3.1\contoso.Entities.dll"));
                    var entitiesAssembly = Assembly.LoadFrom(entitiesDllPath.FullName);
                    var agent = new NetCoreReflectionAgent(false, true);
                    var service = new StandardStreamService(false);
                    Type appResourcesType;
                    IAppResources appResources;
                    StreamReader reader;
                    StreamWriter writer;
                    List<Type> types;
                    List<Type> allTypes = null;
                    List<QueryInfo> queryInfos;
                    dynamic loginResources;
                    string title;

                    reader = new StreamReader(new AnonymousPipeClientStream(PipeDirection.In, outputServerStream.GetClientHandleAsString()));
                    writer = new StreamWriter(new AnonymousPipeClientStream(PipeDirection.Out, inputServerStream.GetClientHandleAsString()));

                    agent.OnStartService += (sender, e) =>
                    {
                        e.Reader = reader;
                        e.Writer = writer;

                        service.Start(Process.GetCurrentProcess(), Environment.CurrentDirectory, inputServerStream, outputServerStream);
                    };


                    service.Started += (sender, e) =>
                    {
                        var entryAssembly = Assembly.GetEntryAssembly();

                        allTypes = entryAssembly.GetAllTypes().Distinct().ToList();
                    };

                    service.GetTypeProxyEvent += (sender, e) =>
                    {
                        var typeProxy = allTypes.FindProxyType(e.TypeToProxy);

                        e.ProxyType = typeProxy;
                    };

                    agent.GetTypeProxyEvent += (sender, e) =>
                    {
                        var typeProxy = allTypes.FindProxyType(e.TypeToProxy);

                        e.ProxyType = typeProxy;
                    };

                    types = entitiesAssembly.GetAllTypes(agent).ToList();
                    appResourcesType = types.Single(t => t.Name == "AppResources");

                    appResources = agent.CreateInstance<IAppResources>((TypeShim)appResourcesType);
                    queryInfos = appResources.GetQueries();
                    loginResources = appResources.GetResources(AbstraX.DataAnnotations.UIKind.LoginPage);

                    service.Stop();
                }
            }
        }

        private static void TestRuntimeProxy()
        {
            IRuntime runtimeObj;
            var proxy = new RuntimeProxy();
            string name;

            runtimeObj = RuntimeProxyExtensions.WrapInstance<IRuntime>(proxy, string.Empty);

            runtimeObj.Test();
            runtimeObj.Name = "Hello";
            name = runtimeObj.Name;
        }
    }
}
