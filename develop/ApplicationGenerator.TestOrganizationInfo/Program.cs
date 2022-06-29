using AbstraX;
using AbstraX.DataModels;
using HydraAppStore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ApplicationGenerator.TestOrganizationInfo
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var services = new ServiceCollection();
            services.AddTransient<IUniqueValidator, UniqueValidator>();
            var serviceProvider = services.BuildServiceProvider();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new frmOrganizationInfo(new OrganizationModel
            {
                AppName = "contoso",
                AppUniqueName = "contoso",
                OrganizationName = "Contoso Inc",
                OrganizationUniqueName = "contosoinc",
                AdministratorFirstName = "Ken",
                AdministratorLastName = "Netherland",
                AdministratorEmailAddress = "ken.l.netherland@gmail.com",
                AdministratorPhoneNumber = "480-603-8930",
                ServiceProvider = serviceProvider,
            }));
        }
    }
}
