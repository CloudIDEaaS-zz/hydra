// file:	Models\AssemblyModels\AssemblyModelEntityBase.cs
//
// summary:	Implements the assembly model entity base class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using AbstraX.ServerInterfaces;
using AbstraX;
using AbstraX.Models.Interfaces;
using Utils;
using AbstraX.Models;

namespace AssemblyModelEntityProvider.Web.Entities
{
    /// <summary>   An assembly model entity base. </summary>
    ///
    /// <remarks>   Ken, 11/1/2020. </remarks>

    public abstract class AssemblyModelEntityBase : BaseObject
    {
        /// <summary>   The assembly. </summary>
        protected AssemblyProvider.Web.Entities.Assembly assembly;
        protected ITypesProvider typesProvider;

        /// <summary>   Gets the namespace. </summary>
        ///
        /// <value> The namespace. </value>

        public string Namespace
        {
            get
            {
                var ancestor = (IBase)this.Parent;
                string _namespace = null;

                while (ancestor != null)
                {
                    if (ancestor is AssemblyModel)
                    {
                        _namespace = ((AssemblyModel)ancestor).Namespace;

                        break;
                    }

                    ancestor = ancestor.Parent;
                }

                return _namespace;
            }
        }

        /// <summary>   Default constructor. </summary>
        ///
        /// <remarks>   Ken, 11/1/2020. </remarks>

        public AssemblyModelEntityBase()
        {
        }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Ken, 11/1/2020. </remarks>
        ///
        /// <param name="parent">   The parent. </param>

        public AssemblyModelEntityBase(BaseObject parent) : base(parent)
        {
        }
    }
}
