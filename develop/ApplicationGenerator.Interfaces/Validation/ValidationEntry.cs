using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.Validation
{
    [DebuggerDisplay(" { DebugInfo } ")]
    public class ValidationEntry
    {
        public string Name { get;  }
        public string FunctionExpression { get; }
        public string ErrorMessageTranslationKey { get; }
        public string FunctionCode { get; }
        public object ValidTestValue { get; }
        public object InvalidTestValue { get; }
        public bool IsForm { get;  }

        public ValidationEntry(string name, string functionExpression, string errorMessageKey, object validTestValue = null, object invalidTestValue = null, bool isForm = false, string functionCode = null)
        {
            this.Name = name;
            this.FunctionExpression = functionExpression;
            this.ErrorMessageTranslationKey = errorMessageKey;
            this.ValidTestValue = validTestValue;
            this.InvalidTestValue = invalidTestValue;
            this.IsForm = isForm;
            this.FunctionCode = functionCode;
        }

        public string DebugInfo
        {
            get
            {
                return string.Format("{0}: {1}", this.Name, this.ErrorMessageTranslationKey);
            }
        }
    }
}
