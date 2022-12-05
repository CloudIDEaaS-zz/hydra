using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hydra.ReleaseManagement.Entities.Models.ValueTypes
{
    public class Folder
    {
        public string Name { get; set; }
        public List<string> Files { get; set; }

        public Folder(string name)
        {
            Name = name;
            this.Files = new List<string>();
        }
    }
}
