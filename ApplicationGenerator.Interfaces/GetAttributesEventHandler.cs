using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstraX.ServerInterfaces;

namespace AbstraX
{
    public delegate void GetAttributesEventHandler(object sender, GetAttributesEventArgs e);

    public class GetAttributesEventArgs : EventArgs
    {
        public IBase BaseObject { get; }
        public BaseType Type { get; }
        public string PropertyName { get; }
        public IEnumerable<Attribute> Attributes { get; set; }
        public bool NoMetadata { get; set; }

        public GetAttributesEventArgs(IBase baseObject, BaseType dataType, string propertyName = null)
        {
            this.BaseObject = baseObject;
            this.Type = dataType;
            this.PropertyName = propertyName;
        }
    }
}
