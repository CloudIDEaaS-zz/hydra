using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Dispatcher;
using Channels = System.ServiceModel.Channels;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;

namespace AbstraX
{
    public class AbstraXProviderMessageInspectorOperationBehaviorAttribute : Attribute, IOperationBehavior 
    { 
        public void AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters) 
        { 
        } 
        
        public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation) 
        { 
        } 
        
        public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation) 
        {
            var inspector = new AbstraXProviderMessageInspector(); 

            dispatchOperation.Parent.MessageInspectors.Add(inspector); 
        } 
        
        public void Validate(OperationDescription operationDescription) 
        { 
        } 
    }

    public class AbstraXProviderMessageInspector : IDispatchMessageInspector
    {
        public object AfterReceiveRequest(ref Channels.Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            return null;
        }

        public void BeforeSendReply(ref Channels.Message reply, object correlationState)
        {
        }
    }
}
