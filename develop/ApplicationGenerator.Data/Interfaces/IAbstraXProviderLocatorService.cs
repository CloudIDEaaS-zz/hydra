using ApplicationGenerator.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationGenerator.Data
{
    public interface IAbstraXProviderLocatorService
    {
        IAbstraXDataProvider LocateProvider(DbContext context);
    }
}
