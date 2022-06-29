// file:	Models\Interfaces\IEntityContainer.cs
//
// summary:	Declares the IEntityContainer interface

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AbstraX.ServerInterfaces;
using AbstraX.TypeMappings;
using AbstraX.XPathBuilder;

namespace AbstraX.Models.Interfaces
{
    /// <summary>   Interface for entity container. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 11/10/2020. </remarks>

    public interface IEntityContainer : IEntityObjectWithFacets, IElement
    {
        /// <summary>   Gets the sets the entity belongs to. </summary>
        ///
        /// <value> The entity sets. </value>

        List<IEntitySet> EntitySets { get; }

        /// <summary>   Gets a value indicating whether the prevent recursion. </summary>
        ///
        /// <value> True if prevent recursion, false if not. </value>

        bool PreventRecursion { get; }

        /// <summary>   Gets the namespace. </summary>
        ///
        /// <value> The namespace. </value>

        string Namespace { get; }
    }
}