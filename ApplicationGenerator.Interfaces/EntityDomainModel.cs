using AbstraX.TsvObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX
{
    public class EntityDomainModel : TsvParserHost<EntityDomainRecord>
    {
        public override List<EntityDomainRecord> ParseFile(string path)
        {
            var records = base.ParseFile(path);

            return records;
        }

        public void ParseJsonFile(string jsonFile)
        {
            throw new NotImplementedException();
        }
    }
}
