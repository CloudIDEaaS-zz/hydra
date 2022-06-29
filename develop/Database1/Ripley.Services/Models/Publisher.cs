using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace Ripley.Services.Models
{
    [DebuggerDisplay(" { DebugInfo } ")]
    public class Publisher
    {
        public Guid PublisherId { get; set; }
        public string PublisherName { get; set; }

        internal string DebugInfo
        {
            get
            {

                return string.Format(
                    "PublisherId: {0}\r\n" +
                    "PublisherName: {1}",
                    this.PublisherId.ToString(),
                    this.PublisherName
                );
            }
        }

        public override string ToString()
        {
            return this.DebugInfo;
        }
    }
}
