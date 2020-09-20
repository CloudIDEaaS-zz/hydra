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
    public class FormField : HandlerObjectBase
    {
        public string Name { get; }
        public string Title { get; }
        public string ClientDataType { get; }
        public string ServerDataType { get; }
        public IValidationSet ValidationSet { get; }
        public FormFieldKind FormFieldKind { get; }
        public bool IsKey { get; }

        public FormField(IAttribute attribute, IGeneratorConfiguration generatorConfiguration) : base(attribute, generatorConfiguration)
        {
            var formFieldAttribute = attribute.GetFacetAttribute<FormFieldAttribute>();
            var displayName = attribute.GetDisplayName();
            var name = attribute.GetNavigationName();

            this.ClientDataType = attribute.GetScriptTypeName();
            this.ServerDataType = attribute.GetDotNetTypeName();
            this.ValidationSet = generatorConfiguration.BuildValidationSet(attribute);
            this.Title = displayName;
            this.Name = name;
            this.FormFieldKind = formFieldAttribute.FormFieldKind;
            this.IsKey = attribute.HasFacetAttribute<KeyAttribute>();
            this.BaseObject = attribute;
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
