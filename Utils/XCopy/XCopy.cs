using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ZetaLongPaths;

namespace Utils.XCopy
{
	/*
	===========================
	 XCOPY.

		Last modified: 
			- 2019-09-02. More logging and a result value to evaluate by yourself, if needed.
			- 2018-05-27. Minor adjustments.
			- 2016-06-10. Changed logging file path.
			- 2016-06-07. Verbose logging.
			- 2016-06-06. ZlpSimpleFileAccessProtector calls added.
			- 2015-11-29. Added callback for exclude/include check.
			- 2015-07-30. Added Regex exclude/include.

	---------------------------
	Description:

		This is a quickly assembled (but rather well tested and used) piece of
		code to simulate parts of the XCOPY command line tool.

		It has fewer features than XCOPY and is intended for copying folders
		(the original XCOPY works for both files and folders). In addition it is 
		being designed to be used in a console environment (i.e. the operations
		are synchronously and blocking).

		I am using this in a C# Script[1] file to copy files and building a setup 
		with NSIS[2] for our  Test[3] and  Producer[4] software.

	---------------------------
	Example usage:

		var options = new FolderXCopyOptions
			{
				FilesPattern = "*.*",
				RecurseFolders = true,
				CopyEmptyFolders = true,
				CopyHiddenAndSystemFiles = true,
				OverwriteExistingFiles = true,
				CopyOnlyIfSourceIsNewer = false,
				FoldersPattern = "*",
				AlwaysMatchFolderIncludes = true
			}
			.AddExcludeSubStrings( 
				"\\.svn\\", 
				"\\_svn\\", 
				"\\_Temporary\\" );

		var xc = new FolderXCopy();
		var result = xc.Copy(
			sourceFolderPath,
			destinationFolderPath,
			options );

	---------------------------
	References:

		[1] C# Script - Script engine based on .NET, http://www.csscript.net
		[2] NSIS - Open Source installer, http://nsis.sourceforge.net/Main_Page
		[3]  Test Management - FREE Test Management Software, https://www.zeta-test.com
		[4]  Producer - FREE Website Generator CMS, https://www.zeta-producer.com
		[5]  Uploader - FREE sending of large files online, https://www.zeta-uploader.com

	---------------------------
	Contact information:

		Author: Uwe Keim

		E-mail: uwe.keim@gmail.com

		Website: https://uwe.co
		Twitter: https://twitter.com/axbm
		Facebook: https://www.facebook.com/uwe.keim

	---------------------------
	Other references:

		This file is also available at:
		
		- https://gist.github.com/UweKeim/280891650c17de682324a13816cdaf28
		- https://pastebin.com/QGqqCCQK

	===========================
	*/

	public class FolderXCopyOptions
	{
		public FolderXCopyOptions()
		{
			FilesPattern = "*.*";
			FoldersPattern = "*";
		}

		private readonly List<string> _excludeRegexSubStrings = new List<string>();
		private readonly List<string> _excludeSubStrings = new List<string>();
		private readonly List<string> _includeSubStrings = new List<string>();

		public bool VerboseLogging { get; set; }
		public bool RecurseFolders { get; set; }
		public bool CopyEmptyFolders { get; set; }
		public bool CopyHiddenAndSystemFiles { get; set; }
		public bool OverwriteExistingFiles { get; set; }
		public bool CopyOnlyIfSourceIsNewer { get; set; }
		public bool AlwaysMatchFolderIncludes { get; set; }

		public Func<string, bool, bool> WantExcludeFunc { get; set; }
		public Func<string, bool, bool> WantIncludeFunc { get; set; }

		public FolderXCopyOptions SetRecurseFolders(bool value) { RecurseFolders = value; return this; }
		public FolderXCopyOptions SetCopyEmptyFolders(bool value) { CopyEmptyFolders = value; return this; }
		public FolderXCopyOptions SetCopyHiddenAndSystemFiles(bool value) { CopyHiddenAndSystemFiles = value; return this; }
		public FolderXCopyOptions SetOverwriteExistingFiles(bool value) { OverwriteExistingFiles = value; return this; }
		public FolderXCopyOptions SetCopyOnlyIfSourceIsNewer(bool value) { CopyOnlyIfSourceIsNewer = value; return this; }
		public FolderXCopyOptions SetAlwaysMatchFolderIncludes(bool value) { AlwaysMatchFolderIncludes = value; return this; }

		public List<string> ExcludeRegexSubStrings { get { return _excludeRegexSubStrings; } }
		public List<string> ExcludeSubStrings { get { return _excludeSubStrings; } }
		public List<string> IncludeSubStrings { get { return _includeSubStrings; } }

		// E.g. "*.exe;*.dll"
		public string FilesPattern { get; set; }
		public string FoldersPattern { get; set; }

		public FolderXCopyOptions AddExcludeWildcardSubStrings(
			params string[] items)
		{
			if (items != null)
			{
				var list = new List<string>();

				foreach (var item in items)
				{
					list.Add(convertWildcardToRegex(item));
				}

				ExcludeRegexSubStrings.AddRange(list.ToArray());
			}
			return this;
		}

		private static string convertWildcardToRegex(string pattern)
		{
			// http://stackoverflow.com/a/6907849/107625
			// http://www.codeproject.com/Articles/11556/Converting-Wildcards-to-Regexes

			return "^" + Regex.Escape(pattern).
				Replace("\\*", ".*").
				Replace("\\?", ".") + "$";
		}

		public FolderXCopyOptions AddExcludeSubStrings(
			params string[] items)
		{
			ExcludeSubStrings.AddRange(items);
			return this;
		}

		public FolderXCopyOptions AddExcludeRegexSubStrings(
			params string[] items)
		{
			ExcludeRegexSubStrings.AddRange(items);
			return this;
		}

		public FolderXCopyOptions AddIncludeSubStrings(
			params string[] items)
		{
			IncludeSubStrings.AddRange(items);
			return this;
		}

		public FolderXCopyOptions SetExcludeCheckDelegate(
			Func<string, bool, bool> wantExclude)
		{
			WantExcludeFunc = wantExclude;
			return this;
		}

		public FolderXCopyOptions SetIncludeCheckDelegate(
			Func<string, bool, bool> wantInclude)
		{
			WantIncludeFunc = wantInclude;
			return this;
		}
	}

	public sealed class FolderXCopyResult
	{
		public int CopiedFolderCount { get; set; }
		public int CopiedFileCount { get; set; }

		public int SkippedFolderCount { get; set; }
		public int SkippedFileCount { get; set; }
	}

	// -------------

	public sealed class FolderXCopy
	{
		// Central switch to turn verbose logging on/off.
		private bool _VERBOSE;

		private void verboseLog(
			string text,
			params object[] args)
		{
			verboseLog(string.Format(text, args));
		}

		private void verboseLog(
			string text)
		{
			if (_VERBOSE)
			{
				log(string.Format("[VERBOSE {0}] {1}", DateTime.Now, text).Trim());
			}
		}

		private void dumpOptions(
			string sourceFolderPath,
			string destinationFolderPath,
			FolderXCopyOptions options)
		{
			verboseLog("+++++++++++++++++");
			verboseLog("Starting XCOPY folder tree.");

			verboseLog("\tSource = '{0}' (exists: {1}).", sourceFolderPath, Directory.Exists(sourceFolderPath));
			verboseLog("\tDestination = '{0}' (exists: {1}).", destinationFolderPath, Directory.Exists(destinationFolderPath));

			verboseLog("\tXCOPY options:");
			verboseLog("\t\tVerbose logging = '{0}'.", options.VerboseLogging);
			verboseLog("\t\tRecurse folders = '{0}'.", options.RecurseFolders);
			verboseLog("\t\tCopy empty folders = '{0}'.", options.CopyEmptyFolders);
			verboseLog("\t\tCopy hidden and system files = '{0}'.", options.CopyHiddenAndSystemFiles);
			verboseLog("\t\tOverwrite existing files = '{0}'.", options.OverwriteExistingFiles);
			verboseLog("\t\tCopy only if source is newer = '{0}'.", options.CopyOnlyIfSourceIsNewer);
			verboseLog("\t\tFiles pattern = '{0}'.", options.FilesPattern);
			verboseLog("\t\tFolders pattern = '{0}'.", options.FoldersPattern);
			verboseLog("\t\tAlways match folder includes = '{0}'.", options.AlwaysMatchFolderIncludes);

			verboseLog("\t\tExclude substrings ({0}):", options.ExcludeSubStrings.Count);
			for (var i = 0; i < options.ExcludeSubStrings.Count; ++i)
			{
				verboseLog("\t\t\tExclude substring = '{0}'.", options.ExcludeSubStrings[i]);
			}
			verboseLog("\t\tExclude regex substrings ({0}):", options.ExcludeRegexSubStrings.Count);
			for (var i = 0; i < options.ExcludeRegexSubStrings.Count; ++i)
			{
				verboseLog("\t\t\tExclude regex substring = '{0}'.", options.ExcludeRegexSubStrings[i]);
			}
			verboseLog("\t\tInclude substrings ({0}):", options.IncludeSubStrings.Count);
			for (var i = 0; i < options.IncludeSubStrings.Count; ++i)
			{
				verboseLog("\t\t\tInclude substring = '{0}'.", options.IncludeSubStrings[i]);
			}
			verboseLog("-----------------");
		}

		private void dumpResult(
			string sourceFolderPath,
			string destinationFolderPath,
			DateTime start,
			FolderXCopyResult result)
		{
			var end = DateTime.Now;
			var delta = end - start;

			verboseLog("-----------------");
			verboseLog("\tXCOPY result:");
			verboseLog("\t\tCopied folder count = '{0}'.", result.CopiedFolderCount);
			verboseLog("\t\tCopied file count = '{0}'.", result.CopiedFileCount);
			verboseLog("\t\tSkipped folder count = '{0}'.", result.SkippedFolderCount);
			verboseLog("\t\tSkipped file count = '{0}'.", result.SkippedFileCount);

			verboseLog("Finished XCOPY folder tree in {0} from '{1}' to '{2}'.", formatTimeSpan(delta), sourceFolderPath, destinationFolderPath);
			verboseLog("+++++++++++++++++");
		}

		public FolderXCopyResult Copy(
			string sourceFolderPath,
			string destinationFolderPath,
			FolderXCopyOptions options)
		{
			_VERBOSE = options.VerboseLogging;

			sourceFolderPath = ZlpPathHelper.GetFullPath(sourceFolderPath);
			destinationFolderPath = ZlpPathHelper.GetFullPath(destinationFolderPath);

			dumpOptions(
				sourceFolderPath,
				destinationFolderPath,
				options);

			var result = new FolderXCopyResult();

			var start = DateTime.Now;

			CopyFolderTree(
				sourceFolderPath,
				destinationFolderPath,
				options,
				result);

			dumpResult(
				sourceFolderPath,
				destinationFolderPath,
				start,
				result);

			return result;
		}

		// See http://stackoverflow.com/questions/11/how-do-i-calculate-relative-time.
		private static string formatTimeSpan(
			TimeSpan ts)
		{
			const int SECOND = 1;
			const int MINUTE = 60 * SECOND;
			const int HOUR = 60 * MINUTE;
			const int DAY = 24 * HOUR;
			const int MONTH = 30 * DAY;

			double delta = ts.TotalSeconds;

			if (delta < 1 * MINUTE)
			{
				return ts.Seconds == 1 ? "one second ago" : ts.Seconds + " seconds ago";
			}
			if (delta < 2 * MINUTE)
			{
				return "a minute ago";
			}
			if (delta < 45 * MINUTE)
			{
				return ts.Minutes + " minutes ago";
			}
			if (delta < 90 * MINUTE)
			{
				return "an hour ago";
			}
			if (delta < 24 * HOUR)
			{
				return ts.Hours + " hours ago";
			}
			if (delta < 48 * HOUR)
			{
				return "yesterday";
			}
			if (delta < 30 * DAY)
			{
				return ts.Days + " days ago";
			}
			if (delta < 12 * MONTH)
			{
				int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
				return months <= 1 ? "one month ago" : months + " months ago";
			}
			else
			{
				int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
				return years <= 1 ? "one year ago" : years + " years ago";
			}
		}

		private static bool doesStringNotContainSubString(
			string searchIn,
			List<string> subStrings,
			out string reason)
		{
			return !doesStringContainSubString(searchIn, subStrings, out reason);
		}

		private static bool doesStringContainSubString(
			string searchIn,
			List<string> subStrings,
			out string reason)
		{
			if (string.IsNullOrEmpty(searchIn))
			{
				reason = "Nothing to search in.";
				return false;
			}
			else
			{
				if (subStrings.Count <= 0)
				{
					// None present means "ALL".
					reason = "Nothing present => ALL.";
					return true;
				}
				else
				{
					foreach (var subString in subStrings)
					{
						if (searchIn.IndexOf(subString, StringComparison.InvariantCultureIgnoreCase) >= 0)
						{
							reason = string.Format("Substring match with '{0}'.", subString);
							return true;
						}
					}

					reason = "No substring match.";
					return false;
				}
			}
		}

		private static bool doesStringContainRegexSubString(
			string searchIn,
			List<string> subStrings,
			out string reason)
		{
			if (string.IsNullOrEmpty(searchIn))
			{
				reason = "Nothing to search in.";
				return false;
			}
			else
			{
				if (subStrings.Count <= 0)
				{
					// None present means "ALL".
					reason = "Nothing present => ALL.";
					return true;
				}
				else
				{
					foreach (var subString in subStrings)
					{
						if (new Regex(subString, RegexOptions.IgnoreCase).IsMatch(searchIn))
						{
							reason = string.Format("Regex match with '{0}'.", subString);
							return true;
						}
					}

					reason = "No regex match.";
					return false;
				}
			}
		}

		private static string PathHelperCombine(
			string path1,
			string path2)
		{
			if (string.IsNullOrEmpty(path1))
			{
				return path2;
			}
			else if (string.IsNullOrEmpty(path2))
			{
				return path1;
			}
			else
			{
				path1 = path1.TrimEnd('\\', '/').Replace('/', '\\');
				path2 = path2.TrimStart('\\', '/').Replace('/', '\\');

				return path1 + "\\" + path2;
			}
		}

		private void CopyFolderTree(
			string sourceFolderPath,
			string destinationFolderPath,
			FolderXCopyOptions options,
			FolderXCopyResult result)
		{
			verboseLog("");
			verboseLog("**************");
			verboseLog("Copying folder tree '{0}' to '{1}'.",
				sourceFolderPath,
				destinationFolderPath);

			var dst = destinationFolderPath;

			if (options.CopyEmptyFolders)
			{
				// Create as late as possible.
				CheckCreateFolder(dst);
			}

			// --
			// All files.

			var sourceFilePaths = getFiles(sourceFolderPath, options.FilesPattern);

			verboseLog("Got {0} files in source folder '{1}' with pattern '{2}'.",
				sourceFilePaths.Length,
				sourceFolderPath,
				options.FilesPattern);

			if (sourceFilePaths != null)
			{
				foreach (var sourceFilePath in sourceFilePaths)
				{
					var fileName = ZlpPathHelper.GetFileNameFromFilePath(sourceFilePath);
					var destinationFilePath =
						PathHelperCombine(
							dst,
							fileName);

					string reason;
					var copy = OnProgressFile(
						new ZlpFileInfo(sourceFilePath),
						new ZlpFileInfo(destinationFilePath),
						options,
						out reason);

					if (copy)
					{
						result.CopiedFileCount++;

						verboseLog("COPYING file '{0}' to '{1}', reason '{2}'.",
							sourceFilePath,
							destinationFilePath,
							reason);

						// Create as late as possible.
						CheckCreateFolder(dst);

						CopyFile(
							sourceFilePath,
							destinationFilePath,
							options);
					}
					else
					{
						result.SkippedFileCount++;

						verboseLog("NOT copying file '{0}' to '{1}', reason '{2}'.",
							sourceFilePath,
							destinationFilePath,
							reason);
					}
				}
			}

			// --
			// All folders.

			if (options.RecurseFolders)
			{
				verboseLog("RECURSING folders.");

				var srcFolders = ZlpIOHelper.GetDirectories(sourceFolderPath, options.FoldersPattern);

				verboseLog("Got {0} child folders in source folder '{1}' with pattern '{2}'.",
					srcFolders.Length,
					sourceFolderPath,
					options.FoldersPattern);

				if (srcFolders != null)
				{
					foreach (var srcFolder in srcFolders)
					{
						var path = srcFolder;

						if (!isFolderEmpty(path) || options.CopyEmptyFolders)
						{
							var diff = srcFolder.FullName.Substring(sourceFolderPath.Length);

							var destinationSubFolderPath =
								new ZlpDirectoryInfo(
									PathHelperCombine(dst, diff));

							string reason;
							var copy =
								OnProgressFolder(
									path,
									destinationSubFolderPath,
									options,
									out reason);

							if (copy)
							{
								result.CopiedFolderCount++;

								verboseLog("Recursing into folder '{0}' (destination folder '{1}', reason '{2}').",
									path.FullName,
									destinationSubFolderPath,
									reason);

								// Recurse into.
								CopyFolderTree(
									path.FullName,
									destinationSubFolderPath.FullName,
									options,
									result);
							}
							else
							{
								result.SkippedFolderCount++;

								verboseLog(
									"NOT recursing into folder '{0}' (destination folder '{1}', reason '{2}').",
									path.FullName,
									destinationSubFolderPath.FullName,
									reason);
							}
						}
					}
				}
			}
			else
			{
				verboseLog("NOT recursing folders.");
			}
		}

		private static bool isFolderEmpty(
			ZlpDirectoryInfo folderPath)
		{
			return folderPath.GetFiles().Length <= 0;
		}

		private static bool OnProgressFolder(
			ZlpDirectoryInfo sourceFolderPath,
			ZlpDirectoryInfo destinationFolderPath,
			FolderXCopyOptions options,
			out string reason)
		{
			string localReason = string.Empty;

			var sfp = sourceFolderPath.FullName.TrimEnd('\\') + "\\";

			if (options.WantExcludeFunc != null && options.WantExcludeFunc(sfp, false))
			{
				reason = string.Format("Folder exclude function match.");
				return false;
			}
			else if (options.ExcludeSubStrings.Count > 0 &&
				doesStringContainSubString(sfp, options.ExcludeSubStrings, out localReason))
			{
				reason = string.Format("Folder Substring exclude match. '{0}'.", localReason);
				return false;
			}
			else if (options.ExcludeRegexSubStrings.Count > 0 &&
				doesStringContainRegexSubString(sfp, options.ExcludeRegexSubStrings, out localReason))
			{
				reason = string.Format("Folder Substring regex exclude match. '{0}'.", localReason);
				return false;
			}
			else
			{
				if (options.WantIncludeFunc != null && options.WantIncludeFunc(sfp, false))
				{
					reason = string.Format("Folder include function match.");
					return true;
				}
				else if (options.AlwaysMatchFolderIncludes)
				{
					reason = string.Format("Options always match folder includes is active.");
					return true;
				}
				else if (options.IncludeSubStrings.Count <= 0 ||
					doesStringContainSubString(sfp, options.IncludeSubStrings, out localReason))
				{
					reason = string.Format("Folder Substring include match. '{0}'.", localReason);
					return true;
				}
				else
				{
					reason = string.Format("Folder Substring include nothing matches. '{0}'.", localReason);
					return false;
				}
			}
		}

		private static bool OnProgressFile(
			ZlpFileInfo sourceFilePath,
			ZlpFileInfo destinationFilePath,
			FolderXCopyOptions options,
			out string reason)
		{
			string localReason = string.Empty;

			if (options.CopyHiddenAndSystemFiles || !IsSystemOrHiddenFile(sourceFilePath))
			{
				if (!options.CopyOnlyIfSourceIsNewer || IsFileOneNewerThanFileTwo(sourceFilePath, destinationFilePath))
				{
					if (options.OverwriteExistingFiles || !destinationFilePath.Exists)
					{
						if (options.WantExcludeFunc != null && options.WantExcludeFunc(sourceFilePath.FullName, true))
						{
							reason = string.Format("File exclude function match.");
							return false;
						}
						else if (options.ExcludeSubStrings.Count > 0 &&
							doesStringContainSubString(sourceFilePath.FullName, options.ExcludeSubStrings, out localReason))
						{
							reason = string.Format("File Substring exclude match. '{0}'.", localReason);
							return false;
						}
						else if (options.ExcludeRegexSubStrings.Count > 0 &&
							doesStringContainRegexSubString(sourceFilePath.FullName, options.ExcludeRegexSubStrings, out localReason))
						{
							reason = string.Format("File Substring regex exclude match. '{0}'.", localReason);
							return false;
						}
						else
						{
							if (options.WantIncludeFunc != null && options.WantIncludeFunc(sourceFilePath.FullName, true))
							{
								reason = string.Format("File include function match.");
								return true;
							}
							else if (options.IncludeSubStrings.Count <= 0 ||
								doesStringContainSubString(sourceFilePath.FullName, options.IncludeSubStrings, out localReason))
							{
								reason = string.Format("File Substring include match. '{0}'.", localReason);
								return true;
							}
							else
							{
								reason = string.Format("File Substring include nothing matches. '{0}'.", localReason);
								return false;
							}
						}
					}
					else
					{
						reason = "File Not overwriting destination file.";
						return false;
					}
				}
				else
				{
					reason = "File Not copying older file.";
					return false;
				}
			}
			else
			{
				reason = "File Not copying system file.";
				return false;
			}
		}

		private void CopyFile(
			string sourceFilePath,
			string destinationFilePath,
			FolderXCopyOptions options)
		{
			var d = destinationFilePath;
			var dd = ZlpPathHelper.GetDirectoryPathNameFromFilePath(d);
			if (!ZlpIOHelper.DirectoryExists(dd))
			{
				verboseLog("Creating folder '{0}'.", dd);
				ZlpSimpleFileAccessProtector.Protect(() => ZlpIOHelper.CreateDirectory(dd));
			}

			verboseLog(
				"Copying file from '{0}' to '{1}'.",
				sourceFilePath,
				destinationFilePath);

			ZlpSimpleFileAccessProtector.Protect(() => ZlpIOHelper.CopyFile(sourceFilePath, destinationFilePath, options.OverwriteExistingFiles));
		}

		private string CheckCreateFolder(
			string folderPath)
		{
			if (folderPath != null && !ZlpIOHelper.DirectoryExists(folderPath))
			{
				verboseLog("Creating folder '{0}'.", folderPath);

				ZlpSimpleFileAccessProtector.Protect(() => ZlpIOHelper.CreateDirectory(folderPath));
			}

			return folderPath;
		}

		private static bool IsSystemOrHiddenFile(ZlpFileInfo filePath)
		{
			if (filePath == null || !filePath.Exists)
			{
				return false;
			}
			else
			{
				var attributes = filePath.Attributes;

				if ((attributes & ZetaLongPaths.Native.FileAttributes.Hidden) != 0 ||
					(attributes & ZetaLongPaths.Native.FileAttributes.System) != 0)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		private static bool IsSystemOrHiddenFolder(ZlpDirectoryInfo folderPath)
		{
			if (folderPath == null || !folderPath.Exists)
			{
				return false;
			}
			else
			{
				var attributes = folderPath.Attributes;

				if ((attributes & ZetaLongPaths.Native.FileAttributes.Hidden) != 0 ||
					(attributes & ZetaLongPaths.Native.FileAttributes.System) != 0)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		private static bool IsFileOneNewerThanFileTwo(
			string one,
			string two)
		{
			if (string.IsNullOrEmpty(one) || !ZlpIOHelper.FileExists(one))
			{
				return false;
			}
			else if (string.IsNullOrEmpty(two) || !ZlpIOHelper.FileExists(two))
			{
				return true;
			}
			else
			{
				var d1 = ZlpIOHelper.GetFileLastWriteTime(one);
				var d2 = ZlpIOHelper.GetFileLastWriteTime(two);

				var b = d1 > d2;
				return b;
			}
		}

		private static bool IsFileOneNewerThanFileTwo(
			ZlpFileInfo one,
			ZlpFileInfo two)
		{
			if (one == null)
			{
				return false;
			}
			else if (two == null)
			{
				return true;
			}
			else
			{
				return
					IsFileOneNewerThanFileTwo(
					one.FullName,
					two.FullName);
			}
		}

		private static string[] getFiles(
			string path,
			string searchPattern)
		{
			return getFiles(path, searchPattern, SearchOption.TopDirectoryOnly);
		}

		// Since Directory.GetFiles() does only allow one wildcard a time,
		// split e.g. "*.jpg;*.gif;*.png" into separate items and query for.
		// See http://social.msdn.microsoft.com/Forums/en-US/netfxbcl/thread/b0c31115-f6f0-4de5-a62d-d766a855d4d1
		private static string[] getFiles(
			string path,
			string searchPattern,
			SearchOption searchOption)
		{
			var searchPatterns = searchPattern.Split(new char[] { ';', ',', '|' }, StringSplitOptions.RemoveEmptyEntries);
			var files = new List<string>();
			foreach (string sp in searchPatterns)
			{
				new List<ZlpFileInfo>(ZlpIOHelper.GetFiles(path, sp.Trim(), searchOption)).ForEach(x => files.Add(x.FullName));
			}

			files.Sort();
			return files.ToArray();
		}

		private static void log()
		{
			log(string.Empty);
		}

		private static void log(string text)
		{
			log(text, new object[] { });
		}

		private static bool _isFirstLog = true;

		private static void log(string text, params object[] args)
		{
			try
			{
				Console.WriteLine(text, args);

				// --

				var filePath = Path.ChangeExtension(scriptFilePath, @".log");

				if (_isFirstLog && File.Exists(filePath)) File.Delete(filePath);
				_isFirstLog = false;

				if (!string.IsNullOrEmpty(text))
				{
					var msg = string.Format(@"[{0}] {1}" + Environment.NewLine,
						DateTime.Now,
						string.Format(text, args));

					File.AppendAllText(filePath, msg);

					var additionalLogFilePath = Environment.GetEnvironmentVariable("CENTRAL_LOGFILEPATH");
					if (!string.IsNullOrEmpty(additionalLogFilePath))
					{
						File.AppendAllText(additionalLogFilePath, msg);
					}
				}
			}
			catch (Exception)
			{
				// Logging errors should _never_ interrupt program flow.
			}
		}

		private static void log(object o)
		{
			log(o == null ? string.Empty : o.ToString());
		}

		private static string _scriptFile = new System.Diagnostics.StackTrace(new System.Diagnostics.StackFrame(true)).GetFrame(0).GetFileName();
		private static string scriptFilePath { get { return _scriptFile; } }
		private static string scriptFolderPath { get { return Path.GetDirectoryName(_scriptFile).TrimEnd('\\'); } }
		private static string scriptFileName { get { return Path.GetFileName(_scriptFile); } }
	}
}