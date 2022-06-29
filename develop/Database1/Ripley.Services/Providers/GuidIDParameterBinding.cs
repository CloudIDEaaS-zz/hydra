using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Metadata;

namespace Ripley.Services.Providers
{
    public class GuidIdParameterBinding : HttpParameterBinding
    {
        public GuidIdParameterBinding(HttpParameterDescriptor parameter) : base(parameter)
        {
        }

        public override Task ExecuteBindingAsync(ModelMetadataProvider metadataProvider, HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            actionContext.ActionArguments[Descriptor.ParameterName] = Guid.NewGuid();

            var tsc = new TaskCompletionSource<object>();
            tsc.SetResult(null);
            return tsc.Task;
        }
    }
}