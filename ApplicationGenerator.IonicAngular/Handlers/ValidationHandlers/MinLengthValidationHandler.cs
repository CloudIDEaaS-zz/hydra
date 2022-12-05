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
    [ValidationHandler(typeof(MinLengthAttribute))]
    public class MinLengthValidationHandler : IValidationHandler
    {
        public void HandleValidations(IBase baseObject, IValidationSet validationSet, Attribute[] attributes)
        {
            var minLengthAttribute = attributes.OfType<MinLengthAttribute>().Single();
            var name = baseObject.GetDisplayName();
            var length = minLengthAttribute.Length;
            var errorMessage = minLengthAttribute.ErrorMessage ?? string.Format("Minimum of {0} characters", length);
            var candidateKey = errorMessage.ToConstantName();
            var key = validationSet.AddTranslation(baseObject, candidateKey, errorMessage, true);
            var validValue = length.CreateTestStringOfLength();
            var invalidValue = (length - 1).CreateTestStringOfLength();

            validationSet.AddValidationEntry("required", string.Format("Validators.minLength({0})", minLengthAttribute.Length), key, validValue, invalidValue);
        }
    }
}
