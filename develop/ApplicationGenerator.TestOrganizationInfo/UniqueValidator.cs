using AbstraX;
using AbstraX.DataModels;
using System.ComponentModel.DataAnnotations;
using Utils;
using System.Linq;
using AbstraX.DataAnnotations;

namespace ApplicationGenerator.TestOrganizationInfo
{
    public class UniqueValidator : IUniqueValidator
    {
        public ValidationResult IsUnique(object value, ValidationContext validationContext)
        {
            var model = (OrganizationModel)validationContext.ObjectInstance;
            var modelType = typeof(OrganizationModel);
            var property = modelType.GetProperty(validationContext.MemberName);
            var validationAttributes = property.GetCustomAttributes(true).OfType<ValidationAttribute>().Where(a => !a.IsOfType<UniqueValidationAttribute>());
            var isNotValid = validationAttributes.ToList().Any(a => !a.IsValid(value));

            if (!isNotValid)
            {

            }

            return ValidationResult.Success;
        }
    }
}