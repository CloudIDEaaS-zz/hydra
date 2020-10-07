using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Utils;

namespace contoso.Services.Providers
{
    public interface ILoginTimeoutService
    {
        void Start();
        void Stop();
    }

}
