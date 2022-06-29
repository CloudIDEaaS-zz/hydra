// file:	IUniqueValidator.cs
//
// summary:	Declares the IUniqueValidator interface

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX
{
    /// <summary>   Interface for unique validator. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 7/7/2021. </remarks>

    public interface IUniqueValidator
    {
        /// <summary>   Is unique. </summary>
        ///
        /// <param name="value">                The value. </param>
        /// <param name="validationContext">    Context for the validation. </param>
        ///
        /// <returns>   A ValidationResult. </returns>

        ValidationResult IsUnique(object value, ValidationContext validationContext);
    }
}
