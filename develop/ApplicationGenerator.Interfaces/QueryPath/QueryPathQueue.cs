using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstraX.DataAnnotations;
using AbstraX.ServerInterfaces;
using Utils;
using AbstraX;
using AbstraX.XPathBuilder;
using EntityProvider.Web.Entities;
using System.Diagnostics;

namespace AbstraX.QueryPath
{
    public class QueryPathQueue
    {
        public QueryPathAttribute LoadParentPath { get; }
        public IBase BaseObject { get; }
        public QueryKind QueryPathKind { get; }

        public Queue<QueryPathQueueItem> Items;

        public QueryPathQueue(QueryPathAttribute queryPath, IBase baseObject)
        {
            var currentObject = baseObject;
            var expression = queryPath.XPathExpression;
            var partQueue = new Queue<IXPathPart>(queryPath.PartQueue);
            QueryPathQueueItem currentQueueItem = null;

            this.Items = new Queue<QueryPathQueueItem>();
            this.LoadParentPath = queryPath;
            this.BaseObject = baseObject;
            this.QueryPathKind = queryPath.QueryPathKind;

            while (partQueue.Count > 0)
            {
                var part = partQueue.Dequeue();

                if (part is XPathElement element)
                {
                    var parentObject = (IParentBase) currentObject;
                    var childObject = parentObject.ChildNodes.SingleOrDefault(n => n.Name == element.Text);

                    if (childObject == null)
                    {
                        if (parentObject is NavigationProperty)
                        {
                            childObject = parentObject.ChildNodes.Single();

                            if (this.QueryPathKind == QueryKind.LoadParentReference && partQueue.Count == 0)
                            {
                                currentQueueItem.AssumeSingleOperator = true;
                            }

                            currentQueueItem = new QueryPathQueueItem(childObject);
                            this.Items.Enqueue(currentQueueItem);

                            parentObject = (IParentBase) childObject;

                            childObject = parentObject.ChildNodes.SingleOrDefault(n => n.Name == element.Text);
                        }
                    }

                    currentQueueItem = new QueryPathQueueItem(childObject);
                    this.Items.Enqueue(currentQueueItem);

                    currentObject = childObject;
                    parentObject = (IParentBase)currentObject;

                    if (element.Predicates != null && element.Predicates.Count > 0)
                    {
                        QueryPathQueueItem dequeuedItem = null;

                        foreach (var predicate in element.Predicates)
                        {
                            var left = predicate.Left;
                            var op = predicate.Operator;
                            IQueryPathOperand queueLeft = null;
                            IQueryPathOperand queueRight = null;
                            QueryPathQueuePredicate queuePredicate;

                            if (left is XPathAttribute xpathAttribute)
                            {
                                var parentSetElement = (IElement)parentObject.ChildNodes.Single();
                                var attribute = (IAttribute) parentSetElement.Attributes.Single(n => n.Name == xpathAttribute.Name);
                                var queueAttribute = new QueryPathQueueAttribute(attribute);
                                var nextPart = partQueue.Peek();

                                if (nextPart is XPathElement xpathElement)
                                {
                                    if (xpathElement.Text == parentSetElement.Name)
                                    {
                                        part = partQueue.Dequeue();
                                        dequeuedItem = new QueryPathQueueItem(parentSetElement);
                                    }

                                    currentObject = parentSetElement;
                                }

                                queueLeft = queueAttribute;
                            }
                            else
                            {
                                DebugUtils.Break();
                            }

                            if (op != CodePlex.XPathParser.XPathOperator.Unknown)
                            {
                                var right = predicate.Right;

                                if (right is XPathFunction pathFunction)
                                {
                                    var kind = EnumUtils.GetValue<QueryPathFunctionKind>(pathFunction.Name);
                                    var args = pathFunction.Args;
                                    var queueFunction = new QueryPathQueueFunction(kind, args);

                                    queueRight = queueFunction;
                                }
                                else
                                {
                                    DebugUtils.Break();
                                }
                            }

                            queuePredicate = new QueryPathQueuePredicate(queueLeft, op, queueRight);
                            currentQueueItem.Predicates.Add(queuePredicate);

                            if (dequeuedItem != null)
                            {
                                currentQueueItem = dequeuedItem;
                                this.Items.Enqueue(currentQueueItem);
                            }
                        }
                    }
                }
                else if (part is XPathAxis pathAxis)
                {
                    if (pathAxis.Axis == CodePlex.XPathParser.XPathAxis.Root)
                    {
                        currentObject = baseObject.GetContainer();
                        var queueItem = new QueryPathQueueItem(currentObject);

                        this.Items.Enqueue(queueItem);

                        currentQueueItem = queueItem;
                    }
                    else
                    {
                        DebugUtils.Break();
                    }
                }
                else
                {
                    DebugUtils.Break();
                }
            }
        }

        public string QueryCode
        {
            get
            {
                var builder = new StringBuilder();
                var queue = new Queue<QueryPathQueueItem>(this.Items);
                QueryPathQueueItem item;

                item = queue.Peek();

                if (item.BaseObject is Entity_Container)
                {
                    queue.Dequeue();

                    builder.Append("{ containerVariable }");
                }

                while (queue.Count > 0)
                {
                    item = queue.Dequeue();

                    builder.Append("." + item.BaseObject.Name);

                    foreach (var predicate in item.Predicates)
                    {
                        var leftOperand = predicate.Left;
                        var rightOperand = predicate.Right;
                        var conditionCode = new StringBuilder();
                        var cardinality = QueryExpectedCardinality.Unknown;

                        if (leftOperand is QueryPathQueueFunction pathQueueFunctionLeft)
                        {
                            var codeExpression = pathQueueFunctionLeft.FunctionKind.GetFunctionCodeExpression();

                            cardinality = pathQueueFunctionLeft.FunctionKind.GetExpectedCardinality();

                            conditionCode.Append(codeExpression);
                        }
                        else if (leftOperand is QueryPathQueueAttribute pathQueueAttribute)
                        {
                            conditionCode.Append(pathQueueAttribute.Attribute.Name);
                        }

                        if (predicate.Operator == CodePlex.XPathParser.XPathOperator.Unknown)
                        {
                            builder.AppendFormat(".{0}", conditionCode.ToString());

                            if (cardinality == QueryExpectedCardinality.Single)
                            {
                                queue.Dequeue();
                            }
                        }
                        else
                        {
                            conditionCode.AppendFormat(" {0} ", predicate.Operator.GetText());

                            if (rightOperand is QueryPathQueueFunction pathQueueFunctionRight)
                            {
                                var codeExpression = pathQueueFunctionRight.FunctionKind.GetFunctionCodeExpression();

                                cardinality = pathQueueFunctionRight.FunctionKind.GetExpectedCardinality();

                                conditionCode.Append(codeExpression);

                                if (cardinality == QueryExpectedCardinality.Single)
                                {
                                    builder.AppendFormat(".Single({0} => {0}.{1})", item.BaseObject.Name.First().ToString().LowerCase(), conditionCode.ToString());
                                    queue.Dequeue();
                                }
                            }
                            else
                            {
                                builder.Append("." + rightOperand.ToString());
                            }
                        }
                    }
                }

                return builder.ToString();
            }
        }

        public string SuggestedControllerMethodName
        {
            get
            {
                string getElement = null;
                string forElement = null;
                var forPrefix = string.Empty;
                var queue = new Queue<QueryPathQueueItem>(this.Items);
                QueryPathQueueItem item;

                item = queue.Peek();

                if (item.BaseObject is Entity_Container)
                {
                    var last = queue.Last();

                    queue.Dequeue();
                    item = queue.Dequeue();

                    foreach (var predicate in item.Predicates)
                    {
                        var operand = predicate.Operands.Last();

                        if (operand is QueryPathQueueFunction pathQueueFunction)
                        {
                            if (pathQueueFunction.FunctionKind == QueryPathFunctionKind.identity_name)
                            {
                                forPrefix = "Current";
                            }
                        }
                    }

                    item = queue.Peek();

                    forElement = item.BaseObject.Name;
                    getElement = last.BaseObject.Name;
                }

                return $"Get{ getElement }For{ forPrefix }{ forElement}";
            }
        }
    }
}
