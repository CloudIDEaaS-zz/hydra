using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ServiceModel.DomainServices.Server;
using Utils;

namespace CodeInterfaces
{
    [DataContract]
    public class Facet
    {
        [DataMember, Key]
        public string ID { get; set; }
        [DataMember]
        public string ParentID { get; set; }
        [DataMember, Include, Association("Facet_FacetType", "ID", "ParentID")]
        public BaseType FacetType { get; set; }
        [DataMember, Include, Association("Facet_Arguments", "ID", "ParentID")]
        public Argument[] Arguments { get; set; }
        [DataMember]
        public string AttributeCode
        {
            get
            {
                return FacetType.Name + "(" + Arguments.Select(a => a.Value).ToCommaDelimitedList() + ")";
            }
        }
    }
}
