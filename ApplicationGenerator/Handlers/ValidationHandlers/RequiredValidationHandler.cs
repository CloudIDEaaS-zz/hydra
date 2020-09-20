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
    [ValidationHandler(typeof(RequiredAttribute))]
    public class RequiredValidationHandler : IValidationHandler
    {
        public void HandleValidations(IBase baseObject, IValidationSet validationSet, Attribute[] attributes)
        {
            var requiredAttribute = attributes.OfType<RequiredAttribute>().Single();
            var errorMessage = string.Format("{0} is required", baseObject.GetDisplayName());
            var candidateKey = errorMessage.ToConstantName();
            var key = validationSet.AddTranslation(baseObject, candidateKey, errorMessage, true);

            validationSet.AddValidationEntry("required", "Validators.required", key, "Required Value", null);
        }
    }
}
