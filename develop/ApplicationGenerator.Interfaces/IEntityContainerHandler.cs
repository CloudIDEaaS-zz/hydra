// file:	IEntityContainerHandler.cs
//
// summary:	Declares the IEntityContainerHandler interface

using AbstraX.TemplateObjects;
using CodeInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX
{
    /// <summary>   Interface for entity container handler. </summary>
    ///
    /// <remarks>   Ken, 10/8/2020. </remarks>

    public interface IEntityContainerHandler : IHandler
    {
        /// <summary>   Process this.  </summary>
        ///
        /// <param name="appUIHierarchyNodeObject"> The application user interface hierarchy node object. </param>
        /// <param name="projectType">              Type of the project. </param>
        /// <param name="projectFolderRoot">        The project folder root. </param>
        /// <param name="entitiesProject">          The entities project. </param>
        /// <param name="generatorConfiguration">   The generator configuration. </param>
        /// <param name="entityContainerObject"></param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        bool Process(AppUIHierarchyNodeObject appUIHierarchyNodeObject, Guid projectType, string projectFolderRoot, IVSProject entitiesProject, IGeneratorConfiguration generatorConfiguration, out EntityObject entityContainerObject);
    }
}
