// file:	DataModels\UniqueValidationAttribute.cs
//
// summary:	Implements the unique validation attribute class

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.DataAnnotations
{
    /// <summary>   Attribute for unique validation. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 7/7/2021. </remarks>

    public class UniqueValidationAttribute : ValidationAttribute
    {
        /// <summary>
        /// Applies formatting to an error message, based on the data field where the error occurred.
        /// </summary>
        ///
        /// <remarks>   CloudIDEaaS, 7/7/2021. </remarks>
        ///
        /// <param name="name"> The name to include in the formatted message. </param>
        ///
        /// <returns>   An instance of the formatted error message. </returns>

        public override string FormatErrorMessage(string name)
        {
            return base.FormatErrorMessage(name);
        }

        /// <summary>   Determines whether the specified value of the object is valid. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 7/7/2021. </remarks>
        ///
        /// <param name="value">    The value of the object to validate. </param>
        ///
        /// <returns>   true if the specified value is valid; otherwise, false. </returns>

        //public override bool IsValid(object value)
        //{
        //    return true;
        //}

        public override bool RequiresValidationContext => true; 

        /// <summary>
        /// Validates the specified value with respect to the current validation attribute.
        /// </summary>
        ///
        /// <remarks>   CloudIDEaaS, 7/7/2021. </remarks>
        ///
        /// <param name="value">                The value to validate. </param>
        /// <param name="validationContext">    The context information about the validation operation. </param>
        ///
        /// <returns>
        /// An instance of the <see cref="T:System.ComponentModel.DataAnnotations.ValidationResult" />
        /// class.
        /// </returns>

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var serviceContainer = validationContext.ServiceContainer;
            var uniqueValidator = (IUniqueValidator) validationContext.GetService(typeof(IUniqueValidator));

            return uniqueValidator.IsUnique(value, validationContext);
        }
    }
}
