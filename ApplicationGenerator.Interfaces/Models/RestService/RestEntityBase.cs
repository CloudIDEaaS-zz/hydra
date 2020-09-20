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
    public abstract class RestEntityBase : BaseObject, IEntityWithPrefix
    {
        public dynamic JsonRootObject { get; set; }
        public dynamic JsonObject { get; set; }
        public dynamic JsonOriginalRootObject { get; set; }
        public dynamic JsonOriginalObject { get; set; }
        public string PathPrefix { get; set; }
        public string ConfigPrefix { get; set; }
        public string ControllerNamePrefix { get; set; }

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

        internal object Select(string path)
        {
            return ((object)this.JsonObject).JsonSelect(path);
        }

        internal object SelectOriginal(string path)
        {
            return ((object)this.JsonOriginalObject).JsonSelect(path);
        }

        public RestEntityBase()
        {
        }

        public RestEntityBase(BaseObject parent) : base(parent)
        {
        }
    }
}
