// file:	GeneratorMode.cs
//
// summary:	Implements the generator mode class

using System;
using System.Collections.Generic;
using System.Text;

namespace AbstraX
{
    /// <summary>   Values that represent generator modes. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 11/15/2020. </remarks>

    public enum GeneratorMode
    {
        /// <summary>   An enum constant representing the console option. </summary>
        Console,
        /// <summary>   An enum constant representing the redirected console option. </summary>
        RedirectedConsole,
        /// <summary>   An enum constant representing the HTTP server option. </summary>
        HttpServer
    }
}
