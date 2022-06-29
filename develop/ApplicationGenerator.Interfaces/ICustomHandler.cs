using AbstraX.ServerInterfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AbstraX
{
    public enum ProcessPoint
    {
        Pre,
        Post
    }

    public interface ICustomHandler : IFacetHandler
    {
        ProcessPoint ProcessPoint { get; }
        string ExecutedProcessPath { get; }
        DefinitionKind ExecutedEntityType { get; }
    }
}
