using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ApplicationGenerator.Data.Interfaces
{
    public interface IAbstraXDataProvider
    {
        string GetIdBase();
        NamingConvention GetNamingConvention();
        IEnumerator CreateEntitySetEnumerator(DbContext context, string setName, string id);
    }
}
