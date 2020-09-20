using AbstraX.TsvObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX
{
    public class BusinessModel : TsvParserHost<BusinessModelRecord>
    {
        public List<BusinessModelRecord> Records { get; private set; }

        public override List<BusinessModelRecord> ParseFile(string path)
        {
            Records = base.ParseFile(path);

            return Records;
        }
    }
}
