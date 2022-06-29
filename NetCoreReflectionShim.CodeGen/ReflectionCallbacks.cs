using NetCoreReflectionShim.CodeGen.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace NetCoreReflectionShim.CodeGen
{
    public static class ReflectionCallbacks
    {
        public static Dictionary<int, ApiMember> APIMembers { get; private set; }

        static ReflectionCallbacks()
        {
            APIMembers = new Dictionary<int, ApiMember>();
        }

        public static void ReflectionGenerator_GetApiBody(object sender, GetApiBodyEventArgs e)
        {
            var member = e.Member;
            var body = ReflectionGenerator.GenerateApiBody(member);

            e.Code = body;
        }

        public static void JsonReflectionGenerator_ReflectMember(object sender, ReflectMemberEventArgs e)
        {
            var typeName = e.MemberInfo.GetOwningType().Name;

            switch (e.MemberInfo.Name)
            {
                case "ToString":
                case "GetHashCode":

                    var owningType = e.MemberInfo.GetOwningType();

                    if (owningType.GetMembers().Any(m => m != e.MemberInfo && m.Name == e.MemberInfo.Name))
                    {
                        return;
                    }

                    if (e.ReflectKind == ReflectKind.ShimTypes)
                    {
                        APIMembers.AddIfNotExists(e, true);
                        return;
                    }
                    else if (e.ReflectKind == ReflectKind.JsonTypes)
                    {
                        e.CacheResult = true;

                        if (e.NoShim)
                        {
                            APIMembers.AddIfNotExists(e, true);
                        }

                        return;
                    }

                    break;
            }

            switch (typeName)
            {
                case "AppResources":  /**************************************************/

                    switch (e.MemberInfo.Name)
                    {
                        case "GetQueries":
                        case "GetResources":

                            if (e.ReflectKind == ReflectKind.ShimTypes)
                            {
                                APIMembers.AddIfNotExists(e, false);
                            }

                            break;

                        default:
                            break;
                    }

                    break;

                case "Assembly":  /**************************************************/

                    switch (e.MemberInfo.Name)
                    {
                        case "EntryPoint":
                        case "ManifestModule":
                        case "GetForwardedTypes":
                        case "GetManifestResourceInfo":
                        case "GetManifestResourceNames":
                        case "GetManifestResourceStream":
                        case "GetCustomAttributesData":
                        case "GetName":
                        case "IsDefined":
                        case "CreateInstance":
                        case "GetModule":
                        case "GetModules":
                        case "GetLoadedModules":
                        case "GetSatelliteAssembly":
                        case "GetObjectData":
                        case "LoadModule":
                        case "GetFile":
                        case "GetFiles":
                        case "Equals":
                        case "GetType":
                        case "Evidence":
                        case "PermissionSet":
                        case "ExportedTypes":
                        case "DefinedTypes":
                        case "CustomAttributes":
                        case "Modules":
                        case "Finalize":
                        case "MemberwiseClone":
                            break;
                        case "GetCustomAttributes":

                            if (e.ReflectKind == ReflectKind.ShimTypes)
                            {
                                var apiMember = APIMembers.AddIfNotExists(e, false, true);

                                apiMember.ReturnElementTypeTextOverride = "Attribute";
                                apiMember.ReturnElementTypeOverride = typeof(Attribute);
                                apiMember.NoCacheResult = true;

                                ReflectionGenerator.GenerateClass(typeof(System.Reflection.CustomAttributeData), true);
                            }

                            break;

                        case "GetExportedTypes":

                            if (e.ReflectKind == ReflectKind.ShimTypes)
                            {
                                APIMembers.AddIfNotExists(e, false);
                                return;
                            }

                            break;

                        case "GetReferencedAssemblies":

                            if (e.ReflectKind == ReflectKind.ShimTypes)
                            {
                                APIMembers.AddIfNotExists(e, false, true);
                            }
                            else
                            {
                                ReflectionGenerator.GenerateClass(typeof(AssemblyName), true);
                            }

                            break;

                        case "GetTypes":

                            if (e.ReflectKind == ReflectKind.ShimTypes)
                            {
                                APIMembers.AddIfNotExists(e);
                                return;
                            }
                            else
                            {
                                ReflectionGenerator.GenerateClass(typeof(Type));
                            }

                            break;

                        default:

                            if (e.MemberInfo is MethodInfo methodInfo)
                            {
                                switch (methodInfo.Name)
                                {
                                    default:

                                        if (!methodInfo.IsAccessor())
                                        {
                                            DebugUtils.Break();
                                        }

                                        break;
                                }
                            }
                            else
                            {
                                DebugUtils.Break();
                            }

                            break;
                    }

                    break;

                case "AssemblyName":  /**************************************************/

                    switch (e.MemberInfo.Name)
                    {
                        default:
                            break;
                    }

                    break;

                case "ConstructorInfo":  /**************************************************/

                    switch (e.MemberInfo.Name)
                    {
                        case "":
                            break;
                        default:

                            if (e.MemberInfo is MethodInfo methodInfo)
                            {
                                switch (methodInfo.Name)
                                {
                                    case "Invoke":
                                    case "Equals":
                                    case "GetHashCode":
                                    case "GetType":
                                    case "Finalize":
                                    case "MemberwiseClone":
                                    case "GetReturnType":
                                        break;
                                    case "":

                                        if (e.ReflectKind == ReflectKind.ShimTypes)
                                        {
                                            e.Code = "throw new NotImplementedException();";
                                            return;
                                        }
                                        else
                                        {
                                            ReflectionGenerator.GenerateClass(typeof(ConstructorInfo));
                                            break;
                                        }

                                    default:

                                        if (!methodInfo.IsAccessor())
                                        {
                                            DebugUtils.Break();
                                        }

                                        break;
                                }
                            }
                            else
                            {
                                DebugUtils.Break();
                            }

                            break;
                    }

                    break;

                case "CustomAttributeData":  /**************************************************/

                    switch (e.MemberInfo.Name)
                    {
                        case "Equals":
                        case "GetType":
                        case "Finalize":
                        case "MemberwiseClone":
                            break;
                        case "ConstructorArguments":

                            if (e.ReflectKind == ReflectKind.JsonTypes)
                            {
                                e.JsonPropertyType = "List<CustomAttributeTypedArgumentJson>";
                                ReflectionGenerator.GenerateClass(typeof(System.Reflection.CustomAttributeTypedArgument), true);
                            }

                            break;

                        case "NamedArguments":

                            if (e.ReflectKind == ReflectKind.JsonTypes)
                            {
                                e.JsonPropertyType = "List<CustomAttributeNamedArgumentJson>";
                                ReflectionGenerator.GenerateClass(typeof(System.Reflection.CustomAttributeNamedArgument), true);
                            }

                            break;

                        case "Constructor":

                            if (e.ReflectKind == ReflectKind.JsonTypes)
                            {
                                e.JsonPropertyType = "ConstructorInfoJson";
                                ReflectionGenerator.GenerateClass(typeof(ConstructorInfo), true);
                            }

                            break;

                        case "AttributeType":

                            e.JsonPropertyType = "string";
                            break;

                        default:

                            if (e.MemberInfo is MethodInfo methodInfo)
                            {
                                switch (methodInfo.Name)
                                {
                                    case "Invoke":
                                    case "Equals":
                                    case "GetHashCode":
                                    case "GetType":
                                        break;
                                    case "":

                                        if (e.ReflectKind == ReflectKind.ShimTypes)
                                        {
                                            e.Code = "throw new NotImplementedException();";
                                            return;
                                        }
                                        else
                                        {
                                            break;
                                        }

                                    default:

                                        if (!methodInfo.IsAccessor())
                                        {
                                            DebugUtils.Break();
                                        }

                                        break;
                                }
                            }
                            else
                            {
                                DebugUtils.Break();
                            }

                            break;
                    }

                    break;

                case "CustomAttributeNamedArgument":  /**************************************************/

                    switch (e.MemberInfo.Name)
                    {
                        case "Finalize":
                        case "MemberwiseClone":
                            break;
                        case "MemberInfo":
                        case "TypedValue":

                            e.JsonPropertyType = "string";
                            break;

                        default:

                            if (e.MemberInfo is MethodInfo methodInfo)
                            {
                                switch (methodInfo.Name)
                                {
                                    case "Invoke":
                                    case "Equals":
                                    case "GetHashCode":
                                    case "GetType":
                                        break;
                                    case "":

                                        if (e.ReflectKind == ReflectKind.ShimTypes)
                                        {
                                            e.Code = "throw new NotImplementedException();";
                                            return;
                                        }
                                        else
                                        {
                                            break;
                                        }

                                    default:

                                        if (!methodInfo.IsAccessor())
                                        {
                                            DebugUtils.Break();
                                        }

                                        break;
                                }

                            }

                            break;
                    }

                    break;

                case "CustomAttributeTypedArgument":  /**************************************************/

                    switch (e.MemberInfo.Name)
                    {
                        case "Finalize":
                        case "MemberwiseClone":
                            break;
                        case "ArgumentType":

                            e.JsonPropertyType = "string";
                            break;

                        default:

                            if (e.MemberInfo is MethodInfo methodInfo)
                            {
                                switch (methodInfo.Name)
                                {
                                    case "Invoke":
                                    case "Equals":
                                    case "GetHashCode":
                                    case "GetType":
                                    case "Finalize":
                                    case "MemberwiseClone":
                                        break;
                                    case "":

                                        if (e.ReflectKind == ReflectKind.ShimTypes)
                                        {
                                            e.Code = "throw new NotImplementedException();";
                                            return;
                                        }
                                        else
                                        {
                                            break;
                                        }

                                    default:

                                        if (!methodInfo.IsAccessor())
                                        {
                                            DebugUtils.Break();
                                        }

                                        break;
                                }

                            }

                            break;
                    }
                    
                    break;

                case "MemberInfo":  /**************************************************/

                    switch (e.MemberInfo.Name)
                    {
                        case "DeclaringType":
                        case "ReflectedType":
                        case "HasSameMetadataDefinitionAs":
                        case "IsDefined":
                        case "GetCustomAttributesData":
                        case "Module":
                        case "Equals":
                        case "GetType":
                        case "CustomAttributes":
                        case "CacheEquals":
                        case "Finalize":
                        case "MemberwiseClone":
                            break;
                        case "GetCustomAttributes":

                            if (e.ReflectKind == ReflectKind.ShimTypes)
                            {
                                var apiMember = APIMembers.AddIfNotExists(e, false, true);

                                apiMember.ReturnElementTypeTextOverride = "Attribute";
                                apiMember.ReturnElementTypeOverride = typeof(Attribute);
                                apiMember.NoCacheResult = true;
                            }

                            break;

                        default:

                            if (e.MemberInfo is MethodInfo methodInfo)
                            {
                                switch (methodInfo.Name)
                                {
                                    case "zzzzzzzzzzzzzzzzz":
                                        break;
                                    case "":

                                        if (e.ReflectKind == ReflectKind.ShimTypes)
                                        {
                                            e.Code = "throw new NotImplementedException();";
                                            return;
                                        }
                                        else
                                        {
                                            ReflectionGenerator.GenerateClass(typeof(ConstructorInfo));
                                            break;
                                        }

                                    default:

                                        if (!methodInfo.IsAccessor())
                                        {
                                            DebugUtils.Break();
                                        }

                                        break;
                                }
                            }
                            else
                            {
                                DebugUtils.Break();
                            }

                            break;
                    }

                    break;

                case "MethodBase":  /**************************************************/

                    switch (e.MemberInfo.Name)
                    {
                        case "MethodHandle":
                        case "GetParameters":

                            if (e.ReflectKind == ReflectKind.JsonTypes)
                            {
                                e.JsonPropertyType = "List<ParameterInfoJson>";
                                e.CacheResult = true;

                                ReflectionGenerator.GenerateClass(typeof(ParameterInfo), true);
                            }

                            break;

                        case "GetMethodImplementationFlags":
                        case "GetMethodBody":
                        case "GetGenericArguments":
                        case "Invoke":
                        case "Equals":
                        case "GetType":
                        case "GetParametersNoCopy":
                        case "FormatNameAndSig":
                        case "GetParameterTypes":
                        case "CheckArguments":
                        case "Finalize":
                        case "MemberwiseClone":
                            break;
                        default:

                            if (e.MemberInfo is MethodInfo methodInfo)
                            {
                                switch (methodInfo.Name)
                                {
                                    case "zzzzzzzzzzzzzzzzz":
                                        break;
                                    case "":

                                        if (e.ReflectKind == ReflectKind.ShimTypes)
                                        {
                                            e.Code = "throw new NotImplementedException();";
                                            return;
                                        }
                                        else
                                        {
                                            ReflectionGenerator.GenerateClass(typeof(ConstructorInfo));
                                            break;
                                        }

                                    default:

                                        if (!methodInfo.IsAccessor())
                                        {
                                            DebugUtils.Break();
                                        }

                                        break;
                                }
                            }
                            else
                            {
                                DebugUtils.Break();
                            }

                            break;
                    }

                    break;

                case "MethodInfo":  /**************************************************/

                    switch (e.MemberInfo.Name)
                    {
                        case "ReturnTypeCustomAttributes":
                        case "ReturnType":
                        case "GetGenericArguments":
                        case "GetGenericMethodDefinition":
                        case "MakeGenericMethod":
                        case "GetBaseDefinition":
                        case "CreateDelegate":
                        case "Equals":
                        case "GetType":
                        case "Finalize":
                        case "MemberwiseClone":
                            break;
                        case "ReturnParameter":

                            if (e.ReflectKind == ReflectKind.ShimTypes)
                            {
                                e.Code = "throw new NotImplementedException();";
                                return;
                            }
                            else
                            {
                                ReflectionGenerator.GenerateClass(typeof(ParameterInfo));
                                break;
                            }

                        default:

                            if (e.MemberInfo is MethodInfo methodInfo)
                            {
                                switch (methodInfo.Name)
                                {
                                    case "":

                                        if (e.ReflectKind == ReflectKind.ShimTypes)
                                        {
                                            e.Code = "throw new NotImplementedException();";
                                            return;
                                        }
                                        else
                                        {
                                            ReflectionGenerator.GenerateClass(typeof(ConstructorInfo));
                                        }

                                        break;

                                    default:

                                        if (!methodInfo.IsAccessor())
                                        {
                                            DebugUtils.Break();
                                        }

                                        break;
                                }
                            }
                            else
                            {
                                DebugUtils.Break();
                            }

                            break;
                    }

                    break;


                case "ParameterInfo":  /**************************************************/

                    switch (e.MemberInfo.Name)
                    {
                        case "ParameterType":

                            if (e.ReflectKind == ReflectKind.JsonTypes)
                            {
                                e.JsonPropertyType = "string";
                                e.CacheResult = true;
                            }

                            break;

                        case "Member":
                        case "IsDefined":
                        case "GetOptionalCustomModifiers":
                        case "GetRequiredCustomModifiers":
                        case "GetRealObject":
                        case "GetCustomAttributesData":
                        case "ReturnParameter":
                        case "Equals":
                        case "GetType":
                        case "CustomAttributes":
                        case "SetName":
                        case "SetAttributes":
                        case "Attributes":
                        case "Finalize":
                        case "MemberwiseClone":
                            break;
                        case "GetCustomAttributes":

                            if (e.ReflectKind == ReflectKind.ShimTypes)
                            {
                                var apiMember = APIMembers.AddIfNotExists(e, false, true);

                                apiMember.ReturnElementTypeTextOverride = "Attribute";
                                apiMember.ReturnElementTypeOverride = typeof(Attribute);
                                apiMember.NoCacheResult = true;
                            }

                            break;

                        default:

                            if (e.MemberInfo is MethodInfo methodInfo)
                            {
                                switch (methodInfo.Name)
                                {
                                    case "":

                                        if (e.ReflectKind == ReflectKind.ShimTypes)
                                        {
                                            e.Code = "throw new NotImplementedException();";
                                            return;
                                        }
                                        else
                                        {
                                            ReflectionGenerator.GenerateClass(typeof(ConstructorInfo));
                                            break;
                                        }

                                    default:

                                        if (!methodInfo.IsAccessor())
                                        {
                                            DebugUtils.Break();
                                        }

                                        break;
                                }
                            }
                            else
                            {
                                DebugUtils.Break();
                            }

                            break;
                    }

                    break;

                case "PropertyInfo":  /**************************************************/

                    switch (e.MemberInfo.Name)
                    {
                        case "PropertyType":
                        case "GetMethod":
                        case "SetMethod":
                        case "GetGetMethod":
                        case "GetSetMethod":
                        case "GetAccessors":
                        case "GetIndexParameters":
                        case "GetOptionalCustomModifiers":
                        case "GetRequiredCustomModifiers":
                        case "GetValue":
                        case "GetConstantValue":
                        case "GetRawConstantValue":
                        case "SetValue":
                        case "Equals":
                        case "GetType":
                        case "Finalize":
                        case "MemberwiseClone":
                            break;
                        default:

                            if (e.MemberInfo is MethodInfo methodInfo)
                            {
                                switch (methodInfo.Name)
                                {
                                    case "":

                                        if (e.ReflectKind == ReflectKind.ShimTypes)
                                        {
                                            e.Code = "throw new NotImplementedException();";
                                            return;
                                        }
                                        else
                                        {
                                            ReflectionGenerator.GenerateClass(typeof(ConstructorInfo));
                                            break;
                                        }

                                    default:

                                        if (!methodInfo.IsAccessor())
                                        {
                                            DebugUtils.Break();
                                        }

                                        break;
                                }
                            }
                            else
                            {
                                DebugUtils.Break();
                            }

                            break;
                    }

                    break;


                case "Type":  /**************************************************/

                    switch (e.MemberInfo.Name)
                    {
                        case "Assembly":
                        case "Module":
                        case "DeclaringType":
                        case "ReflectedType":
                        case "DeclaringMethod":
                        case "UnderlyingSystemType":
                        case "GenericTypeArguments":
                        case "StructLayoutAttribute":
                        case "TypeInitializer":
                        case "TypeHandle":
                        case "GetConstructors":
                        case "GetEvent":
                        case "GetEvents":
                        case "GetField":
                        case "GetFields":
                        case "GetMember":
                        case "GetMembers":
                        case "GetMethods":
                        case "GetNestedType":
                        case "GetNestedTypes":
                        case "GetDefaultMembers":
                        case "InvokeMember":
                        case "GetInterface":
                        case "GetInterfaceMap":
                        case "InstanceOfType":
                        case "IsInstanceOfType":
                        case "IsEquivalentTo":
                        case "GetEnumUnderlyingType":
                        case "GetEnumValues":
                        case "MakeArrayType":
                        case "MakeByRefType":
                        case "MakeGenericType":
                        case "MakePointerType":
                        case "IsEnumDefined":
                        case "GetEnumName":
                        case "GetEnumNames":
                        case "FindInterfaces":
                        case "FindMembers":
                        case "IsSubclassOf":
                        case "IsAssignableFrom":
                        case "Equals":
                        case "GetType":
                        case "GetElementType":
                        case "GetArrayRank":
                        case "GetGenericTypeDefinition":
                        case "GetGenericArguments":
                        case "GetGenericParameterConstraints":
                        case "GetConstructor":
                        case "GetTypeHandleInternal":
                        case "IsWindowsRuntimeObjectImpl":
                        case "IsExportedToWindowsRuntimeImpl":
                        case "HasProxyAttributeImpl":
                        case "GetRootElementType":
                        case "ImplementInterface":
                        case "FormatTypeName":
                            break;
                        case "GetMethod":

                            if (e.ReflectKind == ReflectKind.ShimTypes)
                            {
                                e.Code = "throw new NotImplementedException();";
                                return;
                            }
                            else
                            {
                                ReflectionGenerator.GenerateClass(typeof(MethodInfo));
                                break;
                            }

                        case "GetProperty":

                            if (e.ReflectKind == ReflectKind.ShimTypes)
                            {
                                e.Code = "throw new NotImplementedException();";
                                return;
                            }
                            else
                            {
                                ReflectionGenerator.GenerateClass(typeof(PropertyInfo));
                                break;
                            }
                        case "BaseType":

                            if (e.ReflectKind == ReflectKind.JsonTypes)
                            {
                                e.JsonPropertyType = "TypeJson";
                                e.CacheResult = true;
                            }
                            else if (e.ReflectKind == ReflectKind.ShimTypes)
                            {
                                e.Code = "return agent.MapType(() => new TypeShim(type.BaseType, parentIdentifier, agent));";
                                return;
                            }

                            break;

                        case "GetInterfaces":

                            if (e.ReflectKind == ReflectKind.JsonTypes)
                            {
                                e.JsonPropertyType = "List<TypeJson>";
                                e.CacheResult = true;
                            }
                            else if (e.ReflectKind == ReflectKind.ShimTypes)
                            {
                                e.Code = "return type.GetInterfaces.Select(i => new TypeShim(i, parentIdentifier, agent)).ToArray();";
                                return;
                            }

                            break;

                        case "GetAttributeFlagsImpl":
                        case "IsValueTypeImpl":

                            if (e.ReflectKind == ReflectKind.JsonTypes)
                            {
                                e.CacheResult = true;
                            }
                            else if (e.ReflectKind == ReflectKind.ShimTypes)
                            {
                                APIMembers.AddIfNotExists(e, true);
                                return;
                            }

                            break;

                        case "GetProperties":

                            if (e.ReflectKind == ReflectKind.ShimTypes)
                            {
                                APIMembers.AddIfNotExists(e);
                                return;
                            }

                            break;

                        case "GetCustomAttributes":

                            if (e.ReflectKind == ReflectKind.ShimTypes)
                            {
                                var apiMember = APIMembers.AddIfNotExists(e, false, true);

                                apiMember.ReturnElementTypeTextOverride = "Attribute";
                                apiMember.ReturnElementTypeOverride = typeof(Attribute);
                                apiMember.NoCacheResult = true;

                                ReflectionGenerator.GenerateClass(typeof(System.Reflection.CustomAttributeData), true);
                            }

                            break;

                        default:

                            if (e.MemberInfo is MethodInfo methodInfo)
                            {
                                switch (methodInfo.Name)
                                {

                                    default:

                                        if (methodInfo.IsFamily)
                                        {
                                        }
                                        else if (!methodInfo.IsAccessor())
                                        {
                                            DebugUtils.Break();
                                        }

                                        break;
                                }
                            }
                            else
                            {
                                DebugUtils.Break();
                            }

                            break;
                    }

                    break;


                default:

                    if (e.ReflectKind == ReflectKind.ShimTypes && e.Code == null)
                    {
                        switch (e.MemberInfo.Name)
                        {
                            case "Equals":
                                {
                                    e.Code = "throw new NotImplementedException();";
                                }

                                break;

                            case "GetHashCode":
                            case "ToString":

                                if (e.ReflectKind == ReflectKind.ShimTypes)
                                {
                                    APIMembers.AddIfNotExists(e, true);
                                    return;
                                }
                                else if (e.ReflectKind == ReflectKind.JsonTypes)
                                {
                                    e.CacheResult = true;
                                    return;
                                }

                                break;

                            default:
                                {
                                    DebugUtils.Break();
                                }

                                break;
                        }
                    }

                    break;
            }

            if (e.ReflectKind == ReflectKind.ShimTypes && e.Code == null)
            {
                e.Code = "throw new NotImplementedException();";
                return;
            }
        }
    }
}
