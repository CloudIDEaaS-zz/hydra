using AbstraX;
using CoreShim.Reflection;
using CoreShim.Reflection.JsonTypes;
using NetCoreReflectionShim.Service;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utils;
using TypeExtensions = Utils.TypeExtensions;

namespace NetCoreReflectionShim.Agent
{
    public partial class NetCoreReflectionAgent : IDisposable, INetCoreReflectionAgent
    {
        private Process shimServiceProcess;
        private TextReader reader;
        private TextWriter writer;
        public Dictionary<string, string> RedirectedNamespaces { get; private set; }

        private bool debug;
        private bool runAsAutomated;

        public bool TestMode { get; set; }
        public Dictionary<string, Type> CachedTypes { get; }
        public Dictionary<string, Assembly> CachedAssemblies { get; }
        public TypeCache TypeCache { get; } 
        public event StartServiceEventHandler OnStartService;
        public event GetTypeProxyEventHandler GetTypeProxyEvent;
        public event GetTypeEventHandler GetTypeEvent;

        public NetCoreReflectionAgent(TypeCache typeCache, bool debug = false, bool testMode = false, bool runAsAutomated = false)
        {
            this.debug = debug;
            this.runAsAutomated = runAsAutomated;
            this.TestMode = testMode;
            this.TypeCache = typeCache;
            this.CachedTypes = new Dictionary<string, Type>();
            this.CachedAssemblies = new Dictionary<string, Assembly>();
        }

        public NetCoreReflectionAgent(bool debug = false, bool testMode = false, bool runAsAutomated = false)
        {
            this.debug = debug;
            this.runAsAutomated = runAsAutomated;
            this.TestMode = testMode;
            this.CachedTypes = new Dictionary<string, Type>();
            this.CachedAssemblies = new Dictionary<string, Assembly>();
        }

        public Assembly LoadCoreAssembly(AssemblyName assemblyName, Dictionary<string, string> redirectedNamespaces)
        {
            var fullName = assemblyName.FullName;
            var parts = assemblyName.GetNameParts();
            var keyValuePairs = fullName.Split(',', StringSplitOptions.RemoveEmptyEntries).PairedListToKeyValuePairs(c => new KeyValuePair<string, string>("Name", parts.AssemblyName));
            var path = keyValuePairs.NameValuesToUrlQueryString("assembly://assemblyname");
            CommandPacket<AssemblyJson> commandPacketReturn;
            var commandPacket = new CommandPacket
            {
                Command = ServerCommands.LOAD_ASSEMBLY,
                Arguments = new Dictionary<string, object>
                {
                    { "Name", path }
                }
                .Select(a => new KeyValuePair<string, object>(a.Key, a.Value)).ToArray()
            };

            if (shimServiceProcess == null)
            {
                StartService();
            }

            this.RedirectedNamespaces = redirectedNamespaces;

            writer.WriteJsonCommand(commandPacket);
            commandPacketReturn = reader.ReadJsonCommand<AssemblyJson>();

            return new AssemblyShim(commandPacketReturn.Response, path, this);
        }

        public Assembly LoadCoreAssembly(Assembly assembly, Dictionary<string, string> redirectedNamespaces)
        {
            var location = assembly.Location;
            var uri = new Uri(location);
            var path = uri.AbsoluteUri.RemoveStart("file:///").Replace(":", string.Empty);
            CommandPacket<AssemblyJson> commandPacketReturn;
            Assembly assemblyShim;
            var commandPacket = new CommandPacket
            {
                Command = ServerCommands.LOAD_ASSEMBLY,
                Arguments = new Dictionary<string, object>
                {
                    { "Location", location }
                }
                .Select(a => new KeyValuePair<string, object>(a.Key, a.Value)).ToArray()
            };

            if (shimServiceProcess == null)
            {
                StartService();
            }

            this.RedirectedNamespaces = redirectedNamespaces;

            writer.WriteJsonCommand(commandPacket);
            commandPacketReturn = reader.ReadJsonCommand<AssemblyJson>();

            assemblyShim = new AssemblyShim(commandPacketReturn.Response, path, this);

            if (!this.CachedAssemblies.ContainsKey(path))
            {
                this.CachedAssemblies.Add(path, assemblyShim);
            }

            return assemblyShim;
        }

        private void StartService()
        {
            CommandPacket commandPacket;

            if (this.TestMode)
            {
                var args = new StartServiceEventArgs();

                OnStartService(this, args);

                reader = args.Reader;
                writer = args.Writer;

                shimServiceProcess = new Process();
            }
            else
            {
                ProcessStartInfo startInfo;
                var entryAssembly = Assembly.GetEntryAssembly();
                var relativeShimServiceExeLocation = ConfigurationSettings.AppSettings["ShimServiceExeLocation"];
                var arguments = ConfigurationSettings.AppSettings["ShimServiceArguments"];
                var shimServiceExeLocation = new FileInfo(Path.GetFullPath(Path.Combine(Path.GetDirectoryName(entryAssembly.Location), relativeShimServiceExeLocation)));

                foreach (var process in Process.GetProcessesByName("NetCoreReflectionShim.Service"))
                {
                    process.Kill();
                }

                if (debug)
                {
                    arguments += " -debug";
                }

                if (runAsAutomated)
                {
                    arguments += " -runAsAutomated";
                }

                startInfo = new ProcessStartInfo
                {
                    FileName = shimServiceExeLocation.FullName,
                    Arguments = arguments
                };

                shimServiceProcess = new Process();

                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardInput = true;
                startInfo.RedirectStandardError = true;
                startInfo.WorkingDirectory = Directory.GetCurrentDirectory();

                shimServiceProcess.StartInfo = startInfo;

                startInfo.CreateNoWindow = false;
                startInfo.UseShellExecute = false;

                shimServiceProcess.ErrorDataReceived += (s, e2) =>
                {
                    var data = e2.Data;

                    if (data != null)
                    {
                        Console.WriteLine(data);
                    }
                };

                shimServiceProcess.Exited += (s, e2) =>
                {
                    var exitCode = shimServiceProcess.ExitCode;

                    if (exitCode != 0)
                    {
                        Console.WriteLine("ShimServiceProcess failed with ExitCode={0}", exitCode);
                    }
                };

                shimServiceProcess.Start();

                reader = shimServiceProcess.StandardOutput;
                writer = shimServiceProcess.StandardInput;
            }

            commandPacket = new CommandPacket
            {
                Command = ServerCommands.CONNECT
            };

            writer.WriteJsonCommand(commandPacket);
            commandPacket = reader.ReadJsonCommand();
        }

        public void Dispose()
        {
            if (shimServiceProcess != null)
            {
                if (writer != null)
                {
                    var commandPacket = new CommandPacket
                    {
                        Command = ServerCommands.TERMINATE
                    };

                    writer.WriteJsonCommand(commandPacket);

                    Thread.Sleep(1000);

                    if (!shimServiceProcess.HasExited)
                    {
                        shimServiceProcess.Kill();
                    }
                }
            }
        }

        public T CreateInstance<T>(TypeShim type) where T : class
        {
            CommandPacket<int> commandPacketReturn;
            var arguments = new string[] { };
            var identifier = string.Format("{0}[@MetadataToken={1}]", type.GetFieldValue<string>("parentIdentifier"), type.MetadataToken.ToString());
            int hashCode;
            T proxyInstance;

            var commandPacket = new CommandPacket
            {
                Command = ObjectCommands.ACTIVATOR_CREATEINSTANCE,
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
            commandPacketReturn = reader.ReadJsonCommand<int>();

            hashCode = commandPacketReturn.Response;

            identifier = string.Format("{0}/ObjectInstance[@HashCode={1}]", identifier, hashCode);

            proxyInstance = this.WrapInstance<T>(identifier);

            return proxyInstance;
        }

        public T CallMethod<T>(string identifier, string methodName, params object[] args)
        {
            CommandPacket<string> commandPacketReturn;
            var arguments = args.Select(a => a is string str ? str.SurroundWithQuotes() : a.ToString());
            var returnType = typeof(T);
            object result;

            identifier = string.Format("{0}/Member[@Name='{1}']", identifier, methodName);

            var commandPacket = new CommandPacket
            {
                Command = ObjectCommands.OBJECT_CALLMETHOD,
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
            commandPacketReturn = reader.ReadJsonCommand<string>();

            result = commandPacketReturn.Response;

            if (returnType.IsEnum)
            {
                result = EnumUtils.GetValue(returnType, int.Parse(result.ToString()));
            }
            else if (returnType.IsScalar())
            {
                result = Convert.ChangeType(result.ToString(), returnType);
            }
            else if (returnType.Name == "Type")
            {
                var typeResult = TypeExtensions.GetType(result.ToString(), this.TypeCache);

                if (typeResult == null)
                {
                    DebugUtils.Break();
                }

                DebugUtils.Break();
            }
            else if (returnType.IsGenericCollection())
            {
                var genericType = returnType.GetGenericArguments().Single();
                var typeProxy = this.GetTypeProxy(genericType);
                var collection = (ICollection) JsonExtensions.ReadJson(result.ToString());

                if (returnType.Name == "List`1")
                {
                    result = collection.Cast<object>().Select(o => Activator.CreateInstance(typeProxy, new object[] { o })).ToList().ToTypedList(genericType);
                }
            }
            else if (returnType.Name != "Object")
            {
                var typeProxy = this.GetTypeProxy(returnType);

                result = Activator.CreateInstance(typeProxy, new object[] { JsonExtensions.ReadJson(result.ToString()) });
            }
            else
            {
                result = JsonExtensions.ReadJson(result.ToString());
            }

            return (T)result;
        }

        public Type GetTypeProxy(Type type)
        {
            var args = new GetTypeProxyEventArgs(type);

            GetTypeProxyEvent(this, args);

            return args.ProxyType;
        }

        public Type GetType(string typeFullName)
        {
            var args = new GetTypeEventArgs(typeFullName);

            GetTypeEvent(this, args);

            return args.Type;
        }

        public T PropertyGet<T>(string identifier, string propertyName)
        {
            return default(T);
        }

        public void PropertySet<T>(string identifier, string propertyName, T value)
        {
        }

        public Assembly GetAssembly(string parentIdentifier)
        {
            return this.CachedAssemblies[parentIdentifier];
        }
    }
}
