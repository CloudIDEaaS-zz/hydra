using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Reflection;
using System.ComponentModel;
using Utils;
using Microsoft.VisualStudio;
using System.IO;
using System.Diagnostics;
using SDKInterfaceLibrary.Entities;
using System.Data.Objects.DataClasses;
using System.Data.Objects;
using System.Collections;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Data.EntityClient;
using System.Globalization;
using Pdb;
using Microsoft.Samples.Debugging.CorSymbolStore;
using System.Text.RegularExpressions;

namespace VisualStudioProvider
{
    public static class VsComObjectUtility
    {
        public static event EventHandlerT<string> OnStatus;

        public static void SaveAll(this SdkInterfaceLibraryEntities entities, Microsoft.VisualStudio.OLE.Interop.IServiceProvider serviceProvider, IEnumerable<KeyValuePair<Guid, object>> services = null, IEnumerable<KeyValuePair<Guid, object>> packages = null)
        {
            var process = Process.GetCurrentProcess();

            entities.TruncateAll();

            if (services != null)
            {
                entities.SaveVsComServices(process, services);
            }

            if (packages != null)
            {
                entities.SaveVsComPackages(process, packages);
            }

            entities.SaveServices(process, serviceProvider);
            entities.SaveTypes(process);

            entities.SaveChanges();
        }

        public static void SaveServices(this SdkInterfaceLibraryEntities entities, Process process, Microsoft.VisualStudio.OLE.Interop.IServiceProvider serviceProvider)
        {
            var executingAssembly = Assembly.GetExecutingAssembly();

            foreach (var assemblyRef in executingAssembly.GetReferencedAssemblies())
            {
                if (assemblyRef.Name.StartsWith("Microsoft.VisualStudio.", "EnvDTE."))
                {
                    var assembly = Assembly.Load(assemblyRef);

                    foreach (var refType in assembly.GetTypes().Where(t => t.IsInterface && t.HasCustomAttribute<GuidAttribute>()))
                    {
                        var attr = refType.GetCustomAttribute<GuidAttribute>();
                        var guid = Guid.Parse(attr.Value);
                        var IID_IUnknown = new Guid("00000000-0000-0000-C000-000000000046");
                        IntPtr pUnk;

                        if (ErrorHandler.Succeeded(serviceProvider.QueryService(ref guid, ref IID_IUnknown, out pUnk)))
                        {
                            var serviceName = GetServiceFromGuid(guid);
                            var obj = Marshal.GetObjectForIUnknown(pUnk);
                            var tblService = entities.SaveIfNotExists<tblService>(s => s.ServiceName == serviceName, () =>
                            {
                                return new tblService
                                {
                                    ServiceId = Guid.NewGuid(),
                                    ServiceName = serviceName,
                                    ComponentId = entities.CreateComponent(refType, Guid.Empty, process, obj).ComponentId
                                };
                            });

                            entities.SaveVsComObjects(process, obj, serviceName, string.Empty, string.Empty);
                        }
                    }
                }
            }
        }

        public static void SaveTypes(this SdkInterfaceLibraryEntities entities, Process process)
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            var allTypes = executingAssembly.GetReferencedAssemblies().Where(r => r.Name.StartsWith("Microsoft.VisualStudio.", "EnvDTE.")).SelectMany(r =>
            {
                try
                {
                    var assembly = Assembly.Load(r);
                    return assembly.GetTypes();
                }
                catch
                {
                    return new Type[0];
                }

            });

            foreach (var assemblyRef in executingAssembly.GetReferencedAssemblies())
            {
                if (assemblyRef.Name.StartsWith("Microsoft.VisualStudio.", "EnvDTE."))
                {
                    var assembly = Assembly.Load(assemblyRef);

                    foreach (var refType in assembly.GetTypes().Where(t => t.IsInterface && t.HasCustomAttribute<GuidAttribute>()))
                    {
                        foreach (var type in allTypes)
                        {
                            var assemblyName = string.Empty;
                            var dllName = string.Empty;

                            if (type.Name == "__ComObject")
                            {
                                var typeAssembly = type.Assembly;

                                assemblyName = typeAssembly.GetNameParts().AssemblyName;
                                dllName = typeAssembly.CodeBase;
                            }

                            if (type.Implements(refType))
                            {
                                //writer.SaveLineFormat("{0},{1},{2},{3},{4},{5}", dllName, assemblyName, string.Empty, string.Empty, type, refType.FullName);
                            }
                        }
                    }
                }
            }
        }

        public static void SaveVsComServices(this SdkInterfaceLibraryEntities entities, Process process, IEnumerable<KeyValuePair<Guid, object>> objects)
        {
            foreach (var pair in objects)
            {
                var obj = pair.Value;
                var pUnk = Marshal.GetIUnknownForObject(pair.Value);
                var service = GetServiceFromGuid(pair.Key);

                entities.SaveVsComObjects(process, obj, service, string.Empty, string.Empty);
            }
        }

        public static void SaveVsComPackages(this SdkInterfaceLibraryEntities entities, Process process, IEnumerable<KeyValuePair<Guid, object>> objects)
        {
            foreach (var pair in objects)
            {
                var obj = pair.Value;
                var pUnk = Marshal.GetIUnknownForObject(pair.Value);
                var service = GetServiceFromGuid(pair.Key);

                entities.SaveVsComObjects(process, obj, service, string.Empty, string.Empty);
            }
        }

        private static Guid GetInterfaceId(this SdkInterfaceLibraryEntities entities, Type refType)
        {
            var guidAttribute = refType.GetCustomAttribute<GuidAttribute>();
            var guid = Guid.Parse(guidAttribute.Value);
            var tblInterface = entities.SaveIfNotExists<tblInterface>(i => i.InterfaceGuid == guid, () =>
            {
                return new tblInterface
                {
                    InterfaceId = Guid.NewGuid(),
                    InterfaceGuid = guid,
                    InterfaceName = refType.FullName,
                };
            });

            return tblInterface.InterfaceId;
        }

        private unsafe static IntPtr GetQueryInterfacePtr(object obj)
        {
            var pUnk = Marshal.GetIUnknownForObject(obj);
            var pIntPtr = (int*)pUnk;

            pIntPtr = (int*) *pIntPtr;

            return (IntPtr)(int)*pIntPtr;
        }

        public static tblModule GetModuleObject(Type type, Process process, object objComponent)
        {
            var tblModule = new tblModule { ModuleId = Guid.NewGuid() };

            if (type.Name == "__ComObject")
            {
                var modules = process.Modules.AppendHiddenModules(process).OrderBy(p => p.ModuleName);
                var pGetQueryInterface = GetQueryInterfacePtr(objComponent);
                var module = modules.Single(m2 => ((uint)pGetQueryInterface).IsBetween((uint)m2.BaseAddress, (uint)(m2.BaseAddress + m2.ModuleMemorySize)));

                tblModule.ModuleName = module.ModuleName;
                tblModule.ModuleFileName = module.FileName;
                tblModule.IsClrModule = false;
            }
            else
            {
                var assembly = type.Assembly;

                tblModule.ModuleName = assembly.GetNameParts().AssemblyName;
                tblModule.ModuleFileName = assembly.CodeBase;
                tblModule.IsClrModule = true;
            }

            return tblModule;
        }

        public static tblComponent CreateComponent(this SdkInterfaceLibraryEntities entities, Type type, Guid typeGuid, Process process, object objComponent)
        {
            var tblModule = GetModuleObject(type, process, objComponent);
            var tblComponent = new tblComponent
            {
                ComponentId = Guid.NewGuid(),
                ComponentType = entities.GetComponentTypeName(tblModule, type, typeGuid),
                ModuleId = entities.SaveIfNotExists<tblModule>(m => m.ModuleFileName == tblModule.ModuleFileName, () =>
                {
                    return tblModule;

                }).ModuleId
            };

            entities.tblComponents.AddObject(tblComponent);
            entities.SaveChanges();

            return tblComponent;
        }

        public static string GetComponentTypeName(this SdkInterfaceLibraryEntities entities, tblModule tblModule, Type type, Guid typeGuid)
        {
            if (tblModule.IsClrModule)
            {
                return type.FullName;
            }
            else
            {
                var imageFile = tblModule.ModuleFileName;

                SetStatus("Searching for component type name for {0}", type.Name);

                if (tblModule.tblModuleSymbols.Count == 0)
                {
                    SetStatus("Building symbol table for {0}. This may take several minutes.", tblModule.ModuleName);

                    entities.BuildModuleSymbolsTable(tblModule);
                }

                throw new NotImplementedException();
            }   
        }

        private static void BuildModuleSymbolsTable(this SdkInterfaceLibraryEntities entities, tblModule tblModule)
        {
            var cvdump = @"C:\Projects\MC\RazorViewsDesigner\Microsoft.Pdb\cvdump\cvdump.exe";
            var startInfo = new ProcessStartInfo(cvdump, string.Format("-p \"{0}\"", tblModule.ModuleFileName));
            var output = string.Empty;
            var error = string.Empty;
            int exitCode;
            Process process;

            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;

            process = new Process();

            process.EnableRaisingEvents = true;
            process.StartInfo = startInfo;

            process.OutputDataReceived += (s, e) =>
            {
                output += e.Data + "\r\n";
            };

            process.ErrorDataReceived += (s, e) =>
            {
                error += e.Data + "\r\n";
            };

            process.Start();

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            process.WaitForExit();

            exitCode = process.ExitCode;

            //exitCode = 0;
            //output = File.ReadAllText(@"C:\Projects\MC\RazorViewsDesigner\Net2Html5Package\InterfaceLibrary\cvdump.txt");
            //error = string.Empty;

            if (error.Trim().IsNullOrEmpty())
            {
                var pattern = @"^S_PUB32: \[(?<segment>[\da-zA-Z]*?):(?<offset>[\da-zA-Z]*?)\], Flags: (?<flags>[\da-zA-Z]*?), (?<symbol>.*$)$";
                var regex = new Regex(pattern);
                var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);

                foreach (var readLine in lines)
                {
                    var line = readLine.Trim();

                    if (regex.IsMatch(line))
                    {
                        var match = regex.Match(line.Trim());
                        var segment = match.Groups["segment"].Value;
                        var offset = match.Groups["offset"].Value;
                        var flags = match.Groups["flags"].Value;
                        var symbol = match.Groups["symbol"].Value;
                        var tblModuleSymbols = entities.tblModuleSymbols;
                        var tblModuleSymbol = new tblModuleSymbol
                        {
                            ModuleSymbolId = Guid.NewGuid(),
                            ModuleId = tblModule.ModuleId,
                            Segment = segment,
                            Offset = offset,
                            Flags = flags,
                            DecoratedSymbol = symbol,
                            UndecoratedSymbol = UndecorateSymbol(symbol)
                        };

                        tblModuleSymbols.AddObject(tblModuleSymbol);
                        entities.SaveChanges();
                    }
                }
            }
            else
            {
                SetStatus("Errors from cvdump: \r\n\r\n{0}", error);
            }

            SetStatus("Exit code from cvdump: {0}", exitCode);
        }

        private static string UndecorateSymbol(string symbol)
        {
            var undname = @"C:\Program Files (x86)\Microsoft Visual Studio 10.0\VC\bin\undname.exe";
            var startInfo = new ProcessStartInfo(undname, string.Format("{0}", symbol));
            var output = string.Empty;
            var error = string.Empty;
            int exitCode;
            Process process;
            string result;

            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;

            process = new Process();

            process.EnableRaisingEvents = true;
            process.StartInfo = startInfo;

            process.OutputDataReceived += (s, e) =>
            {
                output += e.Data + "\r\n";
            };

            process.ErrorDataReceived += (s, e) =>
            {
                error += e.Data + "\r\n";
            };

            process.Start();

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            process.WaitForExit();

            exitCode = process.ExitCode;

            //exitCode = 0;
            //output = File.ReadAllText(@"C:\Projects\MC\RazorViewsDesigner\Net2Html5Package\InterfaceLibrary\cvdump.txt");
            //error = string.Empty;

            if (error.Trim().IsNullOrEmpty())
            {
                var pattern = "^is :- \"(?<undecoratedsymbol>.*?)\"$";
                var regex = new Regex(pattern);
                var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);

                foreach (var readLine in lines)
                {
                    var line = readLine.Trim();

                    if (regex.IsMatch(line))
                    {
                        var match = regex.Match(line.Trim());
                        var undecoratedSymbol = match.Groups["undecoratedsymbol"].Value;

                        return undecoratedSymbol;
                    }
                }

                result = string.Format("Unexpected output from undname: \r\n\r\n{0}", output);
                SetStatus(result);

                return result; 
            }
            else
            {
                result = string.Format("Errors from undname: \r\n\r\n{0}\r\nExitCode:{1}", error, exitCode);
                SetStatus(result);

                return result;
            }
        }

        public static void AddModuleInfo(this tblModule tblModule, Type type, Process process, object objComponent)
        {
            if (type.Name == "__ComObject")
            {
                var modules = process.Modules.AppendHiddenModules(process);
                var pUnk = Marshal.GetIUnknownForObject(objComponent);
            }
            else
            {
                var assembly = type.Assembly;

                tblModule.ModuleName = assembly.GetNameParts().AssemblyName;
                tblModule.ModuleFileName = assembly.CodeBase;
                tblModule.IsClrModule = true;
            }
        }

        public static void SaveVsComObjects(this SdkInterfaceLibraryEntities entities, Process process, object obj, string serviceName, string packageName, string typeName)
        {
            var type = obj.GetType();
            var pUnk = Marshal.GetIUnknownForObject(obj);

            foreach (var assemblyRef in Assembly.GetExecutingAssembly().GetReferencedAssemblies())
            {
                if (assemblyRef.Name.StartsWith("Microsoft.VisualStudio.", "EnvDTE"))
                {
                    var assembly = Assembly.Load(assemblyRef);

                    foreach (var refType in assembly.GetTypes().Where(t => t.IsInterface && t.HasCustomAttribute<GuidAttribute>()))
                    {
                        if (type.Implements(refType))
                        {
                            var interfaceId = entities.GetInterfaceId(refType);

                            if (serviceName != null)
                            {
                                var tblService = entities.SaveIfNotExists<tblService>(s => s.ServiceName == serviceName, () =>
                                {
                                    return new tblService
                                    {
                                        ServiceId = Guid.NewGuid(),
                                        ServiceName = serviceName,
                                        ComponentId = entities.CreateComponent(type, Guid.Empty, process, obj).ComponentId
                                    };
                                });
                            }
                            else if (packageName != null)
                            {
                                Debugger.Break();
                            }
                        }
                        else
                        {
                            foreach (var guidAttr in refType.GetCustomAttributes<GuidAttribute>())
                            {
                                var guid = Guid.Parse(guidAttr.Value);
                                IntPtr ppv;

                                if (Marshal.QueryInterface(pUnk, ref guid, out ppv) == VSConstants.S_OK)
                                {
                                    var tblService = entities.SaveIfNotExists<tblService>(s => s.ServiceName == serviceName, () =>
                                    {
                                        return new tblService
                                        {
                                            ServiceId = Guid.NewGuid(),
                                            ServiceName = serviceName,
                                            ComponentId = entities.CreateComponent(type, Guid.Empty, process, obj).ComponentId
                                        };
                                    });
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void WriteAll(this Microsoft.VisualStudio.OLE.Interop.IServiceProvider serviceProvider, StreamWriter writer, IEnumerable<KeyValuePair<Guid, object>> services = null, IEnumerable<KeyValuePair<Guid, object>> packages = null)
        {
            var process = Process.GetCurrentProcess();
            writer.WriteLineFormat("{0},{1},{2},{3},{4},{5}", "Dll", "Assembly", "Service", "Package", "Type", "Interface");

            serviceProvider.WriteServices(writer);
            WriteTypes(writer);

            if (services != null)
            {
                process.WriteVsComServices(services, writer);
            }

            if (packages != null)
            {
                process.WriteVsComPackages(packages, writer);
            }

            writer.Flush();
        }

        public static void WriteServices(this Microsoft.VisualStudio.OLE.Interop.IServiceProvider serviceProvider, StreamWriter writer)
        {
            var executingAssembly = Assembly.GetExecutingAssembly();

            foreach (var assemblyRef in executingAssembly.GetReferencedAssemblies())
            {
                if (assemblyRef.Name.StartsWith("Microsoft.VisualStudio.", "EnvDTE."))
                {
                    var assembly = Assembly.Load(assemblyRef);

                    foreach (var refType in assembly.GetTypes().Where(t => t.IsInterface && t.HasCustomAttribute<GuidAttribute>()))
                    {
                        var attr = refType.GetCustomAttribute<GuidAttribute>();
                        var guid = Guid.Parse(attr.Value);
                        var IID_IUnknown = new Guid("00000000-0000-0000-C000-000000000046");
                        IntPtr pUnk;

                        if (ErrorHandler.Succeeded(serviceProvider.QueryService(ref guid, ref IID_IUnknown, out pUnk)))
                        {
                            var obj = Marshal.GetObjectForIUnknown(pUnk);

                            obj.WriteVsComObjects(assembly.CodeBase, refType.Name, string.Empty, string.Empty, writer);
                        }
                    }
                }
            }
        }

        public static void WriteTypes(StreamWriter writer)
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            var allTypes = executingAssembly.GetReferencedAssemblies().Where(r => r.Name.StartsWith("Microsoft.VisualStudio.", "EnvDTE.")).SelectMany(r =>
            {
                try
                {
                    var assembly = Assembly.Load(r);
                    return assembly.GetTypes();
                }
                catch
                {
                    return new Type[0];
                }

            });

            foreach (var assemblyRef in executingAssembly.GetReferencedAssemblies())
            {
                if (assemblyRef.Name.StartsWith("Microsoft.VisualStudio.", "EnvDTE."))
                {
                    var assembly = Assembly.Load(assemblyRef);

                    foreach (var refType in assembly.GetTypes().Where(t => t.IsInterface && t.HasCustomAttribute<GuidAttribute>()))
                    {
                        foreach (var type in allTypes)
                        {
                            var assemblyName = string.Empty;
                            var dllName = string.Empty;

                            if (type.Name == "__ComObject")
                            {
                                var typeAssembly = type.Assembly;

                                assemblyName = typeAssembly.GetNameParts().AssemblyName;
                                dllName = typeAssembly.CodeBase;
                            }

                            if (type.Implements(refType))
                            {
                                writer.WriteLineFormat("{0},{1},{2},{3},{4},{5}", dllName, assemblyName, string.Empty, string.Empty, type, refType.FullName);
                            }
                        }
                    }
                }
            }
        }

        public static void WriteVsComServices(this Process process, IEnumerable<KeyValuePair<Guid, object>> objects, StreamWriter writer)
        {
            var modules = process.Modules.AppendHiddenModules(process);

            foreach (var pair in objects)
            {
                string dll;
                var obj = pair.Value;
                var pUnk = Marshal.GetIUnknownForObject(pair.Value);
                var service = GetServiceFromGuid(pair.Key);

                dll = string.Empty;

                obj.WriteVsComObjects(dll, service, string.Empty, string.Empty, writer);
            }
        }

        public static void WriteVsComPackages(this Process process, IEnumerable<KeyValuePair<Guid, object>> objects, StreamWriter writer)
        {
            var modules = process.Modules.AppendHiddenModules(process);

            foreach (var pair in objects)
            {
                string dll;
                var obj = pair.Value;
                var pUnk = Marshal.GetIUnknownForObject(pair.Value);
                var service = GetServiceFromGuid(pair.Key);

                dll = string.Empty;

                obj.WriteVsComObjects(dll, service, string.Empty, string.Empty, writer);
            }
        }

        public static string GetServiceFromGuid(Guid guid)
        {
            string service = null;
            string serviceShort = null;
            var types = new List<Type>();

            foreach (var assemblyRef in Assembly.GetExecutingAssembly().GetReferencedAssemblies())
            {
                if (assemblyRef.Name.StartsWith("Microsoft.VisualStudio.", "EnvDTE."))
                {
                    var assembly = Assembly.Load(assemblyRef);

                    foreach (var refType in assembly.GetTypes().Where(t => t.IsInterface && t.HasCustomAttribute<GuidAttribute>()))
                    {
                        foreach (var guidAttr in refType.GetCustomAttributes<GuidAttribute>())
                        {
                            var refGuid = Guid.Parse(guidAttr.Value);

                            if (refGuid == guid)
                            {
                                if (service == null && refType.GetMembers().Length == 0)
                                {
                                    service = refType.FullName;
                                    serviceShort = refType.Name;
                                }
                                else
                                {
                                    service = refType.FullName;
                                    serviceShort = refType.Name;
                                }
                            }
                        }

                        types.Add(refType);
                    }
                }
            }

            if (serviceShort != null && serviceShort.StartsWith("S", false, CultureInfo.CurrentCulture))
            {
                var serviceName = "I" + serviceShort.RemoveStart(1);

                foreach (var type in types)
                {
                    if (type.Name == serviceName)
                    {
                        service = type.FullName;
                    }
                }
            }

            if (service != null)
            {
                return service;
            }
            else
            {
                return guid.ToString();
            }
        }

        public static void WriteVsComObjects(this object obj, string dllName, string serviceName, string packageName, string typeName, StreamWriter writer)
        {
            var type = obj.GetType();
            var pUnk = Marshal.GetIUnknownForObject(obj);
            var assemblyName = string.Empty;

            if (type.Name != "__ComObject")
            {
                var typeAssembly = type.Assembly;

                assemblyName = typeAssembly.GetNameParts().AssemblyName;
            }

            foreach (var assemblyRef in Assembly.GetExecutingAssembly().GetReferencedAssemblies())
            {
                if (assemblyRef.Name.StartsWith("Microsoft.VisualStudio.", "EnvDTE."))
                {
                    var assembly = Assembly.Load(assemblyRef);

                    foreach (var refType in assembly.GetTypes().Where(t => t.IsInterface && t.HasCustomAttribute<GuidAttribute>()))
                    {
                        if (type.Implements(refType))
                        {
                            writer.WriteLineFormat("{0},{1},{2},{3},{4},{5}", dllName, assemblyName, serviceName, packageName, typeName, refType.FullName);
                        }
                        else
                        {
                            foreach (var guidAttr in refType.GetCustomAttributes<GuidAttribute>())
                            {
                                var guid = Guid.Parse(guidAttr.Value);
                                IntPtr ppv;

                                if (Marshal.QueryInterface(pUnk, ref guid, out ppv) == VSConstants.S_OK)
                                {
                                    writer.WriteLineFormat("{0},{1},{2},{3},{4},{5}", dllName, assemblyName, serviceName, packageName, typeName, refType.FullName);
                                }
                            }
                        }
                    }
                }
            }
        }

        public static string InspectServices(this Microsoft.VisualStudio.OLE.Interop.IServiceProvider serviceProvider)
        {
            var builder = new StringBuilder();
            var executingAssembly = Assembly.GetExecutingAssembly();

            foreach (var assemblyRef in executingAssembly.GetReferencedAssemblies())
            {
                if (assemblyRef.Name.StartsWith("Microsoft.VisualStudio.", "EnvDTE."))
                {
                    var assembly = Assembly.Load(assemblyRef);

                    foreach (var refType in assembly.GetTypes().Where(t => t.IsInterface && t.HasCustomAttribute<GuidAttribute>()))
                    {
                        var attr = refType.GetCustomAttribute<GuidAttribute>();
                        var guid = Guid.Parse(attr.Value);
                        var IID_IUnknown = new Guid("00000000-0000-0000-C000-000000000046");
                        IntPtr pUnk;

                        if (ErrorHandler.Succeeded(serviceProvider.QueryService(ref guid, ref IID_IUnknown, out pUnk)))
                        {
                            var obj = Marshal.GetObjectForIUnknown(pUnk);

                            builder.AppendLineFormat("{0}\r\n\r\n{1}\r\n", refType.Name, obj.InspectVsComObject());
                        }
                    }
                }
            }

            return builder.ToString();
        }

        public static string InspectTypes()
        {
            var builder = new StringBuilder();
            var executingAssembly = Assembly.GetExecutingAssembly();
            var allTypes = executingAssembly.GetTypes().Concat(executingAssembly.GetReferencedAssemblies().SelectMany(r =>
            {
                try
                {
                    var assembly = Assembly.Load(r);
                    return assembly.GetTypes();
                }
                catch
                {
                    return new Type[0];
                }

            }));

            foreach (var assemblyRef in executingAssembly.GetReferencedAssemblies())
            {
                if (assemblyRef.Name.StartsWith("Microsoft.VisualStudio.", "EnvDTE."))
                {
                    var assembly = Assembly.Load(assemblyRef);

                    foreach (var refType in assembly.GetTypes().Where(t => t.IsInterface && t.HasCustomAttribute<GuidAttribute>()))
                    {
                        builder.AppendLineFormat("Types that implement {0}", refType.Name);

                        foreach (var type in allTypes)
                        {
                            if (type.Implements(refType))
                            {
                                builder.AppendLineFormatTabIndent(1, "Implements {0}", refType.FullName);
                            }
                        }
                    }
                }
            }

            return builder.ToString();
        }

        public static string InspectVsComObject(object obj)
        {
            var builder = new StringBuilder();
            var type = obj.GetType();
            var pUnk = Marshal.GetIUnknownForObject(obj);

            foreach (var assemblyRef in Assembly.GetExecutingAssembly().GetReferencedAssemblies())
            {
                if (assemblyRef.Name.StartsWith("Microsoft.VisualStudio.", "EnvDTE."))
                {
                    var assembly = Assembly.Load(assemblyRef);

                    builder.AppendLineFormat("Types from {0}", assemblyRef.Name);

                    foreach (var refType in assembly.GetTypes().Where(t => t.IsInterface && t.HasCustomAttribute<GuidAttribute>()))
                    {
                        if (type.Implements(refType))
                        {
                            builder.AppendLineFormatTabIndent(1, "Implements {0}", refType.FullName);
                        }
                        else
                        {
                            foreach (var guidAttr in refType.GetCustomAttributes<GuidAttribute>())
                            {
                                var guid = Guid.Parse(guidAttr.Value);
                                IntPtr ppv;

                                if (Marshal.QueryInterface(pUnk, ref guid, out ppv) == VSConstants.S_OK)
                                {
                                    builder.AppendLineFormatTabIndent(1, "Implements {0}", refType.FullName);
                                }
                            }
                        }
                    }
                }
            }

            return builder.ToString();
        }

        private static void SetStatus(string format, params object[] args)
        {
            OnStatus.Raise(typeof(VsComObjectUtility), string.Format(format, args));
        }
    }
}
