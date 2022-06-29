using System;
using System.Collections.Generic;
using System.Text;
using CodePlex.XPathParser;
using System.Diagnostics;
using System.Xml.XPath;
using AbstraX.XPathBuilder;
using Utils;

namespace AbstraX.XPathBuilder
{
    public class XPathStringBuilder : IXPathBuilder<string> 
    {
        public Queue<IXPathPart> PartQueue { get; set; }
        private Queue<IXPathOperand> operandQueue;
        private Queue<IXPathPredicate> predicateQueue;
        private string lastElement;
        private XPathAttribute lastAttribute;
        private IXPathPredicate lastPredicate;
        private bool skipPredicate;
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                          
        public void StartBuild() 
        {
            this.PartQueue = new Queue<IXPathPart>();
            operandQueue = new Queue<IXPathOperand>();
            predicateQueue = new Queue<IXPathPredicate>();
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
            var xpathString = new XPathString(value);

            operandQueue.Enqueue(xpathString);

            return "'" + value + "'";
        }

        public string Number(string value) 
        {
            var xpathNumber = new XPathNumber(value);

            operandQueue.Enqueue(xpathNumber);

            return value;
        }

        public string Operator(XPathOperator op, string left, string right) 
        {
            Debug.Assert(op != XPathOperator.Union);

            if (op == XPathOperator.And)
            {
                if (right.StartsWith("attribute::"))
                {
                    right = right.Remove(0, "attribute::".Length);

                    lastPredicate = new XPathBooleanPredicate(predicateQueue.Dequeue(), op, new XPathAttribute(right));
                    skipPredicate = true;

                    predicateQueue.Enqueue(lastPredicate);
                }
                else
                {
                    lastPredicate = new XPathBooleanPredicate(predicateQueue.Dequeue(), op, predicateQueue.Dequeue());
                    skipPredicate = true;

                    predicateQueue.Enqueue(lastPredicate);
                }

                return left + opStrings[(int)op] + right;
            }
            else
            {
                if (op == XPathOperator.UnaryMinus)
                {
                    return "-" + left;
                }

                if (left.StartsWith("attribute::"))
                {
                    left = left.Remove(0, "attribute::".Length);
                }

                lastPredicate = new XPathPredicate(operandQueue.Dequeue(), op, operandQueue.Dequeue());
                skipPredicate = true;

                predicateQueue.Enqueue(lastPredicate);

                return left + opStrings[(int)op] + right;
            }
        }

        public string Axis(CodePlex.XPathParser.XPathAxis axis, XPathNodeType nodeType, string prefix, string name) 
        {
            string nodeTest;
            XPathAxis xPathAxis;
            var createAxis = false;

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
                    createAxis = true;
                    break;
                case XPathNodeType.Attribute:

                    lastAttribute = new XPathAttribute(name);
                    nodeTest = QNameOrWildcard(prefix, name);

                    operandQueue.Enqueue(lastAttribute);

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

            if (createAxis)
            {
                xPathAxis = new XPathAxis(axis, nodeType, prefix, name, nodeTest);
                this.PartQueue.Enqueue(xPathAxis);
            }

            return axisStrings[(int)axis] + nodeTest;
        }

        private void CreateElement()
        {
            var element = new XPathElement(lastElement);

            if (lastPredicate != null)
            {
                element.Predicates.Add(lastPredicate);
            }

            predicateQueue.Clear();
            this.PartQueue.Enqueue(element);

            lastPredicate = null;
            lastElement = null;
            skipPredicate = false;
        }

        public string JoinStep(string left, string right) 
        {
            return left + '/' + right;
        }

        public string Predicate(string node, string condition, bool reverseStep) 
        {
            if (!reverseStep) 
            {
                node = '(' + node + ')';
            }

            condition = condition.RegexRemove(".*::");

            if (!skipPredicate)
            {
                lastPredicate = new XPathPredicate(operandQueue.Dequeue());
            }

            return node + '[' + condition + ']';
        }

        public string Variable(string prefix, string name) 
        {
            var variable = new XPathVariable(prefix, name);

            this.PartQueue.Enqueue(variable);

            return '$' + QName(prefix, name);
        }

        public string Function(string prefix, string name, IList<string> args) 
        {
            var result = QName(prefix, name) + '(';
            var function = new XPathFunction(name, args);

            operandQueue.Enqueue(function);

            for (var i = 0; i < args.Count; i++)
            {
                if (i != 0)
                {
                    result += ',';
                }

                result += args[i];
            }

            result += ')';

            function.Text = result;

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

