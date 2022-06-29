using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CodeInterfaces;
using System.Windows.Forms;
using Utils;
using Hydra.GenerationObjects;
using Newtonsoft.Json.Linq;
using Hydra.Configuration;


namespace Net2Html5ConfigurationTool.NodeInfo
{
    public class ProjectFolderNode : CodeFolderNode
    {
        private IVSProjectItem projectItem;
        public bool IsScriptsFolder { get; private set; }
        private TreeNode treeNode;
        public IRuntimeProjectItem VirtualItem { get; private set; }
        public override bool IsVirtual { get; protected set; }

        public ProjectFolderNode(TreeNode treeNode, IVSProjectItem projectItem, FolderType folderType, bool isWebProjectFolder, bool isScriptsFolder)
        {
            this.TreeNode = treeNode;
            this.Text = treeNode.Text;
            this.SpecialNodeType = SpecialNodeType.VSProjectFolder;
            this.projectItem = projectItem;

            this.IsScriptsFolder = isScriptsFolder;
            this.treeNode = treeNode;

            if (folderType == FolderType.ProjectFolder && !isWebProjectFolder)
            {
                this.NodeInfoType = NodeInfoType.ProjectFolder;
            }
            else if (folderType == FolderType.Members)
            {
                this.NodeInfoType = NodeInfoType.Folder;
            }
            else
            {
                this.NodeInfoType = NodeInfo.NodeInfoType.None;
            }
        }

        public ProjectFolderNode(TreeNode treeNode, IRuntimeProjectItem projectItem, FolderType folderType, bool isWebProjectFolder, bool isScriptsFolder)
        {
            this.TreeNode = treeNode;
            this.Text = treeNode.Text;
            this.SpecialNodeType = SpecialNodeType.VSProjectFolder;
            this.VirtualItem = projectItem;
            this.IsVirtual = true;

            this.IsScriptsFolder = isScriptsFolder;
            this.treeNode = treeNode;

            if (folderType == FolderType.ProjectFolder && !isWebProjectFolder)
            {
                this.NodeInfoType = NodeInfoType.ProjectFolder;
            }
            else if (folderType == FolderType.Members)
            {
                this.NodeInfoType = NodeInfoType.Folder;
            }
            else
            {
                this.NodeInfoType = NodeInfo.NodeInfoType.None;
            }
        }

        public override IVSProjectItem ProjectItem
        {
            get 
            {
                return projectItem;
            }
        }

        public override DirectoryInfo DirectoryInfo
        {
            get 
            {
                var ancestors = treeNode.GetAncestorsAndSelf().TakeWhile(a => a.Tag is DirectoryNode).Select(f => f.Text).ToList();
                var folderPath = string.Join(@"/", ancestors);
                var itemParts = projectItem.RelativePath.Split('\\').Reverse<string>().ToList();
                var item = projectItem;
                string itemPath = null;
                string targetPath = null;
                IVSProjectItem targetItem = null;

                while (item.ItemType != "Folder")
                {
                    itemParts.RemoveAt(0);
                    item = item.ParentItem;
                }

                do
                {
                    itemPath = string.Join(@"/", itemParts);
                    targetItem = item;

                    if (itemParts.Count > 0)
                    {
                        itemParts.RemoveAt(0);
                        item = item.ParentItem;
                    }
                }
                while (itemPath.AsCaseless() != folderPath);

                targetPath = targetItem.FilePath;

                if (File.Exists(targetPath))
                {
                    targetPath = Path.GetDirectoryName(targetPath);
                }

                return new DirectoryInfo(targetPath);
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
