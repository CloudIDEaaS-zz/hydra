using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using AbstraX.Contracts;
using AbstraX.ServerInterfaces;

namespace AbstraX.BindingsTreeEntities
{
    [DebuggerDisplay("{ DebugInfo }")]
    public abstract class BaseBindingsTreeNode
    {
        protected string id;
        protected string name;
        public abstract string ID { get; set; }
        public abstract string ParentID { get; }
        public abstract string Name { get; set; }
        public abstract string DebugInfo { get; }
        public abstract bool HasChildren { get; }
        public abstract float ChildOrdinal { get; }
        public abstract object InternalObject { get; }
        public BaseBindingsTreeNode ParentNode { get; set; }

        public BaseBindingsTreeNode()
        {
            id = Guid.NewGuid().ToString();
        }
    }
}
