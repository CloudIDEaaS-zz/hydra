using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace CodeInterfaces
{
    [DataContract]
    public class Argument
    {
        [DataMember, Key]
        public string ID { get; set; }
        [DataMember]
        public string ParentID { get; set; }
        [DataMember]
        public string Value { get; set; }
    }
}
