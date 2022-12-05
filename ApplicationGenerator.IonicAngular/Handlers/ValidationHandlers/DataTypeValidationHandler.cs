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
    [ValidationHandler(typeof(DataTypeAttribute))]
    public class DataTypeValidationHandler : IValidationHandler
    {
        public void HandleValidations(IBase baseObject, IValidationSet validationSet, Attribute[] attributes)
        {
            var dataTypeAttribute = attributes.OfType<DataTypeAttribute>().Single();

            validationSet.AddValidationElement(new TextAreaValidationElement());
        }
    }
}
