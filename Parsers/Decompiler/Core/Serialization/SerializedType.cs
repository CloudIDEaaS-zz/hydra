#region License
/* 
 * Copyright (C) 1999-2015 John K�ll�n.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; see the file COPYING.  If not, write to
 * the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 */
#endregion

using Decompiler.Core.Types;
using System;
using System.Diagnostics;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Reflection;

namespace Decompiler.Core.Serialization
{
    /// <summary>
    /// Abstract base class for serialized types.
    /// </summary>
    public abstract class SerializedType
    {
        public SerializedType()
        {
        }

        public abstract DataType BuildDataType(TypeFactory factory);
        public abstract T Accept<T>(ISerializedTypeVisitor<T> visitor);

        /// <summary>
        /// Generates attribute overrides for each field that holds a serialized DataType.
        /// </summary>
        /// <param name="typesToDecorate"></param>
        /// <param name="xmlNamespace"></param>
        /// <returns></returns>
        public static XmlAttributeOverrides GetAttributeOverrides(IEnumerable<Type> typesToDecorate, string xmlNamespace)
        {
            var overrides = typesToDecorate
                .SelectMany(x => x.GetFields())
                .Select(f => new
                {
                    Field = f,
                    AttrCreator = GetAttributeCreator(f)
                })
                .Where(f => f.AttrCreator != null)
                .Aggregate(
                    new XmlAttributeOverrides(),
                    (ov, field) =>
                    { 
                        ov.Add(field.Field.DeclaringType, field.Field.Name, field.AttrCreator(xmlNamespace)); 
                        return ov;
                    });
            return overrides;
        }

        private static Func<string, XmlAttributes> GetAttributeCreator(FieldInfo f)
        {
            if (f.FieldType == typeof(SerializedType))
                return CreateElementAttributes;
            if (!f.FieldType.IsArray)
                return null;
            if (f.FieldType.GetElementType() == typeof(SerializedType))
                return CreateArrayElementAttributes;
            else
                return null;
        }

        private static XmlAttributes CreateElementAttributes(string @namespace)
        {
            var sertypeAttributes = new XmlAttributes
            {
                XmlElements = 
                {
                    new XmlElementAttribute("prim", typeof(PrimitiveType_v1)) { Namespace = @namespace},
                    new XmlElementAttribute("code", typeof(CodeType_v1)) { Namespace = @namespace},
                    new XmlElementAttribute("ptr", typeof(PointerType_v1)) { Namespace = @namespace},
                    new XmlElementAttribute("arr", typeof(ArrayType_v1)) { Namespace = @namespace},
                    new XmlElementAttribute("enum", typeof(SerializedEnumType)) { Namespace = @namespace},
                    new XmlElementAttribute("str", typeof(StringType_v2)) { Namespace = @namespace},
                    new XmlElementAttribute("struct", typeof(SerializedStructType)) { Namespace = @namespace},
                    new XmlElementAttribute("union", typeof(UnionType_v1)) { Namespace = @namespace},
                    new XmlElementAttribute("fn", typeof(SerializedSignature)) { Namespace = @namespace},
                    new XmlElementAttribute("typedef", typeof(SerializedTypedef)) { Namespace = @namespace},
                    new XmlElementAttribute("type", typeof(SerializedTypeReference)) { Namespace = @namespace},
                    new XmlElementAttribute("void", typeof(VoidType_v1)) { Namespace = @namespace},
                }
            };
            return sertypeAttributes;
        }

        private static XmlAttributes CreateArrayElementAttributes(string @namespace)
        {
            var sertypeAttributes = new XmlAttributes
            {
                XmlArrayItems = 
                {
                    new XmlArrayItemAttribute("prim", typeof(PrimitiveType_v1)) { Namespace = @namespace},
                    new XmlArrayItemAttribute("ptr", typeof(PointerType_v1)) { Namespace = @namespace},
                    new XmlArrayItemAttribute("arr", typeof(ArrayType_v1)) { Namespace = @namespace},
                    new XmlArrayItemAttribute("enum", typeof(SerializedEnumType)) { Namespace = @namespace},
                    new XmlArrayItemAttribute("str", typeof(StringType_v2)) { Namespace = @namespace},
                    new XmlArrayItemAttribute("struct", typeof(SerializedStructType)) { Namespace = @namespace},
                    new XmlArrayItemAttribute("union", typeof(UnionType_v1)) { Namespace = @namespace},
                    new XmlArrayItemAttribute("fn", typeof(SerializedSignature)) { Namespace = @namespace},
                    new XmlArrayItemAttribute("typedef", typeof(SerializedTypedef)) { Namespace = @namespace},
                    new XmlArrayItemAttribute("type", typeof(SerializedTypeReference)) { Namespace = @namespace},
                    new XmlArrayItemAttribute("void", typeof(VoidType_v1)) { Namespace = @namespace},
                }
            };
            return sertypeAttributes;
        }
    }

    public abstract class SerializedTaggedType : SerializedType
    {
        [XmlAttribute("name")]
        public string Name;
    }
}