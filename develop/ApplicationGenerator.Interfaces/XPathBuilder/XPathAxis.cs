using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using CodePlex.XPathParser;
using System.Xml.XPath;

namespace AbstraX.XPathBuilder
{
    [DebuggerDisplay("{ DebugInfo }")]
    public class XPathAxis : IXPathPart
    {
        public CodePlex.XPathParser.XPathAxis Axis { get; }
        public XPathNodeType NodeType { get; }
        public string Prefix { get; }
        public string Name { get; }
        public string NodeTest { get; }


        public XPathAxis(CodePlex.XPathParser.XPathAxis axis, XPathNodeType nodeType, string prefix, string name, string nodeTest)
        {
            this.Axis = axis;
            this.NodeType = nodeType;
            this.Prefix = prefix;
            this.Name = name;
            this.NodeTest = nodeTest;
        }

        public string DebugInfo
        {
            get
            {
                return this.NodeTest;
            }
        }
    }
}
