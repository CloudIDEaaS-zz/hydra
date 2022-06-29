using AbstraX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Utils;
using AssemblyExtensions = Utils.AssemblyExtensions;

namespace AbstraX
{
    public static class Extensions
    {
        public static Type FindProxyType(this List<Type> types, Type type)
        {
            var proxyType = types.SingleOrDefault(t => t.HasCustomAttribute<TypeProxyAttribute>() && t.GetCustomAttribute<TypeProxyAttribute>().ProxiedType == type);

            return proxyType;
        }

        public static IEnumerable<Type> GetAllTypes(this Assembly assembly)
        {
            var thisAssembly = Assembly.GetEntryAssembly();

            AppDomain.CurrentDomain.AssemblyResolve += AssemblyExtensions.AssemblyResolve;

            foreach (var type in assembly.GetTypes().Concat(assembly.GetExportedTypes().Distinct()))
            {
                yield return type;
            }

            foreach (var assemblyName in assembly.GetReferencedAssemblies())
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

            foreach (var type in thisAssembly.GetTypes().Concat(thisAssembly.GetExportedTypes()).Distinct())
            {
                yield return type;
            }

            foreach (var assemblyName in thisAssembly.GetReferencedAssemblies().Where(a => a.Name == "Hydra.Interfaces"))
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
