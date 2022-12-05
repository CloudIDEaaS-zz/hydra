using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace HydraDebugAssistant.Info
{
    [Serializable]
    public class UserInfo
    {
        public List<RootFolderInfo> RootFolders { get; set; }
        public string UserName { get; set; }

        public UserInfo(string userName)
        {
            this.UserName = userName;
            this.RootFolders = new List<RootFolderInfo>();
        }

        public static UserInfo Read(FileInfo fileInfo)
        {
            UserInfo userInfo;
            var json = InterprocessVariables.OpenAndGetVariable(fileInfo, "HydraDebugAssistant", "UserInfo");

            if (json == null)
            {
                userInfo = new UserInfo(Environment.UserName);
            }
            else
            {
                userInfo = JsonExtensions.ReadJson<UserInfo>(json);
            }

            return userInfo;
        }

        public void Save(FileInfo fileInfo)
        {
            var json = this.ToJsonText();
            
            InterprocessVariables.SetVariable(fileInfo, "HydraDebugAssistant", "UserInfo", json);
        }
    }
}
