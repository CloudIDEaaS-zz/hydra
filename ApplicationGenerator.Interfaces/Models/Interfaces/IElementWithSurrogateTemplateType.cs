using AbstraX.ServerInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.Models.Interfaces
{
    public interface IElementWithSurrogateTemplateType : IElement
    {
        Type GetSurrogateTemplateType<T>();
        bool HasSurrogateTemplateType<T>();
        void RegisterSurrogateTemplateType<T, TSurrogate>();
    }
}
