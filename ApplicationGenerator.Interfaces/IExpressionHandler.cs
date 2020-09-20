using AbstraX.ServerInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX
{
    public class ExpressionHandlerAttribute : Attribute
    {
        public Guid AbstraXProviderGuid { get; }

        public ExpressionHandlerAttribute(string abstraXProviderGuid)
        {
            this.AbstraXProviderGuid = Guid.Parse(abstraXProviderGuid);
        }
    }

    public interface IExpressionHandler
    {
        string Handle(IBase baseObject, Enum type, string expression);
        T Handle<T>(IBase baseObject, Enum type, Enum expressionType, Enum returnType, string expression, params object[] parms);
        string Handle(IBase baseObject, Enum type, object jsonObject);
        T Handle<T>(IBase baseObject, Enum type, Enum expressionType, Enum returnType, object jsonObject, params object[] parms);
    }
}
