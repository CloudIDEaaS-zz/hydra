using CoreShim.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreShim.Reflection.JsonTypes;
using System.Reflection;
using Utils;

namespace NetCoreReflectionShim.Agent
{
    public partial class NetCoreReflectionAgent
    {
        public System.Object[] ParameterInfo_GetCustomAttributes(string identifier, bool inherit)
        {
            CommandPacket<List<AttributeJson>> commandPacketReturn;
            var arguments = new string[] { inherit.ToString() };

            var commandPacket = new CommandPacket
            {
                Command = ServerCommands.PARAMETERINFO_GETCUSTOMATTRIBUTES__B,
                Arguments = new Dictionary<string, object>
                {
                    { "Identifier", identifier },
                    { "Arguments", arguments }
                }
                .Select(a => new KeyValuePair<string, object>(a.Key, a.Value)).ToArray()
            };

            if (shimServiceProcess == null)
            {
                StartService();
            }

            writer.WriteJsonCommand(commandPacket);
            commandPacketReturn = reader.ReadJsonCommand<List<AttributeJson>>();

            return commandPacketReturn.Response.Select(o => o.ToAttribute(this)).ToArray();
        }

        public System.Object[] ParameterInfo_GetCustomAttributes(string identifier, Type attributeType, bool inherit)
        {
            CommandPacket<List<AttributeJson>> commandPacketReturn;
            var arguments = new string[] { attributeType.ToString(), inherit.ToString() };

            var commandPacket = new CommandPacket
            {
                Command = ServerCommands.PARAMETERINFO_GETCUSTOMATTRIBUTES__TB,
                Arguments = new Dictionary<string, object>
                {
                    { "Identifier", identifier },
                    { "Arguments", arguments }
                }
                .Select(a => new KeyValuePair<string, object>(a.Key, a.Value)).ToArray()
            };

            if (shimServiceProcess == null)
            {
                StartService();
            }

            writer.WriteJsonCommand(commandPacket);
            commandPacketReturn = reader.ReadJsonCommand<List<AttributeJson>>();

            return commandPacketReturn.Response.Select(o => o.ToAttribute(this)).ToArray();
        }

        public System.Object[] MemberInfo_GetCustomAttributes(string identifier, bool inherit)
        {
            CommandPacket<List<AttributeJson>> commandPacketReturn;
            var arguments = new string[] { inherit.ToString() };

            var commandPacket = new CommandPacket
            {
                Command = ServerCommands.MEMBERINFO_GETCUSTOMATTRIBUTES__B,
                Arguments = new Dictionary<string, object>
                {
                    { "Identifier", identifier },
                    { "Arguments", arguments }
                }
                .Select(a => new KeyValuePair<string, object>(a.Key, a.Value)).ToArray()
            };

            if (shimServiceProcess == null)
            {
                StartService();
            }

            writer.WriteJsonCommand(commandPacket);
            commandPacketReturn = reader.ReadJsonCommand<List<AttributeJson>>();

            return commandPacketReturn.Response.Select(o => o.ToAttribute(this)).ToArray();
        }

        public System.Object[] MemberInfo_GetCustomAttributes(string identifier, Type attributeType, bool inherit)
        {
            CommandPacket<List<AttributeJson>> commandPacketReturn;
            var arguments = new string[] { attributeType.ToString(), inherit.ToString() };

            var commandPacket = new CommandPacket
            {
                Command = ServerCommands.MEMBERINFO_GETCUSTOMATTRIBUTES__TB,
                Arguments = new Dictionary<string, object>
                {
                    { "Identifier", identifier },
                    { "Arguments", arguments }
                }
                .Select(a => new KeyValuePair<string, object>(a.Key, a.Value)).ToArray()
            };

            if (shimServiceProcess == null)
            {
                StartService();
            }

            writer.WriteJsonCommand(commandPacket);
            commandPacketReturn = reader.ReadJsonCommand<List<AttributeJson>>();

            return commandPacketReturn.Response.Select(o => o.ToAttribute(this)).ToArray();
        }

        public System.Reflection.PropertyInfo[] Type_GetProperties(string identifier)
        {
            CommandPacket<List<PropertyInfoJson>> commandPacketReturn;
            var arguments = new string[] {  };

            var commandPacket = new CommandPacket
            {
                Command = ServerCommands.TYPE_GETPROPERTIES__,
                Arguments = new Dictionary<string, object>
                {
                    { "Identifier", identifier },
                    { "Arguments", arguments }
                }
                .Select(a => new KeyValuePair<string, object>(a.Key, a.Value)).ToArray()
            };

            if (shimServiceProcess == null)
            {
                StartService();
            }

            writer.WriteJsonCommand(commandPacket);
            commandPacketReturn = reader.ReadJsonCommand<List<PropertyInfoJson>>();

            return commandPacketReturn.Response.Select(p => new PropertyInfoShim(p, identifier, this)).ToArray();
        }

        public System.Reflection.PropertyInfo[] Type_GetProperties(string identifier, BindingFlags bindingAttr)
        {
            CommandPacket<List<PropertyInfoJson>> commandPacketReturn;
            var arguments = new string[] { bindingAttr.ToString() };

            var commandPacket = new CommandPacket
            {
                Command = ServerCommands.TYPE_GETPROPERTIES__B,
                Arguments = new Dictionary<string, object>
                {
                    { "Identifier", identifier },
                    { "Arguments", arguments }
                }
                .Select(a => new KeyValuePair<string, object>(a.Key, a.Value)).ToArray()
            };

            if (shimServiceProcess == null)
            {
                StartService();
            }

            writer.WriteJsonCommand(commandPacket);
            commandPacketReturn = reader.ReadJsonCommand<List<PropertyInfoJson>>();

            return commandPacketReturn.Response.Select(p => new PropertyInfoShim(p, identifier, this)).ToArray();
        }

        public System.Type[] Assembly_GetExportedTypes(string identifier)
        {
            CommandPacket<List<TypeJson>> commandPacketReturn;
            var arguments = new string[] {  };

            var commandPacket = new CommandPacket
            {
                Command = ServerCommands.ASSEMBLY_GETEXPORTEDTYPES,
                Arguments = new Dictionary<string, object>
                {
                    { "Identifier", identifier },
                    { "Arguments", arguments }
                }
                .Select(a => new KeyValuePair<string, object>(a.Key, a.Value)).ToArray()
            };

            if (shimServiceProcess == null)
            {
                StartService();
            }

            writer.WriteJsonCommand(commandPacket);
            commandPacketReturn = reader.ReadJsonCommand<List<TypeJson>>();

            return commandPacketReturn.Response.Select(t => new TypeShim(t, identifier, this)).CacheTypes(this);
        }

        public System.Type[] Assembly_GetTypes(string identifier)
        {
            CommandPacket<List<TypeJson>> commandPacketReturn;
            var arguments = new string[] {  };

            var commandPacket = new CommandPacket
            {
                Command = ServerCommands.ASSEMBLY_GETTYPES,
                Arguments = new Dictionary<string, object>
                {
                    { "Identifier", identifier },
                    { "Arguments", arguments }
                }
                .Select(a => new KeyValuePair<string, object>(a.Key, a.Value)).ToArray()
            };

            if (shimServiceProcess == null)
            {
                StartService();
            }

            writer.WriteJsonCommand(commandPacket);
            commandPacketReturn = reader.ReadJsonCommand<List<TypeJson>>();

            return commandPacketReturn.Response.Select(t => new TypeShim(t, identifier, this)).CacheTypes(this);
        }

        public System.Object[] Assembly_GetCustomAttributes(string identifier, bool inherit)
        {
            CommandPacket<List<AttributeJson>> commandPacketReturn;
            var arguments = new string[] { inherit.ToString() };

            var commandPacket = new CommandPacket
            {
                Command = ServerCommands.ASSEMBLY_GETCUSTOMATTRIBUTES__B,
                Arguments = new Dictionary<string, object>
                {
                    { "Identifier", identifier },
                    { "Arguments", arguments }
                }
                .Select(a => new KeyValuePair<string, object>(a.Key, a.Value)).ToArray()
            };

            if (shimServiceProcess == null)
            {
                StartService();
            }

            writer.WriteJsonCommand(commandPacket);
            commandPacketReturn = reader.ReadJsonCommand<List<AttributeJson>>();

            return commandPacketReturn.Response.Select(o => o.ToAttribute(this)).ToArray();
        }

        public System.Object[] Assembly_GetCustomAttributes(string identifier, Type attributeType, bool inherit)
        {
            CommandPacket<List<AttributeJson>> commandPacketReturn;
            var arguments = new string[] { attributeType.ToString(), inherit.ToString() };

            var commandPacket = new CommandPacket
            {
                Command = ServerCommands.ASSEMBLY_GETCUSTOMATTRIBUTES__TB,
                Arguments = new Dictionary<string, object>
                {
                    { "Identifier", identifier },
                    { "Arguments", arguments }
                }
                .Select(a => new KeyValuePair<string, object>(a.Key, a.Value)).ToArray()
            };

            if (shimServiceProcess == null)
            {
                StartService();
            }

            writer.WriteJsonCommand(commandPacket);
            commandPacketReturn = reader.ReadJsonCommand<List<AttributeJson>>();

            return commandPacketReturn.Response.Select(o => o.ToAttribute(this)).ToArray();
        }

        public System.Reflection.AssemblyName[] Assembly_GetReferencedAssemblies(string identifier)
        {
            CommandPacket<List<AssemblyNameJson>> commandPacketReturn;
            var arguments = new string[] {  };

            var commandPacket = new CommandPacket
            {
                Command = ServerCommands.ASSEMBLY_GETREFERENCEDASSEMBLIES,
                Arguments = new Dictionary<string, object>
                {
                    { "Identifier", identifier },
                    { "Arguments", arguments }
                }
                .Select(a => new KeyValuePair<string, object>(a.Key, a.Value)).ToArray()
            };

            if (shimServiceProcess == null)
            {
                StartService();
            }

            writer.WriteJsonCommand(commandPacket);
            commandPacketReturn = reader.ReadJsonCommand<List<AssemblyNameJson>>();

            return commandPacketReturn.Response.Select(a => new System.Reflection.AssemblyName(a.FullName)).ToArray();
        }
    }
}
