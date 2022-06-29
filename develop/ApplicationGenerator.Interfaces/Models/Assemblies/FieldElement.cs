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
    public class FieldElement : PropertyElement, IPropertyElement
    {
        private FieldInfo field;

        public FieldElement(FieldInfo field, BaseObject parent) : base(parent)
        {
            this.field = field;
            this.childOrdinal = 3;
            this.name = field.Name;

            modifiers = Modifiers.Unknown;

            if (field.IsInitOnly)
            {
                modifiers = Modifiers.CanRead;
            }
            else
            {
                modifiers = Modifiers.CanRead | Modifiers.CanWrite;
            }

            if (field.DeclaringType.FullName == ((IElement)parent).DataType.FullyQualifiedName)
            {
                modifiers |= Modifiers.IsLocal;
            }

            id = this.MakeID("Property='" + field.Name + "'");

            dataType = new BaseType(field.FieldType, this);
        }

        public FieldInfo Field
        {
            get
            {
                return field;
            }
        }

        protected override List<AssemblyType> GetTypes()
        {
            var typeElement = new AssemblyType(field.FieldType, this);

            return new List<AssemblyType>() { typeElement };
        }

        protected override IEnumerable<IElement> GetChildElements()
        {
            var typeElement = new AssemblyType(field.FieldType, this);

            if (typeElement.DataType.IsSystemCollectionType)
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
    }
}
