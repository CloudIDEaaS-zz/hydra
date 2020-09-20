using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.ServiceModel.DomainServices.Tools;
using Microsoft.ServiceModel.DomainServices.Tools.TextTemplate;
using Microsoft.ServiceModel.DomainServices.Tools.TextTemplate.CSharpGenerators;
using System.IO;
using System.Diagnostics;
using AbstraX.Generators;

namespace EntityProvider.Web.Generators
{
    [DomainServiceClientCodeGenerator(typeof(EntityProviderCodeGenerator), "C#")]
    public class EntityProviderCodeGenerator : CodeGeneratorX
    {
        public EntityProviderCodeGenerator()
        {
        }
    }
}