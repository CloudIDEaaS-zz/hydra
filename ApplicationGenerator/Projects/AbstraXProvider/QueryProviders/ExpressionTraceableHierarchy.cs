using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if SILVERLIGHT
using AbstraX.ClientInterfaces;
#else
using AbstraX.ServerInterfaces;
using AbstraX.QueryCache;
#endif
using AbstraX.Expressions;
using System.Reflection;
using System.Diagnostics;
using AbstraX.XPathBuilder;

namespace AbstraX.QueryProviders
{
    /// <summary>
    ///  Traced based on LINQ lambda expressions
    /// </summary>
    public abstract class ExpressionTraceableHierarchy<T> : PathTraceableHierarchy<T> where T : IBase
    {
        protected string parentID;
        protected int traversalDepthLimit;

        protected abstract IRoot Root { get; }

        public ExpressionTraceableHierarchy()
        {
        }

        public ExpressionTraceableHierarchy(string parentID)
        {
            this.parentID = parentID;
        }

        protected override IQueryable<T> QueryResults
        {
            get 
            {
                if (parentID == null)
                {
#if !SILVERLIGHT
                    bool doStandardTraversal;
                    IEnumerable<T> elements;
                    int traversalDepthLimit = -1;

                    if (this.Root.GetItemsOfType<T>(out doStandardTraversal, out traversalDepthLimit, out elements))
                    {
                        if (doStandardTraversal)
                        {
                            this.traversalDepthLimit = traversalDepthLimit;

                            return this.OfType<T>().AsQueryable();
                        }
                        else
                        {
                            return elements.AsQueryable();   
                        }
                    }
#endif
                }

                return null;
            }
        }

        public override IQueryable<TElement> CreateQuery<TElement>(System.Linq.Expressions.Expression expression)
        {
            lock (lockObject)
            {
                try
                {
                    var existing = (IQueryable)null;
                    var expressionString = expression.ToString();
                    var root = this.Root;

                    // CachableQueryProvider.Log.InfoFormat("Looking for the following query in cache: '{0}' ", expressionString);

                    if (EnableCaching && this.InCache(expressionString, out existing))
                    {
                        CachableQueryProvider.Log.InfoFormat("Found the following query in cache with {0} records: '{1}'", existing.Cast<object>().Count(), expressionString);

                        return (IQueryable<TElement>)existing;
                    }
                    else
                    {
                        var visitor = new AbstraXVisitor(expression);

                        // CachableQueryProvider.Log.InfoFormat("The following query not found in cache and processing as normal: '{0}'", expressionString);

                        if (visitor.BaseNode is MethodNode)
                        {
                            MethodNode methodNode = (MethodNode)visitor.BaseNode;

                            switch (methodNode.Method.Name)
                            {
                                case "OfType":
                                    {
                                        Action<IBase, int> recurseAdd = null;
                                        List<TElement> list = new List<TElement>();

                                        recurseAdd = (item, depth) =>
                                        {
                                            var itemType = item.GetType();

                                            if (item is T)
                                            {
                                                list.Add((TElement) item);
                                            }

                                            if (depth < traversalDepthLimit)
                                            {
                                                if (item is IRoot)
                                                {
                                                    ((IRoot)item).RootElements.ToList().ForEach(e => recurseAdd(e, ++depth));
                                                }
                                                else if (item is IParentBase)
                                                {
                                                    ((IParentBase)item).ChildElements.ToList().ForEach(e => recurseAdd(e, ++depth));

                                                    if (item is IElement)
                                                    {
                                                        ((IElement)item).Attributes.ToList().ForEach(a => recurseAdd(a, ++depth));
                                                        ((IElement)item).Operations.ToList().ForEach(o => recurseAdd(o, ++depth));
                                                    }
                                                }
                                            }
                                        };

                                        recurseAdd(root, 1);

                                        return list.AsQueryable();
                                    }
                                case "Where":

                                    WriteHistory(1, "Handling where clause");

                                    var listItem = (ListItemNode)methodNode.Parameters.ElementAt(1).Value;

                                    if (listItem.Value is BinaryExpressionNode)
                                    {
                                        var node = (BinaryExpressionNode)listItem.Value;
                                        var listItemLeft = (ListItemNode)node.Left;
                                        var listItemRight = (ListItemNode)node.Right;

                                        WriteHistory(2, "Handling binary expression");

                                        if (listItemLeft.Value is MemberNode)
                                        {
                                            var memberNode = (MemberNode)listItemLeft.Value;
                                            var loop = 0;

                                            WriteHistory(3, "Handling member node left");

                                            if (listItemRight.Value is PrimitiveNode)
                                            {
                                                IQueryable queryable = null;
                                                var primitiveNode = (PrimitiveNode)listItemRight.Value;
                                                string value = null;
                                                string filterField = null;
                                                var filterParent = false;

                                                WriteHistory(4, "Handling primitive node right");

                                                if (primitiveNode.Value.GetType() == typeof(string))
                                                {
                                                    value = (string)primitiveNode.Value;
                                                }
                                                else
                                                {
                                                    var field = primitiveNode.Value.GetType().GetFields().Where(f => f.FieldType.Name == "String").First();

                                                    filterField = field.Name;
                                                    value = (string)field.GetValue(primitiveNode.Value);
                                                }

                                                root.ClearPredicates();

                                                Indent = 4;
                                                WhereClause = value;

                                                var whereClauses = ProcessWhere(value);

                                                WhereProcessed = whereClauses;

                                                var where = whereClauses.Dequeue();
                                                var indent = 5;

                                                WriteHistory(indent, "Executing global where '{0}'", where.DebugInfo);

                                                root.ExecuteGlobalWhere(where);

                                                if (whereClauses.Count > 0)
                                                {
                                                    if (filterField == root.ParentFieldName)
                                                    {
                                                        filterParent = true;

                                                        WriteHistory(indent, "Filtering on parent");
                                                    }
                                                    else
                                                    {
                                                        loop = 1;

                                                        WriteHistory(indent, "Filtering on specific field");
                                                    }

                                                    where = whereClauses.Dequeue();

                                                    WriteHistory(indent, "Executing where - '{0}', on IRoot", where.DebugInfo);

                                                    root.ExecuteWhere(where);
                                                }

                                                var elements = root.RootElements.AsQueryable();
                                                Type type = null;

                                                if (listItemLeft.Value is MemberNode)
                                                {
                                                    type = ((MemberNode)listItemLeft.Value).Member.DeclaringType;
                                                }

                                                queryable = elements;

                                                while (whereClauses.Count > loop)
                                                {
                                                    where = whereClauses.Dequeue();

                                                    if (queryable.Cast<IBase>().Any(b => b is IElement))
                                                    {
                                                        var elementQueryable = queryable.Cast<IElement>();
                                                        var element = elementQueryable.Single();

                                                        WriteHistory(indent, "Executing where - '{0}', on IElement", where.DebugInfo);

                                                        element.ClearPredicates();
                                                        queryable = element.ExecuteWhere(memberNode.Member.Name, where);
                                                    }
                                                    else if (queryable.Cast<IBase>().Any(b => b is IOperation))
                                                    {
                                                        var operationQueryable = queryable.Cast<IOperation>();
                                                        var operation = operationQueryable.Single();

                                                        WriteHistory(indent, "Executing where - '{0}', on IOperation", where.DebugInfo);

                                                        operation.ClearPredicates();
                                                        queryable = operation.ExecuteWhere(memberNode.Member.Name, where);
                                                    }
                                                    else
                                                    {
                                                        Debugger.Break();
                                                    }
                                                }

                                                if (filterParent)
                                                {
                                                    if (typeof(T) == typeof(IElement) || typeof(T).GetInterfaces().Cast<Type>().Any(t => t.Name == "IElement"))
                                                    {
                                                        if (queryable.Cast<IBase>().Any(b => b is IElement))
                                                        {
                                                            var element = queryable.Cast<IElement>().Single();

                                                            queryable = element.ChildElements.AsQueryable();
                                                        }
                                                        else if (queryable.Cast<IBase>().Any(b => b is IOperation))
                                                        {
                                                            var operation = queryable.Cast<IOperation>().Single();

                                                            queryable = operation.ChildElements.AsQueryable();
                                                        }
                                                        else
                                                        {
                                                            Debugger.Break();
                                                        }
                                                    }
                                                    else if (typeof(T) == typeof(IAttribute) || typeof(T).GetInterfaces().Cast<Type>().Any(t => t.Name == "IAttribute"))
                                                    {
                                                        var element = queryable.Cast<IElement>().Single();

                                                        queryable = element.Attributes.AsQueryable();
                                                    }
                                                    else if (typeof(T) == typeof(IOperation) || typeof(T).GetInterfaces().Cast<Type>().Any(t => t.Name == "IOperation"))
                                                    {
                                                        var element = queryable.Cast<IElement>().Single();

                                                        queryable = element.Operations.AsQueryable();
                                                    }
                                                }
                                                else if (loop == 1)
                                                {
                                                    if (whereClauses.Count > 0)
                                                    {
                                                        where = whereClauses.Dequeue();

                                                        if (typeof(T) == typeof(IElement) || typeof(T).GetInterfaces().Cast<Type>().Any(t => t.Name == "IElement"))
                                                        {
                                                            if (queryable.Cast<IBase>().Any(b => b is IElement))
                                                            {
                                                                var element = queryable.Cast<IElement>().Single();

                                                                WriteHistory(indent, "Executing where - '{0}', on IElement", where.DebugInfo);

                                                                element.ClearPredicates();
                                                                element.ExecuteWhere(where);

                                                                queryable = element.ChildElements.AsQueryable();
                                                            }
                                                            else if (queryable.Cast<IBase>().Any(b => b is IOperation))
                                                            {
                                                                var operation = queryable.Cast<IOperation>().Single();

                                                                WriteHistory(indent, "Executing where - '{0}', on IOperation", where.DebugInfo);

                                                                operation.ClearPredicates();
                                                                operation.ExecuteWhere(where);

                                                                queryable = operation.ChildElements.AsQueryable();
                                                            }
                                                            else
                                                            {
                                                                Debugger.Break();
                                                            }
                                                        }
                                                        else if (typeof(T) == typeof(IAttribute) || typeof(T).GetInterfaces().Cast<Type>().Any(t => t.Name == "IAttribute"))
                                                        {
                                                            var element = queryable.Cast<IElement>().Single();

                                                            WriteHistory(indent, "Executing where - '{0}', on IAttribute", where.DebugInfo);

                                                            element.ClearPredicates();
                                                            element.ExecuteWhere(where);

                                                            queryable = element.Attributes.AsQueryable();
                                                        }
                                                        else if (typeof(T) == typeof(IOperation) || typeof(T).GetInterfaces().Cast<Type>().Any(t => t.Name == "IOperation"))
                                                        {
                                                            var element = queryable.Cast<IElement>().Single();

                                                            WriteHistory(indent, "Executing where - '{0}', on IOperation", where.DebugInfo);

                                                            element.ClearPredicates();
                                                            element.ExecuteWhere(where);

                                                            queryable = element.Operations.AsQueryable();
                                                        }
                                                    }
                                                }

                                                if (type != null)
                                                {
                                                    var returnResults = queryable.Cast<IBase>().Where(e => AncestorTypes(e.GetType()).Any(t => t == type)).Cast<TElement>();

                                                    return returnResults;
                                                }
                                                else
                                                {
                                                    var returnResults = queryable.Cast<TElement>();

                                                    return returnResults;
                                                }
                                            }
                                        }
                                    }

                                    break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    HistoryItem.Exception = ex;

                    var output = HistoryItem.QueryProcessingOutput;

                    Debugger.Break();
                }
            }

            throw new NotImplementedException();
        }

        private List<Type> AncestorTypes(Type type)
        {
            List<Type> ancestors = new List<Type>();
            Type baseType = type;

            while (baseType != null)
            {
                ancestors.Add(baseType);

                foreach (Type interfaceType in baseType.GetInterfaces())
                {
                    ancestors.Add(interfaceType);
                }

                baseType = baseType.BaseType;
            }

            return ancestors;
        }
        
        public override IQueryable CreateQuery(System.Linq.Expressions.Expression expression)
        {
            return this.CreateQuery<IBase>(expression);
        }

        public override TResult Execute<TResult>(System.Linq.Expressions.Expression expression)
        {
            var visitor = new AbstraXVisitor(expression);

            throw new NotImplementedException();
        }

        public override object Execute(System.Linq.Expressions.Expression expression)
        {
            var visitor = new AbstraXVisitor(expression);

            throw new NotImplementedException();
        }
    }
}
