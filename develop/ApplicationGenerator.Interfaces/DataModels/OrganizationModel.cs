// file:	DataModels\Organization.cs
//
// summary:	Implements the organization class

using AbstraX.DataAnnotations;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;

namespace AbstraX.DataModels
{
    /// <summary>   A data Model for the organization. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 7/5/2021. </remarks>

    public class OrganizationModel : IDataErrorInfo
    {
        /// <summary>   Gets or sets the service provider. </summary>
        ///
        /// <value> The service provider. </value>

        public ServiceProvider ServiceProvider { get; set; }

        /// <summary>   Gets or sets the invalid properties. </summary>
        ///
        /// <value> The invalid properties. </value>

        public List<string> InvalidProperties { get; private set; }
        private List<ValidationResult> lastValidationResults;

        /// <summary>   Default constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 7/8/2021. </remarks>

        public OrganizationModel()
        {
            lastValidationResults = new List<ValidationResult>();
            InvalidProperties = new List<string>();
        }

        /// <summary>   Gets the error message for the property with the given name. </summary>
        ///
        /// <param name="property"> The name of the property whose error message to get. </param>
        ///
        /// <returns>   The error message for the property. The default is an empty string (""). </returns>

        public string this[string property]
        {
            get
            {
                bool valid;
                object value;
                List<ValidationResult> validationResults;
                var propertyDescriptor = TypeDescriptor.GetProperties(this)[property];
                var validationContext = new ValidationContext(this, this.ServiceProvider, null)
                { 
                    MemberName = property 
                };

                if (propertyDescriptor == null)
                {
                    return string.Empty;
                }

                validationResults = new List<ValidationResult>();
                value = propertyDescriptor.GetValue(this);
                valid = Validator.TryValidateProperty(value, validationContext, validationResults);

                lastValidationResults.AddRange(validationResults);

                if (!valid)
                {
                    var validationResult = validationResults.First();

                    if (!InvalidProperties.Contains(property))
                    {
                        InvalidProperties.Add(property);
                    }

                    return validationResult.ErrorMessage;
                }
                else if (InvalidProperties.Contains(property))
                {
                    InvalidProperties.Remove(property);
                }

                return string.Empty;
            }
        }

        /// <summary>   Validates the given text box base. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 7/7/2021. </remarks>
        ///
        /// <param name="textBoxBase">  The text box base. </param>

        public Func<bool> Validate(TextBoxBase textBoxBase)
        {
            var binding = textBoxBase.DataBindings.Cast<Binding>().Single();

            return new Func<bool>(() =>
            {
                lastValidationResults.Clear();

                binding.WriteValue();

                return lastValidationResults.Count == 0;
            });
        }

        /// <summary>   Gets a value indicating whether this  is invalid. </summary>
        ///
        /// <value> True if this  is invalid, false if not. </value>

        public bool IsInvalid
        {
            get
            {
                return InvalidProperties.Count > 0;
            }
        }

        /// <summary>   Gets or sets the name of the organization. </summary>
        ///
        /// <value> The name of the organization. </value>

        public string OrganizationName { get; set; }

        /// <summary>   Gets or sets a unique name for the organization. </summary>
        ///
        /// <value> Unique name of the organization. </value>

        [Required]
        [MinLength(3)]
        [RegularExpression(StringExtensions.REGEX_IDENTIFIER, ErrorMessage="Name must start with a alphabetic character. Remaining characters of alphanumeric or underscore")]
        [UniqueValidation()]
        public string OrganizationUniqueName { get; set; }

        /// <summary>   Gets or sets the name of the application. </summary>
        ///
        /// <value> The name of the application. </value>

        public string AppName { get; set; }

        /// <summary>   Gets or sets a unique name for the application. </summary>
        ///
        /// <value> Unique name of the application. </value>

        [Required]
        [MinLength(3)]
        [RegularExpression(StringExtensions.REGEX_IDENTIFIER, ErrorMessage= "Name must start with a alphabetic character. Remaining characters of alphanumeric or underscore")]
        [UniqueValidation]
        public string AppUniqueName { get; set; }

        /// <summary>   Gets or sets the name of the administrator first. </summary>
        ///
        /// <value> The name of the administrator first. </value>

        [Required]
        [MinLength(3)]
        public string AdministratorFirstName { get; set; }

        /// <summary>   Gets or sets the name of the administrator last. </summary>
        ///
        /// <value> The name of the administrator last. </value>

        [Required]
        [MinLength(3)]
        public string AdministratorLastName { get; set; }

        /// <summary>   Gets or sets the administrator email address. </summary>
        ///
        /// <value> The administrator email address. </value>

        [Required]
        [EmailAddress]
        public string AdministratorEmailAddress { get; set; }

        /// <summary>   Gets or sets the administrator phone number. </summary>
        ///
        /// <value> The administrator phone number. </value>

        [Required]
        [Phone]
        public string AdministratorPhoneNumber { get; set; }

        /// <summary>   Gets or sets the organization city. </summary>
        ///
        /// <value> The organization city. </value>

        public string OrganizationCity { get; set; }

        /// <summary>   Gets or sets the organization state or province. </summary>
        ///
        /// <value> The organization state or province. </value>

        public string OrganizationStateOrProvince { get; set; }

        /// <summary>   Gets or sets the organization country. </summary>
        ///
        /// <value> The organization country. </value>

        public string OrganizationCountry { get; set; }

        /// <summary>   Gets or sets the organization postal code. </summary>
        ///
        /// <value> The organization postal code. </value>

        public string OrganizationPostalCode { get; set; }

        /// <summary>   Gets an error message indicating what is wrong with this object. </summary>
        ///
        /// <value>
        /// An error message indicating what is wrong with this object. The default is an empty string
        /// ("").
        /// </value>

        public string Error
        {
            get
            {
                return null;
            }
        }
    }
}