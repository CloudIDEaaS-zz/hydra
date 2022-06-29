// file:	IRouteGuard.cs
//
// summary:	Declares the IRouteGuard interface

using AbstraX.DataAnnotations;
using AbstraX.FolderStructure;
using AbstraX.ServerInterfaces;
using System.Collections.Generic;

namespace AbstraX
{
    /// <summary>   Values that represent route guard kinds. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 2/25/2021. </remarks>

    public enum RouteGuardKind
    {
        /// <summary>   An enum constant representing the can load option. </summary>
        CanLoad,
        /// <summary>   An enum constant representing the can activate option. </summary>
        CanActivate
    }
}