using ApplicationGenerator.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationGenerator.Data
{
    public interface IAbstraXProviderServices
    {
        IAbstraXDataProvider LocateProvider(DbContext context);
    }
}
