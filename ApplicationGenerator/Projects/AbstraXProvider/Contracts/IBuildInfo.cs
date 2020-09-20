using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstraX.Contracts
{
    public enum BuildStatus
    {
        Started,
        Finished,
        WaitingForUserFeedback,
        Aborted
    }

    public class BuildStatusInfo
    {
        public DateTime TimeStamp { get; set; }
        public BuildStatus BuildStatus { get; set; }

        public BuildStatusInfo(BuildStatus buildStatus, DateTime timeStamp)
        {
            this.BuildStatus = buildStatus;
            this.TimeStamp = timeStamp;
        }
    }

    public interface IBuildInfo
    {
        Guid ID { get; set; }
        DateTime BuildStart { get; set; }
        string UserName { get; set; }
        void AppendStatus(BuildStatus buildStatus);
        Queue<BuildStatusInfo> BuildStatusQueue { get; }
        BuildStatus LastBuildStatus { get; }
    }
}
