// file:	IDynamicModuleBuilder.cs
//
// summary:	Declares the IDynamicModuleBuilder interface

using AbstraX.ServerInterfaces;
using AbstraX.TemplateObjects;
using CodeInterfaces;
using EntityProvider.Web.Entities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace AbstraX
{
    /// <summary>   Interface for dynamic module builder. </summary>
    ///
    /// <remarks>   Ken, 10/11/2020. </remarks>

    public interface IDynamicModuleBuilder : IDisposable
    {
        /// <summary>   Gets the priority. </summary>
        ///
        /// <value> The priority. </value>

        float Priority { get; }

        /// <summary>   Creates memory module builder. </summary>
        ///
        /// <param name="project">  The project. </param>
        ///
        /// <returns>   The new memory module builder. </returns>

        ModuleBuilder CreateDynamicModuleBuilder(IVSProject project);
        /// <summary>   Loads the assembly. </summary>
        Assembly LoadAndAttachToAssembly(AppUIHierarchyNodeObject appUIHierarchyNodeObject);
    }
}
