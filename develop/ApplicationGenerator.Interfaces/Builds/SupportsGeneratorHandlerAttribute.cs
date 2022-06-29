using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.Builds
{
    /// <summary>   Attribute for supports generator handler. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 4/9/2021. </remarks>

    public class SupportsGeneratorHandlerAttribute : Attribute
    {
        /// <summary>   Gets the generator handler. </summary>
        ///
        /// <value> The generator handler. </value>

        public string GeneratorHandlerType { get; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 4/9/2021. </remarks>
        ///
        /// <param name="generatorHandlerType"> The generator handler. </param>
        ///

        public SupportsGeneratorHandlerAttribute(string generatorHandlerType)
        {
            this.GeneratorHandlerType = generatorHandlerType;
        }
    }
}
