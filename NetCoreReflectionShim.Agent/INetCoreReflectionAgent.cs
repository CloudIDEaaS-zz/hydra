using AbstraX;
using CoreShim.Reflection;
using CoreShim.Reflection.JsonTypes;
using System;
using System.Collections.Generic;
using System.Reflection;
using Utils;

namespace NetCoreReflectionShim.Agent
{
    public interface INetCoreReflectionAgent : ITypeShimActivator, IRuntimeProxy, IDisposable
    {
        TypeCache TypeCache { get; }
        bool TestMode { get; set; }
        Dictionary<string, string> RedirectedNamespaces { get; }
        Dictionary<string, Type> CachedTypes { get; }
        Assembly LoadCoreAssembly(Assembly assembly, Dictionary<string, string> redirectedNamespaces);
        System.Object[] ParameterInfo_GetCustomAttributes(string identifier, bool inherit);
        System.Object[] ParameterInfo_GetCustomAttributes(string identifier, Type attributeType, bool inherit);
        System.Object[] MemberInfo_GetCustomAttributes(string identifier, bool inherit);
        System.Object[] MemberInfo_GetCustomAttributes(string identifier, Type attributeType, bool inherit);
        System.Reflection.PropertyInfo[] Type_GetProperties(string identifier);
        System.Reflection.PropertyInfo[] Type_GetProperties(string identifier, BindingFlags bindingAttr);
        System.Type[] Assembly_GetExportedTypes(string identifier);
        System.Type[] Assembly_GetTypes(string identifier);
        System.Object[] Assembly_GetCustomAttributes(string identifier, bool inherit);
        System.Object[] Assembly_GetCustomAttributes(string identifier, Type attributeType, bool inherit);
        System.Reflection.AssemblyName[] Assembly_GetReferencedAssemblies(string identifier);
        Type GetType(string typeFullName);
        Assembly GetAssembly(string parentIdentifier);
    }
}
