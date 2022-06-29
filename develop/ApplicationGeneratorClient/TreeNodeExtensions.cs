using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using ApplicationGenerator.Client.NodeInfo;

namespace ApplicationGenerator.Client
{
    public static class NodeExtensions
    {
        private static ImageList imageList;
        private static ImageList stateImageList;
        private static Bitmap blankStateImage;
        public static IManagedLockObject LockObject { get; set; }

        static NodeExtensions()
        {
            var type = typeof(frmMain);
            var blankStateImageFullName = @"Images\StateIcons\Blank.png";
            Bitmap bitmap;

            imageList = new ImageList();
            stateImageList = new ImageList();
            stateImageList.ImageSize = new Size(12, 12);

            blankStateImage = type.ReadResource<Bitmap>(blankStateImageFullName);
            bitmap = new Bitmap(blankStateImage);

            bitmap.MakeTransparent();

            blankStateImage = bitmap;
        }

        public static TreeViewEvents GetEvents(this TreeView treeView)
        {
            return new TreeViewEvents(treeView);
        }

        public static void AssignToImageList(this TreeView treeView)
        {
            var type = typeof(frmMain);
            var imageName = "Blank";
            var imageFullName = @"Images\" + imageName + ".png";
            Image image = null;
            int index;

            treeView.ImageList = imageList;
            treeView.StateImageList = stateImageList;

            if (imageList.Images.ContainsKey(imageName))
            {
                index = imageList.Images.IndexOfKey(imageName);
            }
            else
            {
                if (image == null)
                {
                    image = type.ReadResource<Bitmap>(imageFullName);
                }

                imageList.Images.Add(imageName, image);
                index = imageList.Images.IndexOfKey(imageName);
            }
        }

        public static TreeNode AddFolderWithSpinster<TParent, TChild>(this TreeNode treeNode, TParent parent, List<TChild> childData, string folderText, string spinsterText = "Loading...")
        {
            var folderTreeNode = new TreeNode(folderText);
            var spinsterTreeNode = new TreeNode(spinsterText);
            var spinsterNode = new SpinsterNode<TParent, TChild>(parent, childData);

            folderTreeNode.Assign(folderText, "Folder");
            spinsterTreeNode.Assign(spinsterNode);

            treeNode.Add(folderTreeNode);
            folderTreeNode.Add(spinsterTreeNode);

            return folderTreeNode;
        }

        public static bool HasSpinster(this TreeNode treeNode)
        {
            return treeNode.Nodes.Count == 1 && treeNode.Nodes.Cast<TreeNode>().Any(n => n.Tag is SpinsterNodeBase);
        }

        public static void Assign(this TreeNode treeNode, string nodeType, string imageName)
        {
            var type = typeof(frmMain);
            string imageFullName;
            Image image = null;
            int index;

            imageFullName = @"Images\" + imageName + ".png";

            if (imageList.Images.ContainsKey(imageName))
            {
                index = imageList.Images.IndexOfKey(imageName);
            }
            else
            {
                if (image == null)
                {
                    image = type.ReadResource<Bitmap>(imageFullName);
                }

                imageList.Images.Add(imageName, image);
                index = imageList.Images.IndexOfKey(imageName);
            }

            treeNode.ImageIndex = index;
            treeNode.SelectedImageIndex = index;

            treeNode.Tag = nodeType;
        }

        public static void Assign(this TreeNode treeNode, object obj, string imageName)
        {
            var type = typeof(frmMain);
            string imageFullName;
            Image image = null;
            int index;

            if (imageName.StartsWith(@"..\"))
            {
                imageFullName = imageName.Replace(@"..\", @"ApplicationGenerator.Client\");
            }
            else
            {
                imageFullName = @"Images\" + imageName + ".png";
            }

            if (imageList.Images.ContainsKey(imageName))
            {
                index = imageList.Images.IndexOfKey(imageName);
            }
            else
            {
                if (image == null)
                {
                    image = type.ReadResource<Bitmap>(imageFullName);
                }

                imageList.Images.Add(imageName, image);
                index = imageList.Images.IndexOfKey(imageName);
            }

            treeNode.ImageIndex = index;
            treeNode.SelectedImageIndex = index;

            treeNode.Tag = obj;
        }

        public static void Assign(this TreeNode treeNode, object obj)
        {
            treeNode.Tag = obj;
        }

        public static void Assign(this TreeNode treeNode, FileNode nodeInfo, Func<Image> getIconImage)
        {
            var type = typeof(frmMain);
            string imageFullName;
            string imageName;
            Image image = null;
            int index;
            var file = (AbstraX.ClientFolderStructure.File)nodeInfo.NodeObject;

            imageName = Path.GetExtension(file.FullName);

            imageFullName = @"Images\" + imageName + ".png";
            treeNode.Tag = nodeInfo;

            if (imageList.Images.ContainsKey(imageName))
            {
                index = imageList.Images.IndexOfKey(imageName);
            }
            else
            {
                if (image == null)
                {
                    image = getIconImage();
                }

                imageList.Images.Add(imageName, image);
                index = imageList.Images.IndexOfKey(imageName);
            }

            treeNode.ImageIndex = index;
            treeNode.SelectedImageIndex = index;
        }

        public static void Assign(this TreeNode treeNode, INodeInfo nodeInfo, NodeInfoType nodeInfoType)
        {
            var type = typeof(frmMain);
            string imageName = EnumUtils.GetName(nodeInfoType);
            string imageFullName;
            Image image = null;
            int index;

            imageName = EnumUtils.GetName(nodeInfoType);

            imageFullName = @"Images\" + imageName + ".png";
            treeNode.Tag = nodeInfo;

            if (imageList.Images.ContainsKey(imageName))
            {
                index = imageList.Images.IndexOfKey(imageName);
            }
            else
            {
                if (image == null)
                {
                    image = type.ReadResource<Bitmap>(imageFullName);
                }

                imageList.Images.Add(imageName, image);
                index = imageList.Images.IndexOfKey(imageName);
            }

            treeNode.ImageIndex = index;
            treeNode.SelectedImageIndex = index;
        }

        public static INodeInfo GetNodeInfo(this TreeNode treeNode)
        {
            return (INodeInfo)treeNode.Tag;
        }

        public static T GetNodeInfo<T>(this TreeNode treeNode) where T : NodeInfoBase
        {
            return (T)treeNode.Tag;
        }

        public static bool IsSpinsterNode(this TreeNode treeNode)
        {
            return treeNode.GetNodeInfo().NodeInfoType == NodeInfoType.Spinster;
        }

        public static bool IsNonSpinsterInfoNode(this TreeNode treeNode)
        {
            return treeNode.Tag != null && !treeNode.IsSpinsterNode();
        }

        public static string GetUniqueName(this INodeInfo nodeInfo, TreeNode treeNode = null)
        {
            var name = nodeInfo.Name;
            TreeNode parent;

            if (treeNode == null)
            {
                treeNode = nodeInfo.TreeNode;
            }

            parent = treeNode.Parent;

            if (parent != null)
            {
                var unitTestSiblings = treeNode.Nodes.Cast<TreeNode>().Where(n => n.IsNonSpinsterInfoNode()).Select(n => n.GetNodeInfo());
                var sameNameUnitTestNodes = unitTestSiblings.Where(n => n.Name == nodeInfo.Name);

                if (sameNameUnitTestNodes.Count() > 0)
                {
                    var id = sameNameUnitTestNodes.Count() + 1;

                    name = string.Format("{0}[{1}]", name, id);
                }
            }

            return name;
        }

        public static string GetName(this TreeNode treeNode)
        {
            var nodeInfo = treeNode.GetNodeInfo();

            return nodeInfo.GetName();
        }

        public static string GetName(this INodeInfo nodeInfo)
        {
            var name = string.Empty;

            switch (nodeInfo.NodeInfoType)
            {
                case NodeInfoType.None:

                    switch (nodeInfo.SpecialNodeType)
                    {
                        case SpecialNodeType.Folder:
                            var folderNode = (FolderNode)nodeInfo;
                            name = string.Format("Type:{0}, Name:{1}", folderNode.FolderType, folderNode.Text);
                            break;
                    }

                    return name;

                default:
                    name = nodeInfo.Text;
                    break;
            }

            name = string.Format("NodeType:{0}, Name:{1}", nodeInfo.NodeInfoType, name);

            return name;
        }

        public static string GetNodePath(this TreeNode treeNode)
        {
            var nodeInfo = treeNode.GetNodeInfo();

            return nodeInfo.NodePath;
        }

        public static NodeInfoType GetNodeInfoType(this TreeNode treeNode)
        {
            var nodeInfo = treeNode.GetNodeInfo();

            return nodeInfo.NodeInfoType;
        }

        public static SpecialNodeType GetNodeInfoSpecialType(this TreeNode treeNode)
        {
            var nodeInfo = treeNode.GetNodeInfo();

            return nodeInfo.SpecialNodeType;
        }
    }
}
