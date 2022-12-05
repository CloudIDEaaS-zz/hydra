using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX.CommandHandlers
{
    /// <summary>   A grunt command handler. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 9/24/2022. </remarks>

    public class GruntCommandHandler : BaseWindowsCommandHandler
    {
        /// <summary>   Default constructor. </summary>
        /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>
        public GruntCommandHandler() : base("grunt")
        {
        }

        /// <summary>   Bumps this. </summary>
        /// <remarks>   CloudIDEaaS, 6/28/2022. </remarks>
        public void Bump()
        {
            base.RunCommand("bump", Environment.CurrentDirectory, Array.Empty<string>());
        }
    }
}
