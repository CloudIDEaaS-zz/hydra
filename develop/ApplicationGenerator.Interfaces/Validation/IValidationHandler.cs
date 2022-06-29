using AbstraX.ServerInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.Validation
{
    public interface IValidationHandler
    {
        void HandleValidations(IBase baseObject, IValidationSet validationSet, Attribute[] attributes);
    }
}
