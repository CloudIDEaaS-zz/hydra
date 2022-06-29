using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX.Angular.Routes
{
    public class Navigation
    {
        public string Name { get; }
        public string Url { get; }
        public string[] Segments { get; }

        public Navigation(string name, string url, bool splitIntoSegments = false)
        {
            this.Name = name;
            this.Url = url;

            if (splitIntoSegments)
            {
                this.Segments = url.Split('/', StringSplitOptions.RemoveEmptyEntries);
            }
        }

        public Navigation(string name, string[] segments)
        {
            this.Name = name;
            this.Segments = segments;
        }

        public Navigation(string name, string url, string[] segments)
        {
            this.Name = name;
            this.Url = url;
            this.Segments = segments;
        }
    }
}
