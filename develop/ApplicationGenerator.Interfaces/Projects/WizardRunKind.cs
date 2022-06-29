// file:	ProjectItemTemplates\WizardRunKind.cs
//
// summary:	Implements the wizard run kind class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.Projects
{
    /// <summary>   Values that represent wizard run kinds. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 3/10/2022. </remarks>

    public enum WizardRunKind
    {
        //A new multi-project template.
        /// <summary>   An enum constant representing as multi project option. </summary>
        AsMultiProject = 3,
        //A new item template.
        /// <summary>   An enum constant representing as new item option. </summary>
        AsNewItem = 1,
        //A new project template.
        /// <summary>   An enum constant representing as new project option. </summary>
        AsNewProject = 2
    }
}
