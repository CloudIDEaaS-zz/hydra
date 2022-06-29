// file:	TestMode.cs
//
// summary:	Implements the test mode class

using System;
using System.Collections.Generic;
using System.Text;

namespace AbstraX
{
    /// <summary>   A bit-field of flags for specifying test modes. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 11/15/2020. </remarks>

    [Flags]
    public enum TestMode
    {
        /// <summary>   A binary constant representing the not set flag. </summary>
        NotSet,
        /// <summary>   A binary constant representing the test process iterate only flag. </summary>
        TestProcessIterateOnly
    }
}
