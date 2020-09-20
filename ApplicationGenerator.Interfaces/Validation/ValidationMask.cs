using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.Validation
{
    [DebuggerDisplay(" { DebugInfo } ")]
    public class ValidationMask
    {
        public string Name { get; }
        public string MaskExpression { get; }
        public string UnmaskExpression { get; }
        public float Priority { get; }

        public ValidationMask(string name, string maskExpression, string unmaskExpression, float priority = 0f)
        {
            this.Name = name;
            this.MaskExpression = maskExpression;
            this.UnmaskExpression = unmaskExpression;
            this.Priority = priority;
        }

        public string DebugInfo
        {
            get
            {
                return string.Format("{0}: '{1}'", this.Name, this.MaskExpression);
            }
        }
    }
}
