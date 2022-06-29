using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using VisualStudioProvider;
using Utils;
using Newtonsoft.Json.Linq;
using Hydra.Configuration;

namespace Net2Html5ConfigurationTool.NodeInfo
{
    public class ConfigFileNode : FileNode, IConfigNodeProvider
    {
        private FileInfo fileInfo;

        public ConfigFileNode(TreeNode treeNode, FileInfo fileInfo)
        {
            this.TreeNode = treeNode;
            this.NodeObject = fileInfo;
            this.Text = treeNode.Text;
            this.NodeInfoType = NodeInfoType.None;
            this.SpecialNodeType = SpecialNodeType.ConfigFile;
            this.fileInfo = fileInfo;
        }

        public override IRegisteredConstruct RegisteredConstruct
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override FileInfo FileInfo
        {
            get 
            {
                return fileInfo;
            }
        }

        public override bool IsVirtual
        {
            get
            {
                return false;
            }

            protected set
            {
                DebugUtils.Break();
            }
        }

        public override NodeSaveState GetSaveState()
        {
            throw new InvalidOperationException("ConfigFileNode does not have a save state.");
        }

        public override bool IsSaveable
        {
            get
            {
                return !this.FileInfo.IsReadOnly;
            }
        }

        public override ConfigNode GetConfigNode()
        {
            throw new InvalidOperationException("ConfigFileNode cannot return a single ConfigNode.");
        }

        public Dictionary<string, ConfigNode> GetConfigNodes()
        {
            return this.TreeNode.GetAllNodes().Where(n => n.Tag is INodeInfo).Select(n => n.GetNodeInfo()).Where(i => i.IsSaveable).ToDictionary(i => i.NodePath, i => i.ToConfigNode());
        }
    }
}
