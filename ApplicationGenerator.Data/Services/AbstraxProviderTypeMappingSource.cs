using System;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Utils;

namespace ApplicationGenerator.Data
{
    public partial class AbstraXProviderDataProvider : ITypeMappingSource
    {
        public CoreTypeMapping FindMapping(IProperty property)
        {
            var mapping = new AbstraXProviderCoreTypeMapping(property, this);

            return mapping;
        }

        public CoreTypeMapping FindMapping(MemberInfo member)
        {
            var mapping = new AbstraXProviderCoreTypeMapping(member, this);
            
            return mapping;
        }

        public CoreTypeMapping FindMapping(Type type)
        {
            var mapping = new AbstraXProviderCoreTypeMapping(type, this);

            return mapping;
        }
    }
}