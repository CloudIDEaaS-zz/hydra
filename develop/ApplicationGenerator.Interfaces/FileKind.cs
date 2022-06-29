// file:	FileKind.cs
//
// summary:	Implements the file kind class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX
{
    /// <summary>   Values that represent file kinds. </summary>
    ///
    /// <remarks>   Ken, 10/11/2020. </remarks>

    public enum FileKind
    {
        /// <summary>   An enum constant representing the project option. </summary>
        Project = 1,
        /// <summary>   An enum constant representing the services option. </summary>
        Services = 2,
        /// <summary>   An enum constant representing the entities option. </summary>
        Entities = 3,
        /// <summary>   An enum constant representing the project no module option. </summary>
        ProjectNoModule = 4
    }
}
