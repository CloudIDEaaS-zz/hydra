using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstraX.Angular.Routes;
using AbstraX.DataAnnotations;
using AbstraX.FolderStructure;
using AbstraX.ServerInterfaces;
using Utils;

namespace AbstraX.Angular
{
    public class RoutingModule : AngularModule
    {
        public RoutingModule(string name) : base(name)
        {
        }
    }
}
