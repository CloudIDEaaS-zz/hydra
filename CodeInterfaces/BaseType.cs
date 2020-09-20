using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ServiceModel.DomainServices.Server;
using System.ComponentModel.DataAnnotations;
using CodeInterfaces.TypeMappings;
using Utils;

namespace CodeInterfaces
{
    [DataContract, KnownType(typeof(ScalarType)), KnownType(typeof(Facet))]
    public class BaseType
    {
        [DataMember, Key]
        public string ID { get; set; }
        [DataMember]
        public string ParentID { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Namespace { get; set; }
        [DataMember]
        public string FullyQualifiedName { get; set; }
        [DataMember]
        public string AssemblyQualifiedName { get; set; }
        [DataMember]
        public string FriendlyName { get; set; }
        [DataMember]
        public bool IsCollectionType { get; set; }
        [DataMember]
        public bool IsSystemCollectionType { get; set; }
        [DataMember]
        public BaseType CollectionType { get; set; }
        [DataMember]
        public bool IsImageType { get; set; }
        [DataMember]
        public bool IsAbstract { get; set; }
        [DataMember]
        public bool IsInterface { get; set; }
        [DataMember]
        public bool IsEnum { get; set; }
        [DataMember]
        public string SuggestedNamespace { get; set; }
        [DataMember]
        public BaseType BaseDataType { get; set; }
        [DataMember]
        public BaseType[] Interfaces { get; set; }
        [DataMember]
        public BaseType[] GenericArguments { get; set; }
        [DataMember]
        public bool IsArray { get; set; }
        [DataMember]
        public bool IsDictionaryGeneric { get; set; }
        [DataMember]
        public bool IsScalarType { get; set; }
        [Exclude]
        public Type UnderlyingType { get; set; }
        [Exclude]
        public BaseType SourceParent { get; set; }  // to prevent recursion

        public BaseType()
        {
        }

        public BaseType(BaseType type, IBase baseParent)
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

        public BaseType(Type type, string parentID)
        {
            this.ID = parentID + "/" + type.FullName;
            this.ParentID = parentID;
            this.FullyQualifiedName = type.FullName;
            this.AssemblyQualifiedName = type.AssemblyQualifiedName;
            this.FriendlyName = type.GenerateTypeString();
            this.Namespace = type.Namespace;
            this.Name = type.Name;
            this.UnderlyingType = type;
            this.BaseDataType = type.GetBaseDataType();
            this.IsInterface = type.IsInterface;
            this.IsEnum = type.IsEnum;
            this.IsArray = type.IsArray;
            this.IsDictionaryGeneric = type.IsDictionaryGeneric();
            this.IsScalarType = type.IsPrimitive || AttributeType.Types.Contains(type);
        }

        public BaseType(Type type)
        {
            this.FullyQualifiedName = type.FullName;
            this.AssemblyQualifiedName = type.AssemblyQualifiedName;
            this.FriendlyName = type.GenerateTypeString();
            this.Namespace = type.Namespace;
            this.Name = type.Name;
            this.UnderlyingType = type;
            this.BaseDataType = type.GetBaseDataType();
            this.IsInterface = type.IsInterface;
            this.IsEnum = type.IsEnum;
            this.IsArray = type.IsArray;
            this.IsDictionaryGeneric = type.IsDictionaryGeneric();
            this.IsScalarType = type.IsPrimitive || AttributeType.Types.Contains(type);
        }

        public BaseType(Type type, BaseType baseParent, bool minimal = false)
            : this(type)
        {
            this.ID = baseParent.ID + "/" + type.FullName;
            this.ParentID = baseParent.ID;
            this.SourceParent = baseParent;

            if (!minimal)
            {
                this.Interfaces = this.GetBaseInterfaces();
                this.GenericArguments = this.GetGenericArguments();
            }

            if (type.IsDictionaryGeneric())
            {
                IsCollectionType = true;
                CollectionType = new BaseType(type.GetDictionaryGenericType(), this);

                if (type.Namespace.StartsWith("System.Collections"))
                {
                    IsSystemCollectionType = true;
                }
            }
            else if (type.IsEnumerableGeneric())
            {
                IsCollectionType = true;
                CollectionType = new BaseType(type.GetEnumerableGenericType(), this);

                if (type.Namespace.StartsWith("System.Collections") || type.IsArray)
                {
                    IsSystemCollectionType = true;
                }
            }
        }

        public BaseType(Type type, IBase baseParent, bool minimal = false) : this(type)
        {
            this.ID = baseParent.ID + "/" + type.FullName;
            this.ParentID = baseParent.ID;

            if (!minimal)
            {
                this.Interfaces = this.GetBaseInterfaces();
                this.GenericArguments = this.GetGenericArguments();
            }

            if (type.IsDictionaryGeneric())
            {
                IsCollectionType = true;
                CollectionType = new BaseType(type.GetDictionaryGenericType(), this);

                if (type.Namespace.StartsWith("System.Collections"))
                {
                    IsSystemCollectionType = true;
                }
            }
            else if (type.IsEnumerableGeneric())
            {
                IsCollectionType = true;
                CollectionType = new BaseType(type.GetEnumerableGenericType(), this);

                if (type.Namespace.StartsWith("System.Collections") || type.IsArray)
                {
                    IsSystemCollectionType = true;
                }
            }
        }
    }
}
