using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstraX.Contracts
{
    public enum BuildUIResponse
    {
        Aborted,
        Succeeded
    }

    public interface IBuildUIDaemon
    {
        bool MoveNext(string lastUIComponentURL);
        string GetProperty(string UIComponentURL, string propertyName);
        void SetProperty(string UIComponentURL, string propertyName, string value);
        void Abort();
        void Finish(string lastUIComponentURL);
        BuildUIResponse ResponseValue { get; }
    }
}
