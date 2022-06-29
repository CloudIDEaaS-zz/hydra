using AbstraX.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX
{
    public class ContainerAction
    {
        public IEntityContainer EntityContainer { get; }
        public Action Action { get; }

        public ContainerAction(IEntityContainer container, Action action)
        {
            this.EntityContainer = container;
            this.Action = action;
        }
    }
}
