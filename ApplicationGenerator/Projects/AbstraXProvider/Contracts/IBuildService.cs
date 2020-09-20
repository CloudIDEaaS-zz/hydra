using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Activation;

namespace AbstraX.Contracts
{
    [ServiceContract]
    public interface IBuildService
    {
        [OperationContract(IsOneWay=true)]
        void BuildMiddleLayer(string id);
        [OperationContract]
        string GetProperty(string UIComponentURL, string propertyName);
        [OperationContract]
        void SetProperty(string UIComponentURL, string propertyName, string value);
        [OperationContract]
        bool MoveNext(string lastUIComponentURL);
        [OperationContract]
        void AbortBuild(string lastUIComponentURL);
        [OperationContract(IsOneWay = true)]
        void Finish(string lastUIComponentURL);
    }
}
