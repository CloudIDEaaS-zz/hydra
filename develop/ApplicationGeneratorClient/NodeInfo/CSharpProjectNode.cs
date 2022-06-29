using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using VisualStudioProvider;
using CodeInterfaces;
using Hydra.GenerationObjects;
using Newtonsoft.Json.Linq;
using Hydra.Configuration;


namespace Net2Html5ConfigurationTool.NodeInfo
{
    public class CSharpProjectNode : NodeInfoBase
    {
        public FileInfo ConfigFileInfo { get; private set; }
        public IVSProject Project { get; private set; }
        public bool IsWebProject { get; private set; }
        public IRuntimeProject VirtualProject { get; private set; }
        public override bool IsVirtual { get; protected set; }

        public CSharpProjectNode(TreeNode treeNode, FileInfo configFileInfo, IVSProject project, bool isWebProject = false)
        {
            this.TreeNode = treeNode;
            this.Text = treeNode.Text;
            this.ConfigFileInfo = configFileInfo;

            if (isWebProject)
            {
                this.SpecialNodeType = SpecialNodeType.VSWebProject;
                this.NodeInfoType = NodeInfoType.None;
                this.IsWebProject = true;
            }
            else
            {
                this.SpecialNodeType = SpecialNodeType.VSProject;
                this.NodeInfoType = NodeInfoType.CSharpLibraryProject;
            }

            this.Project = project;
        }

        public CSharpProjectNode(TreeNode treeNode, FileInfo configFileInfo, IRuntimeProject project)
        {
            this.TreeNode = treeNode;
            this.Text = treeNode.Text;
            this.ConfigFileInfo = configFileInfo;
            this.SpecialNodeType = SpecialNodeType.VSProject;
            this.NodeInfoType = NodeInfoType.None;
            this.VirtualProject = project;
            this.IsVirtual = true;
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

        public override NodeSaveState GetSaveState()
        {
            return this.DetermineSaveState();
        }

        public override bool IsSaveable
        {
            get
            {
                return this.DetermineIsSaveable();
            }
        }

        public override ConfigNode GetConfigNode()
        {
            return this.ToConfigNode();
        }
    }
}
