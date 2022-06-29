// file:	WebApiService.cs
//
// summary:	Implements the web API service class

using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX
{
    /// <summary>   A service for accessing web apis information. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 1/15/2021. </remarks>

    public class WebApiService : IDisposable
    {
        /// <summary>   URL of the resource. </summary>
        private string url;
        /// <summary>   True to log service messages. </summary>
        private bool logServiceMessages;
        private IDisposable disposable;

        /// <summary>   Gets or sets the generator handler. </summary>
        ///
        /// <value> The generator handler. </value>

        public IGeneratorHandler GeneratorHandler { get; set; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/15/2021. </remarks>
        ///
        /// <param name="url">                  URL of the resource. </param>
        /// <param name="logServiceMessages">   True to log service messages. </param>

        public WebApiService(string url, bool logServiceMessages)
        {
            this.url = url;
            this.logServiceMessages = logServiceMessages;
        }

        /// <summary>   Starts this.  </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/15/2021. </remarks>

        public void Start()
        {
            disposable = WebApp.Start(url, (a) => new Startup(logServiceMessages).Configuration(a));
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/19/2021. </remarks>

        public void Dispose()
        {
            disposable.Dispose();
        }
    }
}
