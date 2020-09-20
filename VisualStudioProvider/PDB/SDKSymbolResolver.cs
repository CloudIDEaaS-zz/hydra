using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Utils;
using System.Diagnostics;
using CppSharp.Parser;
using CppSharp;
using VisualStudioProvider.PDB.diaapi;
using VisualStudioProvider.PDB.Headers;
using CppParser;
using System.Runtime.InteropServices;
using SDKInterfaceLibrary.Entities;
using System.Data.Objects;
using Utils.WindowsSearch;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using System.Reflection;

namespace VisualStudioProvider.PDB
{
    [Serializable, XmlRoot("SDKSymbolResolver")]
    public class SDKSymbolResolver : MarshalByRefObject
    {
        private const bool TESTING = true;
        private Dictionary<tblVendorSDK, DirectoryInfo> includeDirectories;
        private Dictionary<tblVendorSDK, DirectoryInfo> libraryDirectories;
        public event EventHandlerT<string> OnStatus;
        public event EventHandlerT<ParserDiagnostic> OnParserDiagnostic;
        public event ParserProgressEventHandler OnParserProgress;
        public event LibraryInfoEventHandler OnGetLibraryInfo;
        private System.Type this_ = typeof(SDKSymbolResolver);
        private unsafe DiaDataSource pdbDataSource;
        private IEnumerable<string> imports;
        private SdkInterfaceLibraryEntities entities;
        private ObjectSet<tblSDKHeaderFile> tblSDKHeaderFiles;
        private SearchManager searchManager;
        private List<EventArgsXml> serializedEventArgs;
        private IManagedLockObject lockObject;
        private ObjectSet<tblVendorSDK> tblVendorSDKs;
        private Regex versionRegex;
        private int logCurrentIndent;
        private FileInfo logFile;

        public SDKSymbolResolver()
        {
            var vcDirectory = new DirectoryInfo(VSEnvironment.ExpandEnvironmentVariables("$(VCInstallDir)"));
            var crtDirectory = new DirectoryInfo(VSEnvironment.ExpandEnvironmentVariables(@"$(VCInstallDir)crt\src"));
            var windowsSdkDirectory = new DirectoryInfo(VSEnvironment.ExpandEnvironmentVariables("$(WindowsSdkDir)"));
            var atlmfcDirectory = new DirectoryInfo(VSEnvironment.ExpandEnvironmentVariables(@"$(VCInstallDir)atlmfc"));
            var frameworkSdkDirectory = new DirectoryInfo(VSEnvironment.ExpandEnvironmentVariables(@"$(FrameworkSDKDir)"));
            var connectionString = SDKLibraryConnection.ConnectionString;
            
            logFile = new FileInfo(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "SDKSymbolResolver.log"));

            if (logFile.Exists)
            {
                logFile.Delete();
            }
            
            versionRegex = new Regex(@"v?(?<version>\d+?\.\d+?\w*)");

            lockObject = LockManager.CreateObject();
            searchManager = new SearchManager();
            searchManager.AddCatalog("MSVCInclude");

            entities = new SdkInterfaceLibraryEntities(connectionString);
            tblSDKHeaderFiles = entities.tblSDKHeaderFiles;
            tblVendorSDKs = entities.tblVendorSDKs;

            HeaderEntityExtensions.Entities = entities;
            HeaderEntityExtensions.WriteLog += new EventHandlerT<string>(HeaderEntityExtensions_WriteLog);
            HeaderEntityExtensions.IndentLog += new EventHandler(HeaderEntityExtensions_IndentLog);
            HeaderEntityExtensions.OutdentLog += new EventHandler(HeaderEntityExtensions_OutdentLog);

            var tblMSVendor = entities.SaveIfNotExists<tblVendor>(v => v.VendorName == "Microsoft", () =>
            {
                return new tblVendor
                {
                    VendorId = Guid.NewGuid(),
                    VendorName = "Microsoft"
                };
            });

            if (crtDirectory.Exists)
            {
                var version = string.Empty;

                if (versionRegex.IsMatch(crtDirectory.FullName))
                {
                    var match = versionRegex.Match(crtDirectory.FullName);

                    version = match.Groups["version"].Value;
                }

                var tblMSVendorSdk = entities.SaveIfNotExists<tblVendorSDK>(s => s.SdkName == "C Runtime Language" && s.SdkVersion == version, () =>
                {
                    return new tblVendorSDK
                    {
                        VendorSdkId = Guid.NewGuid(),
                        VendorId = tblMSVendor.VendorId,
                        SdkName = "C Runtime Language",
                        SdkVersion = version,
                        SdkRootFolder = crtDirectory.FullName,
                        IncludeFolders = string.Empty,
                        SourceFolders = string.Empty,
                        LibraryFolders = string.Empty,
                        ExecutableFolders = string.Empty,
                    };
                });
            }
           
            if (vcDirectory.Exists)
            {
                var version = string.Empty;

                if (versionRegex.IsMatch(vcDirectory.FullName))
                {
                    var match = versionRegex.Match(vcDirectory.FullName);

                    version = match.Groups["version"].Value;
                }

                var tblMSVendorSdk = entities.SaveIfNotExists<tblVendorSDK>(s => s.SdkName == "Visual C++" && s.SdkVersion == version, () =>
                {
                    return new tblVendorSDK
                    {
                        VendorSdkId = Guid.NewGuid(),
                        VendorId = tblMSVendor.VendorId,
                        SdkName = "Visual C++",
                        SdkVersion = version,
                        SdkRootFolder = vcDirectory.FullName,
                        IncludeFolders = "include",
                        SourceFolders = "src",
                        LibraryFolders = "lib",
                        ExecutableFolders = "bin",
                    };
                });
            }

            if (windowsSdkDirectory.Exists)
            {
                var version = string.Empty;

                if (versionRegex.IsMatch(vcDirectory.FullName))
                {
                    var match = versionRegex.Match(windowsSdkDirectory.FullName);

                    version = match.Groups["version"].Value;
                }

                var tblMSVendorSdk = entities.SaveIfNotExists<tblVendorSDK>(s => s.SdkName == "Windows SDK" && s.SdkVersion == version, () =>
                {
                    return new tblVendorSDK
                    {
                        VendorSdkId = Guid.NewGuid(),
                        VendorId = tblMSVendor.VendorId,
                        SdkName = "Windows SDK",
                        SdkVersion = version,
                        SdkRootFolder = windowsSdkDirectory.FullName,
                        IncludeFolders = "include",
                        SourceFolders = "src",
                        LibraryFolders = "lib",
                        ExecutableFolders = "bin"
                    };
                });
            }

            if (atlmfcDirectory.Exists)
            {
                var version = string.Empty;

                if (versionRegex.IsMatch(vcDirectory.FullName))
                {
                    var match = versionRegex.Match(atlmfcDirectory.FullName);

                    version = match.Groups["version"].Value;
                }

                var tblMSVendorSdk = entities.SaveIfNotExists<tblVendorSDK>(s => s.SdkName == "ATL MFC" && s.SdkVersion == version, () =>
                {
                    return new tblVendorSDK
                    {
                        VendorSdkId = Guid.NewGuid(),
                        VendorId = tblMSVendor.VendorId,
                        SdkName = "ATL MFC",
                        SdkVersion = version,
                        SdkRootFolder = atlmfcDirectory.FullName,
                        IncludeFolders = "include",
                        SourceFolders = "src",
                        LibraryFolders = "lib",
                        ExecutableFolders = "bin"
                    };
                });
            }

            if (frameworkSdkDirectory.Exists)
            {
                var version = string.Empty;

                if (versionRegex.IsMatch(vcDirectory.FullName))
                {
                    var match = versionRegex.Match(frameworkSdkDirectory.FullName);

                    version = match.Groups["version"].Value;
                }

                var tblMSVendorSdk = entities.SaveIfNotExists<tblVendorSDK>(s => s.SdkName == "Framework SDK" && s.SdkVersion == version, () =>
                {
                    return new tblVendorSDK
                    {
                        VendorSdkId = Guid.NewGuid(),
                        VendorId = tblMSVendor.VendorId,
                        SdkName = "Framework SDK",
                        SdkVersion = version,
                        SdkRootFolder = frameworkSdkDirectory.FullName,
                        IncludeFolders = "include",
                        SourceFolders = "src",
                        LibraryFolders = "lib",
                        ExecutableFolders = "bin"
                    };
                });
            }

            serializedEventArgs = new List<EventArgsXml>();
            includeDirectories = new Dictionary<tblVendorSDK, DirectoryInfo>();
            libraryDirectories = new Dictionary<tblVendorSDK, DirectoryInfo>();
            tblVendorSDKs = entities.tblVendorSDKs;

            foreach (var tblVendorSdk in tblVendorSDKs)
            {
                AddIncludes(tblVendorSdk);
            }

            this.OnParserDiagnostic += new Utils.EventHandlerT<CppSharp.Parser.ParserDiagnostic>(symbolResolver_OnParserDiagnostic);
            this.OnParserProgress += new ParserProgressEventHandler(symbolResolver_OnParserProgress);
        }

        private void HeaderEntityExtensions_OutdentLog(object sender, EventArgs e)
        {
            Debug.Assert(logCurrentIndent != 0);

            logCurrentIndent--;
        }

        private void HeaderEntityExtensions_IndentLog(object sender, EventArgs e)
        {
            logCurrentIndent++;
        }

        private void HeaderEntityExtensions_WriteLog(object sender, EventArgs<string> e)
        {
            var logText = e.Value;

            using (var logWriter = new StreamWriter(logFile.FullName, true))
            {
                logWriter.WriteLineFormatTabIndent(logCurrentIndent, logText);
                logWriter.Flush();
            }
        }

        private void AddIncludes(tblVendorSDK tblVendorSdk)
        {
            var rootFolder = tblVendorSdk.SdkRootFolder;

            if (tblVendorSdk.IncludeFolders.IsNullOrEmpty())
            {
                DirectoryInfo includeDirectory;

                includeDirectory = new DirectoryInfo(tblVendorSdk.SdkRootFolder);

                if (includeDirectory.Exists)
                {
                    if (!includeDirectories.ContainsKey(tblVendorSdk))
                    {
                        if (!includeDirectories.Any(p => p.Value.FullName == includeDirectory.FullName))
                        {
                            includeDirectories.Add(tblVendorSdk, includeDirectory);
                        }
                    }
                }
            }
            else
            {
                foreach (var includeFolder in tblVendorSdk.IncludeFolders.Split(';', StringSplitOptions.RemoveEmptyEntries))
                {
                    DirectoryInfo includeDirectory;

                    if (Path.IsPathRooted(includeFolder))
                    {
                        includeDirectory = new DirectoryInfo(includeFolder);
                    }
                    else
                    {
                        var path = Path.GetFullPath(Path.Combine(rootFolder, includeFolder));

                        includeDirectory = new DirectoryInfo(Path.GetFullPath(path));
                    }

                    if (includeDirectory.Exists)
                    {
                        if (!includeDirectories.ContainsKey(tblVendorSdk))
                        {
                            if (!includeDirectories.Any(p => p.Value.FullName == includeDirectory.FullName))
                            {
                                includeDirectories.Add(tblVendorSdk, includeDirectory);
                            }
                        }
                    }
                }
            }

            if (tblVendorSdk.LibraryFolders.IsNullOrEmpty())
            {
                DirectoryInfo libDirectory;

                libDirectory = new DirectoryInfo(tblVendorSdk.SdkRootFolder);

                if (libDirectory.Exists)
                {
                    if (!libraryDirectories.ContainsKey(tblVendorSdk))
                    {
                        if (!libraryDirectories.Any(p => p.Value.FullName == libDirectory.FullName))
                        {
                            libraryDirectories.Add(tblVendorSdk, libDirectory);
                        }
                    }
                }
            }
            else
            {
                foreach (var libFolder in tblVendorSdk.LibraryFolders.Split(';', StringSplitOptions.RemoveEmptyEntries))
                {
                    DirectoryInfo libDirectory;

                    if (Path.IsPathRooted(libFolder))
                    {
                        libDirectory = new DirectoryInfo(libFolder);
                    }
                    else
                    {
                        var path = Path.GetFullPath(Path.Combine(rootFolder, libFolder));

                        libDirectory = new DirectoryInfo(Path.GetFullPath(path));
                    }

                    if (libDirectory.Exists)
                    {
                        if (!libraryDirectories.ContainsKey(tblVendorSdk))
                        {
                            if (!libraryDirectories.Any(p => p.Value.FullName == libDirectory.FullName))
                            {
                                libraryDirectories.Add(tblVendorSdk, libDirectory);
                            }
                        }
                    }
                }
            }
        }

        private void symbolResolver_OnParserProgress(object sender, ParserProgressEventArgs e)
        {
            using (lockObject.Lock())
            {
                serializedEventArgs.Add(new ParserProgressEventArgsXml(e));
            }
        }

        private void symbolResolver_OnParserDiagnostic(object sender, Utils.EventArgs<CppSharp.Parser.ParserDiagnostic> e)
        {
            using (lockObject.Lock())
            {
                serializedEventArgs.Add(new ParserDiagnosticsEventArgsXml(e));
            }
        }

        [XmlArray("EventArgs")]
        [XmlArrayItem(typeof(EventArgsXml), ElementName = "EventArgs")]
        public List<EventArgsXml> EventArgs
        {
            get
            {
                List<EventArgsXml> list;

                using (lockObject.Lock())
                {
                    list = serializedEventArgs.ToList();

                    serializedEventArgs.Clear();
                }

                return list;
            }
        }

        [XmlArray("HeaderList")]
        [XmlArrayItem(typeof(string), ElementName = "Header")]
        public List<string> HeaderList
        {
            get
            {
                List<string> list;

                using (lockObject.Lock())
                {
                    list = this.GetAllHeaders().ToList();
                }

                return list;
            }
        }

        [XmlElement]
        public string HeaderToParse
        {
            set
            {
                this.ParseHeader(value);
            }
        }

        private IEnumerable<KeyValuePair<tblVendorSDK, DirectoryInfo>> AllDirectories
        {
            get
            {
                return includeDirectories.Concat(libraryDirectories);
            }
        }

        public void Initialize(DiaDataSource pdbDataSource, IEnumerable<string> imports)
        {
            this.pdbDataSource = pdbDataSource;
            this.imports = imports;

            AnalyzeLibraries();
        }

        private void AnalyzeLibraries()
        {
            var libraryFiles = libraryDirectories.Values.SelectMany(d => d.GetFiles()).OrderBy(f => f.Name);
            var compilands = pdbDataSource.GlobalScope.GetChildren(raw.SymTagEnum.SymTagCompiland).DistinctBy(s => s.LibraryName).OrderBy(s => Path.GetFileName(s.LibraryName));

            SetDiagnosticsStatus("Analyzing Used Libraries");

            foreach (var compiland in compilands)
            {
                var libraryFile = Path.GetFileName(compiland.LibraryName);
                var library = libraryFiles.SingleOrDefault(f => f.Name.AsCaseless() == libraryFile);

                if (library != null)
                {
                    OnGetLibraryInfo(this, new LibraryInfoEventArgs(library.FullName));
                }
            }

            foreach (var import in imports)
            {
                var library = libraryFiles.SingleOrDefault(f => Path.GetFileNameWithoutExtension(f.Name).AsCaseless() == Path.GetFileNameWithoutExtension(import));

                if (library != null)
                {
                    OnGetLibraryInfo(this, new LibraryInfoEventArgs(library.FullName));
                }
            }
        }

        public void ParseHeader(string file)
        {
            var parserProject = new Project();
            var parser = new CppSharp.ClangParser();
            var sourceFile = parserProject.AddFile(file);
            var options = new ParserOptions();
            parser.SourcesParsed = OnParserResult;

            options.SetupMSVC(VisualStudioVersion.VS2012);

            foreach (var includeDirectory in includeDirectories.Values)
            {
                options.addSystemIncludeDirs(includeDirectory.FullName);
            }

            sourceFile.Options = options;

            try
            {
                parser.ParseProject(parserProject, false);
            }
            catch (Exception ex)
            {
                SetDiagnosticsStatus("Error parsing {0}, Error:{1}", file, ex.Message);
            }

            parser.ASTContext.Dispose();

            entities.SaveChanges();
        }

        public void ResolveSymbol(string symbolName)
        {
            var searchResults = searchManager.Search(symbolName);

            foreach (var result in searchResults)
            {
                ParseHeader(result);
            }
        }

        public IEnumerable<string> GetAllHeaders()
        {
            SetDiagnosticsStatus("Retrieving SDK Headers");

            if (TESTING)
            {
                foreach (var pair in includeDirectories)
                {
                    var includeDirectory = pair.Value;
                    var vendorSdk = pair.Key;
                    var sdkId = vendorSdk.VendorSdkId;

                    foreach (var file in includeDirectory.GetFiles("*.*", SearchOption.AllDirectories).Take(20).Where(f => f.Extension.IsOneOf(".h", ".c", string.Empty)))
                    {
                        var notExists = false;
                        var tblHeaderFile = entities.SaveIfNotExists<tblSDKHeaderFile>(h => h.HeaderFileName == file.FullName && h.VendorSdkId == sdkId, () =>
                        {
                            notExists = true;

                            return new tblSDKHeaderFile
                            {
                                HeaderFileId = Guid.NewGuid(),
                                HeaderFileName = file.FullName,
                                VendorSdkId = sdkId
                            };
                        });

                        if (notExists || !tblHeaderFile.IsParsedSuccessfully())
                        {
                            yield return file.FullName;
                        }
                    }
                }
            }
            else
            {
                foreach (var pair in includeDirectories)
                {
                    var includeDirectory = pair.Value;
                    var vendorSdk = pair.Key;
                    var sdkId = vendorSdk.VendorSdkId;

                    foreach (var file in includeDirectory.GetFiles("*.*", SearchOption.AllDirectories).Where(f => f.Extension.IsOneOf(".h", ".c", string.Empty)))
                    {
                        var notExists = false;
                        var tblHeaderFile = entities.SaveIfNotExists<tblSDKHeaderFile>(h => h.HeaderFileName == file.FullName && h.VendorSdkId == sdkId, () =>
                        {
                            notExists = true;

                            return new tblSDKHeaderFile
                            {
                                HeaderFileId = Guid.NewGuid(),
                                HeaderFileName = file.FullName,
                                VendorSdkId = sdkId
                            };
                        });

                        if (notExists || !tblHeaderFile.IsParsedSuccessfully())
                        {
                            yield return file.FullName;
                        }
                    }
                }
            }
        }

        public void ParseAllHeaders()
        {
            SetDiagnosticsStatus("Parsing SDK Headers");

            foreach (var includeDirectory in includeDirectories.Values)
            {
                foreach (var file in includeDirectory.GetFiles("*.*", SearchOption.AllDirectories).Where(f => f.Extension.IsOneOf(".h", ".c", string.Empty)))
                {
                    var notExists = false;
                    var tblHeaderFile = entities.SaveIfNotExists<tblSDKHeaderFile>(h => h.HeaderFileName == file.FullName, () =>
                    {
                        notExists = true;

                        return new tblSDKHeaderFile
                        {
                            HeaderFileId = Guid.NewGuid(),
                            HeaderFileName = file.FullName,
                        };
                    });

                    if (notExists || !tblHeaderFile.IsParsedSuccessfully())
                    {
                        ParseHeader(file.FullName);
                    }
                }
            }
        }

        private void SetDiagnosticsStatus(string format, params object[] args)
        {
            var message = string.Format(format, args);
            var diagnostic = new ParserDiagnostic();

            diagnostic.Message = message;
            diagnostic.LineNumber = -1;

            OnStatus.Raise(this_, message);
            OnParserDiagnostic.Raise(this_, diagnostic);
        }

        private void SetStatus(string format, params object[] args)
        {
            var message = string.Format(format, args);
            var diagnostic = new ParserDiagnostic();

            diagnostic.Message = message;
            diagnostic.LineNumber = -1;

            OnStatus.Raise(this_, message);
            OnParserDiagnostic.Raise(this_, diagnostic);
        }

        private void SetParserProgress(int progress, int total, string format, params object[] args)
        {
            var message = string.Format(format, args);

            OnParserProgress(this_, new ParserProgressEventArgs(message, progress, total));
        }

        private void OnParserResult(IList<SourceFile> sources, ParserResult result)
        {
            string status;

            switch (result.Kind)
            {
                case ParserResultKind.Error:

                    status = string.Format("Parsed '{0}' with errors.", sources.Select(s => s.Path).ToCommaDelimitedList());

                    SetDiagnosticsStatus(status);

                    foreach (var source in sources)
                    {
                        var tblHeaderFile = tblSDKHeaderFiles.Single(f => f.HeaderFileName == source.Path);
                        tblHeaderFile.ParsedSuccessfully = false;
                    }

                    entities.SaveChanges();

                    break;

                case ParserResultKind.FileNotFound:

                    status = string.Format("File(s) not found '{0}'.", sources.Select(s => s.Path).ToCommaDelimitedList());
                    SetDiagnosticsStatus(status);

                    break;
                case ParserResultKind.Success:

                    status = string.Format("Parsed '{0}' successfully.", sources.Select(s => s.Path).ToCommaDelimitedList());
                    SetDiagnosticsStatus(status);

                    foreach (var source in sources)
                    {
                        var astContext = source.Options.ASTContext;

                        foreach (var unit in astContext.GetUnits())
                        {
                            var notExists = false;
                            var directoryPair = includeDirectories.SingleOrDefault(p => unit.FileName.AsCaseless().StartsWith(p.Value.FullName));
                            tblSDKHeaderFile tblHeaderFile;
                            tblVendorSDK similarSdk = null;
                            bool hasSimilarSdk = false;

                            if (unit.FileName == "<invalid>")
                            {
                                continue;
                            }

                            if (directoryPair.Key == null)
                            {
                                // has include file from a different version or vendor, attempt to find similar

                                var file = unit.FileName;

                                hasSimilarSdk = tblVendorSDKs.ToList().Any(v => 
                                {
                                    if (similarSdk != null)
                                    {
                                        return false;
                                    }
                                    else if (versionRegex.IsMatch(file))
                                    {
                                        var match = versionRegex.Match(file);
                                        var versionGroup = match.Groups["version"];
                                        var version = versionGroup.Value;
                                        var fileBackDated = file.Substitute(versionGroup.Index, versionGroup.Length, v.SdkVersion);

                                        if (includeDirectories.Any(i => fileBackDated.StartsWith(i.Value.FullName)))
                                        {
                                            foreach (var similarPair in includeDirectories.Where(i => fileBackDated.StartsWith(i.Value.FullName)))
                                            {
                                                var tblVendorSdk = similarPair.Key;
                                            
                                                if (tblVendorSdk.VendorSdkId == v.VendorSdkId)
                                                {
                                                    if (versionRegex.IsMatch(tblVendorSdk.SdkRootFolder))
                                                    {
                                                        string newRoot;
                                                        tblVendorSDK tblVendorSdkNew;

                                                        match = versionRegex.Match(tblVendorSdk.SdkRootFolder);
                                                        versionGroup = match.Groups["version"];
                                                        newRoot = tblVendorSdk.SdkRootFolder.Substitute(versionGroup.Index, versionGroup.Length, version);

                                                        tblVendorSdkNew = new tblVendorSDK
                                                        {
                                                            VendorSdkId = Guid.NewGuid(),
                                                            VendorId = tblVendorSdk.VendorId,
                                                            SdkName = tblVendorSdk.SdkName,
                                                            SdkVersion = version,
                                                            SdkRootFolder = newRoot,
                                                            IncludeFolders = tblVendorSdk.IncludeFolders,
                                                            SourceFolders = tblVendorSdk.SourceFolders,
                                                            LibraryFolders = tblVendorSdk.LibraryFolders,
                                                            ExecutableFolders = tblVendorSdk.ExecutableFolders
                                                        };

                                                        tblVendorSDKs.AddObject(tblVendorSdkNew);
                                                        entities.SaveChanges();

                                                        similarSdk = tblVendorSdkNew;

                                                        return true;
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    return false;
                                });
                            }

                            if (hasSimilarSdk)
                            {
                                var sdkId = similarSdk.VendorSdkId;

                                AddIncludes(similarSdk);

                                tblHeaderFile = entities.SaveIfNotExists<tblSDKHeaderFile>(h => h.HeaderFileName == unit.FileName && h.VendorSdkId == sdkId, () =>
                                {
                                    notExists = true;

                                    return new tblSDKHeaderFile
                                    {
                                        HeaderFileId = Guid.NewGuid(),
                                        HeaderFileName = unit.FileName,
                                        VendorSdkId = similarSdk.VendorSdkId
                                    };
                                });
                            }
                            else if (directoryPair.Key == null)
                            {
                                Debugger.Break();

                                tblHeaderFile = entities.SaveIfNotExists<tblSDKHeaderFile>(h => h.HeaderFileName == unit.FileName, () =>
                                {
                                    notExists = true;

                                    return new tblSDKHeaderFile
                                    {
                                        HeaderFileId = Guid.NewGuid(),
                                        VendorSdkId = null,
                                        HeaderFileName = unit.FileName
                                    };
                                });
                            }
                            else
                            {
                                var sdkId = directoryPair.Key.VendorSdkId;

                                tblHeaderFile = entities.SaveIfNotExists<tblSDKHeaderFile>(h => h.HeaderFileName == unit.FileName && h.VendorSdkId == sdkId, () =>
                                {
                                    notExists = true;

                                    return new tblSDKHeaderFile
                                    {
                                        HeaderFileId = Guid.NewGuid(),
                                        VendorSdkId = directoryPair.Key.VendorSdkId,
                                        HeaderFileName = unit.FileName
                                    };
                                });
                            }

                            if (notExists || !tblHeaderFile.IsParsedSuccessfully())
                            {
                                var header = new Header(unit);
                                var elementCount = header.CacheObjects();
                                var x = 1;
                                Action<DeclarationContext> recurseAdd = null;

                                recurseAdd = (declarationContext) =>
                                {
                                    var tblDeclarationContext = entities.tblSDKHeaderDeclarationContexts.Single(declarationContext.GetWhere(tblHeaderFile));

                                    foreach (var _namespace in declarationContext.Namespaces)
                                    {
                                        if (declarationContext == header)
                                        {
                                            SetParserProgress(++x, elementCount, "Analyzing {0} with {1} elements", unit.Name, elementCount);
                                        }

                                        _namespace.Save(tblHeaderFile, tblDeclarationContext);

                                        recurseAdd(_namespace);
                                    }

                                    foreach (var _class in declarationContext.Classes)
                                    {
                                        if (declarationContext == header)
                                        {
                                            SetParserProgress(++x, elementCount, "Analyzing {0} with {1} elements", unit.Name, elementCount);
                                        }

                                        _class.Save(tblHeaderFile, tblDeclarationContext);

                                        recurseAdd(_class);
                                    }

                                    foreach (var template in declarationContext.Templates)
                                    {
                                        if (declarationContext == header)
                                        {
                                            SetParserProgress(++x, elementCount, "Analyzing {0} with {1} elements", unit.Name, elementCount);
                                        }

                                        template.Save(tblHeaderFile, tblDeclarationContext);
                                    }

                                    foreach (var preprocessedEntity in declarationContext.PreprocessedEntities)
                                    {
                                        preprocessedEntity.Save(tblHeaderFile, tblDeclarationContext.tblSDKHeaderDeclaration);
                                    }

                                    foreach (var _enum in declarationContext.Enums)
                                    {
                                        if (declarationContext == header)
                                        {
                                            SetParserProgress(++x, elementCount, "Analyzing {0} with {1} elements", unit.Name, elementCount);
                                        }

                                        _enum.Save(tblHeaderFile, tblDeclarationContext);
                                    }

                                    foreach (var typeAlias in declarationContext.TypeAliases)
                                    {
                                        if (declarationContext == header)
                                        {
                                            SetParserProgress(++x, elementCount, "Analyzing {0} with {1} elements", unit.Name, elementCount);
                                        }

                                        typeAlias.Save(tblHeaderFile, tblDeclarationContext);
                                    }

                                    foreach (var typeDef in declarationContext.TypeDefs)
                                    {
                                        if (declarationContext == header)
                                        {
                                            SetParserProgress(++x, elementCount, "Analyzing {0} with {1} elements", unit.Name, elementCount);
                                        }

                                        typeDef.Save(tblHeaderFile, tblDeclarationContext);
                                    }

                                    foreach (var variable in declarationContext.Variables)
                                    {
                                        if (declarationContext == header)
                                        {
                                            SetParserProgress(++x, elementCount, "Analyzing {0} with {1} elements", unit.Name, elementCount);
                                        }

                                        variable.Save(tblHeaderFile, tblDeclarationContext);
                                    }

                                    foreach (var function in declarationContext.Functions)
                                    {
                                        if (declarationContext == header)
                                        {
                                            SetParserProgress(++x, elementCount, "Analyzing {0} with {1} elements", unit.Name, elementCount);
                                        }

                                        function.Save(tblHeaderFile, tblDeclarationContext);
                                    }

                                    foreach (var friend in declarationContext.Friends)
                                    {
                                        if (declarationContext == header)
                                        {
                                            SetParserProgress(++x, elementCount, "Analyzing {0} with {1} elements", unit.Name, elementCount);
                                        }

                                        friend.Save(tblHeaderFile, tblDeclarationContext);
                                    }

                                    if (declarationContext == header)
                                    {
                                        foreach (var macro in header.Macros)
                                        {
                                            if (declarationContext == header)
                                            {
                                                SetParserProgress(++x, elementCount, "Analyzing {0} with {1} elements", unit.Name, elementCount);
                                            }

                                            macro.Save(tblHeaderFile, tblDeclarationContext.tblSDKHeaderDeclaration);
                                        }
                                    }
                                };

                                header.Save(tblHeaderFile, true);

                                recurseAdd(header);

                                tblHeaderFile.ParsedSuccessfully = true;

                                entities.SaveChanges();
                            }
                        }
                    }

                    break;
            }
        }
    }
}
