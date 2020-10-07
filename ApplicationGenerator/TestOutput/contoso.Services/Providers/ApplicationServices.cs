using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace contoso.Services.Providers
{
    public interface IApplicationServices : IDisposable
    {
    }

    public class ApplicationServices : IApplicationServices
    {
    public void Dispose()
        {
        }

    }
}
