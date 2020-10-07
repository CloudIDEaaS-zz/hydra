using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Proxies;
using Toolbelt.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Data.Common;

namespace contoso.Entities.Models
{
    public class contosoContext : DbContext
    {
        private IConfiguration configuration;
        private IHostEnvironment environment;

        public contosoContext()  // for unit testing
        { 
        }

        public contosoContext(DbContextOptions<contosoContext> options, IHostEnvironment environment, IConfiguration configuration) : base(options)
        {
            var database = this.Database;
            DbConnection connection;

            this.configuration = configuration;
            this.environment = environment;

            if (database.ProviderName == "Microsoft.EntityFrameworkCore.Sqlite")
            {
                connection = database.GetDbConnection();

                if (connection.State == System.Data.ConnectionState.Closed)
                {
                    database.OpenConnection();
                    database.EnsureCreated();
                }
            }
            else
            {
                database.EnsureCreated();
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
