// file:	ResourceManager.cs
//
// summary:	Implements the resource manager class

using AbstraX.Resources;
using HydraResourceTracer;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;
using Utils.Hierarchies;

namespace AbstraX
{
    /// <summary>   Manager for resources. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 11/24/2020. </remarks>

    public class ResourceManager : IResourceManager
    {
        /// <summary>   Gets the full pathname of the root file. </summary>
        ///
        /// <value> The full pathname of the root file. </value>

        public string RootPath { get; }

        /// <summary>   Gets the sass content. </summary>
        ///
        /// <value> The sass content. </value>

        public string SassContent { get; }

        /// <summary>   Gets the document comparison resolver. </summary>
        ///
        /// <value> The document comparison resolver. </value>

        public IDocumentComparisonResolver DocumentComparisonResolver { get; }

        private ITraceResourcePersist traceResource;

        /// <summary>   Information describing the resource. </summary>
        /// 
        private ResourceData resourceData;
        private ResourceTracer resourceTracer;

        /// <summary>   Event queue for all listeners interested in onSaveBackup events. </summary>
        public event SaveBackupEventHandler OnInitiateSaveBackupEvent;
        /// <summary>   Event queue for all listeners interested in onRestoreBackup events. </summary>
        public event RestoreBackupEventHandler OnInitiateRestoreBackupEvent;
        /// <summary>   Event queue for all listeners interested in onInitiateNew events. </summary>
        public event NewEventHandler OnInitiateNewEvent;

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/24/2020. </remarks>
        ///
        /// <param name="rootPath">         Full pathname of the root file. </param>
        /// <param name="sassContent">      (Optional) The sass content. </param>
        /// <param name="traceResource">    (Optional) The trace resource. </param>
        /// <param name="resolver">         (Optional) The resolver. </param>

        public ResourceManager(string rootPath, string sassContent = null, ITraceResourcePersist traceResource = null, IDocumentComparisonResolver resolver = null)
        {
            this.RootPath = rootPath;
            this.SassContent = sassContent;
            this.DocumentComparisonResolver = resolver;
            this.traceResource = traceResource;

            if (!Directory.Exists(rootPath))
            {
                Directory.CreateDirectory(rootPath);
            }

            if (traceResource != null && !traceResource.TraceResourceDocument.IsNullOrEmpty())
            {
                resourceTracer = new ResourceTracer(rootPath, traceResource);
            }
        }

        /// <summary>   Gets information describing the resource. </summary>
        ///
        /// <value> Information describing the resource. </value>

        public IResourceData ResourceData
        {
            get
            {
                if (resourceData == null)
                {
                    var manifestFilePath = Path.Combine(this.RootPath, "resourceManifest.json");
                    var zipPath = Path.Combine(this.RootPath, "resources.zip");
                    Dictionary<string, object> keyValues = null;

                    if (File.Exists(zipPath))
                    {
                        Unzip();

                        using (var stream = File.Open(manifestFilePath, FileMode.OpenOrCreate))
                        {
                            var reader = new StreamReader(stream);
                            var contents = reader.ReadToEnd();
                            var reset = new StreamReset(stream);

                            if (contents.Length > 0)
                            {
                                using (reset)
                                {
                                    keyValues = JsonExtensions.ReadJson<KeyValuePair<string, object>[]>(contents).ToDictionary();
                                }
                            }
                            else
                            {
                                keyValues = new Dictionary<string, object>();
                            }
                        }
                    }
                    else if (File.Exists(manifestFilePath))
                    {
                        using (var stream = File.Open(manifestFilePath, FileMode.OpenOrCreate))
                        {
                            var reader = new StreamReader(stream);
                            var contents = reader.ReadToEnd();
                            var reset = new StreamReset(stream);

                            if (contents.Length > 0)
                            {
                                using (reset)
                                {
                                    keyValues = JsonExtensions.ReadJson<KeyValuePair<string, object>[]>(contents).ToDictionary();
                                }
                            }
                            else
                            {
                                keyValues = new Dictionary<string, object>();
                            }
                        }

                        Zip(new DirectoryInfo(this.RootPath));
                    }
                    else
                    {
                        keyValues = new Dictionary<string, object>();
                    }

                    resourceData = new ResourceData(this.SassContent, this, keyValues);
                }

                return resourceData;
            }
        }

        /// <summary>   Reports resource change. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/13/2021. </remarks>
        ///
        /// <param name="resourcePath"> Full pathname of the resource file. </param>
        /// <param name="change">       The change. </param>

        public void ReportResourceChange(string resourcePath, string change)
        {
            if (resourceTracer != null && Path.GetFileName(resourcePath).AsCaseless() == traceResource.TraceResourceDocument)
            {
                resourceTracer.ReportResourceChange(resourcePath, change);
            }
        }

        /// <summary>   Reports resource change. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/13/2021. </remarks>
        ///
        /// <param name="resourcePath"> Full pathname of the resource file. </param>
        /// <param name="changeFormat"> The change format. </param>
        /// <param name="args">         A variable-length parameters list containing arguments. </param>

        public void ReportResourceChange(string resourcePath, string changeFormat, params object[] args)
        {
            if (resourceTracer != null && Path.GetFileName(resourcePath).AsCaseless() == traceResource.TraceResourceDocument)
            {
                resourceTracer.ReportResourceChange(resourcePath, string.Format(changeFormat, args), (hash, size) =>
                {
                    if (changeFormat == "Setting file path to {0}")
                    {
                    }
                });
            }
        }

        /// <summary>   Adds a file resource to 'path'. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/24/2020. </remarks>
        ///
        /// <param name="name">     The name. </param>
        /// <param name="value">    Filename of the file. </param>
        ///

        public void AddResource(string name, object value)
        {
            var directory = new DirectoryInfo(this.RootPath);
            var manifestFilePath = Path.Combine(this.RootPath, "resourceManifest.json");

            Unzip();

            UpdateManifest(manifestFilePath, name, value);

            Zip(directory);
        }

        /// <summary>   Updates this.  </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/5/2020. </remarks>
        ///
        /// <param name="name">     The name. </param>
        /// <param name="value">    Filename of the file. </param>

        public void Update(string name, object value)
        {
            var directory = new DirectoryInfo(this.RootPath);
            var manifestFilePath = Path.Combine(this.RootPath, "resourceManifest.json");

            UpdateManifest(manifestFilePath, name, value);
        }

        /// <summary>   Clears the resource data. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/5/2020. </remarks>

        public void FreeResourceData()
        {
            var directory = new DirectoryInfo(this.RootPath);

            if (resourceData != null)
            {
                if (resourceData.Dirty)
                {
                    Zip(directory);
                }

                resourceData = null;
            }
        }

        private void Zip(DirectoryInfo directory)
        {
            using (var package = ZipPackage.Open(Path.Combine(this.RootPath, "resources.zip"), FileMode.Create))
            {
                directory.GetDescendantsAndSelf(d => d.GetDirectories(), d =>
                {
                    foreach (var file in d.GetFiles().Where(f => f.Name != "resources.zip"))
                    {
                        var path = d.FullName.RemoveStartIfMatches(this.RootPath).ForwardSlashes();
                        Uri uri;
                        PackagePart part;

                        if (path.IsNullOrEmpty())
                        {
                            uri = new Uri("/" + file.Name.UriEncode(), UriKind.Relative);
                        }
                        else
                        {
                            uri = new Uri(path + "/" + file.Name.UriEncode(), UriKind.Relative);
                        }

                        if (package.PartExists(uri))
                        {
                            part = package.GetPart(uri);
                        }
                        else
                        {
                            part = package.CreatePart(uri, "");
                        }

                        using (var stream = new MemoryStream(File.ReadAllBytes(file.FullName)))
                        {
                            part.GetStream().WriteAll(stream);
                        }
                    }
                });
            }
        }

        private void Unzip()
        {
            var path = Path.Combine(this.RootPath, "resources.zip");
            var zipFile = new FileInfo(path);

            if (!zipFile.Exists)
            {
                return;
            }

            if (zipFile.Length == 0)
            {
                if (this.DocumentComparisonResolver != null)
                {
                    var directory = new DirectoryInfo(this.RootPath);

                    foreach (var file in directory.GetFiles().Where(f => f.Name != "resources.zip"))
                    {
                        var existingFileContent = File.ReadAllBytes(file.FullName);

                        this.DocumentComparisonResolver.ReportUnsaved(file, zipFile, new byte[0], 0, existingFileContent.Length, () =>
                        {
                            file.Delete();
                        });
                    }
                }

                zipFile.MakeWritable();
                zipFile.Delete();
            }

            using (var package = ZipPackage.Open(zipFile.FullName, FileMode.OpenOrCreate))
            {
                foreach (var part in package.GetParts())
                {
                    using (var stream = part.GetStream())
                    {
                        var reader = new BinaryReader(stream);
                        var content = reader.ReadBytes((int)stream.Length);
                        var outputFile = new FileInfo(Path.Combine(this.RootPath, part.Uri.OriginalString.UriDecode().ReverseSlashes().RemoveStartIfMatches("\\")));

                        if (outputFile.Exists)
                        {
                            var existingFileContent = File.ReadAllBytes(outputFile.FullName);

                            if (!existingFileContent.Compare(content))
                            {
                                if (this.DocumentComparisonResolver != null)
                                {
                                    this.DocumentComparisonResolver.ReportUnsaved(outputFile, zipFile, content, content.Length, existingFileContent.Length, () =>
                                    {
                                        outputFile.MakeWritable();
                                        File.WriteAllBytes(outputFile.FullName, content);

                                    });

                                    continue;
                                }
                            }

                            outputFile.MakeWritable();
                        }

                        this.ReportResourceChange(outputFile.FullName, "Unzipping to {0}", outputFile.FullName);

                        File.WriteAllBytes(outputFile.FullName, content);
                    }
                }
            }
        }


        /// <summary>   Gets resource value. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/26/2020. </remarks>
        ///
        /// <param name="name"> The name. </param>
        ///
        /// <returns>   The resource value. </returns>

        public string GetResourceValue(string name)
        {
            var directory = new DirectoryInfo(this.RootPath);
            var manifestFilePath = Path.Combine(this.RootPath, "resourceManifest.json");

            Unzip();

            using (var stream = File.Open(manifestFilePath, FileMode.OpenOrCreate))
            {
                var reader = new StreamReader(stream);
                var contents = reader.ReadToEnd();
                var reset = new StreamReset(stream);
                Dictionary<string, object> keyValues;

                if (contents.Length > 0)
                {
                    using (reset)
                    {
                        keyValues = JsonExtensions.ReadJson<KeyValuePair<string, object>[]>(contents).ToDictionary();

                        if (keyValues.ContainsKey(name))
                        {
                            return (string)keyValues[name];
                        }
                    }
                }

                return null;
            }
        }

        /// <summary>   Adds a file resource to 'path'. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/24/2020. </remarks>
        ///
        /// <param name="name">     The name. </param>
        /// <param name="filePath"> Full pathname of the file. </param>
        /// <param name="noCopy">   (Optional) True to no copy. </param>

        public void AddFileResource(string name, string filePath, bool noCopy = false)
        {
            var directory = new DirectoryInfo(this.RootPath);
            var manifestFilePath = Path.Combine(this.RootPath, "resourceManifest.json");
            var fileName = Path.GetFileName(filePath);

            Unzip();

            if (!noCopy)
            {
                var copyToPath = Path.Combine(directory.FullName, fileName);

                ReportResourceChange(filePath, "Copying from {0} to {1}", filePath, copyToPath);

                File.Copy(filePath, copyToPath, true);
            }

            UpdateManifest(manifestFilePath, name, fileName);

            Zip(directory);
        }

        /// <summary>   Updates the manifest. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/26/2020. </remarks>
        ///
        /// <param name="name">             The name. </param>
        /// <param name="manifestFilePath"> Full pathname of the manifest file. </param>
        /// <param name="value">         Filename of the file. </param>

        private void UpdateManifest(string manifestFilePath, string name, object value)
        {
            using (var stream = File.Open(manifestFilePath, FileMode.OpenOrCreate))
            {
                var reader = new StreamReader(stream);
                var writer = new StreamWriter(stream);
                var contents = reader.ReadToEnd();
                var reset = new StreamReset(stream);
                Dictionary<string, object> keyValues;

                if (resourceData != null)
                {
                    keyValues = resourceData.KeyValues;
                }
                else
                {
                    if (contents.Length > 0)
                    {
                        using (reset)
                        {
                            keyValues = JsonExtensions.ReadJson<KeyValuePair<string, object>[]>(contents).ToDictionary();
                        }
                    }
                    else
                    {
                        keyValues = new Dictionary<string, object>();
                    }
                }

                if (keyValues.ContainsKey(name))
                {
                    keyValues[name] = value;
                }
                else
                {
                    keyValues.Add(name, value);
                }

                contents = JsonExtensions.ToJsonText(keyValues.Select(k => k).ToArray(), Newtonsoft.Json.Formatting.Indented);
                stream.SetLength(0);

                writer.Write(contents);
                writer.Flush();
            }
        }

        /// <summary>   Determines if we can ask close. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/13/2021. </remarks>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        public bool AskClose()
        {
            if (resourceTracer != null)
            {
                return resourceTracer.AskClose();
            }

            return true;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/24/2020. </remarks>

        public void Dispose()
        {
            var directory = new DirectoryInfo(this.RootPath);

            if (resourceData == null)
            {
                if (resourceTracer != null)
                {
                    var file = directory.GetFiles(traceResource.TraceResourceDocument, SearchOption.AllDirectories).SingleOrDefault();

                    if (file != null)
                    {
                        this.ReportResourceChange(file.FullName, "Deleting {0}", file.FullName);
                    }
                }

                GC.Collect();

                Task.Run(() =>
                {
                    directory.ForceDeleteAllFilesAndSubFolders(false, (s) => !(s.FileSystemInfo is FileInfo file && file.Name == "resources.zip"));
                });
            }
        }

        /// <summary>   Saves this.  </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/11/2021. </remarks>

        public void Save()
        {
            if (resourceData != null)
            {
                var directory = new DirectoryInfo(this.RootPath);

                this.Zip(directory);

                resourceData.Dirty = false;
            }
        }

        /// <summary>   Saves the backup. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/23/2021. </remarks>

        public void InitiateSaveBackup(Action<string, IResourceData> beforeExecuteCallback)
        {
            OnInitiateSaveBackupEvent(this, new BackupEventArgs(resourceData, beforeExecuteCallback));
        }

        /// <summary>   Restore backup. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/23/2021. </remarks>

        public void InitiateRestoreBackup(Action<string, IResourceData> beforeExecuteCallback)
        {
            OnInitiateRestoreBackupEvent(this, new BackupEventArgs(resourceData, beforeExecuteCallback));
        }

        /// <summary>   Initiate new. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/23/2021. </remarks>

        public void InitiateNew(Action<string, IResourceData> beforeExecuteCallback)
        {
            OnInitiateNewEvent(this, new BackupEventArgs(resourceData, beforeExecuteCallback));
        }
    }
}
