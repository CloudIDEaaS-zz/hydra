// file:	IDataAnnotationTypeHandler.cs
//
// summary:	Declares the IDataAnnotationTypeHandler interface

using AbstraX.DataAnnotations;
using AbstraX.ServerInterfaces;
using AbstraX.TemplateObjects;
using CodeInterfaces;
using EntityProvider.Web.Entities;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace AbstraX
{ 
    /// <summary>   Interface for data annotation type handler. </summary>
    ///
    /// <remarks>   Ken, 10/5/2020. </remarks>

    public interface IDataAnnotationTypeHandler : IHandler
    {
        /// <summary>   Determine if we can handle. </summary>
        ///
        /// <param name="propertyName"> Name of the property. </param>
        /// <param name="type">         The type. </param>
        ///
        /// <returns>   True if we can handle, false if not. </returns>

        bool CanHandle(string propertyName, Type type);

        /// <summary>   Process this. </summary>
        ///
        /// <param name="entityObject">             The entity object. </param>
        /// <param name="entityPropertyItem">       The entity property item. </param>
        /// <param name="annotationType">           Type of the annotation. </param>
        /// <param name="typeBuilder">              The type builder. </param>
        /// <param name="appHierarchyNodeObject">   The application hierarchy node object. </param>
        /// <param name="generatorConfiguration">   The generator configuration. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        bool Process(EntityObject entityObject, EntityPropertyItem entityPropertyItem, Type annotationType, TypeBuilder typeBuilder, UIHierarchyNodeObject appHierarchyNodeObject, IGeneratorConfiguration generatorConfiguration);

        /// <summary>   Process this.  </summary>
        ///
        /// <param name="entityObject">             The entity object. </param>
        /// <param name="attributeObject">          The attribute object. </param>
        /// <param name="entityPropertyItem">       The entity property item. </param>
        /// <param name="annotationType">           Type of the annotation. </param>
        /// <param name="propertyBuilder">          The property builder. </param>
        /// <param name="appHierarchyNodeObject">   The application hierarchy node object. </param>
        /// <param name="generatorConfiguration">   The generator configuration. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        bool Process(EntityObject entityObject, AttributeObject attributeObject, EntityPropertyItem entityPropertyItem, Type annotationType, PropertyBuilder propertyBuilder, UIHierarchyNodeObject appHierarchyNodeObject, IGeneratorConfiguration generatorConfiguration);
    }
}
