using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using Utils;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using Timer = System.Threading.Timer;
using System.Diagnostics;
using AbstraX;

namespace HydraResourceTracer
{
    public class ResourceTracer
    {
        private Dictionary<string, DocumentInfo> documentInfoLookup = new Dictionary<string, DocumentInfo>();
        private Queue<DocumentInfo> documentReviewQueue = new Queue<DocumentInfo>();
        private IManagedLockObject lockObject = LockManager.CreateObject();
        private ITraceResourcePersist traceResourcePersist;
        private Utils.frmConsole frmConsole;

        public ResourceTracer(string rootPath, ITraceResourcePersist traceResourcePersist)
        {
            var fileWatcher = new FileSystemWatcher(rootPath);
            var timer = new Timer(ReviewDocuments, null, 1000, 1000);
            var form = Application.OpenForms.Cast<Form>().FirstOrDefault();
            var traceResourceDocument = traceResourcePersist.TraceResourceDocument;
            var document = new FileInfo(traceResourceDocument);
            string hash = "N/A";

            if (document.Exists)
            {
                try
                {
                    hash = document.GetHash();
                }
                catch
                {
                }
            }

            this.traceResourcePersist = traceResourcePersist;

            frmConsole = form.CreateConsoleForm("Resource Tracer Utility");

            ReadZip(rootPath);
            LookForExistingFiles(rootPath);

            fileWatcher.EnableRaisingEvents = true;
            fileWatcher.IncludeSubdirectories = true;

            fileWatcher.Changed += FileWatcher_Changed;
            fileWatcher.Created += FileWatcher_Created;
            fileWatcher.Deleted += FileWatcher_Deleted;
            fileWatcher.Filter = traceResourceDocument;

            fileWatcher.EnableRaisingEvents = true;

            frmConsole.WindowState = FormWindowState.Minimized;
            frmConsole.Show();

            frmConsole.DelayInvoke(100, () =>
            {
                frmConsole.ShowInSecondaryMonitor(FormWindowState.Maximized);
            });

            frmConsole.WriteLine("Watching for changes. Starting file hash: {0}, last hash: {1}", hash, traceResourcePersist.TraceResourceLastHash);
        }

        public bool AskClose()
        {
            if (MessageBox.Show("Close Resource Tracer?", "Resource Tracer", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                return true;
            }

            return false;
        }

        private void ReviewDocuments(object state)
        {
            DocumentInfo documentInfo;
            DocumentInfo documentInfoClone;
            int count;

            using (lockObject.Lock())
            {
                count = documentReviewQueue.Count;
            }

            if (count > 0)
            {
                using (lockObject.Lock())
                {
                    documentInfo = documentReviewQueue.Dequeue();

                    documentInfoClone = new DocumentInfo
                    {
                        Path = documentInfo.Path,
                        CurrentState = documentInfo.CurrentState,
                        InZip = documentInfo.InZip,
                        LastReview = documentInfo.LastReview,
                        Size = documentInfo.Size,
                        Hash = documentInfo.Hash
                    };
                }

                if (documentInfoClone.CurrentState.IsOneOf("Deleted", "Reduced in size"))
                {
                    if (documentInfoClone.FirstReview - DateTime.Now > TimeSpan.FromSeconds(5))
                    {
                        var document = new FileInfo(documentInfoClone.Path);
                        string size = "N/A";
                        string hash = "N/A";

                        if (document.Exists)
                        {
                            size = string.Format("{0}b", document.Length.ToKb());

                            try
                            {
                                hash = document.GetHash();

                                if (hash != traceResourcePersist.TraceResourceLastHash)
                                {
                                    traceResourcePersist.TraceResourceLastHash = hash;
                                    traceResourcePersist.Save();
                                }
                            }
                            catch
                            {
                            }
                        }

                        frmConsole.WriteLine("{0} Alert, file: {1}, size {2}, hash {3}", documentInfoClone.CurrentState, documentInfoClone.Path, size, hash);
                    }
                }
            }
        }

        private void LookForExistingFiles(string rootPath)
        {
            var directory = new DirectoryInfo(rootPath);

            foreach (var file in directory.GetFiles("*.*", SearchOption.AllDirectories))
            {
                if (documentInfoLookup.ContainsKey(file.FullName))
                {
                    var documentInfo = documentInfoLookup[file.FullName];

                    documentInfo.InZip = true;
                    documentInfo.Size = file.Length;
                }
                else if (!file.FullName.EndsWith("~"))
                {
                    var documentInfo = new DocumentInfo { Path = file.FullName, InZip = false, Size = file.Length, Hash = file.GetHash() };

                    documentInfoLookup.Add(file.FullName, documentInfo);
                }
            }
        }

        public void ReportResourceChange(string resourcePath, string change, Action<string, string> reportHashDifference = null)
        {
            var document = new FileInfo(resourcePath);
            string size = "N/A";
            string hash = "N/A";

            if (document.Exists)
            {
                size = string.Format("{0}k", document.Length.ToKb());

                try
                {
                    hash = document.GetHash();

                    if (hash != traceResourcePersist.TraceResourceLastHash)
                    {
                        if (reportHashDifference != null)
                        {
                            reportHashDifference(hash, size);
                        }

                        traceResourcePersist.TraceResourceLastHash = hash;
                        traceResourcePersist.Save();
                    }
                }
                catch
                {
                }
            }

            frmConsole.WriteLine(change + ", size: {0}, hash: {1}", size, hash);
        }

        private void ReadZip(string rootPath)
        {
            var path = Path.Combine(rootPath, "resources.zip");
            var zipFile = new FileInfo(path);

            if (zipFile.Length == 0)
            {
                frmConsole.WriteLine("Zip file empty Alert, file: {0}", zipFile.FullName);
                return;
            }

            using (var package = ZipPackage.Open(zipFile.FullName, FileMode.Open))
            {
                foreach (var part in package.GetParts())
                {
                    var zipPartFile = new FileInfo(Path.Combine(rootPath, part.Uri.OriginalString.UriDecode().ReverseSlashes().RemoveStartIfMatches("\\")));
                    var zipPartFileName = zipPartFile.FullName;

                    documentInfoLookup.Add(zipPartFileName, new DocumentInfo { Path = zipPartFileName });
                }
            }
        }

        private void FileWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            FileModified(e, "Deleted");
        }

        private void FileWatcher_Created(object sender, FileSystemEventArgs e)
        {
            FileModified(e, "Created");
        }

        private void FileWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            FileModified(e, "Changed");
        }

        private void FileModified(FileSystemEventArgs e, string action)
        {
            var file = new FileInfo(e.FullPath);

            using (lockObject.Lock())
            {
                if (documentInfoLookup.ContainsKey(file.FullName))
                {
                    var documentInfo = documentInfoLookup[file.FullName];
                    var document = new FileInfo(documentInfo.Path);
                    string size = "N/A";
                    string hash = "N/A";

                    if (document.Exists)
                    {
                        size = string.Format("{0}k", document.Length.ToKb());

                        try
                        {
                            hash = document.GetHash();
                            
                            if (hash != traceResourcePersist.TraceResourceLastHash)
                            {
                                traceResourcePersist.TraceResourceLastHash = hash;
                                traceResourcePersist.Save();
                            }
                        }
                        catch
                        {
                        }
                    }

                    if (action == "Deleted")
                    {
                        documentInfo.CurrentState = "Deleted";
                        documentInfo.FirstReview = DateTime.Now;

                        documentReviewQueue.Enqueue(documentInfo);
                    }
                    else
                    {
                        if (file.Length < documentInfo.Size)
                        {
                            documentInfo.CurrentState = "Reduced in size";
                            documentInfo.FirstReview = DateTime.Now;

                            documentReviewQueue.Enqueue(documentInfo);
                        }
                        else if (file.Length > documentInfo.Size)
                        {
                            documentInfo.Size = file.Length;
                            documentInfo.CurrentState = string.Empty;

                            frmConsole.WriteLine("Increase in size Info, file: {0}, size: {1}, hash: {2}", documentInfo.Path, size, hash);
                        }
                        else
                        {
                            documentInfo.Size = file.Length;
                            documentInfo.CurrentState = string.Empty;

                            frmConsole.WriteLine("Modification Info, file: {0}, size: {1}, hash: {2}", documentInfo.Path, size, hash);
                        }
                    }
                }
                else if (!file.FullName.EndsWith("~"))
                {
                    var documentInfo = new DocumentInfo { Path = file.FullName, InZip = false, Size = file.Length, Hash = file.GetHash() };

                    documentInfoLookup.Add(file.FullName, documentInfo);
                }
            }
        }
    }
}
