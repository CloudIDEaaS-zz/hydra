using System;
using System.Collections.Generic;
using System.Text;
using CodePlex.XPathParser;
using System.Diagnostics;
using System.Xml.XPath;

namespace CodeInterfaces.XPathBuilder
{
    public class XPathStringBuilder : IXPathBuilder<string> 
    {
        public Queue<XPathAxisElement> AxisElementQueue { get; set; }
        private string lastElement;
        private string lastAttribute;
        private XPathPredicate lastPredicate;
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                          
        public void StartBuild() 
        {
            this.AxisElementQueue = new Queue<XPathAxisElement>();
        }

        public string EndBuild(string result) 
        {
            if (lastElement != null)
            {
                CreateElement();
            }

            return result;
        }

        public string String(string value) 
        {
            return "'" + value + "'";
        }

        public string Number(string value) 
        {
            return value;
        }

        public string Operator(XPathOperator op, string left, string right) 
        {
            Debug.Assert(op != XPathOperator.Union);

            if (op == XPathOperator.UnaryMinus) 
            {
                return "-" + left;
            }

            if (left.StartsWith("attribute::"))
            {
                left = left.Remove(0, "attribute::".Length);
            }

            lastPredicate = new XPathPredicate(left, op, right);

            return left + opStrings[(int)op] + right;
        }

        public string Axis(XPathAxis xpathAxis, XPathNodeType nodeType, string prefix, string name) 
        {
            string nodeTest;

            switch (nodeType) 
            {
                case XPathNodeType.ProcessingInstruction:
                    Debug.Assert(prefix == "");
                    nodeTest = "processing-instruction(" + name + ")";
                    break;
                case XPathNodeType.Text:
                    Debug.Assert(prefix == null && name == null);
                    nodeTest = "text()";
                    break;
                case XPathNodeType.Comment:
                    Debug.Assert(prefix == null && name == null);
                    nodeTest = "comment()";
                    break;
                case XPathNodeType.All:
                    nodeTest = "node()";
                    break;
                case XPathNodeType.Attribute:

                    lastAttribute = name;
                    nodeTest = QNameOrWildcard(prefix, name);

                    break;

                case XPathNodeType.Element:

                    if (lastElement != null)
                    {
                        CreateElement();
                    }

                    lastElement = name;
                    nodeTest = QNameOrWildcard(prefix, name);

                    break;

                case XPathNodeType.Namespace:
                    nodeTest = QNameOrWildcard(prefix, name);
                    break;
                default:
                    throw new ArgumentException("unexpected XPathNodeType", "XPathNodeType");
            }

            return axisStrings[(int)xpathAxis] + nodeTest;
        }

        private void CreateElement()
        {
            var element = new XPathAxisElement(lastElement);

            if (lastPredicate != null)
            {
                element.Predicates.Add(lastPredicate);
            }

            this.AxisElementQueue.Enqueue(element);

            lastPredicate = null;
            lastElement = null;
        }

        public string JoinStep(string left, string right) 
        {
            return left + '/' + right;
        }

        public string Predicate(string node, string condition, bool reverseStep) 
        {
            if (!reverseStep) 
            {
                // In this method we don't know how axis was represented in original XPath and the only 
                // difference between ancestor::*[2] and (ancestor::*)[2] is the reverseStep parameter.
                // to not store the axis from previous builder events we simply wrap node in the () here.
                node = '(' + node + ')';
            }
            return node + '[' + condition + ']';
        }

        public string Variable(string prefix, string name) 
        {
            return '$' + QName(prefix, name);
        }

        public string Function(string prefix, string name, IList<string> args) 
        {
            string result = QName(prefix, name) + '(';
            for (int i = 0; i < args.Count; i++)
            {
                if (i != 0)
                {
                    result += ',';
                }
                result += args[i];
            }
            result += ')';
            return result;
        }

        private static string QName(string prefix, string localName) 
        {
            if (prefix == null) 
            {
                throw new ArgumentNullException("prefix");
            }
            if (localName == null)
            {
                throw new ArgumentNullException("localName");
            }
            return prefix == "" ? localName : prefix + ':' + localName;
        }

        private static string QNameOrWildcard(string prefix, string localName) 
        {
            if (prefix == null)
            {
                Debug.Assert(localName == null);
                return "*";
            }
            if (localName == null) 
            {
                Debug.Assert(prefix != "");
                return prefix + ":*";
            }
            return prefix == "" ? localName : prefix + ':' + localName;
        }

        string[] opStrings = 
        { 
            /* Unknown    */ " Unknown ",
            /* Or         */ " or " ,
            /* And        */ " and ",
            /* Eq         */ "="    ,
            /* Ne         */ "!="   ,
            /* Lt         */ "<"    ,
            /* Le         */ "<="   ,
            /* Gt         */ ">"    ,
            /* Ge         */ ">="   ,
            /* Plus       */ "+"    ,
            /* Minus      */ "-"    ,
            /* Multiply   */ "*"    ,
            /* Divide     */ " div ",
            /* Modulo     */ " mod ",
            /* UnaryMinus */ "-"    ,
            /* Union      */ "|"  
        };

        string[] axisStrings = 
        {
            /*Unknown          */ "Unknown::"           ,
            /*Ancestor         */ "ancestor::"          ,
            /*AncestorOrSelf   */ "ancestor-or-self::"  ,
            /*Attribute        */ "attribute::"         ,
            /*Child            */ "child::"             ,
            /*Descendant       */ "descendant::"        ,
            /*DescendantOrSelf */ "descendant-or-self::",
            /*Following        */ "following::"         ,
            /*FollowingSibling */ "following-sibling::" ,
            /*Namespace        */ "namespace::"         ,
            /*Parent           */ "parent::"            ,
            /*Preceding        */ "preceding::"         ,
            /*PrecedingSibling */ "preceding-sibling::" ,
            /*Self             */ "self::"              ,
            /*Root             */ "root::"              ,
        };
    }
}

