using NetCoreReflectionShim.Service;
using System;
using System.Collections.Generic;
using System.Text;
using Utils;
using System.Linq;
using System.Reflection;
using System.IO;
using CoreShim.Reflection.JsonTypes;
using CodePlex.XPathParser;
using AbstraX.XPathBuilder;

namespace NetCoreReflectionShim
{
    public static class ReflectionExtensions
    {
        public static void HandleReflectionCommand(this INetCoreReflectionService service, StreamWriter outputWriter, CommandPacket commandPacket)
        {
            switch (commandPacket.Command)
            {
                case ServerCommands.LOAD_ASSEMBLY:
                    {
                        Assembly assembly;
                        AssemblyJson assemblyJson;
                        var location = (string)commandPacket.Arguments.SingleOrDefault(a => a.Key == "Location").Value;
                        string assemblyName = null;

                        if (location == null)
                        {
                            location = (string)commandPacket.Arguments.Single(a => a.Key == "Name").Value;
                            assemblyName = commandPacket.Arguments.GetAssemblyName();
                        }

                        if (service.CachedAssemblies.ContainsKey(location))
                        {
                            assembly = service.CachedAssemblies[location];
                        }
                        else if (assemblyName != null)
                        {
                            assembly = Assembly.Load(assemblyName);
                            service.CachedAssemblies.Add(location, assembly);
                        }
                        else
                        {
                            assembly = Assembly.LoadFrom(location);
                            service.CachedAssemblies.Add(location, assembly);
                        }

                        assemblyJson = ClientMapper.Map(assembly).MapCachedValues(assembly);

                        commandPacket = new CommandPacket(commandPacket.Command, commandPacket.SentTimestamp, assemblyJson);
                        outputWriter.WriteJsonCommand(commandPacket);
                    }

                    break;

                case ServerCommands.PARAMETERINFO_GETCUSTOMATTRIBUTES__B:
                    {
                        Assembly assembly;
                        (string locationOrName, int metadataToken, string[] args) = commandPacket.Arguments.GetParts();
                        var inherit = (bool) Convert.ChangeType(args[0], typeof(bool));

                        if (service.CachedAssemblies.ContainsKey(locationOrName))
                        {
                            assembly = service.CachedAssemblies[locationOrName];
                        }
                        else
                        {
                            assembly = Assembly.LoadFrom(locationOrName);
                            service.CachedAssemblies.Add(locationOrName, assembly);
                        }

                        if (metadataToken != 0)
                        {
                            var module = assembly.GetModules().Single();
                            CommandPacket<List<AttributeJson>> returnCommandPacket;
                            List<AttributeJson> objects;
                            ParameterInfo parameterInfo;

                            if (service.CachedTokenObjects.ContainsKey(metadataToken))
                            {
                                parameterInfo = (ParameterInfo) service.CachedTokenObjects[metadataToken];
                            }
                            else
                            {
                                DebugUtils.Break();
                                parameterInfo = null;
                            }

                            objects = parameterInfo.CustomAttributes.Cast<CustomAttributeData>().Select(o => ClientMapper.Map(o)).ToList();

                            returnCommandPacket = new CommandPacket<List<AttributeJson>>(commandPacket.Command, commandPacket.SentTimestamp, objects);
                            outputWriter.WriteJsonCommand(returnCommandPacket);
                        }
                        else
                        {
                            DebugUtils.Break();
                        }
                    }

                    break;
                case ServerCommands.PARAMETERINFO_GETCUSTOMATTRIBUTES__TB:
                    {
                        Assembly assembly;
                        (string locationOrName, int metadataToken, string[] args) = commandPacket.Arguments.GetParts();
                        var attributeType = Type.GetType(args[0]);
                        var inherit = (bool) Convert.ChangeType(args[1], typeof(bool));

                        if (service.CachedAssemblies.ContainsKey(locationOrName))
                        {
                            assembly = service.CachedAssemblies[locationOrName];
                        }
                        else
                        {
                            assembly = Assembly.LoadFrom(locationOrName);
                            service.CachedAssemblies.Add(locationOrName, assembly);
                        }

                        if (metadataToken != 0)
                        {
                            var module = assembly.GetModules().Single();
                            CommandPacket<List<AttributeJson>> returnCommandPacket;
                            List<AttributeJson> objects;
                            ParameterInfo parameterInfo;

                            if (service.CachedTokenObjects.ContainsKey(metadataToken))
                            {
                                parameterInfo = (ParameterInfo) service.CachedTokenObjects[metadataToken];
                            }
                            else
                            {
                                DebugUtils.Break();
                                parameterInfo = null;
                            }

                            objects = parameterInfo.CustomAttributes.Cast<CustomAttributeData>().Select(o => ClientMapper.Map(o)).ToList();

                            returnCommandPacket = new CommandPacket<List<AttributeJson>>(commandPacket.Command, commandPacket.SentTimestamp, objects);
                            outputWriter.WriteJsonCommand(returnCommandPacket);
                        }
                        else
                        {
                            DebugUtils.Break();
                        }
                    }

                    break;
                case ServerCommands.MEMBERINFO_GETCUSTOMATTRIBUTES__B:
                    {
                        Assembly assembly;
                        (string locationOrName, int metadataToken, string[] args) = commandPacket.Arguments.GetParts();
                        var inherit = (bool) Convert.ChangeType(args[0], typeof(bool));

                        if (service.CachedAssemblies.ContainsKey(locationOrName))
                        {
                            assembly = service.CachedAssemblies[locationOrName];
                        }
                        else
                        {
                            assembly = Assembly.LoadFrom(locationOrName);
                            service.CachedAssemblies.Add(locationOrName, assembly);
                        }

                        if (metadataToken != 0)
                        {
                            var module = assembly.GetModules().Single();
                            CommandPacket<List<AttributeJson>> returnCommandPacket;
                            List<AttributeJson> objects;
                            MemberInfo memberInfo;

                            if (service.CachedTokenObjects.ContainsKey(metadataToken))
                            {
                                memberInfo = (MemberInfo) service.CachedTokenObjects[metadataToken];
                            }
                            else
                            {
                                DebugUtils.Break();
                                memberInfo = null;
                            }

                            objects = memberInfo.CustomAttributes.Cast<CustomAttributeData>().Select(o => ClientMapper.Map(o)).ToList();

                            returnCommandPacket = new CommandPacket<List<AttributeJson>>(commandPacket.Command, commandPacket.SentTimestamp, objects);
                            outputWriter.WriteJsonCommand(returnCommandPacket);
                        }
                        else
                        {
                            DebugUtils.Break();
                        }
                    }

                    break;
                case ServerCommands.MEMBERINFO_GETCUSTOMATTRIBUTES__TB:
                    {
                        Assembly assembly;
                        (string locationOrName, int metadataToken, string[] args) = commandPacket.Arguments.GetParts();
                        var attributeType = Type.GetType(args[0]);
                        var inherit = (bool) Convert.ChangeType(args[1], typeof(bool));

                        if (service.CachedAssemblies.ContainsKey(locationOrName))
                        {
                            assembly = service.CachedAssemblies[locationOrName];
                        }
                        else
                        {
                            assembly = Assembly.LoadFrom(locationOrName);
                            service.CachedAssemblies.Add(locationOrName, assembly);
                        }

                        if (metadataToken != 0)
                        {
                            var module = assembly.GetModules().Single();
                            CommandPacket<List<AttributeJson>> returnCommandPacket;
                            List<AttributeJson> objects;
                            MemberInfo memberInfo;

                            if (service.CachedTokenObjects.ContainsKey(metadataToken))
                            {
                                memberInfo = (MemberInfo) service.CachedTokenObjects[metadataToken];
                            }
                            else
                            {
                                DebugUtils.Break();
                                memberInfo = null;
                            }

                            objects = memberInfo.CustomAttributes.Cast<CustomAttributeData>().Select(o => ClientMapper.Map(o)).ToList();

                            returnCommandPacket = new CommandPacket<List<AttributeJson>>(commandPacket.Command, commandPacket.SentTimestamp, objects);
                            outputWriter.WriteJsonCommand(returnCommandPacket);
                        }
                        else
                        {
                            DebugUtils.Break();
                        }
                    }

                    break;
                case ServerCommands.TYPE_GETPROPERTIES__:
                    {
                        Assembly assembly;
                        (string locationOrName, int metadataToken, string[] args) = commandPacket.Arguments.GetParts();

                        if (service.CachedAssemblies.ContainsKey(locationOrName))
                        {
                            assembly = service.CachedAssemblies[locationOrName];
                        }
                        else
                        {
                            assembly = Assembly.LoadFrom(locationOrName);
                            service.CachedAssemblies.Add(locationOrName, assembly);
                        }

                        if (metadataToken != 0)
                        {
                            var module = assembly.GetModules().Single();
                            CommandPacket<List<PropertyInfoJson>> returnCommandPacket;
                            List<PropertyInfoJson> propertyInfoes;
                            Type type;

                            if (service.CachedTokenObjects.ContainsKey(metadataToken))
                            {
                                type = (Type) service.CachedTokenObjects[metadataToken];
                            }
                            else
                            {
                                DebugUtils.Break();
                                type = null;
                            }

                            propertyInfoes = type.GetProperties().AddTo(service.CachedTokenObjects).Select(p => ClientMapper.Map(p).MapCachedValues(p)).ToList();

                            returnCommandPacket = new CommandPacket<List<PropertyInfoJson>>(commandPacket.Command, commandPacket.SentTimestamp, propertyInfoes);
                            outputWriter.WriteJsonCommand(returnCommandPacket);
                        }
                        else
                        {
                            DebugUtils.Break();
                        }
                    }

                    break;
                case ServerCommands.TYPE_GETPROPERTIES__B:
                    {
                        Assembly assembly;
                        (string locationOrName, int metadataToken, string[] args) = commandPacket.Arguments.GetParts();
                        var bindingAttr = EnumUtils.GetValue<System.Reflection.BindingFlags>(args[0]);

                        if (service.CachedAssemblies.ContainsKey(locationOrName))
                        {
                            assembly = service.CachedAssemblies[locationOrName];
                        }
                        else
                        {
                            assembly = Assembly.LoadFrom(locationOrName);
                            service.CachedAssemblies.Add(locationOrName, assembly);
                        }

                        if (metadataToken != 0)
                        {
                            var module = assembly.GetModules().Single();
                            CommandPacket<List<PropertyInfoJson>> returnCommandPacket;
                            List<PropertyInfoJson> propertyInfoes;
                            Type type;

                            if (service.CachedTokenObjects.ContainsKey(metadataToken))
                            {
                                type = (Type) service.CachedTokenObjects[metadataToken];
                            }
                            else
                            {
                                DebugUtils.Break();
                                type = null;
                            }

                            propertyInfoes = type.GetProperties(bindingAttr).AddTo(service.CachedTokenObjects).Select(p => ClientMapper.Map(p).MapCachedValues(p)).ToList();

                            returnCommandPacket = new CommandPacket<List<PropertyInfoJson>>(commandPacket.Command, commandPacket.SentTimestamp, propertyInfoes);
                            outputWriter.WriteJsonCommand(returnCommandPacket);
                        }
                        else
                        {
                            DebugUtils.Break();
                        }
                    }

                    break;
                case ServerCommands.ASSEMBLY_GETEXPORTEDTYPES:
                    {
                        Assembly assembly;
                        (string locationOrName, int metadataToken, string[] args) = commandPacket.Arguments.GetParts();

                        if (service.CachedAssemblies.ContainsKey(locationOrName))
                        {
                            assembly = service.CachedAssemblies[locationOrName];
                        }
                        else
                        {
                            assembly = Assembly.LoadFrom(locationOrName);
                            service.CachedAssemblies.Add(locationOrName, assembly);
                        }

                        if (metadataToken == 0)
                        {
                            var module = assembly.GetModules().Single();
                            CommandPacket<List<TypeJson>> returnCommandPacket;
                            List<TypeJson> types;

                            types = assembly.GetExportedTypes().AddTo(service.CachedTokenObjects).Select(t => ClientMapper.Map(t).MapCachedValues(t)).ToList();

                            returnCommandPacket = new CommandPacket<List<TypeJson>>(commandPacket.Command, commandPacket.SentTimestamp, types);
                            outputWriter.WriteJsonCommand(returnCommandPacket);
                        }
                        else
                        {
                            DebugUtils.Break();
                        }
                    }

                    break;
                case ServerCommands.ASSEMBLY_GETTYPES:
                    {
                        Assembly assembly;
                        (string locationOrName, int metadataToken, string[] args) = commandPacket.Arguments.GetParts();

                        if (service.CachedAssemblies.ContainsKey(locationOrName))
                        {
                            assembly = service.CachedAssemblies[locationOrName];
                        }
                        else
                        {
                            assembly = Assembly.LoadFrom(locationOrName);
                            service.CachedAssemblies.Add(locationOrName, assembly);
                        }

                        if (metadataToken == 0)
                        {
                            var module = assembly.GetModules().Single();
                            CommandPacket<List<TypeJson>> returnCommandPacket;
                            List<TypeJson> types;

                            types = assembly.GetTypes().AddTo(service.CachedTokenObjects).Select(t => ClientMapper.Map(t).MapCachedValues(t)).ToList();

                            returnCommandPacket = new CommandPacket<List<TypeJson>>(commandPacket.Command, commandPacket.SentTimestamp, types);
                            outputWriter.WriteJsonCommand(returnCommandPacket);
                        }
                        else
                        {
                            DebugUtils.Break();
                        }
                    }

                    break;
                case ServerCommands.ASSEMBLY_GETCUSTOMATTRIBUTES__B:
                    {
                        Assembly assembly;
                        (string locationOrName, int metadataToken, string[] args) = commandPacket.Arguments.GetParts();
                        var inherit = (bool) Convert.ChangeType(args[0], typeof(bool));

                        if (service.CachedAssemblies.ContainsKey(locationOrName))
                        {
                            assembly = service.CachedAssemblies[locationOrName];
                        }
                        else
                        {
                            assembly = Assembly.LoadFrom(locationOrName);
                            service.CachedAssemblies.Add(locationOrName, assembly);
                        }

                        if (metadataToken == 0)
                        {
                            var module = assembly.GetModules().Single();
                            CommandPacket<List<AttributeJson>> returnCommandPacket;
                            List<AttributeJson> objects;

                            objects = assembly.CustomAttributes.Cast<CustomAttributeData>().Select(o => ClientMapper.Map(o)).ToList();

                            returnCommandPacket = new CommandPacket<List<AttributeJson>>(commandPacket.Command, commandPacket.SentTimestamp, objects);
                            outputWriter.WriteJsonCommand(returnCommandPacket);
                        }
                        else
                        {
                            DebugUtils.Break();
                        }
                    }

                    break;
                case ServerCommands.ASSEMBLY_GETCUSTOMATTRIBUTES__TB:
                    {
                        Assembly assembly;
                        (string locationOrName, int metadataToken, string[] args) = commandPacket.Arguments.GetParts();
                        var attributeType = Type.GetType(args[0]);
                        var inherit = (bool) Convert.ChangeType(args[1], typeof(bool));

                        if (service.CachedAssemblies.ContainsKey(locationOrName))
                        {
                            assembly = service.CachedAssemblies[locationOrName];
                        }
                        else
                        {
                            assembly = Assembly.LoadFrom(locationOrName);
                            service.CachedAssemblies.Add(locationOrName, assembly);
                        }

                        if (metadataToken == 0)
                        {
                            var module = assembly.GetModules().Single();
                            CommandPacket<List<AttributeJson>> returnCommandPacket;
                            List<AttributeJson> objects;

                            objects = assembly.CustomAttributes.Cast<CustomAttributeData>().Select(o => ClientMapper.Map(o)).ToList();

                            returnCommandPacket = new CommandPacket<List<AttributeJson>>(commandPacket.Command, commandPacket.SentTimestamp, objects);
                            outputWriter.WriteJsonCommand(returnCommandPacket);
                        }
                        else
                        {
                            DebugUtils.Break();
                        }
                    }

                    break;
                case ServerCommands.ASSEMBLY_GETREFERENCEDASSEMBLIES:
                    {
                        Assembly assembly;
                        (string locationOrName, int metadataToken, string[] args) = commandPacket.Arguments.GetParts();

                        if (service.CachedAssemblies.ContainsKey(locationOrName))
                        {
                            assembly = service.CachedAssemblies[locationOrName];
                        }
                        else
                        {
                            assembly = Assembly.LoadFrom(locationOrName);
                            service.CachedAssemblies.Add(locationOrName, assembly);
                        }

                        if (metadataToken == 0)
                        {
                            var module = assembly.GetModules().Single();
                            CommandPacket<List<AssemblyNameJson>> returnCommandPacket;
                            List<AssemblyNameJson> assemblyNames;

                            assemblyNames = assembly.GetReferencedAssemblies().Select(a => ClientMapper.Map(a).MapCachedValues(a)).ToList();

                            returnCommandPacket = new CommandPacket<List<AssemblyNameJson>>(commandPacket.Command, commandPacket.SentTimestamp, assemblyNames);
                            outputWriter.WriteJsonCommand(returnCommandPacket);
                        }
                        else
                        {
                            DebugUtils.Break();
                        }
                    }

                    break;

                default:

                    service.HandleObjectReflectionCommand(outputWriter, commandPacket);
                    break;
            }
        }

        public static ParameterInfoJson MapCachedValues(this ParameterInfoJson parameterInfoJson, ParameterInfo parameterInfo)
        {
            parameterInfoJson.ToStringMember = parameterInfo.ToString();
            parameterInfoJson.GetHashCodeMember = parameterInfo.GetHashCode();

            return parameterInfoJson;
        }

        public static MethodInfoJson MapCachedValues(this MethodInfoJson methodInfoJson, MethodInfo methodInfo)
        {
            methodInfoJson.GetHashCodeMember = methodInfo.GetHashCode();
            methodInfoJson.ToStringMember = methodInfo.ToString();

            return methodInfoJson;
        }

        public static PropertyInfoJson MapCachedValues(this PropertyInfoJson propertyInfoJson, PropertyInfo propertyInfo)
        {
            propertyInfoJson.GetHashCodeMember = propertyInfo.GetHashCode();

            return propertyInfoJson;
        }

        public static TypeJson MapCachedValues(this TypeJson typeJson, Type type)
        {
            typeJson.GetHashCodeMember = type.GetHashCode();
            typeJson.ToStringMember = type.ToString();
            typeJson.IsValueTypeImplMember = type.CallProtectedMethod<bool>("IsValueTypeImpl");
            typeJson.GetAttributeFlagsImplMemberEnum = type.CallProtectedMethod<int>("GetAttributeFlagsImpl");

            return typeJson;
        }

        public static AssemblyNameJson MapCachedValues(this AssemblyNameJson assemblyNameJson, AssemblyName assemblyName)
        {
            assemblyNameJson.ToStringMember = assemblyName.ToString();

            return assemblyNameJson;
        }

        public static AssemblyJson MapCachedValues(this AssemblyJson assemblyJson, Assembly assembly)
        {
            assemblyJson.GetHashCodeMember = assembly.GetHashCode();
            assemblyJson.ToStringMember = assembly.ToString();

            return assemblyJson;
        }

        public static ConstructorInfoJson MapCachedValues(this ConstructorInfoJson constructorInfoJson, ConstructorInfo constructorInfo)
        {
            constructorInfoJson.GetHashCodeMember = constructorInfo.GetHashCode();

            return constructorInfoJson;
        }

        public static CustomAttributeTypedArgumentJson MapCachedValues(this CustomAttributeTypedArgumentJson customAttributeTypedArgumentJson, CustomAttributeTypedArgument customAttributeTypedArgument)
        {
            customAttributeTypedArgumentJson.ToStringMember = customAttributeTypedArgument.ToString();
            customAttributeTypedArgumentJson.GetHashCodeMember = customAttributeTypedArgument.GetHashCode();

            return customAttributeTypedArgumentJson;
        }

        public static CustomAttributeNamedArgumentJson MapCachedValues(this CustomAttributeNamedArgumentJson customAttributeNamedArgumentJson, CustomAttributeNamedArgument customAttributeNamedArgument)
        {
            customAttributeNamedArgumentJson.GetHashCodeMember = customAttributeNamedArgument.GetHashCode();
            customAttributeNamedArgumentJson.ToStringMember = customAttributeNamedArgument.ToString();

            return customAttributeNamedArgumentJson;
        }

        public static CustomAttributeDataJson MapCachedValues(this CustomAttributeDataJson customAttributeDataJson, CustomAttributeData customAttributeData)
        {
            customAttributeDataJson.GetHashCodeMember = customAttributeData.GetHashCode();
            customAttributeDataJson.ToStringMember = customAttributeData.ToString();

            return customAttributeDataJson;
        }

        public static MemberInfoJson MapCachedValues(this MemberInfoJson memberInfoJson, MemberInfo memberInfo)
        {
            memberInfoJson.GetHashCodeMember = memberInfo.GetHashCode();

            return memberInfoJson;
        }

        public static MethodBaseJson MapCachedValues(this MethodBaseJson methodBaseJson, MethodBase methodBase)
        {
            methodBaseJson.GetHashCodeMember = methodBase.GetHashCode();

            return methodBaseJson;
        }

        public static ValueTypeJson MapCachedValues(this ValueTypeJson valueTypeJson, ValueType valueType)
        {
            valueTypeJson.ToStringMember = valueType.ToString();
            valueTypeJson.GetHashCodeMember = valueType.GetHashCode();

            return valueTypeJson;
        }

        public static IEnumerable<T> AddTo<T>(this IEnumerable<T> customAttributeProviders, Dictionary<int, ICustomAttributeProvider> tokenObjects) where T : ICustomAttributeProvider
        {
            customAttributeProviders.ForEach(p =>
            {
                switch (p)
                {
                    case MemberInfo memberInfo:

                        if (!tokenObjects.ContainsKey(memberInfo.MetadataToken))
                        {
                            tokenObjects.Add(memberInfo.MetadataToken, memberInfo);
                        }

                        break;

                    case ParameterInfo parameterInfo:
                        
                        if (!tokenObjects.ContainsKey(parameterInfo.MetadataToken))
                        {
                            tokenObjects.Add(parameterInfo.MetadataToken, parameterInfo);
                        }
                        
                        break;

                    default:
                        DebugUtils.Break();
                        break;
                }
            });

            return customAttributeProviders;
        }

    }
}
