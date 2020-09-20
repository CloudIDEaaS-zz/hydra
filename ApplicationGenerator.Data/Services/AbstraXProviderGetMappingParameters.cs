using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq;
using Utils;
using Microsoft.Extensions.DependencyInjection;
using ApplicationGenerator.Data.Interfaces;
using ApplicationGenerator.State;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Utils;

namespace ApplicationGenerator.Data
{
    public partial class AbstraXProviderDataProvider
    {
        public T GetMappingParameters<T>(MemberInfo member)
        {
            var constructor = typeof(T).GetConstructor(new Type[] { typeof(Type), typeof(ValueConverter), typeof(ValueComparer), typeof(ValueComparer), typeof(ValueComparer), typeof(Func<IProperty, IEntityType, ValueGenerator>) });
            var parameters = new AbstraXProviderMappingParameters(this, member.GetMemberType(), valueGeneratorFactory: (p, e) =>
            {
                return DebugUtils.BreakReturnNull();
            });

            return (T) Activator.CreateInstance(typeof(T), new object[] { parameters.ClrType, parameters.ValueConverter, parameters.ValueComparer, parameters.KeyComparer, parameters.StructuralComparer, parameters.ValueGeneratorFactory });
        }

        public T GetMappingParameters<T>(IProperty property)
        {
            var constructor = typeof(T).GetConstructor(new Type[] { typeof(Type), typeof(ValueConverter), typeof(ValueComparer), typeof(ValueComparer), typeof(ValueComparer), typeof(Func<IProperty, IEntityType, ValueGenerator>) });
            var parameters = new AbstraXProviderMappingParameters(this, property.PropertyInfo.PropertyType, valueGeneratorFactory: (p, e) =>
            {
                return DebugUtils.BreakReturnNull();
            });

            return (T)Activator.CreateInstance(typeof(T), new object[] { parameters.ClrType, parameters.ValueConverter, parameters.ValueComparer, parameters.KeyComparer, parameters.StructuralComparer, parameters.ValueGeneratorFactory });
        }

        public T GetMappingParameters<T>(Type type)
        {
            var constructor = typeof(T).GetConstructor(new Type[] { typeof(Type), typeof(ValueConverter), typeof(ValueComparer), typeof(ValueComparer), typeof(ValueComparer), typeof(Func<IProperty, IEntityType, ValueGenerator>) });
            var parameters = new AbstraXProviderMappingParameters(this, type, valueGeneratorFactory: (p, e) =>
            {
                return DebugUtils.BreakReturnNull();
            });

            return (T)Activator.CreateInstance(typeof(T), new object[] { parameters.ClrType, parameters.ValueConverter, parameters.ValueComparer, parameters.KeyComparer, parameters.StructuralComparer, parameters.ValueGeneratorFactory });
        }
    }
}
