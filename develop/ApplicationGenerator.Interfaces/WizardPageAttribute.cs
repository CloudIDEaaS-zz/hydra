// file:	WizardPageAttribute.cs
//
// summary:	Implements the wizard page attribute class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX
{
    /// <summary>   Attribute for wizard page. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 3/22/2021. </remarks>

    public class WizardPageAttribute : Attribute
    {
        /// <summary>   Gets the name. </summary>
        ///
        /// <value> The name. </value>

        public string Name { get; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/22/2021. </remarks>
        ///
        /// <param name="name"> The name. </param>

        public WizardPageAttribute(string name)
        {
            this.Name = name;
        }
    }
}
