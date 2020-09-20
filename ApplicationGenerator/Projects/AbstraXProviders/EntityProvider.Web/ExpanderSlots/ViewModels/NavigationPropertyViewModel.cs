using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityProvider.Web.ExpanderSlots.ViewModels
{
    class NavigationPropertyViewModel
    {
        public string NewOrExisting { get; set; }
        public string ExistingEntity { get; set; }
        public string NewEntity { get; set; }
        public string Multiplicity { get; set; }
        public string NavigationPropertyName { get; set; }
    }
}
