using CoreShim.Reflection.JsonTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Utils;
using TypeExtensions = Utils.TypeExtensions;

namespace NetCoreReflectionShim.Service
{
    public static class ReflectionExtensions
    {
        public static void HandleObjectReflectionCommand(this INetCoreReflectionService service, StreamWriter outputWriter, CommandPacket commandPacket)
        {
            switch (commandPacket.Command)
            {
                case ObjectCommands.ACTIVATOR_CREATEINSTANCE:
                    {
                        Assembly assembly;
                        Type type;
                        object obj;
                        (var locationOrName, var metadataToken, var args) = commandPacket.Arguments.GetParts();

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
                            CommandPacket<int> returnCommandPacket;

                            if (service.CachedTokenObjects.ContainsKey(metadataToken))
                            {
                                type = (Type)service.CachedTokenObjects[metadataToken];
                            }
                            else
                            {
                                DebugUtils.Break();
                                type = null;
                            }

                            obj = type.CreateInstance<object>();
                            service.CachedObjects.Add(obj.GetHashCode(), obj);

                            returnCommandPacket = new CommandPacket<int>(commandPacket.Command, commandPacket.SentTimestamp, obj.GetHashCode());
                            outputWriter.WriteJsonCommand(returnCommandPacket);
                        }
                        else
                        {
                            DebugUtils.Break();
                        }
                    }

                    break;

                case ObjectCommands.OBJECT_CALLMETHOD:
                    {
                        Assembly assembly;
                        Type type;
                        Type returnType;
                        (var locationOrName, var metadataToken, var hashCode, var memberName, var textArgs) = commandPacket.Arguments.GetObjectParts();

                        if (service.CachedAssemblies.ContainsKey(locationOrName))
                        {
                            assembly = service.CachedAssemblies[locationOrName];
                        }
                        else
                        {
                            assembly = Assembly.LoadFrom(locationOrName);
                            service.CachedAssemblies.Add(locationOrName, assembly);
                        }

                        if (metadataToken != 0 && hashCode != 0)
                        {
                            object obj;
                            MethodInfo methodInfo;
                            var x = 0;
                            var args = new List<object>();
                            CommandPacket<string> returnCommandPacket;
                            object objResult;
                            string json;

                            if (service.CachedTokenObjects.ContainsKey(metadataToken))
                            {
                                type = (Type)service.CachedTokenObjects[metadataToken];
                            }
                            else
                            {
                                DebugUtils.Break();
                                type = null;
                            }

                            if (service.CachedObjects.ContainsKey(hashCode))
                            {
                                obj = service.CachedObjects[hashCode];
                            }
                            else
                            {
                                DebugUtils.Break();
                                obj = null;
                            }

                            methodInfo = type.GetMethod(memberName);
                            returnType = methodInfo.ReturnType;

                            foreach (var parm in methodInfo.GetParameters())
                            {
                                var argType = parm.ParameterType;
                                var argText = textArgs.ElementAt(x);

                                if (argType.IsEnum)
                                {
                                    var arg = EnumUtils.GetValue(argType, argText);

                                    args.Add(arg);
                                }
                                else if (argType.IsScalar())
                                {
                                    var arg = Convert.ChangeType(argText, argType);

                                    args.Add(arg);
                                }
                                else if (argType.Name == "Type")
                                {
                                    argType = TypeExtensions.GetType(argText);

                                    if (argType == null)
                                    {
                                        DebugUtils.Break();
                                    }

                                    args.Add(argType);
                                }

                                x++;
                            }

                            if (returnType.IsGenericCollection() || returnType.IsArray)
                            {
                                objResult = obj.CallMethod<IEnumerable<object>>(memberName, args.ToArray());
                            }
                            else
                            {
                                objResult = obj.CallMethod<object>(memberName, args.ToArray());
                            }

                            if (returnType.IsEnum)
                            {
                                var enumResult = (int) objResult;
                                DebugUtils.Break();
                            }
                            else if (returnType.IsScalar())
                            {
                                var scalarResult = objResult.ToString();
                                DebugUtils.Break();
                            }
                            else if (returnType.Name == "Type")
                            {
                                var typeResult = TypeExtensions.GetType(objResult.ToString());

                                if (typeResult == null)
                                {
                                    DebugUtils.Break();
                                }

                                DebugUtils.Break();
                            }
                            else if (returnType.IsGenericCollection())
                            {
                                var genericType = returnType.GetGenericArguments().Single();
                                var typeProxy = service.GetTypeProxy(genericType);
                                var collection = (ICollection)objResult;

                                objResult = collection.Cast<object>().Select(o => Activator.CreateInstance(typeProxy, new object[] { o }));
                            }
                            else if (returnType.Name != "Object")
                            {
                                var typeProxy = service.GetTypeProxy(returnType);

                                objResult = Activator.CreateInstance(typeProxy, new object[] { objResult });
                            }

                            json = objResult.ToJsonText();
                            returnCommandPacket = new CommandPacket<string>(commandPacket.Command, commandPacket.SentTimestamp, json);

                            outputWriter.WriteJsonCommand(returnCommandPacket);
                        }
                        else
                        {
                            DebugUtils.Break();
                        }
                    }

                    break;

                default:
                    DebugUtils.Break();
                    break;
            }
        }
    }
}
