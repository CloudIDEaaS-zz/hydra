using AbstraX.DataAnnotations;
using AbstraX.Generators.Base;
using AbstraX.Models.Interfaces;
using AbstraX.ServerInterfaces;
using AbstraX.TypeMappings;
using AbstraX.Validation;
using EntityProvider.Web.Entities;
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
    public class RelatedEntity : HandlerObjectBase
    {
        public IAttribute ThisPropertyRef { get; }
        public IAttribute ParentPropertyRef { get; }
        public IElement EntityParentRef { get; }
        public string ThisPropertyRefScriptType { get; }
        public string ThisPropertyRefShortType { get; }
        public string Title { get; }
        public string Name { get; }
        public IEntityContainer Container { get; }
        public IEntitySet ContainerSet { get; }
        public string SortBy { get; }
        public SortDirection SortDirection { get; }

        public RelatedEntity(IRelationProperty navigationProperty, IGeneratorConfiguration generatorConfiguration) : base(navigationProperty, generatorConfiguration)
        {
            var displayName = navigationProperty.GetDisplayName();
            var name = navigationProperty.Name;
            var sortByAttribute = navigationProperty.GetFacetAttribute<SortByAttribute>();

            this.SortBy = sortByAttribute.PropertyName;
            this.SortDirection = sortByAttribute.SortDirection;
            this.EntityParentRef = (IElement) navigationProperty.Parent;
            this.ThisPropertyRef = navigationProperty.ThisPropertyRef;
            this.ParentPropertyRef = navigationProperty.ParentPropertyRef;

            if (this.ThisPropertyRef != null)
            {
                this.ThisPropertyRefScriptType = this.ThisPropertyRef.GetScriptTypeName();
                this.ThisPropertyRefShortType = this.ThisPropertyRef.GetShortType();
                this.ContainerSet = this.EntityParentRef.GetContainerSet();
            }

            this.Title = displayName;
            this.Name = name;
            this.BaseObject = navigationProperty;
            this.Container = navigationProperty.GetContainer();
        }

        public bool IsEntitySet
        {
            get
            {
                return this.BaseObject is IEntitySet;
            }
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
