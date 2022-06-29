using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Windows.Forms;
using Utils;
using ApplicationGenerator.Client;

namespace ApplicationGenerator.Client.NodeInfo
{
    public abstract class NodeInfoBase : INodeInfo
    {
        private string path;
        public SpecialNodeType SpecialNodeType { get; set; }
        public object NodeObject { get; set; }
        public string Text { get; set; }
        public TreeNode TreeNode { get; set; }
        public virtual NodeInfoType NodeInfoType { get; set; }

        private string BuildPath(TreeNode treeNode)
        {
            var builder = new StringBuilder();
            var list = new List<string>();
            string path;

            while (treeNode != null)
            {
                var nodeInfo = (INodeInfo)treeNode.Tag;
                string featureNameEncoded;

                if (nodeInfo == null)
                {
                    DebugUtils.Break();
                }

                featureNameEncoded = HttpUtility.HtmlEncode(nodeInfo.GetUniqueName(treeNode)).Replace(@"\", "%2F");

                list.Add(featureNameEncoded);

                treeNode = treeNode.Parent;
            }

            foreach (var featureNameEncoded in list.Reverse<string>())
            {
                builder.AppendWithLeadingIfLength(@"\", featureNameEncoded.Replace("{", "{{").Replace("}", "}}"));
            }

            path = builder.ToString();
            return path;
        }

        public string NodePath
        {
            get
            {
                if (path == null)
                {
                    var treeNode = this.TreeNode;

                    path = BuildPath(treeNode);
                }

                return path;
            }
        }

        public string[] PathParts
        {
            get
            {
                return this.NodePath.Split('\\', StringSplitOptions.None);
            }
        }

        public string PathStack
        {
            get
            {
                return string.Join("\r\n", this.PathParts.Reverse());
            }
        }

        public string Name
        {
            get
            {
                return this.GetName();
            }
        }        
    }
}