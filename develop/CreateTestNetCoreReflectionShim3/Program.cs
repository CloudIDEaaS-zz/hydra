using System;
using System.Reflection;
using System.Linq;
using Utils;
using PostSharp.Aspects.Advices;
using CreateTest.NetCoreReflectionShim.Generators;

namespace CreateTest.NetCoreReflectionShim
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var assemblyType = typeof(Assembly);
            var memberInfoType = typeof(MemberInfo);
            var methodBaseInfoType = typeof(MethodBase);

            // DoTests();

            ReflectionGenerator.ReflectMember += JsonReflectionGenerator_ReflectMember;

            ReflectionGenerator.GenerateClass(assemblyType);
            ReflectionGenerator.GenerateClass(memberInfoType);
            ReflectionGenerator.GenerateClass(methodBaseInfoType);

            Console.WriteLine("Complete! Press any key to exit.");
            Console.ReadKey();
        }

        private static void DoTests()
        {
            //var assembly = Assembly.GetEntryAssembly();
            //var types = assembly.GetTypes().OrderBy(n => n.Name).Select(t => ClientMapper.Map(t)).ToList();
            Type type;

            //foreach (var type in types)
            //{
            //    var json = JsonExtensions.ToJsonText(type);
            //    var convertedType = JsonExtensions.ReadJson<TypeJson>(json);

            //    Console.WriteLine(convertedType.FullName);
            //}
        }

        private static void JsonReflectionGenerator_ReflectMember(object sender, ReflectMemberEventArgs e)
        {
            switch (e.MemberInfo.DeclaringType.Name)
            {
                case "Assembly":

                    switch (e.MemberInfo.Name)
                    {
                        case "EntryPoint":
                        case "ManifestModule":
                        case "GetExportedTypes":
                        case "GetForwardedTypes":
                        case "GetManifestResourceInfo":
                        case "GetManifestResourceNames":
                        case "GetManifestResourceStream":
                        case "GetCustomAttributesData":
                        case "GetName":
                        case "IsDefined":
                        case "GetCustomAttributes":
                        case "CreateInstance":
                        case "GetModule":
                        case "GetModules":
                        case "GetLoadedModules":
                        case "GetReferencedAssemblies":
                        case "GetSatelliteAssembly":
                        case "GetObjectData":
                        case "LoadModule":
                        case "GetFile":
                        case "GetFiles":
                        case "Equals":
                        case "GetHashCode":
                        case "ToString":
                        case "GetType":
                            break;
                        default:

                            if (e.MemberInfo is MethodInfo methodInfo)
                            {
                                switch (methodInfo.Name)
                                {
                                    case "GetTypes":

                                        ReflectionGenerator.GenerateClass(typeof(Type));
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

                case "Type":

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
                        case "BaseType":
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
                        case "GetProperties":
                        case "GetDefaultMembers":
                        case "InvokeMember":
                        case "GetInterface":
                        case "GetInterfaces":
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
                        case "GetHashCode":
                        case "ToString":
                        case "GetType":
                        case "GetElementType":
                        case "GetArrayRank":
                        case "GetGenericTypeDefinition":
                        case "GetGenericArguments":
                        case "GetGenericParameterConstraints":
                        case "GetConstructor":
                            break;
                        default:

                            if (e.MemberInfo is MethodInfo methodInfo)
                            {
                                switch (methodInfo.Name)
                                {
                                    case "GetMethod":

                                        ReflectionGenerator.GenerateClass(typeof(MethodInfo));
                                        break;

                                    case "GetProperty":

                                        ReflectionGenerator.GenerateClass(typeof(PropertyInfo));
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

                case "ConstructorInfo":

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
                                        break;
                                    case "":

                                        ReflectionGenerator.GenerateClass(typeof(ConstructorInfo));
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

                case "MethodInfo":

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
                        case "GetHashCode":
                        case "ToString":
                        case "GetType":
                            break;
                        case "ReturnParameter":

                            ReflectionGenerator.GenerateClass(typeof(ParameterInfo));
                            break;

                        default:

                            if (e.MemberInfo is MethodInfo methodInfo)
                            {
                                switch (methodInfo.Name)
                                {
                                    case "":

                                        ReflectionGenerator.GenerateClass(typeof(ConstructorInfo));
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


                case "ParameterInfo":

                    switch (e.MemberInfo.Name)
                    {
                        case "Member":
                        case "ParameterType":
                        case "IsDefined":
                        case "GetCustomAttributes":
                        case "GetOptionalCustomModifiers":
                        case "GetRequiredCustomModifiers":
                        case "GetRealObject":
                        case "GetCustomAttributesData":
                        case "ReturnParameter":
                        case "Equals":
                        case "GetHashCode":
                        case "ToString":
                        case "GetType":
                            break;
                        default:

                            if (e.MemberInfo is MethodInfo methodInfo)
                            {
                                switch (methodInfo.Name)
                                {
                                    case "":

                                        ReflectionGenerator.GenerateClass(typeof(ConstructorInfo));
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

                case "PropertyInfo":

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
                        case "GetHashCode":
                        case "ToString":
                        case "GetType":
                            break;
                        default:

                            if (e.MemberInfo is MethodInfo methodInfo)
                            {
                                switch (methodInfo.Name)
                                {
                                    case "":

                                        ReflectionGenerator.GenerateClass(typeof(ConstructorInfo));
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

                case "MemberInfo":

                    switch (e.MemberInfo.Name)
                    {
                        case "DeclaringType":
                        case "ReflectedType":
                        case "HasSameMetadataDefinitionAs":
                        case "IsDefined":
                        case "GetCustomAttributes":
                        case "GetCustomAttributesData":
                        case "Module":
                        case "Equals":
                        case "GetHashCode":
                        case "ToString":
                        case "GetType":
                            break;
                        default:

                            if (e.MemberInfo is MethodInfo methodInfo)
                            {
                                switch (methodInfo.Name)
                                {
                                    case "zzzzzzzzzzzzzzzzz":
                                        break;
                                    case "":

                                        ReflectionGenerator.GenerateClass(typeof(ConstructorInfo));
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

                case "MethodBase":

                    switch (e.MemberInfo.Name)
                    {
                        case "MethodHandle":
                        case "GetParameters":
                        case "GetMethodImplementationFlags":
                        case "GetMethodBody":
                        case "GetGenericArguments":
                        case "Invoke":
                        case "Equals":
                        case "GetHashCode":
                        case "ToString":
                        case "GetType":
                            break;
                        default:

                            if (e.MemberInfo is MethodInfo methodInfo)
                            {
                                switch (methodInfo.Name)
                                {
                                    case "zzzzzzzzzzzzzzzzz":
                                        break;
                                    case "":

                                        ReflectionGenerator.GenerateClass(typeof(ConstructorInfo));
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


                case "":

                    switch (e.MemberInfo.Name)
                    {
                        case "":
                            break;
                        default:

                            if (e.MemberInfo is MethodInfo methodInfo)
                            {
                                switch (methodInfo.Name)
                                {
                                    case "zzzzzzzzzzzzzzzzz":
                                        break;
                                    case "":

                                        ReflectionGenerator.GenerateClass(typeof(ConstructorInfo));
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

                default:
                    DebugUtils.Break();
                    break;
            }
        }
    }
}
