using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX
{
    public class HandlerNotFoundException : Exception
    {
        public HandlerNotFoundException(string supportingWhat) : base(string.Format("Handler not found supporting {0}", supportingWhat))
        {
        }
    }
}
