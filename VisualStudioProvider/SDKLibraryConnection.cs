using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;
using Utils;

namespace VisualStudioProvider
{
    public static class SDKLibraryConnection
    {
        public static string ConnectionString { get; private set; }
        public static string InnerConnectionString { get; private set; }

        static SDKLibraryConnection()
        {
            var configLocation = Environment.ExpandEnvironmentVariables(@"%HYDRASOLUTIONPATH%\SDKInterfaceLibrary.Entities\app.config");
            var document = XDocument.Load(configLocation);
            var connectionElement = document.XPathSelectElement("configuration/connectionStrings/add[@name='SDKInterfaceLibraryEntities']");
            var connectionString = connectionElement.Attribute("connectionString").Value;

            if (connectionString != null)
            {
                ConnectionString = connectionString;
                InnerConnectionString = ConnectionString.RegexGet("connection string=\"(?<connectionstring>.*?)\"", "connectionstring");
            }
        }
    }
}
