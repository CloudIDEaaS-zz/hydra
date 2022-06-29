using CoreShim.Reflection.JsonTypes;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace CreateTest.NetCoreReflectionShim
{
    public static class ClientMapper
    {
        public static AssemblyJson Map(Assembly assembly)
        {
            return new AssemblyJson
            {
                CodeBase = assembly.CodeBase ?? throw new ArgumentNullException(nameof(assembly), "The value of 'assembly.CodeBase' should not be null"),
                FullName = assembly.FullName ?? throw new ArgumentNullException(nameof(assembly), "The value of 'assembly.FullName' should not be null"),
                ImageRuntimeVersion = assembly.ImageRuntimeVersion,
                IsDynamic = assembly.IsDynamic,
                Location = assembly.Location,
                ReflectionOnly = assembly.ReflectionOnly,
                IsCollectible = assembly.IsCollectible,
                IsFullyTrusted = assembly.IsFullyTrusted,
                EscapedCodeBase = assembly.EscapedCodeBase,
                GlobalAssemblyCache = assembly.GlobalAssemblyCache,
                HostContext = assembly.HostContext,
                SecurityRuleSetEnum = assembly.SecurityRuleSet.ToString()
            };
        }

        public static TypeJson Map(Type type)
        {
            return new TypeJson
            {
                IsInterface = type.IsInterface,
                Namespace = type.Namespace,
                AssemblyQualifiedName = type.AssemblyQualifiedName,
                FullName = type.FullName,
                IsNested = type.IsNested,
                IsTypeDefinition = type.IsTypeDefinition,
                IsArray = type.IsArray,
                IsByRef = type.IsByRef,
                IsPointer = type.IsPointer,
                IsConstructedGenericType = type.IsConstructedGenericType,
                IsGenericParameter = type.IsGenericParameter,
                IsGenericTypeParameter = type.IsGenericTypeParameter,
                IsGenericMethodParameter = type.IsGenericMethodParameter,
                IsGenericType = type.IsGenericType,
                IsGenericTypeDefinition = type.IsGenericTypeDefinition,
                IsSZArray = type.IsSZArray,
                IsVariableBoundArray = type.IsVariableBoundArray,
                IsByRefLike = type.IsByRefLike,
                HasElementType = type.HasElementType,
                GenericParameterPosition = type.IsGenericType ? type.GenericParameterPosition : 0,
                IsAbstract = type.IsAbstract,
                IsImport = type.IsImport,
                IsSealed = type.IsSealed,
                IsSpecialName = type.IsSpecialName,
                IsClass = type.IsClass,
                IsNestedAssembly = type.IsNestedAssembly,
                IsNestedFamANDAssem = type.IsNestedFamANDAssem,
                IsNestedFamily = type.IsNestedFamily,
                IsNestedFamORAssem = type.IsNestedFamORAssem,
                IsNestedPrivate = type.IsNestedPrivate,
                IsNestedPublic = type.IsNestedPublic,
                IsNotPublic = type.IsNotPublic,
                IsPublic = type.IsPublic,
                IsAutoLayout = type.IsAutoLayout,
                IsExplicitLayout = type.IsExplicitLayout,
                IsLayoutSequential = type.IsLayoutSequential,
                IsAnsiClass = type.IsAnsiClass,
                IsAutoClass = type.IsAutoClass,
                IsUnicodeClass = type.IsUnicodeClass,
                IsCOMObject = type.IsCOMObject,
                IsContextful = type.IsContextful,
                IsEnum = type.IsEnum,
                IsMarshalByRef = type.IsMarshalByRef,
                IsPrimitive = type.IsPrimitive,
                IsValueType = type.IsValueType,
                IsSignatureType = type.IsSignatureType,
                IsSecurityCritical = type.IsSecurityCritical,
                IsSecuritySafeCritical = type.IsSecuritySafeCritical,
                IsSecurityTransparent = type.IsSecurityTransparent,
                GUID = type.GUID,
                IsSerializable = type.IsSerializable,
                ContainsGenericParameters = type.ContainsGenericParameters,
                IsVisible = type.IsVisible,
                Name = type.Name,
                IsCollectible = type.IsCollectible,
                MetadataToken = type.MetadataToken
            };
        }

        public static ConstructorInfoJson Map(ConstructorInfo type)
        {
            return null;
        }

        public static MethodInfoJson Map(MethodInfo type)
        {
            return null;
        }

        public static ParameterInfoJson Map(ParameterInfo type)
        {
            return null;
        }

        public static PropertyInfoJson Map(PropertyInfo type)
        {
            return null;
        }
    }
}