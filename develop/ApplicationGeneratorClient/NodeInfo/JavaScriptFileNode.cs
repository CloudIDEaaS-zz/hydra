using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using VisualStudioProvider;
using CodeInterfaces;
using Utils;
using Newtonsoft.Json.Linq;
using Hydra.Configuration;


namespace Net2Html5ConfigurationTool.NodeInfo
{
    public class JavaScriptFileNode : CodeFileNode
    {
        private IVSProjectItem projectItem;

        public JavaScriptFileNode(TreeNode treeNode, IVSProjectItem projectItem)
        {
            this.TreeNode = treeNode;
            this.NodeObject = projectItem;
            this.Text = treeNode.Text;
            this.NodeInfoType = NodeInfoType.JavaScriptFile;
            this.projectItem = projectItem;
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

        public override IVSProjectItem ProjectItem
        {
            get 
            {
                return projectItem;
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
