using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AbstraX.BuildEvents;
using AbstraX.Contracts;

namespace AbstraX
{
    public class BuildInfo : IBuildInfo
    {
        public DateTime BuildStart { get; set; }
        public string UserName { get; set; }
        public Guid ID { get; set; }
        private Queue<BuildStatusInfo> buildStatusQueue;

        public BuildInfo(string userName)
        {
            /// userName will eventually be user session when made to handle multiple builds
            /// Session may include solutions opened, files opened, cached objects, etc.

            buildStatusQueue = new Queue<BuildStatusInfo>();
            ID = Guid.NewGuid();
        }

        public Queue<BuildStatusInfo> BuildStatusQueue
        {
            get
            {
                return buildStatusQueue;
            }
        }

        public BuildStatus LastBuildStatus
        {
            get 
            {
                return buildStatusQueue.Last().BuildStatus;
            }
        }

        public void AppendStatus(BuildStatus buildStatus)
        {
            buildStatusQueue.Enqueue(new BuildStatusInfo(buildStatus, DateTime.Now));
        }
    }
}
