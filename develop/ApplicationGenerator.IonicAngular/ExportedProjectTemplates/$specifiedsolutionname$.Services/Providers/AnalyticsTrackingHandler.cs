using GoogleMeasurementProtocol;
using GoogleMeasurementProtocol.Parameters.ContentInformation;
using GoogleMeasurementProtocol.Parameters.User;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace $safeprojectname$.Providers
{
    internal class AnalyticsTrackingHandler : DelegatingHandler
    {
        private readonly RequestDelegate next;
        private readonly IHostEnvironment environment;
        private string measurementId;
        private GoogleAnalyticsRequestFactory factory;

        public AnalyticsTrackingHandler(RequestDelegate next, IHostEnvironment environment, IConfiguration configuration)
        {
            this.next = next;
            this.environment = environment;

            measurementId = configuration["GoogleAnalyticsMeasurementId"];
            factory = new GoogleAnalyticsRequestFactory(measurementId);
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                //var request = context.Request;
                //var host = request.Headers["Host"].Single();
                //var referer = request.Headers["Referer"].SingleOrDefault();
                //var userAgent = request.Headers["User-Agent"].SingleOrDefault();
                //var origin = request.Headers["Origin"].SingleOrDefault();
                //var fetchSite = request.Headers["Sec-Fetch-Site"].SingleOrDefault();
                //var fetchMode = request.Headers["Sec-Fetch-Mode"].SingleOrDefault();
                //var fetchDestination = request.Headers["Sec-Fetch-Dest"].SingleOrDefault();
                //var trackingRequest = factory.CreateRequest(HitTypes.PageView);

                ////Add parameters to your request, each parameter has a corresponding class which has name = parameter name from google reference docs
                //trackingRequest.Parameters.Add(new DocumentHostName("test.com"));
                //trackingRequest.Parameters.Add(new DocumentPath("/test/testPath2"));
                //trackingRequest.Parameters.Add(new DocumentTitle("test title2"));

                //var clientId = new ClientId(Guid.NewGuid());

                ////Make a get request which will contain all information from above
                //await trackingRequest.GetAsync(clientId);

                ////Make a Post request which will contain all information from above
                //await trackingRequest.PostAsync(clientId);

                await next(context);
            }
            catch (Exception ex)
            {
            }
        }
    }
}
