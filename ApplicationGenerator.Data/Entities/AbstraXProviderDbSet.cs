using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ApplicationGenerator.AST;
using ApplicationGenerator.Data.Interfaces;
using ApplicationGenerator.State;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json.Linq;
using Utils;

namespace ApplicationGenerator.Data
{
    public partial class AbstraXProviderDbSet<TEntity> : DbSet<TEntity>, IQueryable<TEntity>, IEnumerable<TEntity>, IQueryProvider, IEnumerable, IAsyncEnumerable<TEntity>, IInfrastructure<IServiceProvider>, IListSource where TEntity : class
    {
        private AbstraXProviderDataProvider provider;
        private AbstraXProviderDataContext abstraXProviderContext;
        private string setName;
        private ConstantExpression expression;
        private string idBase;
        private IAbstraXDataProvider dataProvider;
        private NamingConvention namingConvention;

        public AbstraXProviderDbSet()
        {
        }

        public AbstraXProviderDbSet(AbstraXProviderDataProvider provider, AbstraXProviderDataContext abstraXProviderContext, string setName)
        {
            this.provider = provider;
            this.abstraXProviderContext = abstraXProviderContext;
            this.setName = setName;
            this.query = new AbstraXProviderQuery();

            expression = Expression.Constant(this);
        }

        public Type ElementType
        {
            get
            {
                return typeof(TEntity);
            }
        }

        public Expression Expression
        {
            get
            {
                return expression;
            }
        }

        public IQueryProvider Provider
        {
            get
            {
                return this;
            }
        }

        public IServiceProvider Instance
        {
            get
            {
                return DebugUtils.BreakReturnNull();
            }
        }

        public bool ContainsListCollection
        {
            get
            {
                return true;
            }
        }

        public override EntityEntry<TEntity> Add(TEntity entity)
        {
            return base.Add(entity);
        }

        public IAsyncEnumerator<TEntity> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return DebugUtils.BreakReturnNull();
        }

        public IEnumerator<TEntity> GetEnumerator()
        {
            var providerServices = abstraXProviderContext.AbstraXProviderServices;
            var syntaxTree = query.SyntaxTree;
            var builder = new StringBuilder();
            var writer = new StringWriter(builder);
            var type = typeof(TEntity);
            var entityProperties = type.GetProperties();
            var model = provider.Model;
            var stateManager = provider.StateManager;
            var values = new List<object>();
            AbstraXProviderIdWriter idWriter;
            string id;
            IEnumerator enumerator;

            dataProvider = providerServices.LocateProvider(abstraXProviderContext);
            namingConvention = dataProvider.GetNamingConvention();
            this.idBase = dataProvider.GetIdBase().FromBase64ToString();

            idWriter = new AbstraXProviderIdWriter(writer, namingConvention);

            syntaxTree.AcceptVisitor(idWriter);
            id = idBase + "/?" + builder.ToString();

            enumerator = dataProvider.CreateEntitySetEnumerator(abstraXProviderContext, this.setName, id.ToBase64());

            while (enumerator.MoveNext())
            {
                var obj = (JObject) enumerator.Current;
                var entity = Activator.CreateInstance<TEntity>();
                var entityType = model.FindEntityType(type.FullName);
                var tracker = provider.Context.ChangeTracker;

                foreach (var property in obj.Properties())
                {
                    var entityProperty = entityProperties.SingleOrDefault(p => p.Name.AsCaseless() == property.Name);

                    if (entityProperty != null)
                    {
                        var value = property.Value.ToObject(entityProperty.PropertyType);

                        values.Add(value);

                        entityProperty.SetValue(entity, value);
                    }
                }

                provider.StateManager.StartTrackingFromQuery(entityType, entity, new ValueBuffer(values.ToArray()));

                yield return entity;
            }
        }

        public IList GetList()
        {
            return DebugUtils.BreakReturnNull();
        }
    }
}