using AbstraX.DataAnnotations;
using AbstraX.Generators.Base;
using AbstraX.ServerInterfaces;
using ApplicationGenerator.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX.Generators.Server.WebAPIRestServiceProvider
{
    public enum ServiceMethodKind
    {
        List,
        Create,
        Get,
        Update,
        Delete
    }

    [DebuggerDisplay(" { MethodFullSignature } ")]
    public class ServiceMethod : HandlerObjectBase
    {
        public string Name { get; }
        public string ServiceName { get; }
        public string ParmsSignature { get; }
        public string MethodFullSignature { get; }
        public string MethodSignature { get; }
        public string InterfaceSignature { get; }
        public ServiceMethodKind ServiceMethodKind { get; }
        public Dictionary<string, string> MethodVariables { get; set; }
        public string HttpMethod { get; }
        public string ReturnType { get; }
        public Func<string[], string> UniqueIdFactory { get; set; }
        public string ResultPath { get; set; }
        public Dictionary<string, string> ReturnProperties { get; internal set; }

        public ServiceMethod(IBase baseObject, string prefix, string serviceName, string parmsSignature, IGeneratorConfiguration generatorConfiguration) : base(baseObject, generatorConfiguration)
        {
            var displayName = prefix + baseObject.GetDisplayName();
            var name = prefix + baseObject.GetNavigationName();

            this.ServiceMethodKind = EnumUtils.GetValue<ServiceMethodKind>(prefix);
            this.Name = name;
            this.ServiceName = serviceName;
            this.ParmsSignature = parmsSignature;

            switch (this.ServiceMethodKind)
            {
                case ServiceMethodKind.List:

                    this.HttpMethod = "GET";
                    this.ReturnType = "IEnumerable<dynamic>";
                    break;

                case ServiceMethodKind.Create:
                    
                    this.HttpMethod = "POST";
                    this.ReturnType = "void";
                    break;

                case ServiceMethodKind.Get:

                    this.HttpMethod = "POST";
                    this.ReturnType = "dynamic";
                    break;

                case ServiceMethodKind.Update:

                    this.HttpMethod = "PUT";
                    this.ReturnType = "void";
                    break;


                case ServiceMethodKind.Delete:

                    this.HttpMethod = "DELETE";
                    this.ReturnType = "void";
                    break;
            }

            this.MethodFullSignature = $"{ this.ReturnType } { this.ServiceName }.{ this.Name }({ this.ParmsSignature })";
            this.MethodSignature = $"public { this.ReturnType } { this.Name }({ this.ParmsSignature })";
            this.InterfaceSignature = $"{ this.ReturnType } { this.Name }({ this.ParmsSignature });";
        }
    }
}
