// file:	DocumentManagementState.cs
//
// summary:	Implements the document management state class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX
{
    /// <summary>   Values that represent document management states. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 3/11/2021. </remarks>

    public enum DocumentManagementState
    {
        /// <summary>   An enum constant representing the dormant option. </summary>
        UserActive,
        /// <summary>   An enum constant representing the initialiting from state option. </summary>
        InitializingFromState,
        /// <summary>   An enum constant representing the saving details option. </summary>
        SavingDetails,
    }
}
