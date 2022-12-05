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

namespace AbstraX.Generators.Server.WebAPIStartupGraphQLSchemas
{
    [DebuggerDisplay(" { SchemaName } ")]
    public class GraphQLSchema : HandlerObjectBase
    {
        public string SchemaName { get; }

        public GraphQLSchema(IBase baseObject, string schemaName, IGeneratorConfiguration generatorConfiguration) : base(baseObject, generatorConfiguration)
        {
            this.SchemaName = schemaName;
        }
    }
}
