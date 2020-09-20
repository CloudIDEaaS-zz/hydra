using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class AppNameAttribute : Attribute
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public AppNameAttribute(string name, string description)
        {
            this.Name = name;
            this.Description = description;
        }
    }
}
