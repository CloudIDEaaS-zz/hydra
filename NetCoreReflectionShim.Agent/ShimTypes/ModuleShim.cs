using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreReflectionShim.Agent.ShimTypes
{
    public class ModuleShim : Module
    {
        private string parentIdentifier;
        private INetCoreReflectionAgent agent;

        public ModuleShim(string parentIdentifier, INetCoreReflectionAgent agent)
        {
            this.agent = agent;
            this.parentIdentifier = parentIdentifier;
        }

        public override IEnumerable<CustomAttributeData> CustomAttributes
        {
            get
            {
                return base.CustomAttributes;
            }
        }

        public override int MDStreamVersion
        {
            get
            {
                return base.MDStreamVersion;
            }
        }

        public override string FullyQualifiedName
        {
            get
            {
                return base.FullyQualifiedName;
            }
        }

        public override Guid ModuleVersionId
        {
            get
            {
                return base.ModuleVersionId;
            }
        }

        public override int MetadataToken
        {
            get
            {
                return base.MetadataToken;
            }
        }

        public override string ScopeName
        {
            get
            {
                return base.ScopeName;
            }
        }

        public override string Name
        {
            get
            {
                return base.Name;
            }
        }

        public override Assembly Assembly
        {
            get
            {
                return agent.GetAssembly(parentIdentifier);
            }
        }

        public override Type[] FindTypes(TypeFilter filter, object filterCriteria)
        {
            return base.FindTypes(filter, filterCriteria);
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            return base.GetCustomAttributes(inherit);
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return base.GetCustomAttributes(attributeType, inherit);
        }

        public override IList<CustomAttributeData> GetCustomAttributesData()
        {
            return base.GetCustomAttributesData();
        }

        public override FieldInfo GetField(string name, BindingFlags bindingAttr)
        {
            return base.GetField(name, bindingAttr);
        }

        public override FieldInfo[] GetFields(BindingFlags bindingFlags)
        {
            return base.GetFields(bindingFlags);
        }

        public override MethodInfo[] GetMethods(BindingFlags bindingFlags)
        {
            return base.GetMethods(bindingFlags);
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        public override void GetPEKind(out PortableExecutableKinds peKind, out ImageFileMachine machine)
        {
            base.GetPEKind(out peKind, out machine);
        }

        public override Type GetType(string className, bool ignoreCase)
        {
            return base.GetType(className, ignoreCase);
        }

        public override Type GetType(string className)
        {
            return base.GetType(className);
        }

        public override Type GetType(string className, bool throwOnError, bool ignoreCase)
        {
            return base.GetType(className, throwOnError, ignoreCase);
        }

        public override Type[] GetTypes()
        {
            return base.GetTypes();
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return base.IsDefined(attributeType, inherit);
        }

        public override bool IsResource()
        {
            return base.IsResource();
        }

        public override FieldInfo ResolveField(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
        {
            return base.ResolveField(metadataToken, genericTypeArguments, genericMethodArguments);
        }

        public override MemberInfo ResolveMember(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
        {
            return base.ResolveMember(metadataToken, genericTypeArguments, genericMethodArguments);
        }

        public override MethodBase ResolveMethod(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
        {
            return base.ResolveMethod(metadataToken, genericTypeArguments, genericMethodArguments);
        }

        public override byte[] ResolveSignature(int metadataToken)
        {
            return base.ResolveSignature(metadataToken);
        }

        public override string ResolveString(int metadataToken)
        {
            return base.ResolveString(metadataToken);
        }

        public override Type ResolveType(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
        {
            return base.ResolveType(metadataToken, genericTypeArguments, genericMethodArguments);
        }

        protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
        {
            return base.GetMethodImpl(name, bindingAttr, binder, callConvention, types, modifiers);
        }
    }
}
