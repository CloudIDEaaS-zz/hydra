using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Utils.Core.Logging;

namespace Utils
{
    [DebuggerDisplay(" { DebugInfo } ")]
    public class Line
    {
        public string FilePath { get; set; }
        public int LineNumber { get; set; }
        public string Caller { get; set; }

        public Line(string filePath, int lineNumber, string caller)
        {
            this.FilePath = filePath;
            this.LineNumber = lineNumber;
            this.Caller = caller;
        }

        public override string ToString()
        {
            return this.DebugInfo;
        }

        public string DebugInfo
        {
            get
            {
                return string.Format("FilePath: '{0}', "
                    + "LineNumber: {1}:{2}",
        			this.FilePath,
                    this.Caller,
                    this.LineNumber
                );
            }
        }

        public string ToLogString()
        {
            return string.Format("{0}\t\t "
                + "{1}:{2}",
                this.FilePath,
                this.Caller,
                this.LineNumber
            );
        }
    }

    public class LineTracker : BaseList<Line>
    {
        public string Scope { get; }
        public string PrimaryFilePath { get; }
        private IServiceProvider serviceProvider;
        private ILineLoggerProvider lineLoggerProvider;
        private ILineLogger lineLogger;
        private DateTime sessionStart;

        public List<string> AdditionalFilePaths { get; }

        public LineTracker(string scope, string filePath, IServiceProvider serviceProvider)
        {
            this.Scope = scope;
            this.PrimaryFilePath = filePath;
            this.serviceProvider = serviceProvider;

            sessionStart = DateTime.Now;

            this.AdditionalFilePaths = new List<string>();
        }

        private string SubFolderName
        {
            get
            {
                return sessionStart.ToSortableDateTimeText();
            }
        }

        private string LogFileName
        {
            get
            {
                if (this.Scope != null)
                {
                    return this.Scope + ".log";
                }
                else
                {
                    var fileInfo = new FileInfo(this.PrimaryFilePath);
                    var category = fileInfo.Directory.Name + "_" + fileInfo.Name + ".log";

                    return category;
                }
            }
        }

        public override void Add(Line item)
        {
            if (lineLogger == null)
            {
                lineLoggerProvider = serviceProvider.GetService<ILineLoggerProvider>();
                lineLogger = (ILineLogger) lineLoggerProvider.CreateLogger(nameof(LineTracker));

                lineLogger.SubFolderName = this.SubFolderName;
                lineLogger.LogFileName = this.LogFileName;
            }

            lineLogger.LogInformation(item.ToLogString());

            base.Add(item);
        }

        public override void Clear()
        {
            base.Clear();
        }
    }
}
