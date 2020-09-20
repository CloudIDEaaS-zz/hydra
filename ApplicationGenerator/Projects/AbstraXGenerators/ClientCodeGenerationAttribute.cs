using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace AbstraX
{
    public class ClientCodeGenerationAttribute : Attribute
    {
        public Type GeneratorType { get; set; }

        public ClientCodeGenerationAttribute(Type generatorType)
        {
            this.GeneratorType = generatorType;
        }
    }

    public class ReturnsExternalAttribute : Attribute
    {
        public Type Type { get; set; }

        public ReturnsExternalAttribute(Type type)
        {
            this.Type = type;
        }
    }

    public class ReturnsEntitiesAttribute : Attribute
    {
        public Type[] EntityTypes { get; set; }

        public ReturnsEntitiesAttribute(params Type[] entityTypes)
        {
            this.EntityTypes = entityTypes;
        }
    }
}
