using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Xml.Linq;
using System.Diagnostics;
using System.Security.Cryptography;
using System.IO.Packaging;
#if !SILVERLIGHT
using System.Drawing;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

[StructLayout(LayoutKind.Sequential)]
public struct SHFILEINFO
{
    public IntPtr hIcon; 
    public int iIcon;
    public uint dwAttributes;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
    public string szDisplayName;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
    public string szTypeName;
};

static class Win32
{
    public const uint SHGFI_ICON = 0x100;
    public const uint SHGFI_LARGEICON = 0x0; // 'Large icon
    public const uint SHGFI_SMALLICON = 0x1; // 'Small icon

    [DllImport("shell32.dll")]
    public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);
    [DllImport("kernel32.dll")]
    public static extern bool CreateDirectory(string lpNewDirectory, IntPtr lpSecurityAttributes);
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern bool RemoveDirectory(string lpPathName);
}

#endif

namespace Utils
{
    public static class IOExtensions
    {
        public const int MAX_PATH = 260;
#if !SILVERLIGHT
        internal static Dictionary<StreamWriter, Stack<TagHandler>> tagHandlerStack;

        static IOExtensions()
        {
            tagHandlerStack = new Dictionary<StreamWriter, Stack<TagHandler>>();
        }
#endif
        #region Watchers
#if !SILVERLIGHT
        private static List<FileSystemWatcher> watchers;

        public static string GetRelativePath(this FileInfo file, string basePath)
        {
            return FileUtilities.MakeRelative(basePath, file.FullName);
        }

        public static void OnChangedOrDeleted(this FileInfo file, Action<FileSystemEventArgs> action)
        {
            var watcher = new FileSystemWatcher(file.DirectoryName, file.Name);

            if (watchers == null)
            {
                watchers = new List<FileSystemWatcher>();
            }

            watchers.Add(watcher);

            watcher.Changed += (sender, e) =>
            {
                action(e);

                watchers.Remove(watcher);
            };

            watcher.Deleted += (sender, e) =>
            {
                action(e);

                watchers.Remove(watcher);
            };

            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;
        }

        public static void OnCreatedOrChanged(this FileInfo file, Action<FileSystemEventArgs> action, bool raiseIfExists = true)
        {
            var creationTime = DateTime.MinValue;
            var modifiedTime = DateTime.MinValue;

            if (File.Exists(file.FullName))
            {
                var fileCheck = new FileInfo(file.FullName);

                creationTime = fileCheck.CreationTime;
                modifiedTime = fileCheck.LastWriteTime;
            }

            OnCreatedOrChanged(file, creationTime, modifiedTime, action);
        }

        public static string GetSubPart(this DirectoryInfo directory, int offsetPlusOrMinus, int count = 1)
        {
            var parts = directory.FullName.Split('\\');

            if (offsetPlusOrMinus > 0)
            {
                return string.Join(@"\", parts.Skip(offsetPlusOrMinus).Take(count));
            }
            else
            {
                return string.Join(@"\", parts.Reverse().Skip(Math.Abs(offsetPlusOrMinus) - 1).Take(count).Reverse());
            }
        }

        public static void OnCreatedOrChanged(this FileInfo file, DateTime creationTime, DateTime modifiedTime, Action<FileSystemEventArgs> action, bool raiseIfExists = true)
        {
            var directory = file.Directory;

            while (!Directory.Exists(directory.FullName))
            {
                directory = directory.Parent;
            }

            if (directory != null)
            {
                var watcher = new FileSystemWatcher(directory.FullName, file.Name);

                if (watchers == null)
                {
                    watchers = new List<FileSystemWatcher>();
                }

                if (raiseIfExists && file.Exists)
                {
                    action(new FileSystemEventArgs(WatcherChangeTypes.Created, file.DirectoryName, file.Name));
                }
                else
                {
                    // in case file was created after we created Watcher but before we hooked events

                    var oneTimeAction = new OneTimeTimer(100);

                    oneTimeAction.Start(() =>
                    {
                        if (File.Exists(file.FullName) && watchers.Contains(watcher))
                        {
                            if (creationTime != DateTime.MinValue)
                            {
                                if (creationTime != file.CreationTime)
                                {
                                    action(new FileSystemEventArgs(WatcherChangeTypes.Created, file.DirectoryName, file.Name));
                                }
                            }
                            else if (modifiedTime != DateTime.MinValue)
                            {
                                if (modifiedTime != file.LastWriteTime)
                                {
                                    action(new FileSystemEventArgs(WatcherChangeTypes.Changed, file.DirectoryName, file.Name));
                                }
                            }

                            watchers.Remove(watcher);
                        }
                    });
                }

                watchers.Add(watcher);

                watcher.Created += (sender, e) =>
                {
                    if (e.FullPath.AsCaseless() == file.FullName)
                    {
                        if (watchers.Contains(watcher))
                        {
                            if (creationTime != DateTime.MinValue)
                            {
                                if (creationTime != file.CreationTime)
                                {
                                    action(e);
                                }
                            }
                            else if (modifiedTime != DateTime.MinValue)
                            {
                                if (modifiedTime != file.LastWriteTime)
                                {
                                    action(e);
                                }
                            }

                            watchers.Remove(watcher);
                        }
                    }
                };

                watcher.Changed += (sender, e) =>
                {
                    if (e.FullPath.AsCaseless() == file.FullName)
                    {
                        if (watchers.Contains(watcher))
                        {
                            if (creationTime != DateTime.MinValue)
                            {
                                if (creationTime != file.CreationTime)
                                {
                                    action(e);
                                }
                            }
                            else if (modifiedTime != DateTime.MinValue)
                            {
                                if (modifiedTime != file.LastWriteTime)
                                {
                                    action(e);
                                }
                            }

                            watchers.Remove(watcher);
                        }
                    }
                };

                watcher.IncludeSubdirectories = true;
                watcher.EnableRaisingEvents = true;
            }
            else
            {
                Debugger.Break();
            }
        }

        public static void OnCreated(this FileInfo file, Action action)
        {
            var directory = file.Directory;

            while (!Directory.Exists(directory.FullName))
            {
                directory = directory.Parent;
            }

            if (directory != null)
            {
                var watcher = new FileSystemWatcher(directory.FullName);

                if (watchers == null)
                {
                    watchers = new List<FileSystemWatcher>();
                }

                if (File.Exists(file.FullName))
                {
                    action();
                    return;
                }
                else
                {
                    // in case file was created after we created Watcher but before we hooked events

                    var oneTimeAction = new OneTimeTimer(100);

                    oneTimeAction.Start(() =>
                    {
                        if (File.Exists(file.FullName) && watchers.Contains(watcher))
                        {
                            watchers.Remove(watcher);
                            action();
                        }
                        else
                        {
                            Debug.WriteLine("'{0}' still does not exist", file.FullName);
                        }
                    });
                }

                Debug.WriteLine("Watching '{0}'", directory.FullName);

                watchers.Add(watcher);

                watcher.Created += (sender, e) =>
                {
                    Debug.WriteLine("'{0}' created", e.FullPath);

                    if (e.FullPath.AsCaseless() == file.FullName)
                    {
                        if (watchers.Contains(watcher))
                        {
                            watchers.Remove(watcher);
                            action();
                        }
                    }
                };

                watcher.IncludeSubdirectories = true;
                watcher.EnableRaisingEvents = true;
            }
            else
            {
                Debugger.Break();
            }
        }

        public static void OnReadOnlyChanged(this FileInfo file, Action action)
        {
            var isReadOnly = file.IsReadOnly;

            var thread = new Thread(() =>
            {
                file = new FileInfo(file.FullName);

                while (isReadOnly == file.IsReadOnly)
                {
                    file = new FileInfo(file.FullName);

                    Thread.Sleep(1000);
                }

                action();
            });

            thread.Priority = ThreadPriority.Lowest;
            thread.IsBackground = true;

            thread.Start();
        }

        public static void OnChanged(this FileInfo file, Action action)
        {
            var watcher = new FileSystemWatcher(file.Directory.FullName);

            watcher.Changed += (sender, e) =>
            {
                if (e.FullPath.AsCaseless() == file.FullName)
                {
                    action();
                }
            };

            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;
        }

        public static void OnDeleted(this FileInfo file, Action action)
        {
            var watcher = new FileSystemWatcher(file.FullName);

            watcher.Deleted += (sender, e) =>
            {
                if (e.FullPath.AsCaseless() == file.FullName)
                {
                    action();
                }
            };

            watcher.EnableRaisingEvents = true;
        }

        public static string GetHash(this string str)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = str.ToStream())
                {
                    return Convert.ToBase64String(md5.ComputeHash(stream));
                }
            }
        }

        public static byte[] GetHashData(this string str)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = str.ToStream())
                {
                    return md5.ComputeHash(stream);
                }
            }
        }

        public static string GetHash(this FileInfo file)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(file.FullName))
                {
                    return Convert.ToBase64String(md5.ComputeHash(stream));
                }
            }
        }
#endif
        #endregion

#if !SILVERLIGHT
        public static T GetIcon<T>(this FileInfo file) 
        {
            var shInfo = new SHFILEINFO();
            var hImgSmall = Win32.SHGetFileInfo(file.FullName, 0, ref shInfo, (uint)Marshal.SizeOf(shInfo), Win32.SHGFI_ICON | Win32.SHGFI_SMALLICON);
            var icon = Icon.FromHandle(shInfo.hIcon);
            object returnVal = null;

            switch (typeof(T).FullName)
            {
                case "System.Drawing.Icon":

                    returnVal = icon;
                    break;

                case "System.Drawing.Bitmap":
                    
                    var bitmap = icon.ToBitmap();
                    returnVal = bitmap;
                    break;

                default:
                    Debugger.Break();
                    break;
            }

            return (T)returnVal;
        }

        public static void Replace(string originalFile, string outputFile, string searchTerm, string replaceTerm)
        {
            byte b;
            var searchBytes = Encoding.UTF8.GetBytes(searchTerm.ToUpper());
            var searchBytesLower = Encoding.UTF8.GetBytes(searchTerm.ToLower());
            var bytesToAdd = new byte[searchBytes.Length];
            var searchBytesLength = searchBytes.Length;
            var searchByte0 = searchBytes[0];
            var searchByte0Lower = searchBytesLower[0];
            var replaceBytes = Encoding.UTF8.GetBytes(replaceTerm);
            var counter = 0;

            using (FileStream inputStream = File.OpenRead(originalFile))
            {
                //input length
                long srcLength = inputStream.Length;
                using (BinaryReader inputReader = new BinaryReader(inputStream))
                {
                    using (FileStream outputStream = File.OpenWrite(outputFile))
                    {
                        using (BinaryWriter outputWriter = new BinaryWriter(outputStream))
                        {
                            for (int nSrc = 0; nSrc < srcLength; ++nSrc)
                                //first byte
                                if ((b = inputReader.ReadByte()) == searchByte0
                                    || b == searchByte0Lower)
                                {
                                    bytesToAdd[0] = b;
                                    int nSearch = 1;
                                    //next bytes
                                    for (; nSearch < searchBytesLength; ++nSearch)
                                        //get byte, save it and test
                                        if ((b = bytesToAdd[nSearch] = inputReader.ReadByte()) != searchBytes[nSearch]
                                            && b != searchBytesLower[nSearch])
                                        {
                                            break;//fail
                                        }
                                    //Avoid overflow. No need, in my case, because no chance to see searchTerm at the end.
                                    //else if (nSrc + nSearch >= srcLength)
                                    //    break;

                                    if (nSearch == searchBytesLength)
                                    {
                                        //success
                                        ++counter;
                                        outputWriter.Write(replaceBytes);
                                        nSrc += nSearch - 1;
                                    }
                                    else
                                    {
                                        //failed, add saved bytes
                                        outputWriter.Write(bytesToAdd, 0, nSearch + 1);
                                        nSrc += nSearch;
                                    }
                                }
                                else
                                    outputWriter.Write(b);
                        }
                    }
                }
            }
        }

        public static void WriteLineFormat(this StreamWriter writer, string format, params object[] args)
        {
            writer.Write(format + "\r\n", args);
        }

        public static void WriteLineFormatSpaceIndent(this StreamWriter writer, int count, string format, params object[] args)
        {
            writer.Write(" ".Repeat(count));
            writer.Write(format + "\r\n", args);
        }

        public static void WriteLineFormatTabIndent(this StreamWriter writer, int count, string format, params object[] args)
        {
            writer.Write('\t'.Repeat(count));
            writer.Write(format + "\r\n", args);
        }

        public static TagHandler WriteTag(this StreamWriter writer, string tag, object attributesOrValue = null)
        {
            Stack<TagHandler> stack = null;
            TagHandler handler;

            if (!tagHandlerStack.ContainsKey(writer))
            {
                stack = new Stack<TagHandler>();
                tagHandlerStack.Add(writer, stack);
            }
            else
            {
                stack = tagHandlerStack[writer];
            }

            handler = WriteTag(writer, stack.Count, tag, attributesOrValue);

            stack.Push(handler);

            handler.Disposed += (sender, e) =>
            {
                var handlerPeek = stack.Peek();

                Debug.Assert(handlerPeek.Tag == handler.Tag);

                stack.Pop();

                if (stack.Count == 0)
                {
                    tagHandlerStack.Remove(writer);
                }
            };

            return handler;
        }

        private static TagHandler WriteTag(this StreamWriter writer, int indent, string tag, object attributesOrValue = null)
        {
            if (attributesOrValue != null)
            {
                var type = attributesOrValue.GetType();

                if (type.Is<string>() || type.Is<DateTime>() || type.IsEnum || type.IsPrimitive)
                {
                    writer.WriteFormatTabIndent(indent, "<{0}>{1}", tag, attributesOrValue.ToString());
                    return new TagHandler(writer, 0, tag);
                }
                else
                {
                    writer.WriteFormatTabIndent(indent, "<{0}", tag);

                    foreach (var property in type.GetProperties())
                    {
                        var propertyName = property.Name;
                        var value = property.GetValue(attributesOrValue, null);

                        writer.Write(" {0}=\"{1}\"", propertyName, value.ToString());
                    }

                    writer.WriteLine(">");
                }
            }
            else
            {
                writer.WriteLineFormatTabIndent(indent, "<{0}>", tag);
            }

            return new TagHandler(writer, indent, tag);
        }

        public static void WriteSpaceIndent(this StreamWriter writer, int count, string text)
        {
            writer.Write(" ".Repeat(count));
            writer.Write(text);
        }

        public static void WriteFormatSpaceIndent(this StreamWriter writer, int count, string format, params object[] args)
        {
            writer.Write(" ".Repeat(count));
            writer.Write(format, args);
        }

        public static void WriteTabIndent(this StreamWriter writer, int count, string text)
        {
            writer.Write('\t'.Repeat(count));
            writer.Write(text);
        }

        public static void WriteFormatTabIndent(this StreamWriter writer, int count, string format, params object[] args)
        {
            writer.Write('\t'.Repeat(count));
            writer.Write(format, args);
        }

        public static void WriteLineSpaceIndent(this StreamWriter writer, int count, string text)
        {
            writer.Write(" ".Repeat(count));
            writer.WriteLine(text);
        }

        public static void WriteLineTabIndent(this StreamWriter writer, int count, string text)
        {
            writer.Write('\t'.Repeat(count));
            writer.WriteLine(text);
        }

        public static StreamReset MarkForReset(this BinaryReader reader)
        {
            return new StreamReset(reader.BaseStream);
        }

        public static StreamReset MarkForReset(this BinaryWriter reader)
        {
            return new StreamReset(reader.BaseStream);
        }

        public static StreamReset MarkForReset(this StreamReader reader)
        {
            return new StreamReset(reader.BaseStream);
        }

        public static StreamReset MarkForReset(this Stream stream)
        {
            return new StreamReset(stream);
        }

        public static string GetBytesHexString(this Array array, int maxLength = 255, bool rewind = false, bool noEOFMarker = false, bool noSpaces = false)
        {
            var targetLength = (int) Math.Min((int) array.Length, maxLength);
            byte[] bytes = new byte[targetLength];
            Stream stream;

            Array.Copy(array, bytes, targetLength);

            stream = bytes.ToMemory();

            return stream.GetBytesHexString(targetLength, rewind, noEOFMarker, noSpaces);
        }

        public static string GetBytesHexString(this Stream stream, int maxLength = 255, bool rewind = false, bool noEOFMarker = false, bool noSpaces = false)
        {
            var reader = new BinaryReader(stream);

            return reader.GetBytesHexString(maxLength, rewind, noEOFMarker, noSpaces);
        }

        public static string GetBytesHexString(this BinaryReader reader, int maxLength = 255, bool rewind = false, bool noEOFMarker = false, bool noSpaces = false)
        {
            var builder = new StringBuilder();
            var stream = reader.BaseStream;
            long pos = stream.Position;
            long end = Math.Min(stream.Length, maxLength == -1 ? int.MaxValue : maxLength);

            using (var reset = reader.MarkForReset())
            {
                if (rewind)
                {
                    reader.Rewind();
                }

                pos = stream.Position;
                end = Math.Min(stream.Length, maxLength == -1 ? int.MaxValue : maxLength);

                while (end > pos)
                {
                    var b = reader.ReadByte();

                    if (noSpaces)
                    {
                        builder.AppendFormat("{0}", b.ToHexString());
                    }
                    else
                    {
                        builder.AppendFormat("{0} ", b.ToHexString());
                    }

                    pos++;
                }

                if (stream.Position == stream.Length && !noEOFMarker)
                {
                    builder.Append("EOF");
                }
            }

            return builder.ToString();
        }

        public static string GetBytesDataString(this Array array, int length = 255, bool rewind = false)
        {
            var targetLength = (int)Math.Min((int)array.Length, length);
            byte[] bytes = new byte[targetLength];
            Stream stream;

            Array.Copy(array, bytes, targetLength);

            stream = bytes.ToMemory();

            return stream.GetBytesDataString(targetLength, rewind);
        }

        public static string GetBytesDataString(this Stream stream, int length = 255, bool rewind = false)
        {
            var reader = new BinaryReader(stream);

            return reader.GetBytesDataString(length, rewind);
        }

        public static string GetBytesDataString(this BinaryReader reader, int length = 255, bool rewind = false)
        {                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                   
            var builder = new StringBuilder();
            var stream = reader.BaseStream;
            long pos = stream.Position;
            long end = Math.Min(stream.Length, length == -1 ? int.MaxValue : length);

            using (var reset = reader.MarkForReset())
            {
                if (rewind)
                {
                    reader.Rewind();
                }

                pos = stream.Position;
                end = Math.Min(stream.Length, length == -1 ? int.MaxValue : length);

                while (end > pos)
                {
                    var c = reader.ReadChar();

                    builder.Append(c);

                    pos++;
                }
            }

            return builder.ToString();
        }

        public static void SeekToLine(this StreamReader reader, int lineNumber)
        {
            string line;

            for (var x = 1; x < lineNumber; x++)
            {
                line = reader.ReadLine();
            }
        }

#if INCLUDE_PROCESSDIAGNOSTICSLIBRARY
        public static void Seek(this ProcessBinaryReader reader, long offset, ProcessSeekOrigin origin = ProcessSeekOrigin.Begin)
        {
            ((ProcessStream)reader.BaseStream).Seek(offset, origin);
        }

        public static void Seek(this ProcessBinaryWriter reader, long offset, ProcessSeekOrigin origin = ProcessSeekOrigin.Begin)
        {
            ((ProcessStream)reader.BaseStream).Seek(offset, origin);
        }
#endif

        public static long Seek(this StreamReader reader, long offset)
        {
            return reader.BaseStream.Seek(offset, SeekOrigin.Begin);
        }

        public static long Seek(this BinaryWriter writer, long offset)
        {
            return writer.BaseStream.Seek(offset, SeekOrigin.Begin);
        }

        public static long Seek(this StreamReader reader, long offset, SeekOrigin origin)
        {
            return reader.BaseStream.Seek(offset, origin);
        }

        public static string Read(this StreamReader reader, int count)
        {
            var buffer = new char[count];

            reader.Read(buffer, (int) 0, count);

            return new string(buffer);
        }

        public static void Seek(this BinaryReader reader, long offset, SeekOrigin origin = SeekOrigin.Begin)
        {
            reader.BaseStream.Seek(offset, origin);
        }

        public static void Advance(this BinaryReader reader, long offset)
        {
            reader.BaseStream.Seek(offset, SeekOrigin.Current);
        }

        public static void Rewind(this BinaryReader reader)
        {
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
        }

        public static void Rewind(this StreamReader reader)
        {
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            reader.DiscardBufferedData();
        }

        public static void Rewind(this Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
        }

        public static void Seek(this BinaryReader reader, ulong offset, SeekOrigin origin = SeekOrigin.Begin)
        {
            reader.BaseStream.Seek((long) offset, origin);
        }

        public static void Advance(this BinaryReader reader, ulong offset)
        {
            reader.BaseStream.Seek((long) offset, SeekOrigin.Current);
        }

        public static byte[] ToArray(this GZipStream inputStream)
        {
            var output = new byte[0];
            var bytes = new byte[100];
            var offset = 0;
            var totalCount = 0;

            while (true)
            {
                var bytesRead = inputStream.Read(bytes, offset, 100);

                if (bytesRead == 0)
                {
                    break;
                }

                offset += bytesRead;
                totalCount += bytesRead;

                output = output.Append(bytes);
            }

            return output;
        }

        public static void ToZipFile(this DirectoryInfo directory, string newZipFileName)
        {
            var outputStream = File.OpenWrite(newZipFileName);

            using (var zipArchive = new ZipArchive(outputStream, ZipArchiveMode.Create))
            {
                foreach (var file in directory.GetFiles("*.*", SearchOption.AllDirectories))
                {
                    if (file.FullName.AsCaseless() != newZipFileName)
                    {
                        var path = file.FullName.RemoveStart(directory.FullName).RemoveStartIfMatches(@"\");
                        var entry = zipArchive.CreateEntry(path);

                        using (var zipStream = entry.Open())
                        {
                            var bytes = File.ReadAllBytes(file.FullName);

                            zipStream.Write(bytes, 0, bytes.Length);
                            zipStream.Flush();
                        }
                    }
                }
            }
        }

        public static void Unzip(this FileInfo zipFile, DirectoryInfo directory)
        {
            var inputStream = File.OpenRead(zipFile.FullName);

            using (var zipArchive = new ZipArchive(inputStream, ZipArchiveMode.Read))
            {
                foreach (var entry in zipArchive.Entries)
                {
                    var name = entry.FullName;
                    var deflateStream = entry.Open();
                    var memoryStream = new MemoryStream();
                    var path = Path.Combine(directory.FullName, name);
                    var newFile = new FileInfo(path);
                    var newDirectory = newFile.Directory;

                    if (!newDirectory.Exists)
                    {
                        newDirectory.Create();
                    }

                    deflateStream.CopyTo(memoryStream);

                    File.WriteAllBytes(path, memoryStream.ToArray());
                }
            }
        }

        public static void ToZipFile(this XDocument document, string fileName)
        {
            GZipStream zipStream = null;
            var outputStream = File.OpenWrite(fileName);

            using (zipStream = new GZipStream(outputStream, CompressionMode.Compress))
            {
                document.Save(zipStream);
            }
        }

        public static XDocument ZipFileToXDocument(string fileName)
        {
            var inputStream = File.OpenRead(fileName);

            using (var zipStream = new GZipStream(inputStream, CompressionMode.Decompress))
            {
                return XDocument.Load(zipStream);
            }
        }

        public static StreamReader ZipFileToReader(string fileName)
        {
            var inputStream = File.OpenRead(fileName);

            var zipStream = new GZipStream(inputStream, CompressionMode.Decompress);

            return new StreamReader(zipStream);
        }

        public static byte[] ToZip(this string text)
        {
            GZipStream zipStream = null;
            var bytes = System.Text.UTF8Encoding.UTF8.GetBytes(text);
            byte[] output;

            using (var outputStream = new MemoryStream())
            {
                using (zipStream = new GZipStream(outputStream, CompressionMode.Compress))
                {
                    zipStream.Write(bytes, 0, bytes.Length);
                    zipStream.Flush();
                }

                output = outputStream.ToArray();
            }

            return output;
        }

        public static byte[] ToZip(this byte[] bytes)
        {
            GZipStream zipStream = null;
            byte[] output;

            using (var outputStream = new MemoryStream())
            {
                using (zipStream = new GZipStream(outputStream, CompressionMode.Compress))
                {
                    zipStream.Write(bytes, 0, bytes.Length);
                    zipStream.Flush();
                }

                output = outputStream.ToArray();
            }

            return output;
        }

        public static string FromZip(this byte[] bytes)
        {
            GZipStream zipStream;
            string output;
            var stream = bytes.ToMemory();

            stream.Write(bytes, 0, bytes.Length);
            stream.Seek(0, SeekOrigin.Begin);

            zipStream = new GZipStream(stream, CompressionMode.Decompress);

            using (var outputStream = new MemoryStream())
            {
                zipStream.CopyTo(outputStream);

                output = Encoding.UTF8.GetString(outputStream.ToArray());
            }

            return output;
        }

        public static Bitmap ToBitmap(this Stream inputream)
        {
            var bitmap = new Bitmap(inputream);

            return bitmap;
        }

        public static Icon ToIcon(this Stream inputream)
        {
            var icon = new Icon(inputream);

            return icon;
        }
#endif
        public static string GetCommonRoot(params string[] paths)
        {
            var minDepth = 999;
            var parsedPaths = new List<string[]>();

            foreach (var path in paths)
            {
                var parts = path.Split('\\');

                parsedPaths.Add(parts);
                minDepth = parts.Length < minDepth ? parts.Length : minDepth;
            }

            var builder = new StringBuilder();

            for (var x = 0; x < minDepth; x++)
            {
                var list = new List<string>();

                foreach (var dir in parsedPaths)
                {
                    list.Add(dir[x].ToLower());
                }

                if (!AllEqual(list.ToArray()))
                {
                    break;
                }

                builder.AppendFormat("{0}\\", list[0]);
            }

            return builder.ToString();
        }

        private static bool AllEqual(params string[] strings)
        {
            var returnValue = true;

            for (int i = 1; i < strings.Length; i++)
            {
                returnValue &= strings[0].AsCaseless() == strings[i];
            }

            return returnValue;
        }

        public static void MakeWritable(this FileInfo file)
        {
            file.Attributes &= (~FileAttributes.ReadOnly);
        }

        public static void Hide(this FileSystemInfo info)
        {
            if (!info.Attributes.HasFlag(FileAttributes.Hidden))
            {
                info.Attributes |= FileAttributes.Hidden;
            } 
        }

        public static DirectoryInfo Append(this DirectoryInfo directory, string path)
        {
            return new DirectoryInfo(Path.Combine(directory.FullName, path));
        }

        public static void WriteText(this MemoryStream stream, string text)
        {
            var bytes = text.ToBytes();

            stream.Write(bytes, 0, bytes.Length);
        }

        public static void ExtendTo(this MemoryStream stream, int width)
        {
            if (stream.Length < width)
            {
                using (var reset = stream.MarkForReset())
                {
                    var delta = (int)(width - stream.Length);

                    stream.Seek(0, SeekOrigin.End);

                    stream.WriteText("\0".Repeat(delta));
                }
            }
        }

        public static MemoryStream ToMemory(this byte[] bytes)
        {
            var stream = new MemoryStream(bytes.Length);

            stream.Write(bytes, 0, bytes.Length);
            stream.Seek(0, SeekOrigin.Begin);

            return stream;
        }

        public static MemoryStream ToMemory(this byte[] bytes, int length)
        {
            var stream = new MemoryStream(length);

            stream.Write(bytes, 0, length);
            stream.Seek(0, SeekOrigin.Begin);

            return stream;
        }

        public static MemoryStream Trim(this Stream inputStream)
        {
            var outputStream = new MemoryStream();
            var bytes = inputStream.ReadUntil(new byte[] { (byte)'\0' }, true, (int)inputStream.Length);

            return bytes.ToMemory();
        }

        public static MemoryStream ToMemory(this Stream inputStream)
        {
            var outputStream = new MemoryStream();

            CopyTo(inputStream, outputStream);

            return outputStream;
        }

        public static long GetActualPosition(this StreamReader reader)
        {
            System.Reflection.BindingFlags flags = System.Reflection.BindingFlags.DeclaredOnly | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.GetField;

            // The current buffer of decoded characters
            char[] charBuffer = (char[])reader.GetType().InvokeMember("charBuffer", flags, null, reader, null);

            // The index of the next char to be read from charBuffer
            int charPos = (int)reader.GetType().InvokeMember("charPos", flags, null, reader, null);

            // The number of decoded chars presently used in charBuffer
            int charLen = (int)reader.GetType().InvokeMember("charLen", flags, null, reader, null);

            // The current buffer of read bytes (byteBuffer.Length = 1024; this is critical).
            byte[] byteBuffer = (byte[])reader.GetType().InvokeMember("byteBuffer", flags, null, reader, null);

            // The number of bytes read while advancing reader.BaseStream.Position to (re)fill charBuffer
            int byteLen = (int)reader.GetType().InvokeMember("byteLen", flags, null, reader, null);

            // The number of bytes the remaining chars use in the original encoding.
            int numBytesLeft = reader.CurrentEncoding.GetByteCount(charBuffer, charPos, charLen - charPos);

            // For variable-byte encodings, deal with partial chars at the end of the buffer
            int numFragments = 0;

            if (byteLen > 0 && !reader.CurrentEncoding.IsSingleByte)
            {
                if (reader.CurrentEncoding.CodePage == 65001) // UTF-8
                {
                    byte byteCountMask = 0;

                    while ((byteBuffer[byteLen - numFragments - 1] >> 6) == 2) // if the byte is "10xx xxxx", it's a continuation-byte
                    {
                        byteCountMask |= (byte)(1 << ++numFragments); // count bytes & build the "complete char" mask
                    }

                    if ((byteBuffer[byteLen - numFragments - 1] >> 6) == 3) // if the byte is "11xx xxxx", it starts a multi-byte char.
                    {
                        byteCountMask |= (byte)(1 << ++numFragments); // count bytes & build the "complete char" mask
                    }

                    // see if we found as many bytes as the leading-byte says to expect
                    if (numFragments > 1 && ((byteBuffer[byteLen - numFragments] >> 7 - numFragments) == byteCountMask))
                    {
                        numFragments = 0; // no partial-char in the byte-buffer to account for
                    }
                }
                else if (reader.CurrentEncoding.CodePage == 1200) // UTF-16LE
                {
                    if (byteBuffer[byteLen - 1] >= 0xd8) // high-surrogate
                    {
                        numFragments = 2; // account for the partial character
                    }
                }
                else if (reader.CurrentEncoding.CodePage == 1201) // UTF-16BE
                {
                    if (byteBuffer[byteLen - 2] >= 0xd8) // high-surrogate
                    {
                        numFragments = 2; // account for the partial character
                    }
                }
            }
        
            return reader.BaseStream.Position - numBytesLeft - numFragments;
        }

        public static MemoryStream ToWriteableMemory(this Stream inputStream)
        {
            var outputStream = new MemoryStream();
            var buffer = new byte[inputStream.Length];
            int read;

            while ((read = inputStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                outputStream.Write(buffer, 0, read);
            }

            return outputStream;
        }

        public static byte[] ToArray(this Stream inputStream)
        {
            if (inputStream.CanSeek)
            {
                var bytes = new byte[inputStream.Length];

                inputStream.Read(bytes, 0, bytes.Length);

                return bytes;
            }
            else
            {
                var thisRead = 0;
                var read = 0;
                var innerStream = new MemoryStream();
                var position = innerStream.Position;
                var bufferSize = 64 * 1024;
                var buffer = new byte[bufferSize];

                do
                {
                    thisRead = inputStream.Read(buffer, 0, bufferSize - read);
                    innerStream.Write(buffer, 0, thisRead);
                    read += thisRead;
                }
                while (inputStream.CanRead && thisRead > 0);

                innerStream.Rewind();

                return innerStream.ToArray();
            }
        }

#if !SILVERLIGHT
        public static char[] ToCharArray(this byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes).ToCharArray();
        }
#endif

        public static byte[] HexToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        public static byte[] ToArray(this string text)
        {
            var bytes = System.Text.UTF8Encoding.UTF8.GetBytes(text);

            return bytes;
        }

        public static MemoryStream ToStream(this string text)
        {
            var stream = new MemoryStream();
            var bytes = System.Text.UTF8Encoding.UTF8.GetBytes(text);

            stream.Write(bytes, 0, bytes.Length);
            stream.Seek(0, SeekOrigin.Begin);

            return stream;
        }

        public static BinaryReader ToReader(this byte[] bytes)
        {
            var stream = new MemoryStream(bytes);
            var reader = new BinaryReader(stream);

            return reader;
        }

        public static void CopyTo(this Stream inputream, Stream outputStream)
        {
            var bytes = new byte[inputream.Length];

            inputream.Read(bytes, 0, bytes.Length);
            outputStream.Write(bytes, 0, (int) inputream.Length);

            outputStream.Seek(0, SeekOrigin.Begin);
        }

        public static string ToText(this Stream inputream)
        {
            var bytes = new byte[inputream.Length];

            inputream.Read(bytes, 0, bytes.Length);

            return bytes.ToText();
        }

        public static XDocument ToXml(this Stream inputream)
        {
            return XDocument.Load(inputream);
        }

        private static IEnumerable<string> GetBestMatchPaths(this Assembly assembly, string path)
        {
            var names = assembly.GetManifestResourceNames();

            return null;
        }

        public static T ReadResource<T>(this Type type, string path)
        {
            object result = null;
            Stream stream;
            var newPath = path;
            
            if (newPath.Contains('\\'))
            {
                string extension;
                string directory;
                var slashedNamespace = type.Namespace.Replace('.', '\\');

                if (!newPath.StartsWith(slashedNamespace))
                {
                    newPath = Path.GetFullPath(slashedNamespace + @"\" + newPath);
                    newPath = newPath.RemoveStart(Directory.GetCurrentDirectory() + 1);
                }

                extension = Path.GetExtension(newPath);
                directory = Path.GetDirectoryName(newPath);
                
                newPath = Path.Combine(directory, Path.GetFileNameWithoutExtension(newPath)).Replace('\\', '.').Append(extension);
            }
            else if (newPath.Contains('/'))
            {
                Debugger.Break();

                var extension = Path.GetExtension(newPath);
                var directory = Path.GetDirectoryName(newPath);

                newPath = Path.Combine(directory, Path.GetFileNameWithoutExtension(newPath)).Replace('/', '.').Append(extension);
            }

            stream = type.Assembly.GetManifestResourceStream(newPath);

            if (stream == null)
            {
                stream = type.Assembly.GetManifestResourceStream(type.Namespace + "." + newPath);
            }

            if (stream == null)
            {
                e.Throw<ArgumentException>("Stream null by ReadResource");
            }

            using (stream)
            {
                switch (typeof(T).Name)
                {
                    case "Byte[]":
                        result = stream.ToArray();
                        break;
                    case "String":
                        result = stream.ToText();
                        break;
                    case "XDocument":
                        result = stream.ToXml();
                        break;
#if !SILVERLIGHT
                    case "Icon":
                        result = stream.ToIcon();
                        break;
                    case "Bitmap":
                        result = stream.ToBitmap();
                        break;
                    case "Stream":

                        var unmanagedStream = (UnmanagedMemoryStream)stream;
                        var bytes = unmanagedStream.ToArray();

                        result = bytes.ToMemory();
                        break;
#endif
                }
            }

            return (T)result;
        }

        public static T ReadResource<T>(this Assembly assembly, string path)
        {
            object result = null;
            var stream = assembly.GetManifestResourceStream(path);

            if (stream == null)
            {
                e.Throw<ArgumentException>("Stream null by ReadResource");
            }

            using (stream)
            {
                switch (typeof(T).Name)
                {
                    case "Byte[]":
                        result = stream.ToArray();
                        break;
                    case "String":
                        result = stream.ToText();
                        break;
                    case "XDocument":
                        result = stream.ToXml();
                        break;
                    case "Stream":

                        var unmanagedStream = (UnmanagedMemoryStream)stream;
                        var bytes = unmanagedStream.ToArray();

                        result = bytes.ToMemory();
                        break;
                }
            }

            return (T)result;
        }

#if !SILVERLIGHT

        public static bool HasChangedFilesSince(this DirectoryInfo directory, DateTime date, string pattern)
        {
            foreach (var file in directory.GetFiles(pattern))
            {
                if (file.LastWriteTime > date)
                {
                    return true;
                }
            }

            foreach (var subDirectory in directory.GetDirectories())
            {
                if (subDirectory.HasChangedFilesSince(date, pattern))
                {
                    return true;
                }
            }

            return false;
        }

        public static string ToBase64(this Stream inputStream)
        {
            var bytes = inputStream.ToArray();

            return Convert.ToBase64String(bytes);
        }

        public static byte[] Read(this Stream stream, int length)
        {
            byte[] data = new byte[length];

            stream.Read(data, 0, length);

            return data;
        }

        public static TExpandTo ExpandTo<TExpandTo>(this Stream stream, object obj) where TExpandTo : new()
        {
            var toLength = TypeExtensions.SizeOf<TExpandTo>();
            var fromLength = obj.SizeOf();
            var difference = toLength - fromLength;
            var buffer = obj.ToByteArray().ExpandRight(difference);

            stream.Read(buffer, fromLength, difference);

            return ReadType<TExpandTo>(buffer);
        }

        public static byte[] ToByteArray(this object obj)
        {
            var size = obj.SizeOf();
            var ptr = Marshal.AllocCoTaskMem(size);
            var buffer = new byte[size];

            Marshal.StructureToPtr(obj, ptr, false);
            Marshal.Copy(ptr, buffer, 0, size);

            return buffer;
        }

        public static byte[] ToByteArray(params object[] objects)
        {
            byte[] combinedBuffer = new byte[0];

            foreach (var obj in objects)
            {
                if (obj is string)
                {
                    var buffer = ((string)obj).ToArray();

                    combinedBuffer = combinedBuffer.Append(buffer);
                }
                else if (obj is byte[])
                {
                    combinedBuffer = combinedBuffer.Append((byte[])obj);
                }
                else
                {
                    var buffer = obj.ToByteArray();

                    combinedBuffer = combinedBuffer.Append(buffer);
                }
            }

            return combinedBuffer;
        }

        public static T ReadType<T>(this byte[] buffer) where T : new()
        {
            var result = new T();
            var size = buffer.Length;
            var ptr = Marshal.AllocCoTaskMem(size);

            Marshal.Copy(buffer, 0, ptr, size);
            Marshal.PtrToStructure(ptr, result);

            return result;
        }

        public static T ReadType<T>(this Stream stream) where T : new()
        {
            var size = TypeExtensions.SizeOf<T>();
            var buffer = new byte[size];

            stream.Read(buffer, 0, size);

            return buffer.ReadType<T>();
        }

        public static void Write(this Stream stream, object obj)
        {
            var data = obj.ToByteArray();

            stream.Write(data);
        }

        public static void Write(this Stream stream, byte[] data)
        {
            stream.Write(data, 0, data.Length);
        }

        public static void Write(this Stream stream, string text)
        {
            var data = ASCIIEncoding.ASCII.GetBytes(text);

            stream.Write(data);
        }

        public static void Write(this Stream stream, params object[] objects)
        {
            foreach (var obj in objects)
            {
                if (obj is string)
                {
                    stream.Write((string)obj);
                }
                else if (obj is byte[])
                {
                    stream.Write((byte[])obj);
                }
                else
                {
                    stream.Write(obj);
                }
            }
        }

        public static byte[] ReadUntil(this Stream stream, string terminator, bool excludeTerminator = false, int limit = -1)
        {
            var terminatorBytes = ASCIIEncoding.ASCII.GetBytes(terminator);

            return stream.ReadUntil(terminatorBytes, excludeTerminator);
        }

        public static byte[] ReadUntil(this Stream stream, byte[] terminator, bool excludeTerminator = false, int limit = -1)
        {
            var data = new byte[0];
            var terminatorIndex = 0;
            var counter = 0;

            while (true)
            {
                var size = 1;
                var buffer = new byte[size];

                stream.Read(buffer, 0, size);

                data = data.Append(buffer);

                counter++;

                if (counter == limit)
                {
                    return data;
                }

                if (buffer[0] == terminator[terminatorIndex])
                {
                    terminatorIndex++;

                    if (terminatorIndex == terminator.Length)
                    {
                        if (excludeTerminator)
                        {
                            data = data.TrimRight(terminator.Length);
                        }

                        return data;
                    }
                }
                else
                {
                    terminatorIndex = 0;
                }
            }
        }

        public static byte[] ReadUntil(this BinaryReader reader, string terminator, bool excludeTerminator = false, int limit = -1)
        {
            var terminatorBytes = ASCIIEncoding.ASCII.GetBytes(terminator);

            return reader.ReadUntil(terminatorBytes, excludeTerminator);
        }

        public static byte[] ReadUntil(this BinaryReader reader, byte[] terminator, bool excludeTerminator = false, int limit = -1)
        {
            var data = new byte[0];
            var terminatorIndex = 0;
            var counter = 0;

            while (true)
            {
                var size = 1;
                var buffer = new byte[size];

                reader.Read(buffer, 0, size);

                data = data.Append(buffer);

                counter++;

                if (counter == limit)
                {
                    return data;
                }

                if (buffer[0] == terminator[terminatorIndex])
                {
                    terminatorIndex++;

                    if (terminatorIndex == terminator.Length)
                    {
                        if (excludeTerminator)
                        {
                            data = data.TrimRight(terminator.Length);
                        }

                        return data;
                    }
                }
                else
                {
                    terminatorIndex = 0;
                }
            }
        }

        public static string Expand(this string pathLocation)
        {
            return Environment.ExpandEnvironmentVariables(pathLocation);
        }

        public static void DeleteAndCreate(this DirectoryInfo directory)
        {
            if (directory.Exists)
            {
                directory.ForceDelete();
                directory.Refresh();
            }

            Win32.CreateDirectory(directory.FullName, IntPtr.Zero);
        }

        public static void ForceDelete(this DirectoryInfo directory)
        {
            foreach (var file in directory.GetFiles())
            {
                if (file.IsReadOnly)
                {
                    file.MakeWritable();
                }

                file.Delete();
            }

            foreach (var subDirectory in directory.GetDirectories())
            {
                subDirectory.ForceDelete();
            }

            try
            {
                directory.Delete();
            }
            catch
            {
                directory = new DirectoryInfo(directory.FullName);

                foreach (var file in directory.GetFiles())
                {
                    if (file.IsReadOnly)
                    {
                        file.MakeWritable();
                    }

                    file.Delete();
                }

                foreach (var subDirectory in directory.GetDirectories())
                {
                    subDirectory.ForceDelete();
                }

                directory.Delete();
            }
        }

        public static void ForceDeleteFiles(this DirectoryInfo directory)
        {
            foreach (var file in directory.GetFiles())
            {
                if (file.IsReadOnly)
                {
                    file.MakeWritable(); 
                }

                file.Delete();
            }
        }

        public static void ForceDeleteAllFilesAndSubFolders(this DirectoryInfo topDirectory)
        {
            foreach (var directory in topDirectory.GetDirectories("*", SearchOption.AllDirectories).OrderByDescending(d => d.FullName.Length).Concat(new List<DirectoryInfo> { topDirectory }))
            {
                DirectoryInfo directoryDelete;

                foreach (var file in directory.GetFiles())
                {
                    if (file.IsReadOnly)
                    {
                        file.MakeWritable();
                    }

                    file.Delete();
                }

                directoryDelete = new DirectoryInfo(directory.FullName);

                while (directoryDelete.Exists)
                {
                    try
                    {
                        directoryDelete.Delete();
                    }
                    catch
                    {
                        try
                        {
                            directoryDelete.Delete();
                        }
                        catch
                        {

                        }
                    }

                    directoryDelete = new DirectoryInfo(directory.FullName);
                }
            }
        }

        public static bool IsClrImage(string fileName)
        {
            FileStream fs = null;

            try
            {
                fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                byte[] dat = new byte[300];
                fs.Read(dat, 0, 128);
                if ((dat[0] != 0x4d) || (dat[1] != 0x5a)) // "MZ" DOS header 
                    return false;

                int lfanew = BitConverter.ToInt32(dat, 0x3c);
                fs.Seek(lfanew, SeekOrigin.Begin);
                fs.Read(dat, 0, 24); // read signature & PE file header 
                if ((dat[0] != 0x50) || (dat[1] != 0x45)) // "PE" signature 
                    return false;

                fs.Read(dat, 0, 96 + 128); // read PE optional header 
                if ((dat[0] != 0x0b) || (dat[1] != 0x01)) // magic 
                    return false;

                int clihd = BitConverter.ToInt32(dat, 208); // get IMAGE_COR20_HEADER rva-address 
                return clihd != 0;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                }
            }
        } 
#endif
    }
}
