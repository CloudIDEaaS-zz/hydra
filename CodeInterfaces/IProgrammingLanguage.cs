using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeInterfaces.CodeParsers;

namespace CodeInterfaces
{
    public enum WellKnownLanguage
    {
        CSharp,
        VisualBasicNet,
        Java,
        NotWellKnown
    }

    public interface IProgrammingLanguage
    {
        WellKnownLanguage WellKnownLanguage { get; }
        ICodeParser CodeParser { get; }
    }
}
