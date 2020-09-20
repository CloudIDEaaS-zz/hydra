using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Utils
{
    public class DefaultHttpMessageHandlerFactory : IHttpMessageHandlerFactory
    {
        private IServiceProvider serviceProvider;

        public DefaultHttpMessageHandlerFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public HttpMessageHandler CreateHandler(string name)
        {
            var messageHandler = (HttpMessageHandler) serviceProvider.GetService(typeof(HttpMessageHandler));

            if (messageHandler != null)
            {
                return messageHandler;
            }

            return null;
        }
    }
}
