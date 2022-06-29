// file:	Generators\GeneratorTemplateAttribute.cs
//
// summary:	Implements the generator template attribute class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.Generators
{
    /// <summary>   Attribute for generator template. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 2/4/2021. </remarks>

    [AttributeUsage(AttributeTargets.Class, AllowMultiple=true)]
    public class GeneratorTemplateAttribute : Attribute
    {
        /// <summary>   Gets the name of the generator. </summary>
        ///
        /// <value> The name of the generator. </value>

        public string GeneratorIdentifier { get; }

        /// <summary>   Gets the full pathname of the attribute relative file. </summary>
        ///
        /// <value> The full pathname of the attribute relative file. </value>

        public string TemplateRelativePath { get; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 2/4/2021. </remarks>
        /// <param name="generatorIdentifier"></param>
        ///
        /// <param name="templateRelativePath">    Full pathname of the attribute relative file. </param>

        public GeneratorTemplateAttribute(string generatorIdentifier, string templateRelativePath)
        {
            this.GeneratorIdentifier = generatorIdentifier;
            this.TemplateRelativePath = templateRelativePath;
        }
    }
}
