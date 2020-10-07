using contoso.Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace contoso.Services
{
    public static partial class Extensions
    {
        private static DateTime configWriteTime;
    }
}
