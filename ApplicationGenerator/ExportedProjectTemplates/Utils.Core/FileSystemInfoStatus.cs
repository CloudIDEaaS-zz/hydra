using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public class FileSystemInfoStatus
    {
        public FileSystemInfo FileSystemInfo { get; private set; }
        public int OriginalCount { get; set; }
        public int Count { get; set; }

        public FileSystemInfoStatus(FileSystemInfo fileSystemInfo)
        {
            this.FileSystemInfo = fileSystemInfo;
            this.Count = -1;
            this.OriginalCount = -1;
        }
    }
}
