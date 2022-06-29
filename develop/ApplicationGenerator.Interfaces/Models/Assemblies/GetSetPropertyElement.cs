using System;
using System.Net;
using AbstraX.ServerInterfaces;
using System.Reflection;
using AbstraX.XPathBuilder;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Linq.Expressions;
using AbstraX;
using AbstraX.AssemblyInterfaces;
using AbstraX.TypeMappings;
using AbstraX.Models;

namespace AssemblyProvider.Web.Entities
{
    internal class GetSetPropertyElement : PropertyElement, IGetSetPropertyElement
    {
        private MethodInfo method;

        public GetSetPropertyElement(MethodInfo method, BaseObject parent) : base(parent)
        {
            this.method = method;
            this.childOrdinal = 3;
            this.name = this.GenerateName();

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

            id = this.MakeID("Property='" + name + "'");
            dataType = new BaseType(method.ReturnType, this);
        }

        public MethodInfo Method
        {
            get
            {
                return method;
            }
        }

        protected override List<AssemblyType> GetTypes()
        {
            var typeElement = new AssemblyType(method.GetGetterSetterType(), this);

            return new List<AssemblyType>() { typeElement };
        }

        public BaseType DataType
        {
            get
            {
                return dataType;
            }
        }

        protected override IEnumerable<IElement> GetChildElements()
        {
            var typeElement = new AssemblyType(method.GetGetterSetterType(), this);

            if (typeElement.DataType.IsCollectionType)
            {
                if (queryWhereProperty != null && queryWhereProperty == "Type")
                {
                    yield return new AssemblyType(typeElement.DataType.CollectionType, this);
                }
                else
                {
                    yield return new AssemblyType(typeElement.DataType.CollectionType, this);
                }
            }
            else
            {
                if (queryWhereProperty != null && queryWhereProperty == "Type")
                {
                    yield return typeElement;
                }
                else
                {
                    yield return typeElement;
                }
            }
        }

        string IGetSetProperty.PropertyName
        {
            get
            {
                return this.GetPropertyName();
            }
        }
    }
}
