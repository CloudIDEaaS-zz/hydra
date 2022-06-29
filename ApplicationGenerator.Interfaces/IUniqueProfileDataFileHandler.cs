// file:	IWorkspaceFileTypeHandler.cs
//
// summary:	Declares the IWorkspaceFileTypeHandler interface

using AbstraX.ServerInterfaces;
using CodeInterfaces;
using EntityProvider.Web.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AbstraX
{
    /// <summary>   Interface for workspace file type handler. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 3/24/2021. </remarks>

    public interface IUniqueProfileDataFileHandler : IHandler
    {
        /// <summary>   Gets the file name expressions. </summary>
        ///
        /// <value> The file name expressions. </value>
        bool Process(Guid projectType, IAppFolderStructureSurveyor appFolderStructureSurveyor, string organizationUniqueName, string appUniqueName, IGeneratorConfiguration generatorConfiguration);
    }
}
