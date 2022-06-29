using AbstraX;
using NetCoreReflectionShim.Agent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Utils;

namespace NetCoreReflectionShim.Test
{
    public static class Extensions
    {
        public static IEnumerable<Type> GetAllTypes(this Assembly assembly, NetCoreReflectionAgent netCoreReflectionAgent)
        {
            var thisAssembly = Assembly.GetEntryAssembly();
            var isCore = false;

            assembly = netCoreReflectionAgent.LoadCoreAssembly(assembly, new Dictionary<string, string>() { { "AbstraX.DataAnnotations", "AbstraX.DataAnnotations" } });
            isCore = true;

            foreach (var type in assembly.GetTypes().Concat(assembly.GetExportedTypes()).Distinct())
            {
                yield return type;
            }

            foreach (var assemblyName in assembly.GetReferencedAssemblies())
            {
                if (isCore)
                {
                    var refAssembly = netCoreReflectionAgent.LoadCoreAssembly(assemblyName, new Dictionary<string, string>() { { "AbstraX.DataAnnotations", "AbstraX.DataAnnotations" } });

                    if (refAssembly.HasCustomAttribute<EntityMetadataSourceAssemblyAttribute>())
                    {
                        foreach (var type in refAssembly.GetTypes())
                        {
                            yield return type;
                        }
                    }
                }
                else
                {
                    var refAssembly = Assembly.Load(assemblyName);

                    if (refAssembly.HasCustomAttribute<EntityMetadataSourceAssemblyAttribute>())
                    {
                        foreach (var type in refAssembly.GetTypes())
                        {
                            yield return type;
                        }
                    }
                }
            }

            foreach (var type in thisAssembly.GetTypes().Concat(thisAssembly.GetExportedTypes()).Distinct())
            {
                yield return type;
            }

            foreach (var assemblyName in thisAssembly.GetReferencedAssemblies().Where(a => a.Name == "ApplicationGenerator.Interfaces"))
            {
                var refAssembly = Assembly.Load(assemblyName);

                foreach (var assemblyName2 in thisAssembly.GetReferencedAssemblies().Where(a => a.Name == "System.ComponentModel.DataAnnotations" || a.Name == "System"))
                {
                    var refAssembly2 = Assembly.Load(assemblyName2);

                    foreach (var type in refAssembly2.GetTypes().Concat(refAssembly2.GetExportedTypes()).Distinct())
                    {
                        yield return type;
                    }
                }

                foreach (var type in refAssembly.GetTypes().Concat(refAssembly.GetExportedTypes()).Distinct())
                {
                    yield return type;
                }
            }
        }
    }
}
