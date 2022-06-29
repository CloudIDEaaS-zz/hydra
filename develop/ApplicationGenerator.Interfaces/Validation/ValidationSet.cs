using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstraX.ServerInterfaces;

namespace AbstraX.Validation
{
    public class ValidationSet : IValidationSet
    {
        private List<ValidationEntry> validationEntries;
        private List<ValidationMask> validationMasks;
        private List<ValidationElement> validationElements;
        private IGeneratorConfiguration generatorConfiguration;

        public ValidationSet(IGeneratorConfiguration generatorConfiguration)
        {
            validationEntries = new List<ValidationEntry>();
            validationMasks = new List<ValidationMask>();
            validationElements = new List<ValidationElement>();
            this.generatorConfiguration = generatorConfiguration;
        }

        public void AddValidationEntry(string name, string functionExpression, string errorMessageKey, object validTestValue = null, object invalidTestValue = null, bool isForm = false, string functionCode = null)
        {
            validationEntries.Add(new ValidationEntry(name, functionExpression, errorMessageKey, validTestValue, invalidTestValue, isForm, functionCode));
        }

        public void AddValidationMask(string name, string maskExpression, string unmaskExpression, float priority = 0f)
        {
            validationMasks.Add(new ValidationMask(name, maskExpression, unmaskExpression, priority));
        }

        public void AddValidationElement(string tagName, string inputType = "text", float priority = 0f)
        {
            validationElements.Add(new ValidationElement(tagName, inputType, priority));
        }

        public void AddValidationElement(string tagName, string inputType, float priority, params KeyValuePair<string, string>[] attributes)
        {
            validationElements.Add(new ValidationElement(tagName, inputType, priority, attributes));
        }

        public void AddValidationElement(string tagName, string inputType, params KeyValuePair<string, string>[] attributes)
        {
            validationElements.Add(new ValidationElement(tagName, inputType, attributes));
        }

        public void AddValidationElement(ValidationElement validationElement)
        {
            validationElements.Add(validationElement);
        }

        public string AddTranslation(IBase baseObject, string key, string value, bool skipIfSame)
        {
            return generatorConfiguration.LanguageDictionary.AddTranslation(baseObject, key, value, skipIfSame);
        }

        public ValidationElement PreferredValidationElement
        {
            get
            {
                if (this.ValidationElements.Count() > 0)
                {
                    return this.ValidationElements.OrderBy(e => e.Priority).Last();
                }
                else
                {
                    return new InputValidationElement();
                }
            }
        }

        public ValidationMask PreferredValidationMask
        {
            get
            {
                return this.ValidationMasks.OrderBy(e => e.Priority).LastOrDefault();
            }
        }

        public IEnumerable<ValidationElement> ValidationElements
        {
            get
            {
                return validationElements.AsEnumerable();
            }
        }

        public IEnumerable<ValidationEntry> ValidationEntries
        {
            get
            {
                return validationEntries.AsEnumerable();
            }
        }

        public IEnumerable<ValidationMask> ValidationMasks
        {
            get
            {
                return validationMasks.AsEnumerable();
            }
        }
    }
}
