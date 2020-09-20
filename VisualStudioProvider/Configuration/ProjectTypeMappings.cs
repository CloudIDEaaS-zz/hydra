using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VisualStudioProvider.Configuration
{
    // i'm sure this is a wrong way to go based on multi-language but here it is
    // couldn't find a mapping otherwise

    public static class ProjectTypeMappings
    {
        public static Dictionary<Guid, string> Mappings;

        static ProjectTypeMappings()
        {
            Mappings = new Dictionary<Guid, string>();

            Mappings.Add(new Guid("fae04ec1-301f-11d3-bf4b-00c04f79efbc"), "CSharp");
            Mappings.Add(new Guid("91a04a73-4f2c-4e7c-ad38-c1a68e7da05c"), "FSharp");
            Mappings.Add(new Guid("164b10b9-b200-11d0-8c61-00a0c91e29d5"), "VisualBasic");
        }
    }
}
