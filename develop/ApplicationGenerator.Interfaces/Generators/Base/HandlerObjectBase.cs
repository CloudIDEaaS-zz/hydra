using AbstraX.ServerInterfaces;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Utils;

namespace AbstraX.Generators.Base
{
    public abstract class HandlerObjectBase
    {
        private IGeneratorConfiguration generatorConfiguration;
        private string translationKey;
        public IBase BaseObject { get; set; }
        protected IGeneratorConfiguration GeneratorConfiguration { set => generatorConfiguration = value; }

        public HandlerObjectBase(IBase baseObject, IGeneratorConfiguration generatorConfiguration)
        {
            this.BaseObject = baseObject;
            this.generatorConfiguration = generatorConfiguration;
        }

        public virtual object This
        {
            get
            {
                return this;
            }
        }

        public string CreateTranslationKey(Expression<Func<string>> propertyExpression)
        {
            if (translationKey == null)
            {
                var visitor = propertyExpression.Visit();
                var propertyName = visitor.MemberName;
                var value = this.This.GetPropertyValue<string>(propertyName);
                var keyPrefix = this.CreateKeyPrefix(this.BaseObject);
                var key = string.Format("{0}{1}_{2}", keyPrefix, value.ToConstantName(), propertyName.ToConstantName());

                key = generatorConfiguration.AddTranslation(this.BaseObject, key, value, true);

                translationKey = key;
            }

            return translationKey;
        }

        internal string CreateTranslationKey(IBase baseObject, Expression<Func<string>> propertyExpression)
        {
            var visitor = propertyExpression.Visit();
            var propertyName = visitor.MemberName;
            string key;

            if (this.This is JObject)
            {
                var jObj = (JObject)this.This;
                var value = jObj.Properties().Single(p => p.Name == propertyName).Value.Value<string>();
                var keyPrefix = this.CreateKeyPrefix(baseObject);

                key = string.Format("{0}{1}_{2}", keyPrefix, value.ToConstantName(), propertyName.ToConstantName());

                key = generatorConfiguration.AddTranslation(baseObject, key, value, true);
            }
            else
            {
                var value = this.This.GetPropertyValue<string>(propertyName);
                var keyPrefix = this.CreateKeyPrefix(baseObject);
                
                key = string.Format("{0}{1}_{2}", keyPrefix, value.ToConstantName(), propertyName.ToConstantName());

                key = generatorConfiguration.AddTranslation(baseObject, key, value, true);
            }

            return key;
        }

        private string CreateKeyPrefix(IBase baseObject)
        {
            switch (baseObject.Kind)
            {
                case DefinitionKind.Class:
                case DefinitionKind.ComplexSetProperty:
                    return string.Empty;
                case DefinitionKind.ComplexProperty:
                case DefinitionKind.SimpleProperty:
                    return baseObject.Parent.Name.ToConstantName("_");
                default:
                    DebugUtils.Break();
                    return null;
            }
        }
    }
}
