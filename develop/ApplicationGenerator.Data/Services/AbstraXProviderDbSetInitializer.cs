using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace ApplicationGenerator.Data
{
    public class AbstraXProviderDbSetInitializer : IDbSetInitializer
    {
        public void InitializeSets(DbContext context)
        {
            var providerContext = (AbstraXProviderContext)context;
            var projectProperty = providerContext.GetType().GetProperty("Projects");
            var projects = new ProjectDbSet();

            projectProperty.SetValue(context, projects);
        }
    }
}