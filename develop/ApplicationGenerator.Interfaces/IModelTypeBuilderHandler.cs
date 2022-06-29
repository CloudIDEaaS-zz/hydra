// file:	IModelTypeBuilderHandler.cs
//
// summary:	Declares the IModelTypeBuilderHandler interface

using AbstraX.TemplateObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX
{
    /// <summary>   Interface for model type builder handler. </summary>
    ///
    /// <remarks>   Ken, 10/12/2020. </remarks>

    public interface IModelTypeBuilderHandler : IHandler
    {
        /// <summary>   Creates type for entity. </summary>
        ///
        /// <param name="moduleBuilder">            The module builder. </param>
        /// <param name="entity">                   The entity. </param>
        /// <param name="appUIHierarchyNodeObject"> The application hierarchy node object. </param>

        void CreateTypeForEntity(ModuleBuilder moduleBuilder, EntityObject entity, AppUIHierarchyNodeObject appUIHierarchyNodeObject, IGeneratorConfiguration generatorConfiguration);

        /// <summary>   Creates type builder for entity. </summary>
        ///
        /// <param name="moduleBuilder">            The module builder. </param>
        /// <param name="entity">                   The entity. </param>
        /// <param name="appUIHierarchyNodeObject"> The application hierarchy node object. </param>

        void CreateTypeBuilderForEntity(ModuleBuilder moduleBuilder, EntityObject entity, AppUIHierarchyNodeObject appUIHierarchyNodeObject, IGeneratorConfiguration generatorConfiguration);
    }
}
