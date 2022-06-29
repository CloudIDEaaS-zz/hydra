// file:	IAddInHandler.cs
//
// summary:	Declares the IAddInHandler interface

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX
{
    /// <summary>   Values that represent add in handler kinds. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 4/15/2021. </remarks>

    public enum AddInHandlerKind
    {
        /// <summary>   An enum constant representing the metadata reflection option. </summary>
        MetadataReflection
    }

    /// <summary>   Attribute for add in handler. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 4/15/2021. </remarks>

    public class AddInHandlerAttribute : Attribute
    {
        /// <summary>   Gets the add in handler kind. </summary>
        ///
        /// <value> The add in handler kind. </value>

        public AddInHandlerKind AddInHandlerKind { get; }

        /// <summary>   Gets a unique identifier of the abstra x coordinate provider. </summary>
        ///
        /// <value> Unique identifier of the abstra x coordinate provider. </value>

        public Guid AbstraXProviderGuid { get; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 4/15/2021. </remarks>
        ///
        /// <param name="kind">                 The kind. </param>
        /// <param name="abstraXProviderGuid">  Unique identifier for the abstra x coordinate provider. </param>

        public AddInHandlerAttribute(AddInHandlerKind kind, string abstraXProviderGuid)
        {
            this.AddInHandlerKind = kind;
            this.AbstraXProviderGuid = Guid.Parse(abstraXProviderGuid);
        }
    }

    /// <summary>   Interface for handler cache. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 4/15/2021. </remarks>

    public interface IHandlerCache
    {
        /// <summary>   Handles. </summary>
        ///
        /// <param name="addInEntitiesEventArgs">   Get add in entities event information. </param>
        /// <param name="metadataClassType">        Type of the metadata class. </param>

        void Handle(GetAddInEntitiesEventArgs addInEntitiesEventArgs, Type metadataClassType);
    }
}
