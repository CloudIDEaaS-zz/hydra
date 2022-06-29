using System;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Utils;

namespace ApplicationGenerator.Data
{
    public class AbstraXProviderCoreTypeMapping : CoreTypeMapping
    {
        private Type type;
        private MemberInfo member;
        private IProperty property;
        private AbstraXProviderDataProvider abstraXProviderDataProvider;

        public AbstraXProviderCoreTypeMapping(MemberInfo member, AbstraXProviderDataProvider abstraXProviderDataProvider) : base(abstraXProviderDataProvider.GetMappingParameters<CoreTypeMappingParameters>(member))
        {
            this.member = member;
            this.abstraXProviderDataProvider = abstraXProviderDataProvider;
        }

        public AbstraXProviderCoreTypeMapping(IProperty property, AbstraXProviderDataProvider abstraXProviderDataProvider) : base(abstraXProviderDataProvider.GetMappingParameters<CoreTypeMappingParameters>(property))
        {
            this.property = property;
            this.abstraXProviderDataProvider = abstraXProviderDataProvider;
        }

        public AbstraXProviderCoreTypeMapping(Type type, AbstraXProviderDataProvider abstraXProviderDataProvider) : base(abstraXProviderDataProvider.GetMappingParameters<CoreTypeMappingParameters>(type))
        {
            this.type = type;
            this.abstraXProviderDataProvider = abstraXProviderDataProvider;
        }

        public override CoreTypeMapping Clone(ValueConverter converter)
        {
            return DebugUtils.BreakReturnNull();
        }
    }
}