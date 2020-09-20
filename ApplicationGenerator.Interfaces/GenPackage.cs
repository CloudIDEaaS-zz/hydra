using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX
{
    [Serializable]
    public class GenPackage
    {
        public string[] dependencies { get; set; }
        public string[] devDependencies { get; set; }
    }
}
