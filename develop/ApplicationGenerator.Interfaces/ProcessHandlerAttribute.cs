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
    /// <summary>   Values that represent process handler kinds. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 4/5/2021. </remarks>

    public enum ProcessHandlerKind
    {
        /// <summary>   An enum constant representing the orphaned processes option. </summary>
        RunningProcesses,
    }

    /// <summary>   Attribute for wizard page. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 3/22/2021. </remarks>

    public class ProcessHandlerAttribute : Attribute
    {
        /// <summary>   Gets the name. </summary>
        ///
        /// <value> The name. </value>

        public ProcessHandlerKind ProcessHandlerKind { get; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/22/2021. </remarks>
        ///
        /// <param name="kind"> The name. </param>

        public ProcessHandlerAttribute(ProcessHandlerKind kind)
        {
            this.ProcessHandlerKind = kind;
        }
    }
}
