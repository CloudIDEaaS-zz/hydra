using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApplicationGenerator.Client.NodeInfo
{
    public class SpinsterNodeBase : NodeInfoBase
    {
        public object ParentObject { get; private set; }

        public SpinsterNodeBase(object parentObject)
        {
            this.ParentObject = parentObject;
        }
    }
}
