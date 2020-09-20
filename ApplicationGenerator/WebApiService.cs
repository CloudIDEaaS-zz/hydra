using AbstraX.Handlers.CommandHandlers;
using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX
{
    public class WebApiService
    {
        private string url;
        public GeneratorHandler GeneratorHandler { get; set; }

        public WebApiService(string url)
        {
            this.url = url;
        }

        public void Start()
        {
            WebApp.Start<Startup>(url);
        }
    }
}
