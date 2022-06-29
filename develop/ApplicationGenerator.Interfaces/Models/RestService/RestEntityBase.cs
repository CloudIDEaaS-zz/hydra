// file:	Models\RestService\RestEntityBase.cs
//
// summary:	Implements the REST entity base class

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

namespace RestEntityProvider.Web.Entities
{
    /// <summary>   A REST entity base. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 3/15/2021. </remarks>

    public abstract class RestEntityBase : BaseObject, IEntityWithPrefix
    {
        /// <summary>   Gets or sets the JSON root object. </summary>
        ///
        /// <value> The JSON root object. </value>

        public dynamic JsonRootObject { get; set; }

        /// <summary>   Gets or sets the JSON object. </summary>
        ///
        /// <value> The JSON object. </value>

        public dynamic JsonObject { get; set; }

        /// <summary>   Gets or sets the JSON original root object. </summary>
        ///
        /// <value> The JSON original root object. </value>

        public dynamic JsonOriginalRootObject { get; set; }

        /// <summary>   Gets or sets the JSON original object. </summary>
        ///
        /// <value> The JSON original object. </value>

        public dynamic JsonOriginalObject { get; set; }

        /// <summary>   Gets or sets the path prefix. </summary>
        ///
        /// <value> The path prefix. </value>

        public string PathPrefix { get; set; }

        /// <summary>   Gets or sets the configuration prefix. </summary>
        ///
        /// <value> The configuration prefix. </value>

        public string ConfigPrefix { get; set; }

        /// <summary>   Gets or sets the controller name prefix. </summary>
        ///
        /// <value> The controller name prefix. </value>

        public string ControllerNamePrefix { get; set; }

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
                    if (ancestor is RestModel)
                    {
                        _namespace = ((RestModel)ancestor).Namespace;

                        break;
                    }

                    ancestor = ancestor.Parent;
                }

                return _namespace;
            }
        }

        /// <summary>   Selects the given file. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/15/2021. </remarks>
        ///
        /// <param name="path"> Full pathname of the file. </param>
        ///
        /// <returns>   An object. </returns>

        public object Select(string path)
        {
            return ((object)this.JsonObject).JsonSelect(path);
        }

        /// <summary>   Select original. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/15/2021. </remarks>
        ///
        /// <param name="path"> Full pathname of the file. </param>
        ///
        /// <returns>   An object. </returns>

        public object SelectOriginal(string path)
        {
            return ((object)this.JsonOriginalObject).JsonSelect(path);
        }

        /// <summary>   Default constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/15/2021. </remarks>

        public RestEntityBase()
        {
        }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/15/2021. </remarks>
        ///
        /// <param name="parent">   The parent. </param>

        public RestEntityBase(BaseObject parent) : base(parent)
        {
        }
    }
}
