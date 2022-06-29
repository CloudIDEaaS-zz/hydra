using Microsoft.Extensions.DependencyModel;
using RuntimeEnvironment = Microsoft.DotNet.InternalAbstractions.RuntimeEnvironment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.Versioning;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.IO;
#if !SILVERLIGHT
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.ComponentModel;
using Microsoft.Win32;
//using PostSharp.Aspects.Advices;
#endif

namespace Utils
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class AssemblyExtensions
    {
        public enum AssemblyType
        {
            CompactFramework,
            Silverlight,
            FullFramework,
            NativeBinary
        }

#if !SILVERLIGHT
        private enum FILE_INFO_BY_HANDLE_CLASS
        {
            FileBasicInfo = 0,
            FileStandardInfo = 1,
            FileNameInfo = 2,
            FileRenameInfo = 3,
            FileDispositionInfo = 4,
            FileAllocationInfo = 5,
            FileEndOfFileInfo = 6,
            FileStreamInfo = 7,
            FileCompressionInfo = 8,
            FileAttributeTagInfo = 9,
            FileIdBothDirectoryInfo = 10,// 0x0A
            FileIdBothDirectoryRestartInfo = 11, // 0xB
            FileIoPriorityHintInfo = 12, // 0xC
            FileRemoteProtocolInfo = 13, // 0xD
            FileFullDirectoryInfo = 14, // 0xE
            FileFullDirectoryRestartInfo = 15, // 0xF
            FileStorageInfo = 16, // 0x10
            FileAlignmentInfo = 17, // 0x11
            FileIdInfo = 18, // 0x12
            FileIdExtdDirectoryInfo = 19, // 0x13
            FileIdExtdDirectoryRestartInfo = 20, // 0x14
            MaximumFileInfoByHandlesClass
        }

        private struct SYSTEM_INFO
        {
            public ushort ProcessorArchitecture;
            ushort Reserved;
            public uint PageSize;
            public IntPtr MinimumApplicationAddress;  // minimum address
            public IntPtr MaximumApplicationAddress;  // maximum address
            public IntPtr ActiveProcessorMask;
            public uint NumberOfProcessors;
            public uint ProcessorType;
            public uint AllocationGranularity;
            public ushort ProcessorLevel;
            public ushort ProcessorRevision;
        }

        [Flags]
        private enum ProcessAccessFlags : uint
        {
            All = 0x001F0FFF,
            Terminate = 0x00000001,
            CreateThread = 0x00000002,
            VirtualMemoryOperation = 0x00000008,
            VirtualMemoryRead = 0x00000010,
            VirtualMemoryWrite = 0x00000020,
            DuplicateHandle = 0x00000040,
            CreateProcess = 0x000000080,
            SetQuota = 0x00000100,
            SetInformation = 0x00000200,
            QueryInformation = 0x00000400,
            QueryLimitedInformation = 0x00001000,
            Synchronize = 0x00100000
        }

        [Flags]
        private enum AllocationProtectEnum : uint
        {
            PAGE_EXECUTE = 0x00000010,
            PAGE_EXECUTE_READ = 0x00000020,
            PAGE_EXECUTE_READWRITE = 0x00000040,
            PAGE_EXECUTE_WRITECOPY = 0x00000080,
            PAGE_NOACCESS = 0x00000001,
            PAGE_READONLY = 0x00000002,
            PAGE_READWRITE = 0x00000004,
            PAGE_WRITECOPY = 0x00000008,
            PAGE_GUARD = 0x00000100,
            PAGE_NOCACHE = 0x00000200,
            PAGE_WRITECOMBINE = 0x00000400
        }

        [Flags]
        private enum StateEnum : uint
        {
            MEM_COMMIT = 0x1000,
            MEM_FREE = 0x10000,
            MEM_RESERVE = 0x2000
        }

        [Flags]
        private enum TypeEnum : uint
        {
            MEM_IMAGE = 0x1000000,
            MEM_MAPPED = 0x40000,
            MEM_PRIVATE = 0x20000
        }
        private struct MEMORY_BASIC_INFORMATION
        {
            public uint BaseAddress;
            public uint AllocationBase;
            public AllocationProtectEnum AllocationProtect;
            public uint RegionSize;
            public StateEnum State;
            public AllocationProtectEnum Protect;
            public TypeEnum Type;
        }

        [DllImport("Kernel32", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr OpenFileMapping(uint dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, string lpName);
        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);
        [DllImport("kernel32.dll")]
        private static extern bool ReadProcessMemory(uint hProcess, uint lpBaseAddress, byte[] lpBuffer, uint dwSize, ref uint lpNumberOfBytesRead);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern int VirtualQueryEx(uint hProcess, IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, uint dwLength);
        [DllImport("kernel32.dll")]
        private static extern uint OpenProcess(ProcessAccessFlags processAccess, bool bInheritHandle, uint processId);
        [DllImport("kernel32.dll")]
        private static extern void GetSystemInfo(out SYSTEM_INFO lpSystemInfo);
        [DllImport("psapi.dll", SetLastError = true)]
        private static extern uint GetMappedFileName(uint m_hProcess, uint lpv, StringBuilder lpFilename, uint nSize);

        public static Assembly AssemblyResolve(object sender, ResolveEventArgs e)
        {
            try
            {
                var location = e.RequestingAssembly.Location;
                var parts = AssemblyExtensions.GetNameParts(e.Name);
                var name = parts.AssemblyName;
                var version = parts.Version;
                var regex = new Regex(@"(?<framework>[A-Za-z]*)?,Version=v(?<major>\d)\.?(?<minor>\d)");
                var assembly = Assembly.GetEntryAssembly();
                var targetFramework = assembly.GetFramework();
                var dotNetVersion = Version.Parse(assembly.GetFrameworkVersion());
                var binariesDirectory = new DirectoryInfo(Path.GetDirectoryName(location));
                var packagesDirectory = new DirectoryInfo(Path.GetFullPath(Path.Combine(binariesDirectory.FullName, @"..\..\..\packages")));
                var packageSubDirectories = new List<DirectoryInfo>();
                DirectoryInfo packageDirectory;
                DirectoryInfo packageLibDirectory;
                FrameworkVersion targetFrameworkVersion;

                if (packagesDirectory.Exists)
                {
                    packageSubDirectories = packagesDirectory.GetDirectories().ToList();
                }

                packagesDirectory = new DirectoryInfo(Path.GetFullPath(Path.Combine(binariesDirectory.FullName, @"..\..\..\..\packages")));

                if (packagesDirectory.Exists)
                {
                    if (packageSubDirectories != null)
                    {
                        packageSubDirectories.AddRange(packagesDirectory.GetDirectories());
                    }
                    else
                    {
                        packageSubDirectories = packagesDirectory.GetDirectories().ToList();
                    }
                }

                if (regex.IsMatch(targetFramework))
                {
                    var match = regex.Match(targetFramework);
                    var framework = match.GetGroupValue("framework");
                    var major = match.GetGroupValue("major");
                    var minor = match.GetGroupValue("minor");

                    targetFrameworkVersion = new FrameworkVersion(framework, Version.Parse(major + "." + minor));
                }
                else
                {
                    targetFrameworkVersion = null;
                    DebugUtils.Break();
                }

                packagesDirectory = new DirectoryInfo(Environment.ExpandEnvironmentVariables(@"%userprofile%\.nuget\packages"));
                packageSubDirectories.AddRange(packagesDirectory.GetDirectories());

                packageSubDirectories = packageSubDirectories.OrderBy(p => p.Name).ToList();

                if (packageSubDirectories.Any(d => d.Name.AsCaseless() == name + "." + version))
                {
                    packageDirectory = packageSubDirectories.Single(d => d.Name.AsCaseless() == name + "." + version);
                }
                else if (packageSubDirectories.Any(d => d.Name.AsCaseless() == name && d.GetDirectories().Any(d2 => d2.Name == version)))
                {
                    packageDirectory = packageSubDirectories.Single(d => d.Name.AsCaseless() == name && d.GetDirectories().Any(d2 => d2.Name == version));
                    packageDirectory = new DirectoryInfo(Path.Combine(packageDirectory.FullName, version));
                }
                else
                {
                    packageDirectory = null;
                    return null;
                }

                packageLibDirectory = new DirectoryInfo(Path.Combine(packageDirectory.FullName, "lib"));

                if (packageLibDirectory.Exists)
                {
                    var frameworkVersions = new List<FrameworkVersion>();
                    FrameworkVersion bestFrameworkVersion;
                    DirectoryInfo componentDirectory;

                    foreach (var frameworkDirectory in packageLibDirectory.GetDirectories())
                    {
                        regex = new Regex(@"(?<framework>[a-z]*)?(?<major>\d)\.?(?<minor>\d)");

                        if (regex.IsMatch(frameworkDirectory.Name))
                        {
                            var match = regex.Match(frameworkDirectory.Name);
                            var major = match.GetGroupValue("major");
                            var minor = match.GetGroupValue("minor");
                            var framework = match.GetGroupValue("framework");
                            var frameworkVersion = new FrameworkVersion(framework, Version.Parse(major + "." + minor));

                            if (framework == string.Empty)
                            {
                                framework = "net";
                            }

                            if (framework.AsCaseless() == targetFrameworkVersion.Framework)
                            {
                                frameworkVersions.Add(frameworkVersion);
                            }
                            else if (framework == "standard")
                            {

                            }
                        }
                        else
                        {
                            DebugUtils.Break();
                        }
                    }

                    if (frameworkVersions.Count == 0)
                    {
                        // fallback to standard if none found

                        foreach (var frameworkDirectory in packageLibDirectory.GetDirectories())
                        {
                            regex = new Regex(@"net(?<framework>[a-z]*)?(?<major>\d)\.?(?<minor>\d)");

                            if (regex.IsMatch(frameworkDirectory.Name))
                            {
                                var match = regex.Match(frameworkDirectory.Name);
                                var major = match.GetGroupValue("major");
                                var minor = match.GetGroupValue("minor");
                                var framework = match.GetGroupValue("framework");
                                var frameworkVersion = new FrameworkVersion(framework, Version.Parse(major + "." + minor));

                                if (framework == "standard" && targetFrameworkVersion.Framework == "net")
                                {
                                    componentDirectory = frameworkDirectory;

                                    foreach (var componentDll in componentDirectory.GetFiles(".dll"))
                                    {
                                        if (Path.GetFileNameWithoutExtension(componentDll.FullName) == name)
                                        {
                                            return Assembly.LoadFrom(componentDll.FullName);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                DebugUtils.Break();
                            }
                        }
                    }
                    else
                    {
                        bestFrameworkVersion = frameworkVersions.FirstOrDefault(v => v.Framework.AsCaseless() == targetFrameworkVersion.Framework && v.Version <= dotNetVersion);
                        componentDirectory = new DirectoryInfo(Path.Combine(packageLibDirectory.FullName, string.Format("{0}{1}.{2}", bestFrameworkVersion.Framework, bestFrameworkVersion.Version.Major, bestFrameworkVersion.Version.Minor)));

                        if (!componentDirectory.Exists)
                        {
                            DebugUtils.Break();
                        }

                        foreach (var componentDll in componentDirectory.GetFiles("*.dll"))
                        {
                            if (Path.GetFileNameWithoutExtension(componentDll.FullName) == name)
                            {
                                return Assembly.LoadFrom(componentDll.FullName);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return null;
        }

        public static void SetToAutoRun(this Assembly assembly)
        {
            var runKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);

            runKey.SetValue(assembly.GetNameParts().AssemblyName, assembly.Location);
        }

        public static AssemblyNameParts GetNameParts(this AssemblyName assemblyName)
        {
            var parts = AssemblyExtensions.ParseAssemblyName(assemblyName.FullName);

            return parts;
        }

        public static AssemblyNameParts GetNameParts(this Assembly assembly)
        {
            var parts = AssemblyExtensions.ParseAssemblyName(assembly.FullName);

            return parts;
        }

        public static IEnumerable<Type> GetTypes(this Assembly assembly, bool skipExceptions)
        {
            IEnumerable<Type> types = null;

            try
            {
                types = assembly.GetTypes();
            }
            catch
            {
                if (!skipExceptions)
                {
                    throw;
                }

                return new List<Type>();
            }

            return types;
        }

        public static IEnumerable<Assembly> GetNetCoreAssemblies(this AppDomain appDomain)
        {
            var runtimeId = RuntimeEnvironment.GetRuntimeIdentifier();
            var assemblies = new List<Assembly>();
            
            DependencyContext.Default.GetRuntimeAssemblyNames(runtimeId).ToList().ForEach(a =>
            {
                try
                {
                    var assembly = Assembly.Load(a);

                    assemblies.Add(assembly);
                }
                catch
                {
                }
            });

            return assemblies;
        }

        public static Assembly FindCoreAssembly(this Assembly assembly, string partialPath)
        {
            var assemblyName = assembly.GetName();
            var nuGetFallbackFolder = @"C:\Program Files\dotnet\sdk\NuGetFallbackFolder";
            string assemblyPath = Path.Combine(nuGetFallbackFolder, partialPath);

            assemblyPath = Path.Combine(nuGetFallbackFolder, partialPath);

            if (File.Exists(assemblyPath))
            {
                return Assembly.ReflectionOnlyLoadFrom(assemblyPath);
            }

            return null;
        }

        public static AssemblyNameParts GetNameParts(string name)
        {
            var parts = AssemblyExtensions.ParseAssemblyName(name);

            return parts;
        }

        public static bool HasCustomAttribute<T>(this Assembly assembly)
        {
            var attribute = (T)assembly.GetCustomAttributes(true).OfType<T>().FirstOrDefault();

            return attribute != null;
        }

        public static string GetFramework(this Assembly assembly)
        {
            var frameworkName = string.Empty;
            var frameworkDisplayName = string.Empty;
            var customAttributes = assembly.GetCustomAttributesData();
            var targetFramework = customAttributes.FirstOrDefault(a => (a as dynamic).AttributeType == typeof(TargetFrameworkAttribute));

            if (targetFramework != null)
            {
                if (targetFramework.ConstructorArguments.Any())
                {
                    frameworkName = (string)targetFramework.ConstructorArguments[0].Value;

                    return frameworkName;
                }
            }
            else
            {
                var assemblyNameParts = AssemblyExtensions.ParseAssemblyName(assembly.FullName);

                if (assemblyNameParts.AssemblyName == "mscorlib")
                {
                    return assemblyNameParts.Version;
                }
                else
                {
                    Debugger.Break();
                }
            }

            return null;
        }

        public static string GetFrameworkVersion(this Assembly assembly)
        {
            // .NETFramework,Version=v4.0

            var framework = assembly.GetFramework();
            var version = framework.RegexGet(@".*?(?<version>\d+\.?\d*)", "version");

            return version;
        }

#endif

        public static AssemblyAttributes GetAttributes(this Assembly assembly)
        {
            return new AssemblyAttributes(assembly);
        }

        public static AssemblyNameParts ParseAssemblyName(string name)
        {
            var regex = new Regex(@"^\[?(?<assembly>[\w\.\-]+)(,\s?Version=(?<version>\d+\.\d+\.\d+\.\d+))?(,\s?Culture=(?<culture>[\w\-]+))?(,\s?PublicKeyToken=(?<token>\w+))?(,\s?processorArchitecture=(?<processorarchitecture>\w+))?\]?$");

            if (regex.IsMatch(name))
            {
                var match = regex.Match(name);
                var assemblyName = match.Groups["assembly"].Value.EmptyToNull();
                var version = match.Groups["version"].Value.EmptyToNull();
                var culture = match.Groups["culture"].Value.EmptyToNull();
                var token = match.Groups["token"].Value.EmptyToNull();
                var processorArchitecture = match.Groups["processorarchitecture"].Value.EmptyToNull();

                return new AssemblyNameParts(assemblyName, version, culture, token, processorArchitecture);
            }
            else
            {
                regex = new Regex(@"^\[?(?<type>[\w\.\-]+),\s(?<assembly>[\w\.]+)(,\s?Version=(?<version>\d+\.\d+\.\d+\.\d+))?(,\s?Culture=(?<culture>[\w\-]+))?(,\s?PublicKeyToken=(?<token>\w+))?(,\s?processorArchitecture=(?<processorarchitecture>\w+))?\]?$");

                if (regex.IsMatch(name))
                {
                    var match = regex.Match(name);
                    var type = match.Groups["type"].Value;
                    var assemblyName = match.Groups["assembly"].Value.EmptyToNull();
                    var version = match.Groups["version"].Value.EmptyToNull();
                    var culture = match.Groups["culture"].Value.EmptyToNull();
                    var token = match.Groups["token"].Value.EmptyToNull();
                    var processorArchitecture = match.Groups["processorarchitecture"].Value.EmptyToNull();

                    return new AssemblyNameParts(assemblyName, version, culture, token, processorArchitecture, type);
                }
            }

            return null;
        }

        public static bool IsSilverlight(this Assembly assembly)
        {
            return GetAssemblyType(assembly) == AssemblyType.Silverlight;
        }

        public static AssemblyType GetAssemblyType(this Assembly assembly)
        {
#if SILVERLIGHT
            return AssemblyType.Silverlight;
#else

            AssemblyName mscorlib;

            if (assembly.GetName().Name == "mscorlib")
            {
                mscorlib = assembly.GetName();
            }
            else
            {
                mscorlib = assembly.GetReferencedAssemblies().FirstOrDefault(a => string.Compare(a.Name, "mscorlib", true) == 0);
            }

            var token = BitConverter.ToUInt64(mscorlib.GetPublicKeyToken(), 0);

            switch (token)
            {
                case 0xac22333d05b89d96:
                    return AssemblyType.CompactFramework;
                case 0x89e03419565c7ab7:
                    return AssemblyType.FullFramework;
                case 0x8e79a7bed785ec7c:
                    return AssemblyType.Silverlight;
                default:
                    throw new NotSupportedException();
            }
#endif
        }
    }
}
