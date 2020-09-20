using AbstraX.ServerInterfaces;
using AbstraX.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.Handlers.ValidationHandlers
{
    [ValidationHandler(typeof(MaxLengthAttribute))]
    public class MaxLengthValidationHandler : IValidationHandler
    {
        public void HandleValidations(IBase baseObject, IValidationSet validationSet, Attribute[] attributes)
        {
            var maxLengthAttribute = attributes.OfType<MaxLengthAttribute>().Single();
            var name = baseObject.GetDisplayName();
            var length = maxLengthAttribute.Length;
            var errorMessage = maxLengthAttribute.ErrorMessage ?? string.Format("Maximum of {0} characters", length);
            var candidateKey = errorMessage.ToConstantName();
            var key = validationSet.AddTranslation(baseObject, candidateKey, errorMessage, true);
            var validValue = length.CreateTestStringOfLength();
            var invalidValue = (length + 1).CreateTestStringOfLength();

            validationSet.AddValidationEntry("required", string.Format("Validators.maxLength({0})", maxLengthAttribute.Length), key, validValue, invalidValue);
        }
    }
}
