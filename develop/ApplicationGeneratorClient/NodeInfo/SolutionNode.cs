using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using VisualStudioProvider;
using Newtonsoft.Json.Linq;
using Hydra.Configuration;


namespace Net2Html5ConfigurationTool.NodeInfo
{
    public class SolutionNode : NodeInfoBase
    {
        public FileInfo ConfigFileInfo { get; private set; }
        public IncludedSolution Solution { get; private set; }
        public string VirtualSolution { get; private set; }
        public override bool IsVirtual { get; protected set; }

        public SolutionNode(TreeNode treeNode, FileInfo configFileInfo, IncludedSolution solution)
        {
            this.TreeNode = treeNode;
            this.Text = treeNode.Text;
            this.NodeInfoType = NodeInfoType.None;
            this.ConfigFileInfo = configFileInfo;
            this.SpecialNodeType = SpecialNodeType.VSSolution;
            this.Solution = solution;
        }

        public SolutionNode(TreeNode treeNode, FileInfo configFileInfo, string virtualSolution)
        {
            this.TreeNode = treeNode;
            this.Text = treeNode.Text;
            this.NodeInfoType = NodeInfoType.None;
            this.ConfigFileInfo = configFileInfo;
            this.SpecialNodeType = SpecialNodeType.VSSolution;
            this.VirtualSolution = virtualSolution;
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
