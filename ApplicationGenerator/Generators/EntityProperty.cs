            using AbstraX.DataAnnotations;
using AbstraX.Generators.Base;
using AbstraX.ServerInterfaces;
using AbstraX.TypeMappings;
using AbstraX.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX.Generators
{
    [DebuggerDisplay(" { Title } ")]
    public class EntityProperty : HandlerObjectBase
    {
        public string Name { get; }
        public string Title { get; }
        public string RegisterTitle { get; }
        public string DataType { get; }
        public bool Nullable { get; }
        public bool IsKey { get; }
        public bool IsPasswordHash { get; }

        public EntityProperty(IAttribute attribute, IGeneratorConfiguration generatorConfiguration) : base(attribute, generatorConfiguration)
        {
            var displayName = attribute.GetDisplayName();
            var name = attribute.GetNavigationName();

            this.Nullable = attribute.Nullable;
            this.DataType = attribute.GetShortType();

            if (this.Nullable)
            {
                this.DataType = this.DataType + "?";
            }

            if (attribute.HasFacetAttribute<IdentityFieldAttribute>())
            {
                var identityFieldAttribute = attribute.GetFacetAttribute<IdentityFieldAttribute>();

                if (identityFieldAttribute.IdentityFieldKind == IdentityFieldKind.PasswordHash)
                {
                    this.RegisterTitle = displayName.RemoveEndIfMatches("Hash");
                    this.IsPasswordHash = true;
                }
            }

            this.Title = displayName;
            this.Name = name;
            this.IsKey = attribute.HasFacetAttribute<KeyAttribute>();
            this.BaseObject = attribute;
        }


        public string GetPropertyAttributeSectionString()
        {
            var builder = new StringBuilder();

            foreach (var facet in this.BaseObject.Facets)
            {
                var attribute = facet.AttributeCode.Trim();

                attribute = attribute.RegexRemove(@"Attribute(?=\()");
                attribute = attribute.RegexRemove(@"\(\)$");

                builder.AppendWithLeadingIfLength(", ", attribute);
            }

            if (builder.Length > 0)
            {
                builder.Insert(0, "[");
                builder.Append("]");
            }

            return builder.ToString();
        }

        public string TranslationKey
        {
            get
            {
                return this.CreateTranslationKey(() => this.Title);
            }
        }
    }
}
