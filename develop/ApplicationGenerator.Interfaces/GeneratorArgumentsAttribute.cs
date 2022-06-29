// file:	GeneratorArgumentsKind.cs
//
// summary:	Implements the generator arguments kind class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX
{
    /// <summary>   A generator arguments kind. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 1/1/2021. </remarks>

    public class GeneratorArgumentsAttribute : Attribute
    {
        /// <summary>   Gets the group. </summary>
        ///
        /// <value> The group. </value>

        public string Group { get; }

        /// <summary>   Gets the description. </summary>
        ///
        /// <value> The description. </value>

        public string Description { get; }

        /// <summary>   Gets the generator kinds. </summary>
        ///
        /// <value> The generator kinds. </value>

        public string GeneratorKinds { get; }

        /// <summary>   Gets or sets the validators. </summary>
        ///
        /// <value> The validators. </value>

        public Type[] Validators { get; set; }

        /// <summary>   Gets or sets a value indicating whether this  is favorite. </summary>
        ///
        /// <value> True if this  is favorite, false if not. </value>

        public bool IsFavorite { get; set; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/9/2021. </remarks>
        ///
        /// <param name="group">            The group. </param>
        /// <param name="description">      The description. </param>
        /// <param name="generatorKinds">   A variable-length parameters list containing generator kinds. </param>

        public GeneratorArgumentsAttribute(string group, string description, params string[] generatorKinds)
        {
            this.Group = group;
            this.Description = description;
            this.GeneratorKinds = generatorKinds.ToCommaDelimitedList();
        }
    }
}
