using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;

namespace PackageCacheStatus
{
    public class FolderItemCompare : IComparer
    {
        private ColumnHeader columnHeaderName;
        private ColumnHeader columnHeaderType;

        public FolderItemCompare(ColumnHeader columnHeaderName, ColumnHeader columnHeaderType)
        {
            this.columnHeaderName = columnHeaderName;
            this.columnHeaderType = columnHeaderType;
        }

        public int Compare(object x, object y)
        {
            try
            {
                var itemX = (ListViewItem)x;
                var itemY = (ListViewItem)y;
                var nameX = itemX.GetSubItem(columnHeaderName);
                var typeX = itemX.GetSubItem(columnHeaderType);
                var nameY = itemY.GetSubItem(columnHeaderName);
                var typeY = itemY.GetSubItem(columnHeaderType);

                if (typeX != typeY)
                {
                    if (typeX == "Folder")
                    {
                        return 1;
                    }
                    else if (typeY == "Folder")
                    {
                        return -1;
                    }
                }

                return nameX.CompareTo(nameY);
            }
            catch
            {
                return 0;
            }
        }
    }

    public static class ListExtensions
    {
        private static ImageList imageList;
        private static Dictionary<string, string> fileTypes;
        private static Dictionary<string, string> shellItemNames;
        private static ColumnHeader columnHeaderName;
        private static ColumnHeader columnHeaderDateModified;
        private static ColumnHeader columnHeaderType;

        static ListExtensions()
        {
            var type = typeof(frmPackageCacheStatus);

            imageList = new ImageList();
            fileTypes = new Dictionary<string, string>();
            shellItemNames = new Dictionary<string, string>();
        }

        public static void AssignToImageList(this ListView listView, ColumnHeader columnHeaderName, ColumnHeader columnHeaderDateModified, ColumnHeader columnHeaderType)
        {
            var sorter = new ListViewColumnSorter();

            ListExtensions.columnHeaderName = columnHeaderName;
            ListExtensions.columnHeaderDateModified = columnHeaderDateModified;
            ListExtensions.columnHeaderType = columnHeaderType;

            listView.SmallImageList = imageList;
            listView.LargeImageList = imageList;
            listView.StateImageList = imageList;
            listView.Sorting = SortOrder.Ascending;

            sorter.SortModifier = ListViewColumnSorter.SortModifiers.SortCustom;
            sorter.CustomCompare = new FolderItemCompare(columnHeaderName, columnHeaderType);
            sorter.ColumnToSort = columnHeaderType.DisplayIndex;
            sorter.OrderOfSort = SortOrder.Ascending;

            listView.AddColumnSortingBehavior(sorter);
        }

        public static void OpenDefault(this ListViewItem item)
        {
            var file = (FileSystemInfo)item.Tag;
            var ext = Path.GetExtension(file.FullName);

            if (shellItemNames.ContainsKey(ext))
            {
                var exe = shellItemNames[ext];
                var process = new Process();

                process.StartInfo = new ProcessStartInfo
                {
                    FileName = exe,
                    Arguments = file.FullName
                };

                process.Start();
            }
        }

        public static ListViewItem Get(this ListView listView, FileSystemInfo fileSystemInfo, Func<Image> getIconImage)
        {
            var type = typeof(frmPackageCacheStatus);
            var fileType = "Folder";
            string imageName;
            Image image = null;
            ListViewItem item;
            int index;
            var sorter = new ListViewColumnSorter();

            if (fileSystemInfo is FileInfo)
            {
                imageName = Path.GetExtension(fileSystemInfo.FullName);

                if (!fileTypes.ContainsKey(imageName))
                {
                    var regKey = Registry.ClassesRoot.OpenSubKey(imageName).ToIndexable();
                    string shellItemName;

                    fileType = (string)regKey["PerceivedType"];
                    shellItemName = ((string)regKey[@"ShellNew\@ItemName"]).RegexGet("^@(?<fileName>[^,]*?),", "fileName");

                    fileTypes.Add(imageName, fileType);
                    shellItemNames.Add(imageName, shellItemName);
                }
            }
            else
            {
                imageName = "Folder";
            }

            if (imageList.Images.ContainsKey(imageName))
            {
                index = imageList.Images.IndexOfKey(imageName);
            }
            else
            {
                if (image == null)
                {
                    Bitmap bitmap;

                    image = getIconImage();
                    bitmap = new Bitmap(image);
                    bitmap.MakeTransparent();

                    image = bitmap;
                }

                imageList.Images.Add(imageName, image);
                index = imageList.Images.IndexOfKey(imageName);
            }

            item = new ListViewItem(fileSystemInfo.Name, index);
            item.Tag = fileSystemInfo;

            item.AddSubItem(columnHeaderName, fileSystemInfo.Name);
            item.AddSubItem(columnHeaderDateModified, fileSystemInfo.LastWriteTime.ToShortDateString() + " " + fileSystemInfo.LastWriteTime.ToShortTimeString());
            item.AddSubItem(columnHeaderType, fileType);

            return item;
        }

        public static void Add(this ListView listView, FileSystemInfo fileSystemInfo, Func<Image> getIconImage)
        {
            var type = typeof(frmPackageCacheStatus);
            var fileType = "Folder";
            string imageName;
            Image image = null;
            ListViewItem item;
            int index;
            var sorter = new ListViewColumnSorter();

            if (fileSystemInfo is FileInfo)
            {
                imageName = Path.GetExtension(fileSystemInfo.FullName);

                if (!fileTypes.ContainsKey(imageName))
                {
                    var regKey = Registry.ClassesRoot.OpenSubKey(imageName).ToIndexable();
                    string shellItemName;

                    fileType = (string) regKey["PerceivedType"];
                    shellItemName = ((string) regKey[@"ShellNew\@ItemName"]).RegexGet("^@(?<fileName>[^,]*?),", "fileName");

                    fileTypes.Add(imageName, fileType);
                    shellItemNames.Add(imageName, shellItemName);
                }
            }
            else
            {
                imageName = "Folder";
            }

            if (imageList.Images.ContainsKey(imageName))
            {
                index = imageList.Images.IndexOfKey(imageName);
            }
            else
            {
                if (image == null)
                {
                    Bitmap bitmap;

                    image = getIconImage();
                    bitmap = new Bitmap(image);
                    bitmap.MakeTransparent();

                    image = bitmap;
                }

                imageList.Images.Add(imageName, image);
                index = imageList.Images.IndexOfKey(imageName);
            }

            item = listView.Items.Add(fileSystemInfo.Name, index);
            item.Tag = fileSystemInfo;

            item.AddSubItem(columnHeaderName, fileSystemInfo.Name);
            item.AddSubItem(columnHeaderDateModified, fileSystemInfo.LastWriteTime.ToShortDateString() + " " + fileSystemInfo.LastWriteTime.ToShortTimeString());
            item.AddSubItem(columnHeaderType, fileType);

            sorter.SortModifier = ListViewColumnSorter.SortModifiers.SortCustom;
            sorter.CustomCompare = new FolderItemCompare(columnHeaderName, columnHeaderType);
            sorter.ColumnToSort = columnHeaderType.DisplayIndex;
            sorter.OrderOfSort = SortOrder.Ascending;

            listView.AddColumnSortingBehavior(sorter);

            listView.Sort();
        }
    }
}
