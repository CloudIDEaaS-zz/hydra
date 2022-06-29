using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Hydra.Apps.Administration.ServicesClient.Test
{
    public class Program
    {
        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.Scan();

            DoClientTest();
        }

        private static void DoClientTest()
        {
            var assembly = Assembly.LoadFrom(@"D:\MC\CloudIDEaaS\root\Hydra.Apps.Administration.ServicesClient.Test\bin\Debug\Hydra.Apps.Administration.ServicesClient.TestProxy.dll");
            var type = assembly.GetType("Hydra.Apps.Administration.ServicesClient.TestProxy.ProxyClass");
            var proxyClass = (dynamic) Activator.CreateInstance(type);

            proxyClass.TestConnection();
        }
    }
}
