using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using CodeInterfaces.TypeMappings;

namespace CodeInterfaces
{
    [DataContract]
    public class ScalarType : BaseType
    {
        [DataMember]
        public int TypeCode { get; set; }

        public ScalarType()
        {
        }

        public ScalarType(Type type)
            : base(type)
        {
            this.TypeCode = (int)type.GetXMLType();
        }

        public ScalarType(BaseType type, IBase baseParent)
        {
            this.ID = baseParent.ID + "/" + type.FullyQualifiedName;
            this.ParentID = baseParent.ID;
            this.FullyQualifiedName = type.FullyQualifiedName;
            this.AssemblyQualifiedName = type.AssemblyQualifiedName;
            this.FriendlyName = type.GenerateTypeString();
            this.Namespace = type.Namespace;
            this.Name = type.Name;
            this.UnderlyingType = type.UnderlyingType;
            this.BaseDataType = type.BaseDataType;
            this.IsInterface = type.IsInterface;
            this.IsEnum = type.IsEnum;
            this.IsArray = type.IsArray;
            this.IsDictionaryGeneric = type.IsDictionaryGeneric;
            this.IsScalarType = type.IsScalarType;
        }

        public ScalarType(BaseType type, BaseType baseParent)
        {
            this.ID = baseParent.ID + "/" + type.FullyQualifiedName;
            this.ParentID = baseParent.ID;
            this.Name = type.Name;
            this.FullyQualifiedName = type.FullyQualifiedName;
            this.AssemblyQualifiedName = type.AssemblyQualifiedName;
            this.SourceParent = baseParent;
        }

        public ScalarType(Type type, BaseType baseParent) : this(type)
        {
            this.ID = baseParent.ID + "/" + type.FullName;
            this.ParentID = baseParent.ID;
            this.SourceParent = baseParent;
        }

        public ScalarType(Type type, IBase baseParent) : this(type)
        {
            this.ID = baseParent.ID + "/" + type.FullName;
            this.ParentID = baseParent.ID;
        }
    }
}
