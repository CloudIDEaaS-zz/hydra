using CoreShim.Reflection.JsonTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Utils;

namespace NetCoreReflectionShim
{
    public static class ClientMapper
    {
        public static AssemblyJson Map(Assembly assembly)
        {
            return new AssemblyJson
            {
                CodeBase = assembly.CodeBase,
                EscapedCodeBase = assembly.EscapedCodeBase,
                FullName = assembly.FullName,
                IsFullyTrusted = assembly.IsFullyTrusted,
                ReflectionOnly = assembly.ReflectionOnly,
                Location = assembly.Location,
                ImageRuntimeVersion = assembly.ImageRuntimeVersion,
                GlobalAssemblyCache = assembly.GlobalAssemblyCache,
                HostContext = assembly.HostContext,
                IsDynamic = assembly.IsDynamic
            };
        }

        public static TypeJson Map(Type type, bool recurse = true)
        {
            return new TypeJson
            {
                GUID = type.GUID,
                FullName = type.FullName,
                Namespace = type.Namespace,
                AssemblyQualifiedName = type.AssemblyQualifiedName,
                BaseType = recurse ? Map(type.BaseType, false) : null,
                GetInterfaces = recurse ? type.GetInterfaces().Select(i =>
                {
                    return Map(i, false);
                }).ToList() : null,
                IsNested = type.IsNested,
                IsVisible = type.IsVisible,
                IsNotPublic = type.IsNotPublic,
                IsPublic = type.IsPublic,
                IsNestedPublic = type.IsNestedPublic,
                IsNestedPrivate = type.IsNestedPrivate,
                IsNestedFamily = type.IsNestedFamily,
                IsNestedAssembly = type.IsNestedAssembly,
                IsNestedFamANDAssem = type.IsNestedFamANDAssem,
                IsNestedFamORAssem = type.IsNestedFamORAssem,
                IsAutoLayout = type.IsAutoLayout,
                IsLayoutSequential = type.IsLayoutSequential,
                IsExplicitLayout = type.IsExplicitLayout,
                IsClass = type.IsClass,
                IsInterface = type.IsInterface,
                IsValueType = type.IsValueType,
                IsAbstract = type.IsAbstract,
                IsSealed = type.IsSealed,
                IsEnum = type.IsEnum,
                IsSpecialName = type.IsSpecialName,
                IsImport = type.IsImport,
                IsSerializable = type.IsSerializable,
                IsAnsiClass = type.IsAnsiClass,
                IsUnicodeClass = type.IsUnicodeClass,
                IsAutoClass = type.IsAutoClass,
                IsArray = type.IsArray,
                IsGenericType = type.IsGenericType,
                IsGenericTypeDefinition = type.IsGenericTypeDefinition,
                GetGenericTypeDefinition = recurse && type.IsGenericTypeDefinition ? Map(type.GetGenericTypeDefinition(), false) : null,
                GetGenericArguments = recurse && type.IsGenericTypeDefinition ? type.GetGenericArguments().Select(a =>
                {
                    return Map(a, false);
                }).ToList() : null,
                IsConstructedGenericType = type.IsConstructedGenericType,
                IsGenericParameter = type.IsGenericParameter,
                GenericParameterPosition = type.IsGenericParameter ? type.GenericParameterPosition : -1,
                ContainsGenericParameters = type.ContainsGenericParameters,
                IsByRef = type.IsByRef,
                IsPointer = type.IsPointer,
                IsPrimitive = type.IsPrimitive,
                IsCOMObject = type.IsCOMObject,
                HasElementType = type.HasElementType,
                IsContextful = type.IsContextful,
                IsMarshalByRef = type.IsMarshalByRef,
                IsSecurityCritical = type.IsSecurityCritical,
                IsSecuritySafeCritical = type.IsSecuritySafeCritical,
                IsSecurityTransparent = type.IsSecurityTransparent,
                Name = type.Name,
                MetadataToken = type.MetadataToken
            };
        }

        public static AssemblyNameJson Map(AssemblyName assemblyName)
        {
            return new AssemblyNameJson
            {
                Name = assemblyName.Name,
                CultureName = assemblyName.CultureName,
                CodeBase = assemblyName.CodeBase,
                EscapedCodeBase = assemblyName.EscapedCodeBase,
                FullName = assemblyName.FullName
            };
        }

        public static CustomAttributeDataJson MapData(CustomAttributeData attribute)
        {
            return new CustomAttributeDataJson
            {
                AttributeType = attribute.AttributeType.FullName,
                Constructor = new ConstructorInfoJson
                {
                    GetParameters = attribute.Constructor.GetParameters().Select(attributeConstructorGetParameter => new ParameterInfoJson
                    {
                        Name = attributeConstructorGetParameter.Name ?? throw new ArgumentNullException(nameof(attributeConstructorGetParameter), "The value of 'attributeConstructorGetParameter.Name' should not be null"),
                        HasDefaultValue = attributeConstructorGetParameter.HasDefaultValue,
                        Position = attributeConstructorGetParameter.Position,
                        IsIn = attributeConstructorGetParameter.IsIn,
                        IsOut = attributeConstructorGetParameter.IsOut,
                        IsLcid = attributeConstructorGetParameter.IsLcid,
                        IsRetval = attributeConstructorGetParameter.IsRetval,
                        IsOptional = attributeConstructorGetParameter.IsOptional,
                        MetadataToken = attributeConstructorGetParameter.MetadataToken,
                        ParameterType = attributeConstructorGetParameter.ParameterType.FullName,
                    }).ToList(),
                    MethodHandle = new List<ParameterInfoJson>(),
                    IsGenericMethodDefinition = attribute.Constructor.IsGenericMethodDefinition,
                    ContainsGenericParameters = attribute.Constructor.ContainsGenericParameters,
                    IsGenericMethod = attribute.Constructor.IsGenericMethod,
                    IsSecurityCritical = attribute.Constructor.IsSecurityCritical,
                    IsSecuritySafeCritical = attribute.Constructor.IsSecuritySafeCritical,
                    IsSecurityTransparent = attribute.Constructor.IsSecurityTransparent,
                    IsPublic = attribute.Constructor.IsPublic,
                    IsPrivate = attribute.Constructor.IsPrivate,
                    IsFamily = attribute.Constructor.IsFamily,
                    IsAssembly = attribute.Constructor.IsAssembly,
                    IsFamilyAndAssembly = attribute.Constructor.IsFamilyAndAssembly,
                    IsFamilyOrAssembly = attribute.Constructor.IsFamilyOrAssembly,
                    IsStatic = attribute.Constructor.IsStatic,
                    IsFinal = attribute.Constructor.IsFinal,
                    IsVirtual = attribute.Constructor.IsVirtual,
                    IsHideBySig = attribute.Constructor.IsHideBySig,
                    IsAbstract = attribute.Constructor.IsAbstract,
                    IsSpecialName = attribute.Constructor.IsSpecialName,
                    IsConstructor = attribute.Constructor.IsConstructor,
                    Name = attribute.Constructor.Name,
                    MetadataToken = attribute.Constructor.MetadataToken
                },
                ConstructorArguments = attribute.ConstructorArguments.Select(attributeConstructorArgument => new CustomAttributeTypedArgumentJson
                {
                    ArgumentType = attributeConstructorArgument.ArgumentType.FullName,
                    ValueObject = attributeConstructorArgument.Value.ToString()
                }).ToList(),
                NamedArguments = attribute.NamedArguments.Select(attributeNamedArgument => new CustomAttributeNamedArgumentJson
                {
                    MemberInfo = attributeNamedArgument.MemberInfo.Name,
                    TypedValue = attributeNamedArgument.TypedValue.ToString(),
                    MemberName = attributeNamedArgument.MemberName,
                    IsField = attributeNamedArgument.IsField
                }).ToList()
            };
        }

        public static AttributeJson Map(CustomAttributeData attribute)
        {
            var customAttributeDataJson = MapData(attribute);

            var attributeJson = new AttributeJson
            {
                CustomAttributeData = customAttributeDataJson
            };

            return attributeJson;
        }

        //public static ConstructorInfoJson Map(ConstructorInfo type)
        //{
        //    return null;
        //}

        //public static MethodInfoJson Map(MethodInfo type)
        //{
        //    return null;
        //}

        //public static ParameterInfoJson Map(ParameterInfo type)
        //{
        //    return null;
        //}

        public static PropertyInfoJson Map(PropertyInfo property)
        {
            return new PropertyInfoJson
            {
                CanRead = property.CanRead,
                CanWrite = property.CanWrite,
                IsSpecialName = property.IsSpecialName,
                Name = property.Name,
                MetadataToken = property.MetadataToken,
                AttributesEnum = property.Attributes.ToString(),
                MemberTypeEnum = property.MemberType.ToString(),
                PropertyType = property.PropertyType.ToString(),
                DeclaringType = property.DeclaringType.ToString(),
                GetHashCodeMember = property.GetHashCode(),
                ToStringMember = property.ToString()
            };
        }
    }
}