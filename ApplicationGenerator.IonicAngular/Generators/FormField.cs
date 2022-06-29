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

        public FormField(IAttribute attribute, Facet entityFacet, IGeneratorConfiguration generatorConfiguration) : base(attribute, generatorConfiguration)
        {
            var uiAttribute = (UIAttribute)entityFacet.Attribute;
            var displayName = attribute.GetDisplayName();
            var name = attribute.GetNavigationName();
            FormFieldAttribute formFieldAttribute;

            if (uiAttribute.UIKind == UIKind.LoginPage)
            {
                var identityFieldAttribute = attribute.GetFacetAttribute<IdentityFieldAttribute>();

                switch (identityFieldAttribute.IdentityFieldKind)
                {
                    case IdentityFieldKind.UserName:
                        this.FormFieldKind = FormFieldKind.UserName;
                        break;
                    case IdentityFieldKind.PasswordHash:
                        this.FormFieldKind = FormFieldKind.Password;
                        break;
                    case IdentityFieldKind.UserId:
                        this.FormFieldKind = FormFieldKind.Label;
                        break;
                    default:
                        this.FormFieldKind = FormFieldKind.DataType;
                        break;
                }
            }
            else
            {
                formFieldAttribute = attribute.GetFacetAttribute<FormFieldAttribute>(a => a.Follows(uiAttribute, generatorConfiguration.PartsAliasResolver));

                if (formFieldAttribute != null)
                {
                    this.FormFieldKind = formFieldAttribute.FormFieldKind;
                }
                else
                {
                    var identityField = attribute.GetFacetAttribute<IdentityFieldAttribute>();

                    switch (identityField.IdentityFieldKind)
                    {
                        case IdentityFieldKind.UserName:
                            this.FormFieldKind = FormFieldKind.UserName;
                            break;
                        case IdentityFieldKind.PasswordHash:
                            this.FormFieldKind = FormFieldKind.Password;
                            break;
                        default:
                            DebugUtils.Break();
                            break;
                    }
                }
            }

            this.ClientDataType = attribute.GetScriptTypeName();
            this.ServerDataType = attribute.GetDotNetTypeName();
            this.ValidationSet = generatorConfiguration.BuildValidationSet(attribute);
            this.Title = displayName;
            this.Name = name;
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
