// file:	PartsAliasResolver.cs
//
// summary:	Implements the parts alias resolver class

using AbstraX.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX
{
    /// <summary>   The parts alias resolver. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 12/20/2020. </remarks>

    public class PartsAliasResolver
    {
        /// <summary>   The aliases. </summary>
        private Dictionary<string, string> aliases;

        /// <summary>   Default constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/20/2020. </remarks>

        public PartsAliasResolver()
        {
            this.aliases = new Dictionary<string, string>();
        }

        /// <summary>   Adds componentAttribute. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/20/2020. </remarks>
        ///
        /// <param name="componentAttribute">   The component attribute to add. </param>

        public void Add(UIAttribute componentAttribute)
        {
            var alias = componentAttribute.PathRootAlias;
            var path = componentAttribute.UIHierarchyPath;

            path = this.Resolve(path);

            this.aliases.Add(alias, path);
        }

        /// <summary>   Gets the number of.  </summary>
        ///
        /// <value> The count. </value>

        public int Count
        {
            get
            {
                return aliases.Count;
            }
        }

        /// <summary>   Resolves the given file. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/20/2020. </remarks>
        ///
        /// <param name="path"> Full pathname of the file. </param>
        ///
        /// <returns>   A string. </returns>

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
