using ApplicationGenerator.Data;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Utils;

namespace ApplicationGenerator.Data
{
    public class AbstraXProviderMappingParameters
    {
        public AbstraXProviderDataProvider DataProvider { get; }
        public Type ClrType { get; }
        public ValueConverter ValueConverter { get; }
        public ValueComparer ValueComparer { get; }
        public ValueComparer KeyComparer { get; }
        public ValueComparer StructuralComparer { get; }
        public Func<IProperty, IEntityType, ValueGenerator> ValueGeneratorFactory { get; }

        public AbstraXProviderMappingParameters(AbstraXProviderDataProvider dataProvider, Type clrType, ValueConverter converter = null, ValueComparer comparer = null, ValueComparer keyComparer = null, ValueComparer structuralComparer = null, Func<IProperty, IEntityType, ValueGenerator> valueGeneratorFactory = null)
        {
            this.DataProvider = dataProvider;
            this.ClrType = clrType;
            this.ValueConverter = converter;
            this.ValueComparer = comparer;
            this.KeyComparer = keyComparer;
            this.StructuralComparer = structuralComparer;
            this.ValueGeneratorFactory = valueGeneratorFactory;
        }
    }
}
