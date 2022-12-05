using System;
using System.Linq;

namespace Hydra.ReleaseManagement.Services.Models
{
    public class ReleaseSubmission
    {
        public DateTime SubmissionDate { get; set; }
        public string Environment { get; set; }
        public string BuildConfiguration { get; set; }
        public string PrimaryBinaryFileName { get; set; }
        public string ReleaseInfoJson { get; set; }
        public string ReleaseType { get; set; }
        public string CommitId { get; set; }
        public string BuildId { get; set; }
        public string RepositoryUri { get; set; }
        public string AdditionalConfigFiles { get; set; }
    }
}