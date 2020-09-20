using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.Generators.Base
{
    public class NullWriterTemplateBase : DelayedWriterTemplateBase
    {
        public override string TransformText()
        {
            throw new NotImplementedException();
        }
    }
}
