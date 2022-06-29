using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using Utils;
using System.Collections;

namespace ApplicationGenerator.Client.NodeInfo
{
    public class SpinsterNode<TParent, TChild> : SpinsterNodeBase
    {
        public TParent Parent { get; private set; }
        private List<TChild> childData;

        public SpinsterNode(TParent parent, List<TChild> childData) : base(parent)
        {
            this.Parent = parent;
            this.childData = childData;
        }

        public IEnumerable<T> GetChildData<T>()
        {
            return this.childData.Cast<T>();
        }
    }
}
