using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.IO.Compression;
using System.Diagnostics;
using ICSharpCode.SharpZipLib.Zip;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml;
using CodeInterfaces;
using System.Configuration;
using Utils;
using System.Web;
using Microsoft.Win32;
using ImpromptuInterface;
using System.Drawing;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using VisualStudioProvider.Configuration.Services;

namespace VisualStudioProvider.Configuration
{
    public delegate void OnIndexingStatusHandler(string status);

    public static class VSConfigProvider
    {
        public static event OnIndexingStatusHandler OnIndexingStatus;
        private static Queue<string> statusQueue;
        private static Thread projectTemplateIndexerThread;
        private static object lockObject;
        private static Thread itemTemplateIndexerThread;
        private static Thread packageIndexerThread;
        private static Thread projectIndexerThread;
        private static ManualResetEvent projectTemplateIndexerEvent;
        private static ManualResetEvent itemTemplateIndexerEvent;
        private static ManualResetEvent packageIndexerEvent;
        private static ManualResetEvent directoriesEvent;
        private static ManualResetEvent projectsEvent;
        private static int templateCount;
        private static int templatesProcessed;
        private static bool indexStarted;
        private static VsServiceProvider vsServiceProvider;
        private static Dictionary<string, ICodeItemTemplate> itemTemplates;
        private static Dictionary<string, VSProject> projects;
        private static Dictionary<string, ICodeProjectTemplate> projectTemplates;
        private static Dictionary<string, ICodeTemplate> projectGroupTemplates;
        private static Dictionary<Guid, VSPackage> packages;
        private static Dictionary<Guid, VSTemplateDirectory> projectTemplateDirectories;
        private static Dictionary<string, VSTemplateDirectory> itemTemplateDirectories;
        private static Dictionary<Guid, VSProjectFactoryProject> factoryProjects;
        private static Dictionary<Guid, VSService> services;
        private const int INDEX_WAIT_MINUTES = 10;
        public static ThreadPriority ThreadPriority { get; set; }

        static VSConfigProvider()
        {
            vsServiceProvider = new VsServiceProvider();

            if (HttpContext.Current != null)
            {
                ThreadPriority = System.Threading.ThreadPriority.Highest;
            }
            else
            {
                ThreadPriority = System.Threading.ThreadPriority.Lowest;
            }

            lockObject = new object();
            statusQueue = new Queue<string>();

            try
            {
                DoCount();
            }
            catch
            {
            }
        }

        public static VsServiceProvider ServiceProvider
        {
            get
            {
                if (!indexStarted)
                {
                    Index();
                }

                if (!packageIndexerEvent.WaitOne(new TimeSpan(0, INDEX_WAIT_MINUTES, 0)))
                {
                    throw new Exception("");
                }

                return vsServiceProvider;
            }
        }

        public static Dictionary<Guid, VSService> Services
        {
            get
            {
                if (!packageIndexerEvent.WaitOne(new TimeSpan(0, INDEX_WAIT_MINUTES, 0)))
                {
                    throw new Exception("");
                }

                return services;
            }
        }

        public static int TemplatesProcessed
        {
            get 
            { 
                var processed = 0;

                lock (lockObject)
                {
                    processed = VSConfigProvider.templatesProcessed;
                }

                return processed; 
            }
            
            set 
            {
                lock (lockObject)
                {
                    VSConfigProvider.templatesProcessed = value;
                }
            }
        }

        public static int TemplateCount
        {
            get 
            {
                var count = 0;

                lock (lockObject)
                {
                    count = VSConfigProvider.templateCount;
                }

                return count; 
            }
            
            set     
            {
                lock (lockObject)
                {
                    VSConfigProvider.templateCount = value;
                }
            }
        }

        public static Dictionary<Guid, VSProjectFactoryProject> ProjectFactoryProjects
        {
            get
            {
                if (!packageIndexerEvent.WaitOne(new TimeSpan(0, INDEX_WAIT_MINUTES, 0)))
                {
                    throw new Exception("");
                }

                return VSConfigProvider.factoryProjects;
            } 
        }

        public static Dictionary<Guid, VSPackage> Packages
        {
            get 
            {
                if (!packageIndexerEvent.WaitOne(new TimeSpan(0, INDEX_WAIT_MINUTES, 0)))
                {
                    throw new Exception("");
                }

                return VSConfigProvider.packages; 
            }
        }

        public static Dictionary<Guid, VSTemplateDirectory> ProjectTemplateDirectories
        {
            get 
            {
                if (!directoriesEvent.WaitOne(new TimeSpan(0, INDEX_WAIT_MINUTES, 0)))
                {
                    throw new Exception("");
                }

                return VSConfigProvider.projectTemplateDirectories; 
            }
        }

        public static Dictionary<string, VSTemplateDirectory> ItemTemplateDirectories
        {
            get
            {
                if (!directoriesEvent.WaitOne(new TimeSpan(0, INDEX_WAIT_MINUTES, 0)))
                {
                    throw new Exception("");
                }

                return VSConfigProvider.itemTemplateDirectories;
            }
        }

        public static VSTemplateDirectory ProjectTemplateDirectoryTree
        {
            get
            {
                if (!directoriesEvent.WaitOne(new TimeSpan(0, INDEX_WAIT_MINUTES, 0)))
                {
                    throw new Exception("");
                }

                var rootDirectory = new VSTemplateDirectory("Root");
                var topLevelDirs = projectTemplateDirectories.Values.Where(d1 =>
                {
                    var isChildFolder = projectTemplateDirectories.Values.Any(d2 => 
                    {
                        return d2.SubDirectories != null && d2.SubDirectories.Any(d3 => 
                        {
                            var parent = d2;
                            var matches = d3.Guid == d1.Guid;

                            return matches;
                        });
                    });

                    return isChildFolder;
                });

                rootDirectory.SubDirectories.AddRange(topLevelDirs);

                return rootDirectory;
            }
        }

        private static void RaiseOnIndexingStatus(string status)
        {
            if (OnIndexingStatus != null)
            {
                while (statusQueue.Count > 0)
                {
                    OnIndexingStatus(statusQueue.Dequeue());
                }

                OnIndexingStatus(status);
            }
            else
            {
                statusQueue.Enqueue(status);
            }
        }

        public static void Index(VSConfigIndexOptions options = null)
        {
            if (!indexStarted)
            {
                var stopWatch = new Stopwatch();

                RaiseOnIndexingStatus("Indexing engine started");
                stopWatch.Start();

                itemTemplates = new Dictionary<string, ICodeItemTemplate>();
                projectTemplates = new Dictionary<string, ICodeProjectTemplate>();
                projectGroupTemplates = new Dictionary<string, ICodeTemplate>();
                packages = new Dictionary<Guid, VSPackage>();
                projectTemplateDirectories = new Dictionary<Guid, VSTemplateDirectory>();
                itemTemplateDirectories = new Dictionary<string, VSTemplateDirectory>();
                factoryProjects = new Dictionary<Guid, VSProjectFactoryProject>();
                services = new Dictionary<Guid, VSService>();

                projectTemplateIndexerEvent = new ManualResetEvent(false);
                itemTemplateIndexerEvent = new ManualResetEvent(false);
                packageIndexerEvent = new ManualResetEvent(false);

                if (options == null || options.LoadEnvironmentServices)
                {
                    EnvironmentService = VSEnvironmentService.LoadEnvironmentServices();
                }

                if (options == null || options.IndexProjectTemplates)
                {
                    // start project templates
                    
                    if (options == null)
                    {
                        options = new VSConfigIndexOptions();
                    }

                    options.IndexPackages = true;

                    projectTemplateIndexerThread = new Thread(ProjectTemplateIndexerThread);

                    projectTemplateIndexerThread.Priority = ThreadPriority;

                    projectTemplateIndexerThread.Start(options);
                }

                if (options == null || options.IndexItemTemplates)
                {
                    // start item templates

                    if (options == null)
                    {
                        options = new VSConfigIndexOptions();
                    }

                    options.IndexPackages = true;

                    itemTemplateIndexerThread = new Thread(ItemTemplateIndexerThread);

                    itemTemplateIndexerThread.Priority = ThreadPriority;

                    itemTemplateIndexerThread.Start();
                }

                // start packages

                if (options == null || options.IndexPackages)
                {
                    packageIndexerThread = new Thread(PackageIndexerThread);

                    packageIndexerThread.Priority = ThreadPriority;

                    packageIndexerThread.Start();
                }

                directoriesEvent = new ManualResetEvent(false);

                var task = new Task(() =>
                {
                    if (options == null || options.IndexItemTemplates)
                    {
                        itemTemplateIndexerEvent.WaitOne();
                    }

                    if (options == null || options.IndexProjectTemplates)
                    {
                        projectTemplateIndexerEvent.WaitOne();
                    }

                    ProcessTemplateDirectories(options);

                    stopWatch.Stop();
                    RaiseOnIndexingStatus("Indexing engine complete in " + stopWatch.Elapsed.Seconds.ToString() + " seconds.");
                });

                task.Start();

                indexStarted = true;
            }
        }

        private static void DoCount()
        {
            var projectTemplateDirectory = new DirectoryInfo(VSConfig.ProjectTemplateDirectory);
            var itemTemplateDirectory = new DirectoryInfo(VSConfig.ItemTemplateDirectory);
            var additionalLocations = (List<CustomTemplateLocation>)ConfigurationManager.GetSection("VSTemplateLocationsSection/VSTemplateLocations");

            Action<DirectoryInfo> recurseCount = null;

            recurseCount = (DirectoryInfo d) =>
            {
                var languageID = Thread.CurrentThread.CurrentCulture.LCID;
                var resultNotUsed = 0;
                var subDirectories = d.GetDirectories();
                var languageRoot = subDirectories.Length > 0 && subDirectories.All(d2 => int.TryParse(d2.Name, out resultNotUsed));

                var files = d.GetFiles("*.zip");

                templateCount += files.Count();

                if (languageRoot)
                {
                    d.GetDirectories().Where(d2 => d2.Name == languageID.ToString()).ToList().ForEach(d2 => recurseCount(d2));
                }
                else
                {
                    d.GetDirectories().ToList().ForEach(d2 => recurseCount(d2));
                }
            };

            if (projectTemplateDirectory.Exists)
            {
                recurseCount(projectTemplateDirectory);
            }

            if (itemTemplateDirectory.Exists)
            {
                recurseCount(itemTemplateDirectory);
            }

            if (additionalLocations != null)
            {
                additionalLocations.ForEach(c => recurseCount(new DirectoryInfo(c.TemplateLocation.Expand())));
            }
        }

        private static void PackageIndexerThread()
        {
            var stopWatch = new Stopwatch();

            RaiseOnIndexingStatus("Package indexer started");
            stopWatch.Start();

            var templateDirsKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\VisualStudio\10.0_Config\NewProjectTemplates\TemplateDirs");
            var packagesKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\VisualStudio\10.0_Config\Packages");
            var projectsKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\VisualStudio\10.0_Config\Projects");
            var servicesKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\VisualStudio\10.0_Config\Services");

            VSTemplateDirectory.LoadParentDirectory += (Guid parentGuid, out VSTemplateDirectory parent) =>
            {
                var pseudoFolderKey = Registry.CurrentUser.OpenSubKey(string.Format(@"Software\Microsoft\VisualStudio\10.0_Config\NewProjectTemplates\PseudoFolders\{{{0}}}", parentGuid.ToString()));
                var keyIndexable = pseudoFolderKey.ToIndexable();
                var packageGuid = Guid.Parse((string) keyIndexable["Package"]);

                if (projectTemplateDirectories.ContainsKey(parentGuid))
                {
                    parent = projectTemplateDirectories[parentGuid];
                }
                else
                {
                    var packageKey = Registry.CurrentUser.OpenSubKey(string.Format(@"Software\Microsoft\VisualStudio\10.0_Config\Packages\{{{0}}}", packageGuid));
                    var package = new VSPackage(packageGuid, packageKey);

                    parent = new VSTemplateDirectory(pseudoFolderKey, package, true);

                    projectTemplateDirectories.Add(parent.Guid, parent);
                }
            };

            foreach (var packageKey in packagesKey.Enumerate())
            {
                try
                {
                    var package = new VSPackage(new Guid(packageKey.SubName), packageKey.Key);

                    packages.Add(package.PackageGuid, package);
                }
                catch 
                {
                }
            }

            foreach (var dirKey in templateDirsKey.Enumerate())
            {
                var packageKey = Registry.CurrentUser.OpenSubKey(string.Format(@"Software\Microsoft\VisualStudio\10.0_Config\Packages\{0}", dirKey.SubName));
                var package = packages[new Guid(dirKey.SubName)];
                var templateDir = new VSTemplateDirectory(dirKey.Key, package);

                package.TemplateDirectory = templateDir;

                projectTemplateDirectories.Add(templateDir.Guid, templateDir);
            }

            VSProjectFactoryProject.LoadPackage += (Guid guid, out VSPackage package) =>
            {
                package = packages[guid];
            };

            VSProjectFactoryProject.LoadDirectory += (Guid guid, out VSTemplateDirectory directory) =>
            {
                if (projectTemplateDirectories.ContainsKey(guid))
                {
                    directory = projectTemplateDirectories[guid];
                }
                else
                {
                    directory = null;
                }
            };

            foreach (var projectKey in projectsKey.Enumerate())
            {
                var project = new VSProjectFactoryProject(new Guid(projectKey.SubName), projectKey.Key);

                factoryProjects.Add(project.ProjectGuid, project);
            }

            foreach (var serviceKey in servicesKey.Enumerate())
            {
                var service = new VSPackageService(new Guid(serviceKey.SubName), serviceKey.Key);

                if (service.PackageGuid != Guid.Empty)
                {
                    service.Package = packages[service.PackageGuid];

                    services.Add(service.ServiceGuid, service);
                }
            }

            packageIndexerEvent.Set();

            stopWatch.Stop();
            RaiseOnIndexingStatus("Package indexer complete in " + stopWatch.Elapsed.Seconds.ToString() + " seconds.");
        }

        private static void ItemTemplateIndexerThread()
        {
            try
            {
                var itemTemplateDirectory = new DirectoryInfo(VSConfig.ItemTemplateDirectory);
                var stopWatch = new Stopwatch();

                RaiseOnIndexingStatus("Item template indexer started");
                stopWatch.Start();

                Debug.Assert(itemTemplateDirectory.Exists);

                if (ItemTempWorkspace.Exists)
                {
                    try
                    {
                        RecurseDelete(ItemTempWorkspace);
                    }
                    catch
                    {
                    }
                }

                RecurseDirectory(itemTemplateDirectory);

                if (ItemTempWorkspace.Exists)
                {
                    try
                    {
                        RecurseDelete(ItemTempWorkspace);
                    }
                    catch
                    {
                    }
                }

                stopWatch.Stop();
                RaiseOnIndexingStatus("Item template indexer complete in " + stopWatch.Elapsed.Seconds.ToString() + " seconds.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            itemTemplateIndexerEvent.Set();
        }

        private static void ProjectTemplateIndexerThread(object parm)
        {
            try
            {
                var projectTemplateDirectory = new DirectoryInfo(VSConfig.ProjectTemplateDirectory);
                var itemTemplateDirectory = new DirectoryInfo(VSConfig.ItemTemplateDirectory);
                var additionalLocations = (List<CustomTemplateLocation>)ConfigurationManager.GetSection("VSTemplateLocationsSection/VSTemplateLocations");
                var stopWatch = new Stopwatch();
                VSConfigIndexOptions options = null;

                if (parm != null)
                {
                    options = (VSConfigIndexOptions)parm;
                }

                RaiseOnIndexingStatus("Project template indexer started");
                stopWatch.Start();

                Debug.Assert(projectTemplateDirectory.Exists);

                if (ProjectTempWorkspace.Exists)
                {
                    try
                    {
                        RecurseDelete(ProjectTempWorkspace);
                    }
                    catch 
                    { 
                    }
                }

                RecurseDirectory(projectTemplateDirectory, null, options);

                if (additionalLocations != null)
                {
                    additionalLocations.ForEach(c => RecurseDirectory(new DirectoryInfo(c.TemplateLocation.Expand()), new DirectoryInfo(c.CommonLocation.Expand()), options));
                }

                if (ProjectTempWorkspace.Exists)
                {
                    try
                    {
                        RecurseDelete(ProjectTempWorkspace);
                    }
                    catch
                    {
                    }
                }

                stopWatch.Stop();
                RaiseOnIndexingStatus("Project template indexer complete in " + stopWatch.Elapsed.Seconds.ToString() + " seconds.");
            }
            catch (Exception ex)
            {
                throw;
            }

            projectTemplateIndexerEvent.Set();
        }

        private static void ProcessTemplateDirectories(VSConfigIndexOptions options = null)
        {
            var stopWatch = new Stopwatch();

            RaiseOnIndexingStatus("Processing template directories started");
            stopWatch.Start();

            var dirs = projectTemplateDirectories.Values.OrderBy(d => d.SortPriority);

            projectTemplateDirectories.Values.ToList().ForEach(d =>
            {
                Action<int, VSTemplateDirectory> recurse = null;

                recurse = (int indent, VSTemplateDirectory dir) =>
                {
                    // search based on TemplateDir in directory entry (most likely finds none)

                    if (dir.AllTemplateDirs != null)
                    {
                        dir.AllTemplateDirs.ForEach(td =>
                        {
                            var dirTemplates = projectTemplates.Values.Where(t => t.TemplateLocation.StartsWith(td.FullName));

                            if (dirTemplates.Count() > 0)
                            {
                                dirTemplates.ToList().ForEach(t => 
                                {
                                    dir.AllTemplates.Add(t);
                                });
                            }
                        });
                    }

                    // search based on project type

                    if (dir.ProjectType != null)
                    {
                        var dirTemplates2 = projectTemplates.Values.Where(t => t.ProjectTypeName == dir.ProjectType);

                        if (dirTemplates2.Count() > 0)
                        {
                            dirTemplates2.ToList().ForEach(t =>
                            {
                                dir.AllTemplates.Add(t);
                            });
                        }

                        dir.BuildOut();
                        dir.SubDirectories.ForEach(d2 => recurse(indent + 1, d2));
                    }
                };

                recurse(0, d);
            });

            if (options.IndexItemTemplates)
            {
                var generalDirectory = new VSTemplateDirectory("General");

                foreach (var itemTemplate in VSConfigProvider.ItemTemplates.Values.Where(t => string.IsNullOrEmpty(t.TemplateGroupID)).OrderBy(t => t.Name))
                {
                    generalDirectory.AllTemplates.Add(itemTemplate);
                }

                generalDirectory.BuildOut();

                itemTemplateDirectories.Add(generalDirectory.Name, generalDirectory);

                var groups = VSConfigProvider.ItemTemplates.Values.Where(t => !string.IsNullOrEmpty(t.TemplateGroupID)).Select(t => t.TemplateGroupID).Distinct();

                foreach (var group in groups)
                {
                    var groupDirectory = new VSTemplateDirectory(group);

                    foreach (var itemTemplate in VSConfigProvider.ItemTemplates.Values.Where(t => !string.IsNullOrEmpty(t.TemplateGroupID) && t.TemplateGroupID == group).OrderBy(t => t.Name))
                    {
                        groupDirectory.AllTemplates.Add(itemTemplate);
                    }

                    groupDirectory.BuildOut();

                    itemTemplateDirectories.Add(groupDirectory.Name, groupDirectory);
                }
            }

            stopWatch.Stop();
            RaiseOnIndexingStatus("Processing template directories complete in " + stopWatch.Elapsed.Seconds.ToString() + " seconds.");

            directoriesEvent.Set();
        }

        private static DirectoryInfo ProjectTempWorkspace
        {
            get
            {
                return new DirectoryInfo(Path.Combine(Path.GetTempPath(), @"Hydra\ProjectTemplateProviderWorkspace"));
            }
        }

        private static DirectoryInfo ItemTempWorkspace
        {
            get
            {
                return new DirectoryInfo(Path.Combine(Path.GetTempPath(), @"Hydra\ItemTemplateProviderWorkspace"));
            }
        }

        private static void RecurseDelete(string directoryPath)
        {
            var directory = new DirectoryInfo(directoryPath);

            RecurseDelete(directory);
        }

        private static void RecurseDelete(DirectoryInfo directory)
        {
            foreach (var fileInfo in directory.GetFiles())
            {
                fileInfo.Delete();
            }

            foreach (var subDir in directory.GetDirectories())
            {
                RecurseDelete(subDir);
            }

            directory.Delete(true);
        }

        private static void RecurseDirectory(DirectoryInfo directory, DirectoryInfo commonLocation = null, VSConfigIndexOptions options = null)
        {
            var resultNotUsed = 0;
            var subDirectories = directory.GetDirectories();
            var languageRoot = subDirectories.Length > 0 && subDirectories.All(d => int.TryParse(d.Name, out resultNotUsed));
            var languageID = Thread.CurrentThread.CurrentCulture.LCID;

            foreach (var fileInfo in directory.GetFiles("*.zip"))
            {
                ProcessZip(fileInfo, commonLocation);
                TemplatesProcessed++;
            }

            if (languageRoot)
            {
                foreach (var subDir in subDirectories.Where(d => d.Name == languageID.ToString()))
                {
                    if (options.ProjectTemplatesFolderRegex != null)
                    {
                        if (subDir.FullName.RegexIsMatch(options.ProjectTemplatesFolderRegex))
                        {
                            RecurseDirectory(subDir, commonLocation, options);
                        }
                    }
                    else
                    {
                        RecurseDirectory(subDir, commonLocation, options);
                    }
                }
            }
            else
            {
                foreach (var subDir in subDirectories)
                {
                    RecurseDirectory(subDir, commonLocation, options);
                }
            }
        }

        private static string GetPathPrefix(FileInfo file)
        {
            if (file.DirectoryName.ToLower().StartsWith(VSConfig.ItemTemplateDirectory.ToLower()))
            {
                return file.DirectoryName.Substring(VSConfig.ItemTemplateDirectory.Length);
            }
            else if (file.DirectoryName.ToLower().StartsWith(VSConfig.ProjectTemplateDirectory.ToLower()))
            {
                return file.DirectoryName.Substring(VSConfig.ProjectTemplateDirectory.Length);
            }
            else
            {
                return file.Directory.Name;
            }
        }

        private static string GetTemplateTypeFromPath(FileInfo file)
        {
            if (file.DirectoryName.ToLower().StartsWith(VSConfig.ItemTemplateDirectory.ToLower()))
            {
                return Enum.GetName(typeof(TemplateType), TemplateType.Item);
            }
            else if (file.DirectoryName.ToLower().StartsWith(VSConfig.ProjectTemplateDirectory.ToLower()))
            {
                return Enum.GetName(typeof(TemplateType), TemplateType.Project);
            }
            else
            {
                return Enum.GetName(typeof(TemplateType), TemplateType.Custom);
            }
        }

        public static Dictionary<string, ICodeTemplate> ProjectGroupTemplates
        {
            get
            {
                if (!projectTemplateIndexerEvent.WaitOne(new TimeSpan(0, INDEX_WAIT_MINUTES, 0)))
                {
                    throw new Exception("");
                }

                return projectGroupTemplates;
            }
        }

        public static bool TemplatesReady
        {
            get
            {
                return projectTemplateIndexerEvent.WaitOne(1);
            }
        }

        public static Dictionary<string, ICodeProjectTemplate> ProjectTemplates
        {
            get
            {
                if (!projectTemplateIndexerEvent.WaitOne(new TimeSpan(0, INDEX_WAIT_MINUTES, 0)))
                {
                    throw new Exception("");
                }

                return projectTemplates;
            }
        }

        public static Dictionary<string, ICodeItemTemplate> ItemTemplates
        {
            get 
            {
                if (!itemTemplateIndexerEvent.WaitOne(new TimeSpan(0, INDEX_WAIT_MINUTES, 0)))
                {
                    throw new Exception("");
                }

                return itemTemplates;
            }
        }

        public static VSEnvironmentService EnvironmentService { get; private set; }

        public static string Decompress(FileInfo fileInfo, string outputDirectory, bool overwriteExisting = true, List<string> skip = null)
        {
            var templateFilePath = (string)null;

            lock (projectTemplateIndexerEvent)
            {
                using (var stream = fileInfo.OpenRead())
                {
                    var curFile = fileInfo.FullName;
                    var origName = curFile.Remove(curFile.Length - fileInfo.Extension.Length);

                    using (var zipStream = new ZipInputStream(stream))
                    {
                        ZipEntry entry;

                        while ((entry = zipStream.GetNextEntry()) != null)
                        {
                            if (entry.IsFile)
                            {
                                if (skip == null || !skip.Any(s => s.ToLower() == entry.Name.ToLower()))
                                {
                                    var filePath = Path.Combine(outputDirectory, entry.Name);
                                    var fileInfo2 = new FileInfo(filePath);
                                    var count = 0;

                                    if (entry.Name.ToLower().EndsWith(".vstemplate"))
                                    {
                                        templateFilePath = filePath;
                                    }

                                    while (count < 10)
                                    {
                                        try
                                        {
                                            var directory = new DirectoryInfo(fileInfo2.DirectoryName);

                                            if (!directory.Exists)
                                            {
                                                directory.Create();
                                            }

                                            if (overwriteExisting)
                                            {
                                                if (fileInfo2.Exists)
                                                {
                                                    if (fileInfo2.IsReadOnly)
                                                    {
                                                        fileInfo2.MakeWritable();
                                                    }

                                                    fileInfo2.Delete();
                                                }
                                            }

                                            using (var streamWriter = File.Create(filePath))
                                            {
                                                var size = 2048;
                                                var data = new byte[2048];

                                                while (true)
                                                {
                                                    size = zipStream.Read(data, 0, data.Length);

                                                    if (size > 0)
                                                    {
                                                        streamWriter.Write(data, 0, size);
                                                    }
                                                    else
                                                    {
                                                        break;
                                                    }
                                                }

                                                streamWriter.Flush();
                                                streamWriter.Close();
                                            }

                                            break;
                                        }
                                        catch (Exception ex)
                                        {
                                            Thread.Sleep(100);
                                        }

                                        count++;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            Debug.Assert(!string.IsNullOrEmpty(templateFilePath));

            return templateFilePath;
        }

        private static DirectoryInfo GetTemplateWorkspace(TemplateType templateType)
        {
            if (templateType == TemplateType.Project || templateType == TemplateType.Custom)
            {
                return ProjectTempWorkspace;
            }
            else if (templateType == TemplateType.Item)
            {
                return ItemTempWorkspace;
            }
            else
            {
                Debugger.Break();
                throw new Exception("");
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private static void ProcessZip(FileInfo fileInfo, DirectoryInfo commonLocation = null)
        {
            var templateTypeName = GetTemplateTypeFromPath(fileInfo);
            var templateType = (TemplateType)Enum.Parse(typeof(TemplateType), templateTypeName);
            var pathPrefix = GetPathPrefix(fileInfo);
            var outputDirectory = Path.Combine(Path.Combine(GetTemplateWorkspace(templateType).FullName, templateTypeName + "Templates"), pathPrefix);
            var templateFilePath = Decompress(fileInfo, outputDirectory);
            var key = @"\" + Path.Combine(pathPrefix, Path.GetFileNameWithoutExtension(fileInfo.Name));
            var template = ParseTemplate(templateFilePath, key, fileInfo, commonLocation);

            if (templateType == TemplateType.Project)
            {
                projectTemplates.Add(key, (VSProjectTemplate) template);
            }
            else if (templateType == TemplateType.Item)
            {
                itemTemplates.Add(key, (VSItemTemplate) template);
            }
            else if (templateType == TemplateType.ProjectGroup)
            {
                // TODO
            }
            else if (templateType == TemplateType.Custom)
            {
                // TODO
            }
            else
            {
                Debugger.Break();
            }
        }

        public static VSTemplate ParseTemplate(string templateFilePath, string key = "", FileInfo zippedFileInfo = null, DirectoryInfo commonLocation = null)
        {
            var document = XDocument.Load(templateFilePath);

            return ParseTemplate(document, key, zippedFileInfo, commonLocation);
        }

        public static VSTemplate ParseTemplate(StringBuilder contents, string key = "", FileInfo zippedFileInfo = null, DirectoryInfo commonLocation = null)
        {
            var document = XDocument.Parse(contents.ToString());

            return ParseTemplate(document, key, zippedFileInfo, commonLocation);
        }

        public static VSTemplate ParseTemplate(XDocument document, string key = "", FileInfo zippedFileInfo = null, DirectoryInfo commonLocation = null)
        {
            TemplateType templateType;
            var r = new XmlNamespaceManager(new NameTable());
            var namespaceURL = "http://schemas.microsoft.com/developer/vstemplate/2005";
            VSTemplate template = null;

            r.AddNamespace("p", namespaceURL);

            var typeName = document.XPathSelectElement("/p:VSTemplate", r).Attribute("Type").Value;
            var templateIDElement = document.XPathSelectElement("/p:VSTemplate/p:TemplateData/p:TemplateID", r);
            var requiredFrameworkVersionElement = document.XPathSelectElement("/p:VSTemplate/p:TemplateData/p:RequiredFrameworkVersion", r);
            var templateGroupIDElement = document.XPathSelectElement("/p:VSTemplate/p:TemplateData/p:TemplateGroupID", r);
            var sortOrder = "3000";
            var sortOrderElement = document.XPathSelectElement("/p:VSTemplate/p:TemplateData/p:SortOrder", r);
            var nameElement = document.XPathSelectElement("/p:VSTemplate/p:TemplateData/p:Name", r);
            var name = string.Empty;
            var icon = (Icon)null;
            var templateID = string.Empty;
            var templateGroupID = string.Empty;
            var requiredFrameworkVersion = string.Empty;

            if (templateIDElement != null)
            {
                templateID = templateIDElement.Value;
            }
            else
            {
                templateID = key;
            }

            if (templateGroupIDElement != null)
            {
                templateGroupID = templateGroupIDElement.Value;
            }

            if (sortOrderElement != null)
            {
                sortOrder = sortOrderElement.Value;
            }

            if (requiredFrameworkVersionElement != null)
            {
                requiredFrameworkVersion = requiredFrameworkVersionElement.Value;
            }

            if (nameElement != null)
            {
                if (string.IsNullOrWhiteSpace(nameElement.Value))
                {
                    var packageID = nameElement.Attribute("Package").Value;
                    var resourceID = nameElement.Attribute("ID").Value;
                    var package = VSConfigProvider.Packages.Values.SingleOrDefault(p => p.PackageGuid == new Guid(packageID));

                    if (package != null)
                    {
                        name = package.GetString(resourceID);
                    }
                }
                else
                {
                    name = nameElement.Value;
                }
            }

            var descriptionElement = document.XPathSelectElement("/p:VSTemplate/p:TemplateData/p:Description", r);
            var description = string.Empty;

            if (descriptionElement != null)
            {
                if (string.IsNullOrWhiteSpace(descriptionElement.Value))
                {
                    var packageID = descriptionElement.Attribute("Package").Value;
                    var resourceID = descriptionElement.Attribute("ID").Value;
                    var package = VSConfigProvider.Packages.Values.SingleOrDefault(p => p.PackageGuid == new Guid(packageID));

                    if (package != null)
                    {
                        description = package.GetString(resourceID);
                    }
                }
                else
                {
                    description = descriptionElement.Value;
                }
            }

            var iconElement = document.XPathSelectElement("/p:VSTemplate/p:TemplateData/p:Icon", r);

            if (iconElement != null)
            {
                if (string.IsNullOrWhiteSpace(iconElement.Value) && iconElement.HasAttributes)
                {
                    var packageID = iconElement.Attribute("Package").Value;
                    var resourceID = iconElement.Attribute("ID").Value;
                    var package = VSConfigProvider.Packages.Values.SingleOrDefault(p => p.PackageGuid == new Guid(packageID));

                    if (package != null)
                    {
                        icon = package.GetIcon(resourceID);
                    }
                }
                else
                {
                    var iconFile = document.XPathSelectElement("/p:VSTemplate/p:TemplateData/p:Icon", r).Value;
                }
            }

            var defaultNameElement = document.XPathSelectElement("/p:VSTemplate/p:TemplateData/p:DefaultName", r);
            var defaultName = string.Empty;

            if (defaultNameElement != null)
            {
                defaultName = defaultNameElement.Value;
            }

            var projectTypeName = document.XPathSelectElement("/p:VSTemplate/p:TemplateData/p:ProjectType", r).Value;
            var projectSubTypeElement = document.XPathSelectElement("/p:VSTemplate/p:TemplateData/p:ProjectSubType", r);
            var projectSubTypeName = string.Empty;

            if (projectSubTypeElement != null)
            {
                projectSubTypeName = projectSubTypeElement.Value;
            }

            if (string.IsNullOrEmpty(name))
            {
                name = defaultName;
            }

            templateType = (TemplateType)Enum.Parse(typeof(TemplateType), typeName);

            if (templateType == TemplateType.Project)
            {
                var vsProjectTemplate = new VSProjectTemplate
                {
                    TemplateID = templateID,
                    TemplateGroupID = templateGroupID,
                    Name = name,
                    TypeName = typeName,
                    Description = description,
                    RequiredFrameworkVersion = requiredFrameworkVersion,
                    DefaultName = defaultName,
                    Icon = icon,
                    SortOrder = int.Parse(sortOrder),
                    ProjectTypeName = projectTypeName,
                    ProjectSubTypeName = projectSubTypeName,
                    ZippedTemplate = zippedFileInfo,
                    TemplateDocument = document,
                    CommonLocation = commonLocation
                };

                var projects = document.XPathSelectElements("/p:VSTemplate/p:TemplateContent/p:Project", r);
                var _namespace = (XNamespace)namespaceURL;

                foreach (var project in projects)
                {
                    var templateProject = new VSTemplateProject
                    {
                        FileName = project.Attribute("File").Value,
                        ReplaceParameters = project.Attribute("ReplaceParameters") != null ? bool.Parse(project.Attribute("ReplaceParameters").Value) : false
                    };

                    vsProjectTemplate.Projects.Add(templateProject);

                    foreach (var projectItem in project.Descendants(_namespace + "ProjectItem"))
                    {
                        var folder = string.Empty;
                        var parent = projectItem.Parent;

                        while (parent != project)
                        {
                            folder = @"\" + parent.Attribute("Name").Value + folder;
                            parent = parent.Parent;
                        }

                        if (folder.StartsWith(@"\"))
                        {
                            folder = folder.Remove(0, 1);
                        }

                        templateProject.ProjectItems.Add(new VSTemplateProjectItem
                        {
                            FileName = projectItem.Value,
                            ReplaceParameters = projectItem.Attributes().Any(a => a.Name == "ReplaceParameters") ? bool.Parse(projectItem.Attribute("ReplaceParameters").Value) : false,
                            TargetFileName = projectItem.Attributes().Any(a => a.Name == "TargetFileName") ? projectItem.Attribute("TargetFileName").Value : null,
                            SubType = projectItem.Attributes().Any(a => a.Name == "SubType") ? projectItem.Attribute("SubType").Value : null,
                            Folder = folder
                        });
                    }
                }

                template = vsProjectTemplate;
            }
            else if (templateType == TemplateType.Item)
            {
                var vsItemTemplate = new VSItemTemplate
                {
                    TemplateID = templateID,
                    TemplateGroupID = templateGroupID,
                    Name = name,
                    TypeName = typeName,
                    Description = description,
                    RequiredFrameworkVersion = requiredFrameworkVersion,
                    DefaultName = defaultName,
                    Icon = icon,
                    SortOrder = int.Parse(sortOrder),
                    ProjectTypeName = projectTypeName,
                    ProjectSubTypeName = projectSubTypeName,
                    ZippedTemplate = zippedFileInfo,
                    TemplateDocument = document,
                    CommonLocation = commonLocation
                };

                var references = document.XPathSelectElements("/p:VSTemplate/p:TemplateContent/p:References/p:Reference", r);
                var projectItems = document.XPathSelectElements("/p:VSTemplate/p:TemplateContent/p:ProjectItem", r);
                var _namespace = (XNamespace)namespaceURL;

                foreach (var reference in references)
                {
                    vsItemTemplate.References.Add(new VSTemplateReference(reference.XPathSelectElement("p:Assembly", r).Value));
                }

                foreach (var projectItem in projectItems)
                {
                    vsItemTemplate.ProjectItems.Add(new VSTemplateProjectItem
                    {
                        FileName = projectItem.Value,
                        ReplaceParameters = projectItem.Attributes().Any(a => a.Name == "ReplaceParameters") ? bool.Parse(projectItem.Attribute("ReplaceParameters").Value) : false,
                        TargetFileName = projectItem.Attributes().Any(a => a.Name == "TargetFileName") ? projectItem.Attribute("TargetFileName").Value : null,
                        SubType = projectItem.Attributes().Any(a => a.Name == "SubType") ? projectItem.Attribute("SubType").Value : null
                    });
                }

                template = vsItemTemplate;
            }
            else if (templateType == TemplateType.ProjectGroup)
            {
                // TODO
            }
            else
            {
                Debugger.Break();
            }

            return template;
        }
    }
}
