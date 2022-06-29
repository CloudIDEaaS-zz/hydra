// file:	GetAddInEntitiesEventHandler.cs
//
// summary:	Implements the get add in entities event handler class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstraX.ServerInterfaces;
using EntityProvider.Web.Entities;

namespace AbstraX
{
    /// <summary>   Delegate for handling GetAddInEntities events. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 11/17/2020. </remarks>
    ///
    /// <param name="sender">   Source of the event. </param>
    /// <param name="e">        Get add in entities event information. </param>

    public delegate void GetAddInEntitiesEventHandler(object sender, GetAddInEntitiesEventArgs e);

    /// <summary>   Additional information for get add in entities events. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 11/17/2020. </remarks>

    public class GetAddInEntitiesEventArgs : EventArgs
    {
        /// <summary>   Gets or sets the entities. </summary>
        ///
        /// <value> The entities. </value>

        public List<IBase> Entities { get; set; }

        /// <summary>   Gets the entity factory. </summary>
        ///
        /// <value> The entity factory. </value>

        public EntityFactory EntityFactory { get; }

        /// <summary>   Gets the name of the property. </summary>
        ///
        /// <value> The name of the property. </value>

        public string PropertyName { get; }

        /// <summary>   Gets the base object. </summary>
        ///
        /// <value> The base object. </value>

        public IBase BaseObject { get;  }

        /// <summary>   Gets the definition kind. </summary>
        ///
        /// <value> The definition kind. </value>

        public DefinitionKind DefinitionKind { get; }

        /// <summary>   Gets the type. </summary>
        ///
        /// <value> The type. </value>

        public BaseType Type { get; }

        /// <summary>   Gets or sets a value indicating whether the no metadata. </summary>
        ///
        /// <value> True if no metadata, false if not. </value>

        public bool NoMetadata { get; set; }

        /// <summary>   Gets or sets the get current children. </summary>
        ///
        /// <value> A function delegate that yields an IEnumerable&lt;IBase&gt; </value>

        public Func<IEnumerable<IBase>> GetCurrentChildren { get; set; }

        /// <summary>   Gets the entities in this collection. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/17/2020. </remarks>
        ///
        /// <typeparam name="T">    Generic type parameter. </typeparam>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the entities in this collection.
        /// </returns>

        public IEnumerable<T> GetEntities<T>() where T : IBase
        {
            return this.Entities.Cast<T>();
        }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/17/2020. </remarks>
        ///
        /// <param name="baseObject">           The base object. </param>
        /// <param name="definitionKind">       The definition kind. </param>
        /// <param name="dataType">             Type of the data. </param>
        /// <param name="entityFactory">        The entity factory. </param>
        /// <param name="getCurrentChildren">   The get current children. </param>
        /// <param name="propertyName">         (Optional) Name of the property. </param>

        public GetAddInEntitiesEventArgs(IBase baseObject, DefinitionKind definitionKind, BaseType dataType, EntityFactory entityFactory, Func<IEnumerable<IBase>> getCurrentChildren, string propertyName = null)
        {
            this.BaseObject = baseObject;
            this.DefinitionKind = definitionKind;
            this.Entities = new List<IBase>();
            this.EntityFactory = entityFactory;
            this.PropertyName = propertyName;
            this.Type = dataType;
            this.GetCurrentChildren = getCurrentChildren;
        }
    }
}
