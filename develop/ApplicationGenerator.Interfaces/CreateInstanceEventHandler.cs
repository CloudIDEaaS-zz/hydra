// file:	CreateInstanceEventHandler.cs
//
// summary:	Implements the create instance event handler class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX
{
    /// <summary>   Delegate for handling CreateInstance events. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 12/29/2020. </remarks>
    ///
    /// <param name="sender">   Source of the event. </param>
    /// <param name="e">        Create instance event information. </param>

    public delegate void CreateInstanceEventHandler(object sender, CreateInstanceEventArgs e);

    /// <summary>   Additional information for create instance events. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 12/29/2020. </remarks>

    public class CreateInstanceEventArgs
    {
        /// <summary>   Gets the name of the type. </summary>
        ///
        /// <value> The name of the type. </value>

        public Type Type { get; }

        /// <summary>   Gets or sets the instance. </summary>
        ///
        /// <value> The instance. </value>

        public object Instance { get; set; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/29/2020. </remarks>
        ///
        /// <param name="type"> The name of the type. </param>

        public CreateInstanceEventArgs(Type type)
        {
            this.Type = type;
        }
    }
}
