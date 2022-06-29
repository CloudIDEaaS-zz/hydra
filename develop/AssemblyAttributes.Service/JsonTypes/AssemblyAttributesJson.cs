using System;
using System.Collections.Generic;
using System.Text;

namespace AssemblyAttributesService.JsonTypes
{
    public class AssemblyAttributesJson
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Product { get; set; }
        public string Company { get; set; }
        public string Copyright { get; set; }
        public string Version { get; set; }
        public string Hash { get; set; }
        public string VersionVariant { get; set; }
    }
}
