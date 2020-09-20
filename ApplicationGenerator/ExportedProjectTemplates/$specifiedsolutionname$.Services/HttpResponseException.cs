using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace $safeprojectname$
{
    public class HttpResponseException : Exception
    {
        public HttpStatusCode Status { get; set; } = HttpStatusCode.InternalServerError;
        public object Value { get; set; }

        public HttpResponseException(string message)
        {
            this.Value = message;
        }

        public HttpResponseException(string format, params object[] args)
        {
            this.Value = string.Format(format, args);
        }

        public HttpResponseException(HttpStatusCode status, string message)
        {
            this.Value = message;
            this.Status = status;
        }

        public HttpResponseException(HttpStatusCode status, string format, params object[] args)
        {
            this.Value = string.Format(format, args);
            this.Status = status;
        }
    }
}
