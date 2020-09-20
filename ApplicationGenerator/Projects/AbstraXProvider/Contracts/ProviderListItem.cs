using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AbstraX;
using System.ComponentModel.DataAnnotations;
using AbstraX.ServerInterfaces;
using System.ServiceModel.DomainServices.Server;
using System.Runtime.Serialization;
using System.Net;
using System.IO;
using Reflection = System.Reflection;
using System.Diagnostics;
using AbstraX.XPathBuilder;

namespace AbstraX.Contracts
{
    [DataContract, Serializable]
    public class ProviderListItem
    {
        [DataMember, Key]
        public string ID { get; set; }
        [DataMember]
        public string FileName { get; set; }
        [DataMember]
        public DateTime ModifiedDate { get; set; }
        [DataMember]
        public bool Cached { get; set; }

        public ProviderListItem()
        {
        }

        public ProviderListItem(string id)
        {
            this.ID = id;
        }
    }
}
