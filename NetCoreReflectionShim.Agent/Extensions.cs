using CoreShim.Reflection;
using CoreShim.Reflection.JsonTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Utils;
using TypeExtensions = Utils.TypeExtensions;

namespace NetCoreReflectionShim.Agent
{
    public static class Extensions
    {
        public static Type[] CacheTypes(this IEnumerable<Type> types, INetCoreReflectionAgent agent)
        {
            foreach (var type in types)
            {
                if (!agent.CachedTypes.ContainsKey(type.FullName))
                {
                    agent.CachedTypes.Add(type.FullName, type);
                }
            }

            return types.ToArray();
        }

        public static bool CompareTo(this TypeShim typeShim, Type type)
        {
            return true;
        }

        public static Type[] ToTypes(this List<TypeJson> typeJsons, INetCoreReflectionAgent agent, string parentIdentifier)
        {
            var types = new List<Type>();

            foreach (var type in typeJsons.Select(i => new TypeShim(i, parentIdentifier, agent)))
            {
                var mappedType = agent.MapType(type);

                if (mappedType == null)
                {
                    types.Add(type);
                }
                else
                {
                    types.Add(mappedType);
                }
            }

            return types.ToArray();
        }

        public static Type MapType(this INetCoreReflectionAgent agent, Type type)
        {
            foreach (var redirectedNamespace in agent.RedirectedNamespaces.Where(n => type.Namespace != null && type.Namespace.StartsWith(n.Key)))
            {
                var replacedType = redirectedNamespace.ReplaceType(type, agent);

                if (replacedType != null)
                {
                    type = replacedType;
                }
                else
                {
                    return type;
                }
            }

            return type;
        }

        public static string MapTypeName(this INetCoreReflectionAgent agent, string typeFullName)
        {
            var typeName = typeFullName.RightAtLastIndexOf('.');
            var typeNamespace = typeFullName.Left(typeFullName.LastIndexOf("."));

            foreach (var redirectedNamespace in agent.RedirectedNamespaces.Where(n => typeNamespace != null && typeNamespace.StartsWith(n.Key)))
            {
                var replacedTypeName = redirectedNamespace.ReplaceTypeName(typeFullName);

                if (replacedTypeName != null)
                {
                    typeFullName = replacedTypeName;
                }
            }

            return typeFullName;
        }

        public static Type[] MapTypes(this INetCoreReflectionAgent agent, Type[] typesToMap)
        {
            var types = typesToMap.ToList();

            foreach (var type in types.ToList())
            {
                foreach (var redirectedNamespace in agent.RedirectedNamespaces.Where(n => type.Namespace != null && type.Namespace.StartsWith(n.Key)))
                {
                    var replacedType = redirectedNamespace.ReplaceType(type, agent);

                    if (replacedType != null)
                    {
                        var index = types.IndexOf(type);

                        types.RemoveAt(index);
                        types.Insert(index, replacedType);
                    }
                }
            }

            return types.ToArray();
        }

        public static string ReplaceTypeName(this KeyValuePair<string, string> redirectedNamespace, string typeFullName)
        {
            var typeName = typeFullName.RightAtLastIndexOf('.');
            var replacedNamespace = typeFullName.Left(typeFullName.LastIndexOf("."));

            replacedNamespace = replacedNamespace.RemoveStart(redirectedNamespace.Key);
            replacedNamespace = redirectedNamespace.Value + replacedNamespace;

            return replacedNamespace + "." + typeName;
        }

        public static Type ReplaceType(this KeyValuePair<string, string> redirectedNamespace, Type type, INetCoreReflectionAgent agent)
        {
            var replacedNamespace = type.Namespace;
            string typeFullName;
            Type replacedType;

            replacedNamespace = replacedNamespace.RemoveStart(redirectedNamespace.Key);
            replacedNamespace = redirectedNamespace.Value + replacedNamespace;

            typeFullName = replacedNamespace + "." + type.Name;
            replacedType = Type.GetType(typeFullName);

            if (replacedType == null)
            {
                replacedType = agent.GetType(typeFullName);

                if (replacedType == null)
                {

                }
            }

            return replacedType;
        }

        public static Attribute ToAttribute(this AttributeJson attributeJson, INetCoreReflectionAgent agent)
        {
            var customAttributeDataJson = attributeJson.CustomAttributeData;
            var typeName = customAttributeDataJson.AttributeType;
            var type = TypeExtensions.GetType(typeName, agent.TypeCache);
            ConstructorInfo constructorInfo;
            var args = new List<object>();
            var redirectedNamespaces = agent.RedirectedNamespaces;
            var cachedTypes = agent.CachedTypes;
            Attribute attribute;

            if (type == null)
            {
                DebugUtils.Break();
            }

            constructorInfo = type.GetConstructor(customAttributeDataJson.Constructor.GetParameterTypes(redirectedNamespaces, agent));

            if (constructorInfo == null)
            {
                DebugUtils.Break();
            }

            foreach (var constructorArg in customAttributeDataJson.ConstructorArguments)
            {
                var argType = TypeExtensions.GetType(constructorArg.ArgumentType, agent.TypeCache);

                if (argType == null)
                {
                    DebugUtils.Break();
                }

                if (argType.IsEnum)
                {
                    var arg = EnumUtils.GetValue(argType, int.Parse(constructorArg.ValueObject));

                    args.Add(arg);
                }
                else if (argType.IsScalar())
                {
                    var arg = Convert.ChangeType(constructorArg.ValueObject, argType);

                    args.Add(arg);
                }
                else if (argType.Name == "Type")
                {
                    argType = TypeExtensions.GetType(constructorArg.ValueObject, agent.TypeCache);

                    if (argType == null)
                    {
                        if (agent.CachedTypes.ContainsKey(constructorArg.ValueObject))
                        {
                            argType = agent.CachedTypes[constructorArg.ValueObject];
                        }
                        else
                        {
                            DebugUtils.Break();
                        }

                        args.Add(argType);
                    }
                }
                else
                {
                    DebugUtils.Break();
                }
            }

            attribute = (Attribute) Activator.CreateInstance(type, args.ToArray());

            foreach (var namedArg in customAttributeDataJson.NamedArguments)
            {
                var propertyInfo = attribute.GetProperty(namedArg.MemberName);
                var argType = propertyInfo.PropertyType;
                var value = namedArg.TypedValue.RegexGet(@"^(\(.*?\))?(?<value>.*)$", "value");

                if (argType == null)
                {
                    DebugUtils.Break();
                }

                if (argType.IsEnum)
                {
                    var arg = EnumUtils.GetValue(argType, int.Parse(value));

                    propertyInfo.SetValue(attribute, arg);
                }
                else if (argType.IsScalar())
                {
                    var arg = Convert.ChangeType(value, argType);

                    propertyInfo.SetValue(attribute, arg);
                }
                else if (argType.Name == "Type")
                {
                    argType = TypeExtensions.GetType(value, agent.TypeCache);

                    if (argType == null)
                    {
                        if (agent.CachedTypes.ContainsKey(value))
                        {
                            argType = agent.CachedTypes[value];
                        }
                        else
                        {
                            DebugUtils.Break();
                        }


                        propertyInfo.SetValue(attribute, argType);
                    }
                }
                else
                {
                    DebugUtils.Break();
                }
            }

            return attribute;
        }

        public static Type[] GetParameterTypes(this ConstructorInfoJson constructorInfoJson, Dictionary<string, string> redirectedNamespaces, INetCoreReflectionAgent agent)
        {
            var types = new List<Type>();

            foreach (var parm in constructorInfoJson.GetParameters)
            {
                var type = TypeExtensions.GetType(parm.ParameterType, agent.TypeCache);

                if (type == null)
                {
                    DebugUtils.Break();
                }

                types.Add(type);
            }

            return types.ToArray();
        }
    }
}