// file:	IRouteGuard.cs
//
// summary:	Declares the IRouteGuard interface

using AbstraX.DataAnnotations;
using AbstraX.FolderStructure;
using AbstraX.ServerInterfaces;
using System.Collections.Generic;

namespace AbstraX
{
    /// <summary>   Interface for route guard. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 2/20/2021. </remarks>

    public interface IRouteGuard
    {
        /// <summary>   Gets all route guards. </summary>
        ///
        /// <value> all route guards. </value>

        Dictionary<string, List<IRouteGuard>> AllRouteGuards { get; }

        /// <summary>   Gets the kind. </summary>
        ///
        /// <value> The kind. </value>

        RouteGuardKind Kind { get; }
        /// <summary>   Gets the base object. </summary>
        ///
        /// <value> The base object. </value>

        IBase BaseObject { get; }

        /// <summary>   Gets the default route name. </summary>
        ///
        /// <value> The default route name. </value>

        string DefaultRouteName { get; }

        /// <summary>   Gets or sets the file. </summary>
        ///
        /// <value> The file. </value>

        File File { get; set; }

        /// <summary>   Gets a value indicating whether this  is home. </summary>
        ///
        /// <value> True if this  is home, false if not. </value>

        bool IsHome { get; }

        /// <summary>   Gets the name of the route. </summary>
        ///
        /// <value> The name of the route. </value>

        string RouteName { get; }

        /// <summary>   Gets the navigation name attribute. </summary>
        ///
        /// <value> The user interface navigation name attribute. </value>

        UINavigationNameAttribute UINavigationNameAttribute { get; }
    }
}