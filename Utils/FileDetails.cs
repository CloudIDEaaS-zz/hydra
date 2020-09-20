using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Shell32;
using System.Diagnostics;

namespace Utils
{
    public class FileDetails : BaseDictionary<string, string>
    {
        private FileInfo fileInfo;
        private Dictionary<string, string> internalDictionary;

        public FileDetails(FileInfo fileInfo)
        {
            this.fileInfo = fileInfo;
        }

        public override int Count
        {
            get 
            {
                if (internalDictionary == null)
                {
                    internalDictionary = ((IEnumerable<KeyValuePair<string, string>>) this).ToDictionary(p => p.Key, p => p.Value);
                }

                return internalDictionary.Count;
            }
        }

        public override void Clear()
        {
            throw new NotImplementedException();
        }

        public override void Add(string key, string value)
        {
            throw new NotImplementedException();
        }

        public override bool ContainsKey(string key)
        {
            if (internalDictionary == null)
            {
                internalDictionary = this.Where(p => !string.IsNullOrEmpty(p.Value)).DistinctBy(p => p.Key).ToDictionary(p => p.Key, p => p.Value);
            }

            return internalDictionary.ContainsKey(key);
        }

        public override bool Remove(string key)
        {
            throw new NotImplementedException();
        }

        public override bool TryGetValue(string key, out string value)
        {
            if (internalDictionary == null)
            {
                internalDictionary = this.Where(p => !string.IsNullOrEmpty(p.Value)).DistinctBy(p => p.Key).ToDictionary(p => p.Key, p => p.Value);
            }

            if (internalDictionary.ContainsKey(key))
            {
                value = internalDictionary[key];
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }

        public override IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            var headers = new List<string>();
            Folder folder;
            FolderItem item;

            try
            {
                var shell = new Shell();

                folder = shell.NameSpace(Path.GetDirectoryName(fileInfo.FullName));
                item = folder.Items().Cast<FolderItem>().SingleOrDefault(i => i.Name == fileInfo.Name);

                for (var x = 0; x < short.MaxValue; x++)
                {
                    var header = folder.GetDetailsOf(null, x);

                    if (String.IsNullOrEmpty(header))
                    {
                        break;
                    }

                    headers.Add(header);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            for (int x = 0; x < headers.Count; x++)
            {
                string header;
                string value;

                try
                {
                    header = headers[x];
                    value = folder.GetDetailsOf(item, x);
                }
                catch (Exception ex)
                {
                    throw;
                }

                yield return new KeyValuePair<string, string>(header, value);
            }
        }

        protected override void SetValue(string key, string value)
        {
            throw new NotImplementedException();
        }
    }
}