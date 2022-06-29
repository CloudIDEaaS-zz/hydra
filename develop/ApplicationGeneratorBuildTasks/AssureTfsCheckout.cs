using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using System.Diagnostics;
using System.IO;
using MindChemistry;
using System.Windows.Forms;

namespace BuildTasks
{
    public class AssureTfsCheckout : ITask
    {
        public IBuildEngine BuildEngine { get; set; }
        public ITaskHost HostObject { get; set; }

        [Required]
        public string Location { get; set; }
        public string Pattern { get; set; }

        public AssureTfsCheckout()
        {
            this.Pattern = "*.*";
        }

        public bool Execute()
        {
            AssureFileCheckout(this.Location, this.Pattern);

            return true;
        }

        public void AssureFileCheckout(string path, string pattern = "*.*")
        {
            try
            {
                var teamFoundationServices = new TfsTeamProjectCollection(new Uri(HydraConstants.TFS_MCTEAM_PROJECT_COLLECTION_V2_URL), System.Net.CredentialCache.DefaultCredentials);

                teamFoundationServices.EnsureAuthenticated();

                var versionControlServer = (VersionControlServer)teamFoundationServices.GetService(typeof(VersionControlServer));

                if (File.Exists(path))
                {
                    var workspace = versionControlServer.GetWorkspace(Path.GetDirectoryName(path));
                    workspace.PendEdit(path);
                }
                else if (Directory.Exists(path))
                {
                    foreach (var file in Directory.GetFiles(path, pattern))
                    {
                        var workspace = versionControlServer.GetWorkspace(Path.GetDirectoryName(file));
                        workspace.PendEdit(file);
                    }
                }
            }
            catch (Exception ex)
            {
                var error = string.Format("Error checking out files from TFS. {0}", ex);

                if (this.BuildEngine != null)
                {
                    var message = new BuildErrorEventArgs(string.Empty, string.Empty, string.Empty, 0, 0, 0, 0, error, "", "", DateTime.Now);
                    this.BuildEngine.LogErrorEvent(message);
                }
                else
                {
                    Console.WriteLine(error);
                    throw new Exception(error);
                }
            }
        }
    }
}
