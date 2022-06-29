using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;

namespace PackageCacheStatus
{
    public class FolderViewService : BaseThreadedService
    {
        private ListView listViewFolders;
        private int updateCount;
        private string packageCachePath;
        private FileSystemWatcher fileSystemWatcher;
        private bool update;
        private List<FileSystemInfo> backingList;
        private Image folderImage;
        private DirectoryInfo cacheDirectory;
        private IManagedLockObject lockObject;
        private bool suppressUpdate;

        public FolderViewService(ListView listViewFolders, string packageCachePath, int updateCount = 100) : base(ThreadPriority.Normal, 100, 20000, 20000)
        {
            cacheDirectory = new DirectoryInfo(packageCachePath);

            lockObject = LockManager.CreateObject();

            backingList = cacheDirectory.GetFileSystemInfos().OrderBy(i => i is FileInfo ? 1 : 0).ThenBy(i => i.Name).ToList();

            this.listViewFolders = listViewFolders;
            this.updateCount = updateCount;
            this.packageCachePath = packageCachePath;

            fileSystemWatcher = new FileSystemWatcher(packageCachePath);
            fileSystemWatcher.EnableRaisingEvents = true;

            fileSystemWatcher.Created += FileSystemWatcher_Created;
            fileSystemWatcher.Changed += FileSystemWatcher_Changed;
            fileSystemWatcher.Deleted += FileSystemWatcher_Deleted;

            this.listViewFolders.VirtualListSize = backingList.Count;
            this.listViewFolders.VirtualMode = true;
            this.listViewFolders.Scrollable = true;

            listViewFolders.RetrieveVirtualItem += ListViewFolders_RetrieveVirtualItem;
        }

        private void ListViewFolders_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            if (backingList.Count >= e.ItemIndex)
            {
                var fileSystemInfo = backingList.ElementAt(e.ItemIndex);

                e.Item = listViewFolders.Get(fileSystemInfo, () =>
                {
                    if (fileSystemInfo is FileInfo)
                    {
                        return ((FileInfo)fileSystemInfo).GetSmallIcon<Bitmap>();
                    }
                    else
                    {
                        if (folderImage == null)
                        {
                            folderImage = typeof(frmPackageCacheStatus).ReadResource<Bitmap>(@"Images\Folder.png");
                        }

                        return folderImage;
                    }
                });
            }
        }

        private void FileSystemWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            using (lockObject.Lock())
            {
                update = true;
            }
        }

        private void FileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            using (lockObject.Lock())
            {
                update = true;
            }
        }

        private void FileSystemWatcher_Created(object sender, FileSystemEventArgs e)
        {
            using (lockObject.Lock())
            {
                update = true;
            }
        }

        public void Update()
        {
            using (lockObject.Lock())
            {
                update = true;
            }
        }

        public IDisposable SuppressUpdate()
        {
            IDisposable disposable;

            using (lockObject.Lock())
            {
                suppressUpdate = true;
                fileSystemWatcher.EnableRaisingEvents = false;
            }

            disposable = this.CreateDisposable(() =>
            {
                using (lockObject.Lock())
                {
                    suppressUpdate = false;
                    fileSystemWatcher.EnableRaisingEvents = true;
                }

            });

            return disposable;
        }

        public override void DoWork(bool stopping)
        {
            if (!suppressUpdate)
            {
                var directory = new DirectoryInfo(this.packageCachePath);
                var fileSystemInfos = cacheDirectory.GetFileSystemInfos().OrderBy(i => i is FileInfo ? 1 : 0).ThenBy(i => i.Name).ToList();
                var update = false;

                using (lockObject.Lock())
                {
                    update = this.update;
                }

                if (update)
                {
                    foreach (var fileSystemInfo in fileSystemInfos)
                    {
                        if (!backingList.Any(i => i.FullName == fileSystemInfo.FullName))
                        {
                            UpdateList(fileSystemInfos);
                            break;
                        }
                    }

                    foreach (var fileSystemInfo in backingList)
                    {
                        if (!fileSystemInfos.Any(i => i.FullName == fileSystemInfo.FullName))
                        {
                            UpdateList(fileSystemInfos);
                            break;
                        }
                    }

                    update = false;
                }
            }
        }

        private void UpdateList(List<FileSystemInfo> fileSystemInfos)
        {
            listViewFolders.Invoke(() =>
            {
                listViewFolders.BeginUpdate();

                backingList = fileSystemInfos;
                listViewFolders.VirtualListSize = backingList.Count;

                listViewFolders.EndUpdate();
            });
        }
    }
}
