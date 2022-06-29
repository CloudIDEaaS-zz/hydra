// file:	LogCleaner.cs
//
// summary:	Implements the log cleaner class

using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX
{
    /// <summary>   A log cleaner. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 1/18/2021. </remarks>

    public class LogCleaner : BaseThreadedService
    {
        private Queue<DirectoryInfo> logDirectories;
        private int logRetentionDays;
        private DirectoryInfo topMostDirectory;

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/18/2021. </remarks>
        ///
        /// <param name="topMostFolder">    The pathname of the top most folder. </param>

        public LogCleaner(string topMostFolder) : base(System.Threading.ThreadPriority.Lowest)
        {
            logRetentionDays = int.Parse(ConfigurationSettings.AppSettings["LogRetentionDays"]);

            this.topMostDirectory = new DirectoryInfo(topMostFolder);
        }


        /// <summary>   Executes the work operation. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/18/2021. </remarks>
        ///
        /// <param name="stopping"> True to stopping. </param>

        public override void DoWork(bool stopping)
        {
            var halted = false;

            using (this.Lock())
            {
                halted = processingHalted;
            }

            if (logDirectories == null)
            {
                logDirectories = new Queue<DirectoryInfo>();

                this.topMostDirectory.GetDirectories("*", SearchOption.AllDirectories).ToList().ForEach(d =>
                {
                    DateTime dateTime;

                    if (DateTimeExtensions.IsSortableDateTimeText(d.FullName, out dateTime))
                    {
                        if (DateTime.Now - dateTime > TimeSpan.FromDays(logRetentionDays))
                        {
                            logDirectories.Enqueue(d);
                        }
                    }
                });
            }
            else if (logDirectories.Count > 0)
            {
                var logDirectory = logDirectories.Dequeue();

                try
                {
                    logDirectory.Delete(true);
                }
                catch
                {
                }
            }
            else if (!halted)
            {
                using (this.Lock())
                {
                    processingHalted = true;
                }

                Task.Run(() =>
                {
                    this.Stop();
                });
            }
        }
    }
}
