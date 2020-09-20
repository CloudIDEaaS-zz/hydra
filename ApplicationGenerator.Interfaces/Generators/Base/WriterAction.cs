using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX.Generators.Base
{
    [DebuggerDisplay(" { DebugInfo } ")]
    public class WriterAction
    {
        public ISemanticTreeBaseNode Node { get; }
        public Action Action { get; }
        public long Counter { get; }

        public WriterAction(ISemanticTreeBaseNode node, Action action)
        {
            this.Node = node;
            this.Action = action;
            this.Counter = DateTimeExtensions.HighResolutionPerformanceCount;
        }

        public string DebugInfo
        {
            get
            {
                return string.Format("Node: {0}, "
            	    + "Action: {1}, "
            	    + "Counter: {2}",
        		    this.Node,
        		    this.Action,
                    this.Counter
                );
            }
        }
    }
}
