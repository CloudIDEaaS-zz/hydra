using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace Ripley.Services.Models
{
    [DebuggerDisplay(" { DebugInfo } ")]
    public class Course
    {
        public Guid CourseId { get; set; }
        public Guid? PublisherId { get; set; }
        public string CourseName { get; set; }
        public string CourseDescription { get; set; }

        internal string DebugInfo
        {
            get
            {

                return string.Format(
                    "CourseId: {0}\r\n" +
                    "PublisherId: {1}\r\n" +
                    "CourseName: {2}\r\n" +
                    "CourseDescription: {3}",
                    this.CourseId.ToString(),
                    this.PublisherId.ToString(),
                    this.CourseName,
                    this.CourseDescription
                );
            }
        }

        public override string ToString()
        {
            return this.DebugInfo;
        }
    }
}
