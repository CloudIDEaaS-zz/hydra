using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Security.Claims;
using System.Threading.Tasks;
using Utils;
using CoreShim.Reflection.JsonTypes;
using System.Reflection;
using System.IO;
using System.Globalization;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using CustomAttributeData = System.Reflection.CustomAttributeData;
using System.Security.Policy;
using System.Security;
using NetCoreReflectionShim.Agent;

namespace CoreShim.Reflection
{
    public class AssemblyShim : Assembly
    {
        private AssemblyJson assembly;
        private INetCoreReflectionAgent agent;
        private string parentIdentifier;

        public AssemblyShim(AssemblyJson assembly, string parentIdentifier, INetCoreReflectionAgent agent)
        {
            this.assembly = assembly;
            this.agent = agent;
            this.parentIdentifier = parentIdentifier;
        } 

        public override string CodeBase 
        { 
            get
            {
                return assembly.CodeBase;
            }
        }

        public override string EscapedCodeBase 
        { 
            get
            {
                return assembly.EscapedCodeBase;
            }
        }

        public override string FullName 
        { 
            get
            {
                return assembly.FullName;
            }
        }

        public override MethodInfo EntryPoint 
        { 
            get
            {
                throw new NotImplementedException();
            }
        }

        public new bool IsFullyTrusted 
        { 
            get
            {
                return assembly.IsFullyTrusted;
            }
        }

        public override System.Security.SecurityRuleSet SecurityRuleSet
        { 
            get
            {
                return EnumUtils.GetValue<System.Security.SecurityRuleSet>(assembly.SecurityRuleSetEnum);
            }
        }

        public override Module ManifestModule 
        { 
            get
            {
                throw new NotImplementedException();
            }
        }

        public override bool ReflectionOnly 
        { 
            get
            {
                return assembly.ReflectionOnly;
            }
        }

        public override string Location 
        { 
            get
            {
                return assembly.Location;
            }
        }

        public override string ImageRuntimeVersion 
        { 
            get
            {
                return assembly.ImageRuntimeVersion;
            }
        }

        public override bool GlobalAssemblyCache 
        { 
            get
            {
                return assembly.GlobalAssemblyCache;
            }
        }

        public override long HostContext 
        { 
            get
            {
                return assembly.HostContext;
            }
        }

        public override bool IsDynamic 
        { 
            get
            {
                return assembly.IsDynamic;
            }
        }

        public override bool Equals(object o)
        {
            return o.GetHashCode() == this.GetHashCode();
        }
        public override int GetHashCode()
        {
            return assembly.GetHashCodeMember;
        }
        public override AssemblyName GetName()
        {
            throw new NotImplementedException();
        }
        public override AssemblyName GetName(bool copiedName)
        {
            throw new NotImplementedException();
        }
        public override Type[] GetExportedTypes()
        {
            return agent.MapTypes(agent.Assembly_GetExportedTypes(parentIdentifier));
        }
        public override Type[] GetTypes()
        {
            return agent.MapTypes(agent.Assembly_GetTypes(parentIdentifier));
        }
        public override Stream GetManifestResourceStream(Type type, string name)
        {
            throw new NotImplementedException();
        }
        public override Stream GetManifestResourceStream(string name)
        {
            throw new NotImplementedException();
        }
        public override Assembly GetSatelliteAssembly(CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        public override Assembly GetSatelliteAssembly(CultureInfo culture, Version version)
        {
            throw new NotImplementedException();
        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }
        public override Object[] GetCustomAttributes(bool inherit)
        {
            return agent.Assembly_GetCustomAttributes(parentIdentifier, inherit);
        }
        public override Object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return agent.Assembly_GetCustomAttributes(parentIdentifier, attributeType, inherit);
        }
        public override bool IsDefined(Type attributeType, bool inherit)
        {
            throw new NotImplementedException();
        }
        public override IList<CustomAttributeData> GetCustomAttributesData()
        {
            throw new NotImplementedException();
        }
        public new Module LoadModule(string moduleName, Byte[] rawModule)
        {
            throw new NotImplementedException();
        }
        public override Module LoadModule(string moduleName, Byte[] rawModule, Byte[] rawSymbolStore)
        {
            throw new NotImplementedException();
        }
        public new object CreateInstance(string typeName)
        {
            throw new NotImplementedException();
        }
        public new object CreateInstance(string typeName, bool ignoreCase)
        {
            throw new NotImplementedException();
        }
        public override object CreateInstance(string typeName, bool ignoreCase, BindingFlags bindingAttr, Binder binder, Object[] args, CultureInfo culture, Object[] activationAttributes)
        {
            throw new NotImplementedException();
        }
        public new Module[] GetLoadedModules()
        {
            throw new NotImplementedException();
        }
        public override Module[] GetLoadedModules(bool getResourceModules)
        {
            throw new NotImplementedException();
        }
        public new Module[] GetModules()
        {
            throw new NotImplementedException();
        }
        public override Module[] GetModules(bool getResourceModules)
        {
            throw new NotImplementedException();
        }
        public override Module GetModule(string name)
        {
            throw new NotImplementedException();
        }
        public override FileStream GetFile(string name)
        {
            throw new NotImplementedException();
        }
        public override FileStream[] GetFiles()
        {
            throw new NotImplementedException();
        }
        public override FileStream[] GetFiles(bool getResourceModules)
        {
            throw new NotImplementedException();
        }
        public override String[] GetManifestResourceNames()
        {
            throw new NotImplementedException();
        }
        public override AssemblyName[] GetReferencedAssemblies()
        {
            return agent.Assembly_GetReferencedAssemblies(parentIdentifier);
        }
        public override ManifestResourceInfo GetManifestResourceInfo(string resourceName)
        {
            throw new NotImplementedException();
        }
        public override string ToString()
        {
            return assembly.ToStringMember;
        }
    }
}
