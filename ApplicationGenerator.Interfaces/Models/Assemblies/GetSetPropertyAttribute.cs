using System;
using System.Net;
using AbstraX.ServerInterfaces;
using System.Reflection;
using AbstraX.XPathBuilder;
using System.Linq;
using System.Xml.Schema;
using System.Diagnostics;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using AbstraX;
using AbstraX.AssemblyInterfaces;
using AbstraX.Models;

namespace AssemblyProvider.Web.Entities
{
    internal class GetSetPropertyAttribute : PropertyAttribute, IGetSetPropertyAttribute
    {
        private MethodInfo method;

        public MethodInfo Method
        {
            get 
            { 
                return method; 
            }
        }

        public GetSetPropertyAttribute(MethodInfo method, BaseObject parent) : base(parent)
        {
            this.method = method;
            this.parent = parent;
            this.childOrdinal = 3;

            name = this.GenerateName();

            var type = method.GetGetterSetterType();
            
            modifiers = Modifiers.Unknown;

            if (method.IsGetter())
            {
                modifiers |= Modifiers.CanRead;
            }

            if (method.IsSetter())
            {
                modifiers |= Modifiers.CanWrite;
            }

            if (method.DeclaringType.FullName == ((IElement)parent).DataType.FullyQualifiedName)
            {
                modifiers |= Modifiers.IsLocal;
            }

            id = this.MakeID("Property='" + this.Name + "'");
            dataType = new ScalarType(type, this);
        }

        public string PropertyName
        {
            get
            {
                return ((IGetSetProperty) this).GetPropertyName();
            }
        }
    }
}
