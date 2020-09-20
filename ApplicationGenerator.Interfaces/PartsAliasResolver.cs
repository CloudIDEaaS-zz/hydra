using AbstraX.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX
{
    public class PartsAliasResolver
    {
        private Dictionary<string, string> aliases;

        public PartsAliasResolver()
        {
            this.aliases = new Dictionary<string, string>();
        }

        public void Add(UIAttribute componentAttribute)
        {
            var alias = componentAttribute.PathRootAlias;
            var path = componentAttribute.UIHierarchyPath;

            path = this.Resolve(path);

            this.aliases.Add(alias, path);
        }

        public int Count
        {
            get
            {
                return aliases.Count;
            }
        }

        public string Resolve(string path)
        {
            foreach (var pair in aliases)
            {
                if (path.StartsWith(pair.Key))
                {
                    path = path.RegexReplace("^" + pair.Key, pair.Value);

                    return path;
                }
            }

            return path;
        }
    }
}
